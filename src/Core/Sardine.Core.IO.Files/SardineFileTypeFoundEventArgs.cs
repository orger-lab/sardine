namespace Sardine.Core.IO.Files
{
    public class SardineFileTypeChangedEventArgs(FileType fileType) : EventArgs
    {
        public FileType FileType { get; } = fileType;
    }
}