using System.Windows;

namespace Sardine.Core.Views.WPF
{
    public sealed class VesselPropertyToEventDependencyObject : DependencyObject, IVesselPropertyToEventLink
    {

        public static readonly DependencyProperty PropNameProperty = DependencyProperty.Register(
            nameof(PropertyName),
            typeof(string),
            typeof(VesselPropertyToEventDependencyObject),
            new FrameworkPropertyMetadata(string.Empty));

        // .NET Property wrapper
        public string PropertyName
        {
            get { return (string)GetValue(PropNameProperty); }
            set { SetValue(PropNameProperty, value); }
        }

        public static readonly DependencyProperty PropEventProperty = DependencyProperty.Register(
            nameof(EventName),
            typeof(string),
            typeof(VesselPropertyToEventDependencyObject),
            new FrameworkPropertyMetadata(string.Empty));

        // .NET Property wrapper
        public string EventName
        {
            get { return (string)GetValue(PropEventProperty); }
            set { SetValue(PropEventProperty, value); }
        }

    }
}
