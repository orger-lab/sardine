namespace Sardine.Core.Settings
{
    public interface ISettingsProvider
    {
        public (string Attribute, string Value)? FetchSetting(params string[]? path);
        public (string Attribute, string Value)[] FetchSettings(params string[]? path);
    }
}
