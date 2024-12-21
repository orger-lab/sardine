namespace Sardine.Core.FileManagement
{
    public class SardineFileTypeChangedEventArgs(FileType fileType) : EventArgs
    {
        public FileType FileType { get; } = fileType;
    }
}