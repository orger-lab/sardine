using Sardine.Core.FileManagement;
using System.IO;

namespace Sardine.Core.Views.WPF.LayoutManagement
{
    public sealed class VesselULayoutFileHandler : SardineFileHandler<VesselUILayoutFileType, SardineWindow>
    {
        public override string Name { get; } = "VesselUI handler";
        protected override bool Handle(SardineWindow? sender, FileStream stream) => VesselUIPersistenceService.Load(sender, stream);
    }
}
