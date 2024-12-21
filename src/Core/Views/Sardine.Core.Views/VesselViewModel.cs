using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
[assembly: CLSCompliant(true)]

namespace Sardine.Core.Views
{
    public abstract class VesselViewModel<T> : INotifyPropertyChanged, IDisposable
    {
        private readonly string[] __PropertyNames;
        private readonly string[] __PropertiesUpdatedWithTimer;
        private readonly System.Timers.Timer __updateTimer;
        private readonly bool __UpdateOnlyWithEvents;
        private bool __disposedValue;

#pragma warning disable CA1051 // Do not declare visible instance fields
        protected readonly Vessel Vessel___;
#pragma warning restore CA1051 // Do not declare visible instance fields


        public event PropertyChangedEventHandler? PropertyChanged;


        protected VesselViewModel(Vessel vesselContainer, string[] propertyNames, bool updateOnlyWithEvents, int updaterInterval, IList<IVesselPropertyToEventLink> eventCollection)
        {
            eventCollection ??= [];

            Vessel___ = vesselContainer;
            Vessel___.Reloaded += Vessel_Reloaded;
            __PropertyNames = propertyNames;
            __PropertiesUpdatedWithTimer = propertyNames.Where(propName => !eventCollection.Select(propEventLink => propEventLink.PropertyName).Contains(propName)).ToArray();
            __UpdateOnlyWithEvents = updateOnlyWithEvents;

            __updateTimer = new System.Timers.Timer(updaterInterval) { AutoReset = true };
            __updateTimer.Elapsed += UpdateTimer_Elapsed;

            foreach (IVesselPropertyToEventLink propEventLink in eventCollection)
            {
                EventInfo eventInfo = typeof(T).GetEvent(propEventLink.EventName)!;
                List<Type> types = [typeof(VesselViewModel<T>)];
                IEnumerable<Type> eventArgsType = eventInfo.EventHandlerType!.GetMethod("Invoke")!.GetParameters().Select(x => x.ParameterType);
                
                foreach (Type type in eventArgsType)
                    types.Add(type);

                DynamicMethod eventhandlerExecuter = new($"{propEventLink.PropertyName}_{propEventLink.EventName}", null, [.. types], typeof(VesselViewModel<T>));
                MethodInfo onPropChangedMethod = typeof(VesselViewModel<T>).GetMethod(nameof(OnPropertyChanged), BindingFlags.Instance | BindingFlags.NonPublic)!;

                ILGenerator il = eventhandlerExecuter.GetILGenerator();
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, propEventLink.PropertyName);
                il.Emit(OpCodes.Call, onPropChangedMethod);
                il.Emit(OpCodes.Nop);
                il.Emit(OpCodes.Ret);

                Delegate delegateObject = eventhandlerExecuter.CreateDelegate(eventInfo.EventHandlerType!, this);

                typeof(T).GetEvent(propEventLink.EventName)!.AddEventHandler(Vessel___.ObjectHandle, delegateObject);
            }

            EventInfo? propertyChangedEvent = typeof(T).GetEvent("PropertyChanged");
            propertyChangedEvent?.AddEventHandler(Vessel___.ObjectHandle, new PropertyChangedEventHandler((_, e) => { OnPropertyChanged(e.PropertyName ?? string.Empty); }));

            if (!__UpdateOnlyWithEvents && Vessel___.IsAvailable)
                __updateTimer.Start();
        }


        public void Set(string propertyName, object propertyValue) => GetType().GetProperty(propertyName)?.SetValue(this, propertyValue);
        public object? Get(string propertyName) => GetType().GetProperty(propertyName)?.GetValue(this);

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected void OnPropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        protected virtual void Dispose(bool disposing)
        {
            if (!__disposedValue)
            {
                if (disposing)
                    ((IDisposable)__updateTimer).Dispose();

                __disposedValue = true;
            }
        }

        private void Vessel_Reloaded(object? sender, VesselReloadedEventArgs e)
        {
            if (e.IsAvailable && !__UpdateOnlyWithEvents)
            {
                __updateTimer.Start();
            }
            else
            {
                __updateTimer.Stop();
            }
        }

        private void UpdateTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (Vessel___.IsAvailable)
            {
                foreach (string propertyName in __PropertiesUpdatedWithTimer)
                    OnPropertyChanged(propertyName);
            }
        }
    }
}
