using Sardine.Core.Views.WPF.UIHelpers;
using System.Collections.Generic;

namespace Sardine.Core.Views.WPF.LayoutManagement
{
    public sealed class VesselUILayoutToFile : IUIHelperProvider
    {
        public string Name { get; } = "File";

        public IReadOnlyList<UIHelper> Actions { get; }
        =
        [
            new UIHelper("Save Layout", VesselUIPersistenceService.Save),
        ];

        public UIHelperMetadata? Metadata { get; }
    }
}
