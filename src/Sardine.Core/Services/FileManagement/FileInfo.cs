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


        public event EventHandler<SardineFileTypeChangedEventArgs>? SardineFileTypeFound;


        internal FileInfo(string? fileLocation)
        {
            Location = fileLocation;
            Name = Path.GetFileNameWithoutExtension(fileLocation) ?? string.Empty;
            Extension = Path.GetExtension(fileLocation) ?? string.Empty;
            new Task(() => { FileType = FileManager.GetFileType(fileLocation); SardineFileTypeFound?.Invoke(this, new SardineFileTypeChangedEventArgs(FileType)); }).Start();
        }


        public override string ToString() => $"[{FileType?.Description ?? "Loading type.."}] - {Name}";
        public FileStream? GetStream() => FileManager.GetStream(this);

        public void Handle(object? sender)
        {
            if (Handler is not null)
                _ = Handler.Handle(sender, this);
        }
    }
}
