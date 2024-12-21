using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Sardine.Core.Views.WPF
{
    /// <summary>
    /// Interaction logic for FleetLoadingWindow.xaml
    /// </summary>
    public partial class FleetLoadingWindow : Window, INotifyPropertyChanged
    {
        private string currentVesselOnLoad = string.Empty;
        private int totalCount = 1;
        readonly Thread loaderThread;
        private int currentCount;


        public string CurrentVesselOnLoad
        {
            get => currentVesselOnLoad;

            set
            {
                currentVesselOnLoad = value;
                OnPropertyChanged();
            }
        }

        public int TotalCount
        {
            get => totalCount;

            set
            {
                totalCount = value;
                OnPropertyChanged();
            }
        }

        public int CurrentCount
        {
            get => currentCount;

            set
            {
                currentCount = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;


        public FleetLoadingWindow()
        {
            InitializeComponent();
            DataContext = this;
            loaderThread = new Thread(Loader);
        }


        public void Load()
        {
            loaderThread.Start();
        }

        void Loader()
        {
            TotalCount = Fleet.Current.VesselCollection.Count;

            if (TotalCount == 0)
            {
                Dispatcher.Invoke(() => Close());
                return;
            }

            CurrentCount = 1;

            foreach (Vessel vessel in Fleet.Current.VesselCollection)
            {
                CurrentVesselOnLoad = vessel.DisplayName;

                vessel.Reload();
                CurrentCount++;
            }

            Dispatcher.Invoke(() => Close());
        }

        void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
