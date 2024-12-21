namespace Sardine.Core.Logger
{
    public class OnNewLogMessageEventArgs : EventArgs
    {
        public LogMessage LogMessage { get; }

        public OnNewLogMessageEventArgs(LogMessage logMessage)
        {
            LogMessage = logMessage;
        }   
    }
}