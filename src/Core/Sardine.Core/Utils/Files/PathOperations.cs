namespace Sardine.Core.Utils.Files
{
    public static class PathOperations
    {
        public static string AddExtensionIfMissing(string path, string extension) => Path.HasExtension(path) ? path : Path.ChangeExtension(path, extension);
    }
}