namespace Sardine.Core.DataModel
{
    public sealed class QueueHandlingFailureEventArgs(Exception exception) : EventArgs
    {
        public Exception Exception { get; } = exception;
    }
}
