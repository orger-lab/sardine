using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Sardine.Core.Views.WPF
{
    public abstract class VesselUserControl : UserControl, IDisposable, INotifyPropertyChanged
    {
        public static readonly DependencyProperty UpdateOnlyWithEventsProperty =
DependencyProperty.Register(nameof(UpdateOnlyWithEvents), typeof(bool),
typeof(VesselUserControl<>), new FrameworkPropertyMetadata(false));

        // .NET Property wrapper
        public bool UpdateOnlyWithEvents
        {
            get { return (bool)GetValue(UpdateOnlyWithEventsProperty); }
            set { SetValue(UpdateOnlyWithEventsProperty, value); }
        }


        public static readonly DependencyProperty LinkedVesselNameProperty =
DependencyProperty.Register(nameof(LinkedVesselName), typeof(string),
typeof(VesselUserControl<>), new FrameworkPropertyMetadata(null));

        // .NET Property wrapper
        public string? LinkedVesselName
        {
            get { return (string)GetValue(LinkedVesselNameProperty); }
            set
            {
                if (LinkedVesselName != value)
                {
                    SetValue(LinkedVesselNameProperty, value);
                    SetVessel(Fleet.Current.VesselCollection.Where(x => x.Name == value).First());
                }
            }
        }

        public static readonly DependencyProperty UpdaterIntervalProperty =
DependencyProperty.Register(nameof(UpdaterInterval), typeof(int),
typeof(VesselUserControl<>), new FrameworkPropertyMetadata(1000));

        // .NET Property wrapper
        public int UpdaterInterval
        {
            get { return (int)GetValue(UpdaterIntervalProperty); }
            set { SetValue(UpdaterIntervalProperty, value); }
        }


        public static readonly DependencyProperty UpdateOnlyValueTypePropertiesProperty =
DependencyProperty.Register(nameof(UpdateOnlyValueTypeProperties), typeof(bool),
typeof(VesselUserControl<>), new FrameworkPropertyMetadata(false));

        // .NET Property wrapper
        public bool UpdateOnlyValueTypeProperties
        {
            get { return (bool)GetValue(UpdateOnlyValueTypePropertiesProperty); }
            set { SetValue(UpdateOnlyValueTypePropertiesProperty, value); }
        }



        public static readonly DependencyProperty EventUpdatesCollectionProperty =
DependencyProperty.Register(nameof(EventUpdatesCollection), typeof(ObservableCollection<VesselPropertyToEventDependencyObject>),
typeof(VesselUserControl<>), new FrameworkPropertyMetadata(new ObservableCollection<VesselPropertyToEventDependencyObject>()));

        public abstract event PropertyChangedEventHandler? PropertyChanged;

        // .NET Property wrapper
        public ObservableCollection<VesselPropertyToEventDependencyObject> EventUpdatesCollection
        {
            get { return (ObservableCollection<VesselPropertyToEventDependencyObject>)GetValue(EventUpdatesCollectionProperty); }
            init { SetValue(EventUpdatesCollectionProperty, value); }
        }

        public Vessel? Vessel { get; private set; }


        protected VesselUserControl()
        {
            EventUpdatesCollection = [];
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposed = true)
        {
            if (Vessel is not null)
                Vessel.Reloaded -= Vessel_Reloaded;
        }

        public void LinkToVessel(Vessel vessel)
        {
            LinkedVesselName = vessel?.Name;
        }

        void SetVessel(Vessel vessel)
        {

            if (Vessel is not null)
                Vessel.Reloaded -= Vessel_Reloaded;

            Vessel = vessel;

            if (Vessel is null)
                return;

            Vessel.Reloaded += Vessel_Reloaded;

            if (Vessel.IsOnline)
                ReloadUIOnReloaded();
        }

        private void Vessel_Reloaded(object? sender, VesselReloadedEventArgs e)
        {
            if (e.IsOnline)
                ReloadUIOnReloaded();
        }

        protected abstract void ReloadUIOnReloaded();
    }


    public abstract class VesselUserControl<THandle> : VesselUserControl
    {
        private VesselViewModel<THandle>? viewModel;


        public string Title => Vessel?.DisplayName ?? "";
        public bool IsOnline => Vessel?.IsOnline ?? false;

        public THandle? Handle
        {
            get
            {
                if (Vessel is not null)
                    return (THandle?)(Vessel.ObjectHandle);

                return default;
            }
        }


        public override event PropertyChangedEventHandler? PropertyChanged;


        public VesselViewModel<THandle> ViewModel
        {
            get
            {
                viewModel ??= Fleet.Current.Get<VesselViewModelGenerator>().GetVesselViewModel<THandle>(Vessel!, UpdateOnlyWithEvents, UpdaterInterval, UpdateOnlyValueTypeProperties, EventUpdatesCollection.Select(x => (IVesselPropertyToEventLink)x).ToList());
                return viewModel;
            }
        }
        protected override void ReloadUIOnReloaded()
        {
            viewModel = null;
            Dispatcher.Invoke(() => { DataContext = ViewModel; });
            OnVesselReloadedAction();
        }

        public virtual void OnVesselReloadedAction() { }

        public virtual void OnVesselUIContentLoaded() { }

        public TOut? ExecuteCall<TOut>(Func<THandle, TOut> function)
        {
            if (Vessel is null)
                return default;

            return Vessel.ExecuteCall((object x) => function((THandle)x));
        }

        public bool ExecuteCall(Action<THandle> action)
        {
            return Vessel?.ExecuteCall((object x) => action((THandle)x)) ?? false;
        }

        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
