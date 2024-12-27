namespace Sardine.Core.FileManagement
{
    public class FileInfo
    {
        private IFileHandler? Handler => FileManager.GetHandler(FileType);
        public string? Location { get; }
        public string Name { get; }
        public string Extension { get; }
        public static FileInfo EmptyItem { get; } = new FileInfo(null);
        public bool HasHandler => Handler is not null;
        public FileType? FileType { get; private set; }

        public FileInfo(string? fileLocation)
        {
            Location = fileLocation;
            Name = Path.GetFileNameWithoutExtension(fileLocation) ?? string.Empty;
            Extension = Path.GetExtension(fileLocation) ?? string.Empty;
            FileType = FileManager.GetFileType(fileLocation);
        }

        public FileStream? GetStream() => FileManager.GetStream(this);

        public void Handle(object? sender)
        {
            if (Handler is not null)
                new Task(() => _ = Handler.Handle(sender, this)).Start();
        }
    }
}
