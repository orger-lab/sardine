using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Sardine.Core.Views.WPF
{
    /// <summary>
    /// Interaction logic for VesselUIWindow.xaml
    /// </summary>
    public partial class VesselUIWindow : Window, INotifyPropertyChanged
    {
        private bool lockPosition = true;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Vessel Vessel { get => UIContainer.Vessel; set => UIContainer.Vessel = value; }

        public bool Locked
        {
            get
            {
                return lockPosition;
            }

            set
            {
                if (lockPosition != value)
                {
                    lockPosition = value;

                    if (!lockPosition)
                        UIContainer.MouseDown += VesselUIContainer_MouseDown;
                    else
                        UIContainer.MouseDown -= VesselUIContainer_MouseDown;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Locked)));
                }
            }
        }


        public VesselUIWindow()
        {
            InitializeComponent();
            DataContext = this;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var helper = new WindowInteropHelper(this).Handle;
            // Performing some magic to hide the form from Alt+Tab
            _ = NativeMethods.SetWindowLong(helper, NativeMethods.GWL_EX_STYLE, (NativeMethods.GetWindowLong(helper, NativeMethods.GWL_EX_STYLE) | NativeMethods.WS_EX_TOOLWINDOW) & ~NativeMethods.WS_EX_APPWINDOW);
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Close();
        }

        private void VesselUIContainer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
