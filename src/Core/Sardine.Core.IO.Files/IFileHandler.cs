namespace Sardine.Core.IO.Files
{
    public interface IFileHandler
    {
        public bool Handle(object? sender, FileInfo file);
        public string Name { get; }
    }
}
