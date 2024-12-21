namespace Sardine.Core.Logs
{
    public interface ILogger
    {
        IReadOnlyList<LogMessage> LogHistory { get; }
        void ClearHistory();
        void StartLogger(Fleet aggregator);
        void Log(LogMessage message, object? source = null);


        public event EventHandler<OnNewLogMessageEventArgs>? OnNewLogMessage;
    }
}