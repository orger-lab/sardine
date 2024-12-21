namespace Sardine.Core.FileManagement
{
    public abstract class FileType
    {
        public IReadOnlyList<string> Extension { get; }
        public string Description { get; }
        public static InvalidSardineFileType InvalidFileType { get; } = new();


        protected FileType(string extension, string description) : this(new string[1] { extension }, description) { }

        protected FileType(string[] extension, string description)
        {
            Extension = extension;
            Description = description;
        }


        public static FileType OfType<T>()
        {
            if (FileManager.KnownSardineFileTypes.TryGetValue(typeof(T), out FileType? value))
                return value;

            return InvalidFileType;
        }

        public virtual bool CheckFileValidity(FileStream stream) => false;
    }
}
