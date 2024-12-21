using System.Collections.Generic;

namespace Sardine.Core.Views.WPF.UIHelpers
{
    public interface IUIHelperProvider
    {
        public string Name { get; }
        UIHelperMetadata? Metadata { get; }
        public IReadOnlyList<UIHelper> Actions { get; }
    }
}