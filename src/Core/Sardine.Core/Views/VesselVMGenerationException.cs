namespace Sardine.Core.Views
{
    [Serializable]
    public sealed class VesselVMGenerationException : Exception
    {
        public VesselVMGenerationException() { }

        public VesselVMGenerationException(string? message) : base(message) { }

        public VesselVMGenerationException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}