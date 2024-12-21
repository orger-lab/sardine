namespace Sardine.Core.Logger
{
    public class LogMessage
    {
        public string Message { get; }
        public string Source { get; internal set; } = "SARDINE";
        public LogLevel Level { get; }
        public DateTime Timestamp { get; }
        public override string ToString() => $"{Timestamp:HH:mm:ss} [{Source} ({Level})] - {Message}";

        public LogMessage(string message, LogLevel level = LogLevel.Information)
        {
            Message = message.Trim('\n','\r');
            Level = level;
            Timestamp = DateTime.Now;
        }
    }
}