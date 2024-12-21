namespace Sardine.Core.Logs
{
    public static class LoggingExtensions
    {
        public static void Log(this Vessel? caller, LogMessage message)
        {
            Fleet.Current.Logger?.Log(message, caller);
        }

        public static void Log(this Vessel? caller, string message, LogLevel level = LogLevel.Information)
        {
            Fleet.Current.Logger?.Log(new LogMessage(message, level), caller);
        }

        public static void Log(this Fleet? caller, LogMessage message)
        {
            Fleet.Current.Logger?.Log(message, caller);
        }

        public static void Log(this Fleet? caller, string message, LogLevel level = LogLevel.Information)
        {
            Fleet.Current.Logger?.Log(new LogMessage(message, level), caller);
        }
    }
}