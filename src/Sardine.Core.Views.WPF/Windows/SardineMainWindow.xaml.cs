using System.Windows;
using System.Resources;
[assembly: NeutralResourcesLanguage("en")]

namespace Sardine.Core.Views.WPF
{
    public partial class SardineMainWindow : SardineWindow
    {
        internal SardineMainWindow() : base()
        {
            InitializeComponent();
        }


        private void SardineMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HelperMenuBar.SardineWindow = this;
            ManagerUI.SardineWindow = this;
        }
    }
}