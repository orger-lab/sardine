using Sardine.Core.Utils.Reflection;

namespace Sardine.Core.FileManagement
{
    public static class FileManager
    {
        public static IReadOnlyDictionary<Type, FileType> KnownSardineFileTypes { get; } = GetKnownFileTypesList();
        public static IDictionary<Type, IFileHandler> FileHandlers { get; } = GetFileHandlersList();
        

        public static byte[] Load(FileInfo? file) => file?.Location is null ? [] : System.IO.File.ReadAllBytes(file.Location);

        public static void Save(byte[] fileData, string name, FileType fileType, string? location = null)
        {
            if (fileType is null)
                return;

            File.WriteAllBytes(name, fileData);
        }

        public static FileType GetFileType(string? fileLocation)
        {
            if (fileLocation is null)
                return FileType.InvalidFileType;

            foreach (FileType fileType in KnownSardineFileTypes.Values)
            {
                    using (FileStream stream = File.OpenRead(fileLocation))
                    {
#pragma warning disable CA1031 // Do not catch general exception types
                        try
                        {
                            if (fileType.CheckFileValidity(stream))
                                return fileType;
                        }
                        catch { }
#pragma warning restore CA1031 // Do not catch general exception types

                    }
            }

            return FileType.InvalidFileType;
        }

        public static IFileHandler? GetHandler(FileType? fileType)
        {
            if (fileType is null)
                return null;

            _ = FileHandlers.TryGetValue(fileType.GetType(), out IFileHandler? handler);

            return handler;
        }

        internal static FileStream? GetStream(FileInfo file) => file.Location is null ? null : System.IO.File.OpenRead(file.Location);

        private static Dictionary<Type, IFileHandler> GetFileHandlersList()
        {
            Type[] types = AssemblyInformation.KnownTypes.Where(x => x.InheritsFrom(typeof(IFileHandler))).ToArray();

            types = types.Where((x) => x.GetConstructor([]) is not null && x.IsAbstract == false).ToArray();

            Dictionary<Type, IFileHandler> dict = [];

            foreach (Type t in types)
                dict.TryAdd(t.BaseType!.GenericTypeArguments[0], (IFileHandler)Activator.CreateInstance(t)!);

            return dict;
        }

        private static Dictionary<Type, FileType> GetKnownFileTypesList()
        {
            Dictionary<Type, FileType> sftl = [];
            IEnumerable<Type> types = AssemblyInformation.KnownTypes.Where(x => x.InheritsFrom(typeof(FileType)) && x.GetConstructor(Type.EmptyTypes) is not null && !x.IsAbstract);

            foreach (Type t in types)
                sftl[t] = (FileType)Activator.CreateInstance(t)!;

            return sftl;
        }
    }
}