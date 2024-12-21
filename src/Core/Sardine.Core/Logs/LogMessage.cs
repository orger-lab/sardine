namespace Sardine.Core.Logs
{
    public class LogMessage(string? message, LogLevel level = LogLevel.Information)
    {
        public string Message { get; } = message?.Trim('\n', '\r') ?? string.Empty;
        public string Source { get; internal set; } = "SARDINE";
        public LogLevel Level { get; } = level;
        public DateTime Timestamp { get; } = DateTime.Now;
        

        public override string ToString() => $"{Timestamp:HH:mm:ss.fff} [{Source} ({Level})] - {Message}";
    }
}