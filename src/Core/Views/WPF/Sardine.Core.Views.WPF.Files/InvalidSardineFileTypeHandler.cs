using System.Diagnostics;
using System.IO;
using Sardine.Core.IO.Files;

namespace Sardine.Core.Views.Files.WPF
{
    public sealed class InvalidSardineFileTypeHandler : SardineFileHandler<InvalidSardineFileType>
    {
        public override string Name => "Default system handler";


        protected override bool Handle(object? sender, FileStream stream)
        {
            new Task(() =>
            {
                using Process fileopener = new();

                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + stream.Name + "\"";
                fileopener.Start();
            }
            ).Start();

            return false;
        }
    }
}
