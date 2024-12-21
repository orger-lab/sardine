namespace Sardine.Core.IO.Files
{
    public sealed class FileProvider
    {
        private IReadOnlyList<FileInfo> fileList = [];


        public IReadOnlyList<FileInfo> FileList => fileList;


        public void ReloadFileList(string? location = null)
        {
            string[] files = Directory.GetFiles(location ?? SardineInfo.BaseLocation);
            
            fileList = files.Select((x) => new FileInfo(x)).ToList().AsReadOnly(); 
        }
    }
}
