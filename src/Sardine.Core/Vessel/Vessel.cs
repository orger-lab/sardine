using Sardine.Core.DataModel;
using Sardine.Core.Exceptions;
using Sardine.Core.Logs;

namespace Sardine.Core
{
    public sealed partial class Vessel<THandle> : Vessel
    {
        public delegate TOut? Transformer<in TIn, out TOut>(THandle handle, TIn data, MessageMetadata metadata);
        public delegate TOut? Source<out TOut>(THandle handle, out bool hasMore);
        public delegate void Sink<in TIn>(THandle handle, TIn data, MessageMetadata metadata);
        public delegate void ActiveStateChangedCallback(THandle handle, bool isActive, IList<Type> outTypes);

        private readonly Func<IList<object>, THandle> builder;
        private readonly Action<IList<object>, THandle>? initializer;
        private readonly Action<IList<object>, THandle>? invalidator;
        private readonly ActiveStateChangedCallback? activeStateChangedCallback;      


        public THandle? Handle
        {
            get => (THandle?)ObjectHandle;
            private set => ObjectHandle = value;
        }
        

        internal Vessel(IList<Vessel> dependencies,
                     Func<IList<object>, THandle> builder,
                     Action<IList<object>, THandle>? initializer,
                     Action<IList<object>, THandle>? invalidator,
                     ActiveStateChangedCallback? activeStateChangedCallback,
                     string? name = null, int queueCapacity = 1000) : base(dependencies, typeof(THandle), name, queueCapacity)
        {
            this.builder = builder;
            this.initializer = initializer;
            this.invalidator = invalidator;
            this.activeStateChangedCallback = activeStateChangedCallback;
        }


        public static explicit operator THandle(Vessel<THandle>? vessel)
        {
            return vessel is null ? throw new ArgumentNullException(nameof(vessel)) : vessel.Handle ?? throw new VesselException(NULL_VESSEL_ERROR_MESSAGE);
        }

        public THandle ToTHandle() => Handle ?? throw new VesselException(NULL_VESSEL_ERROR_MESSAGE);
        public new void GenerateData(Type type, double rate = 0) => base.GenerateData(type, rate);
        public void GenerateData(double rate = 0) => GenerateData(sourceRate: rate);

        public TOut? ExecuteCall<TOut>(Func<THandle, TOut> function)
        {
            return ExecuteCall((object x) => function((THandle)x));
        }

        public bool ExecuteCall(Action<THandle> action)
        {
            return ExecuteCall((object x) => action((THandle)x));
        }            

        public override void Reload()
        {
            Invalidate();

            foreach (Vessel dependency in DependencyList)
            {
                if (!dependency.IsOnline)
                    dependency.Reload();
            }

            if (AreDependenciesAvailable())
            {
                IList<object> dObjs = GetDependencyObjects();

                Handle = ExecuteBuilder(dObjs);
                ExeciteInitializer(dObjs);
            }

            OnReload(this);
        }

        public override void Invalidate()
        {
            ExecuteInvalidator(GetDependencyObjects());

            if (Handle is not null and IDisposable disposable)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    disposable.Dispose();
                }
                catch { }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            ObjectHandle = null;

            OnReload(this);
        }

        protected override void OnActiveStatusChanged() => activeStateChangedCallback?.Invoke(Handle!, IsActive, OutTypes.Distinct().ToList().AsReadOnly());

        protected override void Dispose(bool disposed)
        {
            if (disposed)
                Invalidate();

            base.Dispose(disposed);
        }
        
        private THandle? ExecuteBuilder(IList<object> dObjs)
        {
            IsLoading = true;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                Task<THandle> task = new(() => builder(dObjs));
                task.Start();

                if (task.Wait(ActionTimeoutMilliseconds) && task.IsCompletedSuccessfully)
                {
                    IsLoading = false;
                    return task.Result;
                }

                this.Log(new LogMessage("Build failed - timed out.", LogLevel.Error));
            }
            catch (Exception ex)
            {
                this.Log(new LogMessage($"Build error: {ex.Message}", LogLevel.Critical));
            }
#pragma warning restore CA1031 // Do not catch general exception types


            IsLoading = false;
            return default;
        }

        private void ExeciteInitializer(IList<object> dObjs)
        {
            if (Handle is null || initializer is null)
                return;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                Task task = new(() => initializer(dObjs, Handle));
                task.Start();

                if (task.Wait(ActionTimeoutMilliseconds))
                    return;

                this.Log(new LogMessage("Initialization failed - timed out.", LogLevel.Error));
            }
            catch (Exception ex)
            {
                this.Log(new LogMessage($"Initialization error: {ex.Message}", LogLevel.Critical));
            }
#pragma warning restore CA1031 // Do not catch general exception types

            Invalidate();
        }

        private void ExecuteInvalidator(IList<object> dObjs)
        {
            if (Handle is null || invalidator is null)
                return;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                Task task = new(() => invalidator(dObjs, Handle));
                task.Start();
                _ = task.Wait(ActionTimeoutMilliseconds);
            }
            catch (Exception ex)
            {
                this.Log(new LogMessage($"Invalidation error: {ex.Message}", LogLevel.Warning));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
