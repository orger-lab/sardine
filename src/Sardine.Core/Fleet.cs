using Sardine.Core.Exceptions;
using Sardine.Core.Graph;
using Sardine.Core.Logs;
using Sardine.Core.Settings;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
[assembly: CLSCompliant(true)]

namespace Sardine.Core
{
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public abstract class Fleet : IDisposable, IReadOnlyCollection<Vessel>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
        private static Fleet current = new EmptyFleet();

        private readonly object loadLock = new();
        private readonly object serviceLockCollectionAccessLock = new();
        private readonly ObservableCollection<Vessel> vesselCollection = [];
        private readonly Dictionary<Type, object> serviceAccessLocks = [];
        private bool loaded;
        private bool linked;
        private bool disposedValue;
        private ISettingsProvider? settingsProvider;
        private ILogger? logger;


        public event EventHandler? OnFleetPopulated;
        public event EventHandler? FleetLoaded;
        public event PropertyChangedEventHandler? PropertyChanged;


        public static Fleet Current
        {
            get => current;
            private set
            {
                if (current is not EmptyFleet)
                    throw new FleetInitalizationException("SARDINE initialization is already complete.");

                current = value;
            }
        }

        private Dictionary<Type, object> Services { get; } = [];
        public DependencyGraph DependencyGraph => new([.. VesselCollection]);
        public int Count => ((IReadOnlyCollection<Vessel>)VesselCollection).Count;
        public IReadOnlyList<Type> RegisteredServiceTypes => [.. Services.Keys];
        

        public ObservableCollection<Vessel> VesselCollection
        {
            get
            {
                if (!loaded)
                    Populate();

                return vesselCollection;
            }
        }

        public ISettingsProvider SettingsProvider
        {
            get
            {
                settingsProvider ??= new DefaultSettingsProvider();
                return settingsProvider;
            }
            private init => settingsProvider = value;
        }

        public ILogger Logger
        {
            get
            {
                logger ??= new DefaultLogger();
                return logger;
            }

            private init => logger = value;
        }


        public static void Start<T>(ISettingsProvider? settingsProvider = null, ILogger? logger = null) where T : Fleet, new()
        {
            T aggregator = new()
            {
                SettingsProvider = settingsProvider ?? new DefaultSettingsProvider(),
                Logger = logger ?? new DefaultLogger(),
            };

            foreach (var kvp in Current.Services)
                aggregator.Services[kvp.Key] = Current.Services[kvp.Key];
            
            Current = aggregator;

            aggregator.Link();
            aggregator.Logger.StartLogger(aggregator);
        }



        public T Get<T>() where T : class, new()
        {
            object lockObject;
            lock (serviceLockCollectionAccessLock)
            {
                if (!serviceAccessLocks.ContainsKey(typeof(T)))
                    serviceAccessLocks[typeof(T)] = new object();

                lockObject = serviceAccessLocks[typeof(T)];
            }

            lock (lockObject)
            {
                if (Services.TryGetValue(typeof(T), out object? service))
                    return (T)service;

                T newService = new();
                Services[typeof(T)] = newService;
                return newService;
            }
        }

        public void Reload()
        {
            foreach (Vessel vessel in  VesselCollection)
                vessel.Reload();
        }

        public void Repair()
        {
            foreach (Vessel vessel in VesselCollection)
            {
                if (!vessel.IsOnline)
                    vessel.Reload();
            }
        }

        public void Invalidate()
        {
            foreach (Vessel vessel in VesselCollection)
                vessel.Invalidate();
        }

        public void Link()
        {
            foreach (Vessel vessel in VesselCollection)
                vessel.Link();

            linked = true;
        }

        public void Unlink()
        {
            if (!loaded)
                Populate();

            foreach (Vessel vessel in VesselCollection)
                vessel.Unlink();

            linked = false;
        }

        public void Activate()
        {
            if (!linked)
                return;

            foreach (Vessel vessel in VesselCollection)
                vessel.IsActive = true;
        }

        public void Stop()
        {
            if (!linked)
                return;

            foreach (Vessel vessel in VesselCollection)
                vessel.IsActive = false;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IEnumerator<Vessel> GetEnumerator() => ((IEnumerable<Vessel>)VesselCollection).GetEnumerator();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (Vessel vessel in VesselCollection.Reverse())
                        vessel.Invalidate();
                }

                disposedValue = true;
            }
        }

        private void Populate()
        {
            if (loaded)
                return;

            lock (loadLock)
            {
                loaded = true;

                Vessel[] vesselBase = GetType().GetProperties()
                                               .Where((x) => x.PropertyType.BaseType == typeof(Vessel))
                                               .Select((x) => (Vessel)(x.GetValue(this)
                                                                       ?? throw new FleetInitalizationException($"Failed to acquire Vessel {x.Name} of type {x.PropertyType}.")))
                                               .ToArray();

                string[] vesselNames = GetType().GetProperties()
                                                .Where((x) => x.PropertyType.BaseType == typeof(Vessel))
                                                .Select((x) => x.Name)
                                                .ToArray();


                for (int i = 0; i < vesselBase.Length; i++)
                {
                    vesselBase[i].Reloaded += Vessel_PropertyChanged;

                    if (string.IsNullOrEmpty(vesselBase[i].Name))
                        vesselBase[i].Name = vesselNames[i];
                }

                GenerateLoadOrderArray([.. vesselBase]);
                OnFleetPopulated?.Invoke(this, EventArgs.Empty);
            }
        }

        private void GenerateLoadOrderArray(List<Vessel> vessels)
        {
            List<DependencyNode> nodes = [.. new DependencyGraph(vessels, linkOnly: true).Nodes];
            Vessel[] orderedLoadArray = new Vessel[nodes.Count];

            nodes.Sort((x, y) => x.NodeLevel - y.NodeLevel);

            for (int i = 0; i < orderedLoadArray.Length; i++)
                orderedLoadArray[i] = nodes[i].Vessel;

            vesselCollection.Clear();

            for (int i = 0; i < orderedLoadArray.Length; i++)
                vesselCollection.Add(orderedLoadArray[i]);

            FleetLoaded?.Invoke(this, EventArgs.Empty);
            OnPropertyChanged(nameof(VesselCollection));
        }

        private void OnPropertyChanged(string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void Vessel_PropertyChanged(object? sender, EventArgs e) => OnPropertyChanged(nameof(VesselCollection));
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)VesselCollection).GetEnumerator();
    }
}