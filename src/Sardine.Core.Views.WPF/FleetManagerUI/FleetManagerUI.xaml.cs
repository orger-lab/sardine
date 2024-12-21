using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Sardine.Core.Views.WPF
{
    /// <summary>
    /// Interaction logic for FleetManagerUI.xaml
    /// </summary>
    public partial class FleetManagerUI : UserControl
    {
        public string Title { get; }
        public SardineWindow? SardineWindow { get; set; }


        public FleetManagerUI()
        {
            InitializeComponent();
            DataContext = this;
            Title = SardineInfo.FleetName;
        }


        private static void ReloadVessel(Vessel vessel)
        { 
            new Task(() =>
            {
                vessel.Reload();
            }).Start();
        }

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                SardineWindow?.OpenVesselUI(((Vessel)((DockPanel)sender).Tag));
        }

        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            SardineWindow?.OpenVesselUI((Vessel)((MenuItem)sender).DataContext);
        }

        private void MenuItem_Reload_Click(object sender, RoutedEventArgs e)
        {
            ReloadVessel((Vessel)((MenuItem)sender).DataContext);
        }

        private void Button_Reload_Click(object sender, RoutedEventArgs e)
        {
            ReloadVessel((Vessel)((Button)sender).DataContext);
        }

        private void Button_Activate_Click(object sender, RoutedEventArgs e)
        {
            Vessel vessel = (Vessel)((Button)sender).DataContext;
            vessel.IsActive = !vessel.IsActive;
        }
    }
}
