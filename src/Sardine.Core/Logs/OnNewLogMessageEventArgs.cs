namespace Sardine.Core.Logs
{
    public sealed class OnNewLogMessageEventArgs(LogMessage logMessage) : EventArgs
    {
        public LogMessage LogMessage { get; } = logMessage;
    }
}