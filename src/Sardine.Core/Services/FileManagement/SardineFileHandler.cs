using Sardine.Core.Logs;

namespace Sardine.Core.FileManagement
{
    public abstract class SardineFileHandler<TFile, TSender> : SardineFileHandler<TFile> where TSender : class where TFile : FileType
    {
        protected abstract bool Handle(TSender? sender, FileStream stream);
        protected override bool Handle(object? sender, FileStream stream) => Handle(sender as TSender, stream);
    }

    public abstract class SardineFileHandler<T> : IFileHandler where T : FileType
    {
        public abstract string Name { get; }


        protected SardineFileHandler() { }


        public bool Handle(object? sender, FileInfo file)
        {
            if (file is null)
                return false;

            Fleet.Current.Logger.Log(new LogMessage($"Handling file {file.Name} using {Name}."), sender);

            if (file.FileType is not T)
                return false;

            bool result = false;

            using (FileStream? stream = file.GetStream())
            {
                if (stream is not null)
                    result = Handle(sender, stream);
            }

            return result;
        }

        protected abstract bool Handle(object? sender, FileStream stream);
    }
}
