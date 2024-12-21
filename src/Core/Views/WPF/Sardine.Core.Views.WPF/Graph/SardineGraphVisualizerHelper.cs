using Sardine.Core.Views.WPF.UIHelpers;
using System.Collections.Generic;

namespace Sardine.Core.Views.Graph.WPF
{
    public sealed class SardineGraphVisualizerHelper : IUIHelperProvider
    {
        public string Name { get; } = "Graph";
        public UIHelperMetadata? Metadata { get; }
        public IReadOnlyList<UIHelper> Actions { get; } =
        [
            new UIHelper("Show", () => new SardineGraphVisualizer(Fleet.Current.DependencyGraph).ShowDialog()),
                    new UIHelper("Save as..", () => SardineGraphVisualizer.SaveImageAs(Fleet.Current.DependencyGraph)),
        ];
    }
}
