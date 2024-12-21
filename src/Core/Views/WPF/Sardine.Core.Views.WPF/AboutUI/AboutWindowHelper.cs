using Sardine.Core.Views.WPF.UIHelpers;
using System.Collections.Generic;

namespace Sardine.Core.Views.WPF
{
    public sealed class AboutWindowHelper : IUIHelperProvider
    {
        public string Name { get; } = "Fleet";
        public UIHelperMetadata Metadata => new() { OrderingIndex = 9999 };

        public IReadOnlyList<UIHelper> Actions { get; } =
        [
            new UIHelper("About SARDINE..", (_) => new AboutWindow().ShowDialog())
        ];
    }
}
