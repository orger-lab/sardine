using Sardine.Core.FileManagement;
using Sardine.Core.Utils.Reflection;
using System.IO;
using System.Xml.Linq;

namespace Sardine.Core.Views.WPF.LayoutManagement
{
    public sealed class VesselUILayoutFileType : FileType
    {
        public VesselUILayoutFileType() : base(".xml", "UI Layout Information") { }

        public override bool CheckFileValidity(FileStream stream)
        {
            XDocument document = XDocument.Load(stream);
            if (document.Root?.Name != "SardineUILayout")
                return false;

            if (document.Root?.Attribute("AggregatorType")?.Value != AssemblyInformation.EntryAssemblyName)
                return false;

            return true;
        }
    }
}
