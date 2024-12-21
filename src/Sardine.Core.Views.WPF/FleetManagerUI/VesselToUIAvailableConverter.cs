using System;
using System.Globalization;
using System.Windows.Data;

namespace Sardine.Core.Views.WPF
{
    public sealed class VesselToUIAvailableConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is not null && values.Length > 0 && values[0] is Vessel vessel)
                return VesselUIProvider.CanProvideUI(vessel);

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
