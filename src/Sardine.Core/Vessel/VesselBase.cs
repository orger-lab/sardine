using Sardine.Core.DataModel;
using Sardine.Core.DataModel.Abstractions;
using Sardine.Core.Logs;
using Sardine.Core.Utils.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sardine.Core
{
    public abstract partial class Vessel : INotifyPropertyChanged, IDisposable
    {
        internal const string LINKED_VESSEL_ERROR_MESSAGE = "Cannot change the data processors of a linked vessel.";
        internal const string NULL_VESSEL_ERROR_MESSAGE = "Handle is null.";

        private readonly List<Type> outTypes = [];
        private readonly System.Timers.Timer dataGenerationTimer = new() { AutoReset = true };
        private DataHandlingStrategy dataHandlingStrategy = DataHandlingStrategy.QueueToInbox;
        private string name = string.Empty;
        private bool isLinked;
        private bool isActive;
        private DataQueue? queue;
        private double sourceRate;
        private bool isLoading;
        private int queueLength;
        private int queueOccupation;
        private int queueHandlerAbortTimeout;
        private object? objectHandle;

        internal SentDataCounter SentDataCounter { get; } = new();
        internal List<ISource> Sources { get; } = [];
        internal Dictionary<ISource, object> SourceLocks { get; } = [];
        internal Dictionary<Type, List<ITransformer>> Transformers { get; } = [];
        internal Dictionary<Type, List<ISink>> Sinks { get; } = [];
        internal IList<Vessel> DependencyList { get; }
        internal Dictionary<IDataConsumer, List<Vessel>?> ReceiverFiltersDictionary { get; } = [];
        internal IList<Type> OutTypes => outTypes;
        public DatalineStatus DatalineStatus { get; } = new();
        public int QueueCapacity { get; }
        public string DisplayName { get; private set; } = string.Empty;
        public Type HandleType { get; private set; } = typeof(object);
        public int Signature { get; private set; }
        public object? ObjectHandle
        {
            get => objectHandle;
            protected internal set
            {
                objectHandle = value;
                OnPropertyChanged(nameof(IsOnline));
            }
        }
        public static int ActionTimeoutMilliseconds { get; set; } = 15000;
        public bool CaptureLogs { get; set; } = true;

        public string Name
        {
            get => name;
            
            internal set
            {
                name = value;
                DisplayName = StringOperations.GenerateNameWithSpacesFromCamelCase(name);
            }
        }

        public int QueueHandlerAbortTimeout
        {
            get => queueHandlerAbortTimeout;

            set
            {
                queueHandlerAbortTimeout = value;

                if (queue is not null)
                    queue.Timeout = value;
            }
        }        

        public double SourceRate
        {
            get => sourceRate;

            set
            {
                if (sourceRate == value)
                    return;

                sourceRate = value;
             
                if (sourceRate <= 0)
                {
                    dataGenerationTimer.Stop();
                }
                else
                {
                    dataGenerationTimer.Interval = 1000 / SourceRate;
                    dataGenerationTimer.Start();
                }

            }
        }

        public bool IsOnline
        {
            get
            {
                bool status = ObjectHandle is not null;

                if (!status)
                    IsActive = false;

                return status;
            }
        }

        public DataHandlingStrategy DataHandlingStrategy
        {
            get => dataHandlingStrategy;

            set
            {
                if (dataHandlingStrategy == value)
                    return;

                dataHandlingStrategy = value;

                switch (dataHandlingStrategy)
                {
                    case DataHandlingStrategy.QueueToInbox:
                        queue?.StartQueueHandling();
                        break;
                    case DataHandlingStrategy.ResolveImmediatly:
                        queue?.StopQueueHandling();
                        break;
                }

                OnPropertyChanged();
            }
        }

        public bool IsLinked
        {
            get => isLinked;

            private set
            {
                if (isLinked != value)
                {
                    isLinked = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive == value)
                    return;

                if (!IsLinked)
                    return;

                isActive = value;

                OnActiveStatusChanged();

                if (dataHandlingStrategy == DataHandlingStrategy.QueueToInbox)
                {
                    if (isActive)
                    {
                        queue?.StartQueueHandling();

                        if (sourceRate > 0)
                            dataGenerationTimer.Start();
                    }
                    else
                    {
                        queue?.StopQueueHandling();
                        dataGenerationTimer.Stop();
                    }
                }

                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => isLoading;

            protected set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }

        public int QueueLength
        {
            get => queueLength;

            private set
            {
                if (queueLength != value)
                {
                    queueLength = value;
                    QueueOccupation = 100 * QueueLength / QueueCapacity;
                }
            }
        }

        public int QueueOccupation
        {
            get => queueOccupation;

            private set
            {
                if (queueOccupation != value)
                {
                    queueOccupation = value;
                    OnPropertyChanged();
                }
            }
        }


        public event EventHandler<VesselReloadedEventArgs>? Reloaded;
        public event EventHandler<OnSentDataEventArgs>? OnSentData;
        public event PropertyChangedEventHandler? PropertyChanged;


        protected internal Vessel(IList<Vessel>? dependencies, Type handleType, string? name = null, int queueCapacity = 1000)
        {
            Name = name ?? string.Empty;
            QueueCapacity = queueCapacity;

            DependencyList = dependencies ?? [];
            
            foreach (Vessel dependency in DependencyList)
                dependency.Reloaded += (_, e) => { if (!e.IsOnline && IsOnline) { Invalidate(); } };

            HandleType = handleType;

            dataGenerationTimer.Elapsed += (_, _) => GenerateData(sourceRate: SourceRate);
        }

        public abstract void Reload();
        public abstract void Invalidate();
        protected abstract void OnActiveStatusChanged();
        
        public bool CanProvideType(Type type) => OutTypes.Contains(type);// Select((x) => x.InheritsFrom(type)).Any((x) => x);
        public bool AreDependenciesAvailable() => DependencyList.All(x => x.IsOnline);

        public bool ExecuteCall(Action<object> action)
        {
            if (action is null)
                return false;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                if (ObjectHandle is not null)
                {
                    action.Invoke(ObjectHandle);
                    return true;
                }
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (Exception ex)
            {
                UnhandledVesselExceptionHandler(ex);
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return false;
        }

        public TOut? ExecuteCall<TOut>(Func<object, TOut> function) 
        {
            if (function is null)
                return default;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                if (ObjectHandle is not null)
                    return function.Invoke(ObjectHandle);
            }
            catch (InvalidCastException)
            {
                return default;
            }
            catch (Exception ex)
            {
                UnhandledVesselExceptionHandler(ex);
                return default;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return default;
        }

        public void UnhandledVesselExceptionHandler(Exception? exception)
        {
            this.Log(new LogMessage($"An exception caused this vessel to be invalidated: {exception?.Message}", LogLevel.Critical));
            Invalidate();
        }

        public void Link()
        {
            if (IsLinked)
                return;

            Signature = GetHashCode();

            foreach (IDataConsumer actuator in ReceiverFiltersDictionary.Keys)
            {
                ReceiverFiltersDictionary[actuator] ??= GetListOfVesselsToLink(actuator);

                foreach (Vessel vessel in ReceiverFiltersDictionary[actuator]!)
                    vessel.OnSentData += ReceiveData;
            }

            if (ReceiverFiltersDictionary.Keys.Count != 0 || OutTypes.Count != 0)
            {
                queue = new(this, ProcessData, QueueCapacity);
                queue.Timeout = QueueHandlerAbortTimeout;
                queue.OnQueueHandlingFailure += (_, e) => { IsActive = false; this.Log(new LogMessage($"Queue handling failure: {e.Exception.Message}", LogLevel.Critical)); };

                IsLinked = true;
            }
        }

        public void Unlink()
        {
            if (!IsLinked)
                return;

            foreach (IDataConsumer actuator in ReceiverFiltersDictionary.Keys)
            {
                List<Vessel>? vesselsToLink = ReceiverFiltersDictionary[actuator] ?? GetListOfVesselsToLink(actuator);

                if (vesselsToLink is null)
                    continue;

                foreach (Vessel vessel in vesselsToLink)
                    vessel.OnSentData -= ReceiveData;
            }

            queue?.Dispose();
            queue = null;

            IsLinked = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected IList<object> GetDependencyObjects() => DependencyList.Select((x) => x.ObjectHandle!).ToList();
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected virtual void Dispose(bool disposed)
        {
            dataGenerationTimer.Dispose();
            queue?.Dispose();
            queue = null;
        }
        
        protected void OnReload(object? caller)
        {
            Reloaded?.Invoke(caller, new VesselReloadedEventArgs(IsOnline));
            OnPropertyChanged(nameof(IsOnline));
        }
        
        private static List<Vessel> GetListOfVesselsToLink(IDataConsumer actuator)
        {
            List<Vessel> vessels = [];

            foreach (Vessel vessel in Fleet.Current.VesselCollection)
            {
                if (vessel.CanProvideType(actuator.InputDataType))
                    vessels.Add(vessel);
            }

            return vessels;
        }
    }
}
