using Sardine.Core.Logs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sardine.Core.Views.WPF
{
    public static class LoggerExtensions
    {
        public static void Log<T>(this VesselUserControl<T>? caller, LogMessage message)
        {
            Fleet.Current.Logger?.Log(message, caller?.Vessel);
        }

        public static void Log<T>(this VesselUserControl<T>? caller, string message, LogLevel level = LogLevel.Information)
        {
            Fleet.Current.Logger?.Log(new LogMessage(message, level), caller?.Vessel);
        }
    }
}
