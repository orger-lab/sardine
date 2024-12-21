namespace Sardine.Core.Helpers
{
    public interface IUIHelper
    {
        public string Name { get; }
        public bool ShowSeparator { get; }
        public string? Description { get; }
        public int OrderingIndex { get; }
        public object? IconSource { get; }
        public IReadOnlyList<UIHelperAction> Actions { get; }
    }
}