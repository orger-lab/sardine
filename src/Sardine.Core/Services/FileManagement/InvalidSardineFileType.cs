namespace Sardine.Core.FileManagement
{
    public sealed class InvalidSardineFileType : FileType
    {
        internal InvalidSardineFileType() : base(string.Empty, "Invalid File") { }
    }
}
