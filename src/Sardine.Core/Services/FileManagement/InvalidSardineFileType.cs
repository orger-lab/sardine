namespace Sardine.Core.FileManagement
{
    public sealed class InvalidSardineFileType : FileType
    {
        internal InvalidSardineFileType() : base(Array.Empty<string>(), "Invalid File") { }
    }
}
