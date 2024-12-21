using Sardine.Core.Views.WPF.UIHelpers;
using System.Collections.Generic;

namespace Sardine.Core.Views.Logs.WPF
{
    public sealed class LoggerWindowHelper : IUIHelperProvider
    {
        public string Name { get; } = "Logger";

        public IReadOnlyList<UIHelper> Actions
        {
            get;
        } = [new UIHelper("Logger", () => new LoggerWindow().ShowDialog())];

        public UIHelperMetadata Metadata { get; } = new() { Description = "Shows SARDINE logging output" };
    }
}
