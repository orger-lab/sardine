using System.Diagnostics;
using System.Windows;

namespace Sardine.Core.Views.WPF
{

    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }


        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var ps = new ProcessStartInfo(@$"mailto:{SardineInfo.ContactPoint}?subject=%5BSARDINE%5D%20-%20")
            {
                UseShellExecute = true,
                Verb = "open"
            };
            Process.Start(ps);
            e.Handled = true;
        }
    }
}
