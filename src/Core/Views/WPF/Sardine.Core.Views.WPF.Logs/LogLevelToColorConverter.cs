using Sardine.Core.Logs;
using System.Windows.Data;
using System.Windows.Media;

namespace Sardine.Core.Views.Logs.WPF
{
    public sealed class LogLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not LogLevel)
                throw new InvalidCastException();

            return (LogLevel)value switch
            {
                LogLevel.Emergency => new SolidColorBrush(Colors.DarkRed),
                LogLevel.Alert => new SolidColorBrush(Colors.OrangeRed),
                LogLevel.Critical => new SolidColorBrush(Colors.Orange),
                LogLevel.Error => new SolidColorBrush(Colors.DarkGoldenrod),
                LogLevel.Warning => new SolidColorBrush(Colors.BlueViolet),
                LogLevel.Notice => new SolidColorBrush(Colors.DarkGreen),
                LogLevel.Information => new SolidColorBrush(Colors.RoyalBlue),
                LogLevel.Debug => new SolidColorBrush(Colors.DimGray),
                _ => throw new ArgumentOutOfRangeException(nameof(value)),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
