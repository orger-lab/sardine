using Sardine.Core.Utils.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;


namespace Sardine.Core.Views.WPF
{
    /// <summary>
    /// Interaction logic for VesselUIContainer.xaml
    /// </summary>
    public partial class VesselUIContainer : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty VesselTypeProperty = DependencyProperty.Register(nameof(UIType), typeof(Type), typeof(VesselUIContainer));

        public static readonly DependencyProperty VesselProperty
            = DependencyProperty.Register(
                  nameof(Vessel),
                  typeof(Vessel),
                  typeof(VesselUIContainer),
                  new PropertyMetadata(null, (o, e) => { ((VesselUIContainer)o).vesselUIs = null; ((VesselUIContainer)o).RebuildContainerContent(); })
              );


        private (VesselUserControl, Type)[]? vesselUIs;


        public IList<(VesselUserControl VesselUI,Type UIType)> VesselUIs
        {
            get
            {
                vesselUIs ??= GetVesselUIs();
                return vesselUIs;
            }
        }

        public Type? UIType
        {
            get => (Type)GetValue(VesselTypeProperty);
            set => SetValue(VesselTypeProperty, value);
        }

        public Vessel Vessel
        {
            get => (Vessel)GetValue(VesselProperty);
            set => SetValue(VesselProperty, value);
        }


        public event PropertyChangedEventHandler? PropertyChanged;


        public VesselUIContainer()
        {
            InitializeComponent();
        }


        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private (VesselUserControl, Type)[] GetVesselUIs()
        {
            if (Vessel is null)
                return [];

            IReadOnlyList<(VesselUserControl VesselUI, Type UIType)> userControls = VesselUIProvider.ProvideUI(Vessel, (UIType is null) ? (null) : (x) => x == UIType);

            if (userControls is null)
                return [];

            if (userControls.Count == 0)
                return [];

            return userControls.Select((x) => (x.VesselUI, x.UIType)).ToArray();
        }

        void RebuildContainerContent()
        {
            DataContext = Vessel;

            if (VesselUIs.Count == 1)
            {
                StackPanel_UIHolder.Children.Add(VesselUIs[0].VesselUI);
                StackPanel_UIHolder.Height = VesselUIs[0].VesselUI.Height;
                StackPanel_UIHolder.Width = VesselUIs[0].VesselUI.Width;
                return;
            }

            TabControl tabControl = new();


            double height = 0;
            double width = 0;

            foreach ((VesselUserControl x, Type type) in VesselUIs)
            {
                MenuItem mItem = new()
                {
                    Header = "Show only this interface",
                };

                mItem.Click += (_, _) => { UIType = type; vesselUIs = null; StackPanel_UIHolder.Children.Clear(); RebuildContainerContent(); };

                TextBlock textBlock = new()
                {
                    Text = StringOperations.FilterToNCharacters(type.Name, 20, showTooLargeIdentifier: true),
                    ContextMenu = new ContextMenu() ,
                };

                textBlock.ContextMenu.Items.Add(mItem);

                TabItem item = new()
                {
                    Content = x,
                    Header = textBlock,
                };

                tabControl.Items.Add(item);
                height = Math.Max(height, x.Height);
                width = Math.Max(width, x.Width);
            }

            StackPanel_UIHolder.Children.Add(tabControl);
            StackPanel_UIHolder.Height = height+ ((TabItem)tabControl.Items[0]).Height;
            StackPanel_UIHolder.Width = width;
        }
    }
}
