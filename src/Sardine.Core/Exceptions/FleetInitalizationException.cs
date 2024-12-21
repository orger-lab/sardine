namespace Sardine.Core.Exceptions
{
    [Serializable]
    public sealed class FleetInitalizationException : Exception
    {
        public FleetInitalizationException()
        {
        }

        public FleetInitalizationException(string? message) : base(message)
        {
        }

        public FleetInitalizationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}