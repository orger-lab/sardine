using Sardine.Core.Views.WPF.UIHelpers;
using System.Collections.Generic;

namespace Sardine.Core.Views.WPF
{
    public sealed class FleetManagerMainHelper : IUIHelperProvider
    {
        public string Name { get; } = "Fleet";
        public UIHelperMetadata Metadata { get; } = new UIHelperMetadata() { OrderingIndex = 9999 };

        public IReadOnlyList<UIHelper> Actions { get; } =
        [
                new UIHelper("Reload All", () => Fleet.Current.Reload()),
                new UIHelper("Stop All", () => Fleet.Current.Stop()),
                new UIHelper("Activate All", () => Fleet.Current.Activate())
        ];
    }
}
