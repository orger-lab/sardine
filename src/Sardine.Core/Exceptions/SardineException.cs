namespace Sardine.Core.Exceptions
{
    [Serializable]
    public class SardineException : Exception
    {
        public SardineException() { }

        public SardineException(string? message) : base(message) { }

        public SardineException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}