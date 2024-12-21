namespace Sardine.Core.Exceptions
{
    [Serializable]
    public sealed class VesselException : Exception
    {
        public VesselException() { }

        public VesselException(string? message) : base(message) { }

        public VesselException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}