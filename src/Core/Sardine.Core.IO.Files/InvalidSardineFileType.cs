namespace Sardine.Core.IO.Files
{
    public sealed class InvalidSardineFileType : FileType
    {
        internal InvalidSardineFileType() : base(Array.Empty<string>(), "Invalid File") { }
    }
}
