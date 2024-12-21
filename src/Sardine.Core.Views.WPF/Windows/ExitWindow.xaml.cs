using System.Windows;

namespace Sardine.Core.Views.WPF
{
    /// <summary>
    /// Interaction logic for ExitWindow.xaml
    /// </summary>
    public partial class ExitWindow : Window
    {
        public ExitWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
