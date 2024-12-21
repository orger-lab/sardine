using Sardine.Core.Utils.Reflection;

namespace Sardine.Core
{
    public static class SardineInfo
    {
        private const string SARDINE_FOLDER = "SARDINE";
        private const string BASELOCATION_SETTING_PATH = "UserDataPath";
        private const string NAME_SETTING_PATH = "Name";
        private const string CONTACT_EMAIL = "lucas.martins@neuro.fchampalimaud.org";


        private static readonly string defaultLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                         SARDINE_FOLDER,
                                                         AssemblyInformation.EntryAssemblyName,
                                                         AssemblyInformation.EntryAssemblyVersion.Replace(".", string.Empty, StringComparison.InvariantCulture));

        private static string? fleetVersion;
        private static string? sardineVersion;
        private static string? baseLocation;
        private static string? fleetName;


        public static string ContactPoint => CONTACT_EMAIL;

        public static string BaseLocation
        {
            get
            {
                baseLocation ??= Fleet.Current.SettingsProvider.FetchSetting(BASELOCATION_SETTING_PATH)?.Value ?? DefaultLocation;
                return baseLocation;
            }
        }

        public static string FleetName
        {
            get
            {
                fleetName ??= Fleet.Current.SettingsProvider.FetchSetting(NAME_SETTING_PATH)?.Value ?? AssemblyInformation.EntryAssemblyName;
                return fleetName;
            }
        }

        public static string DefaultLocation
        {
            get
            {
                if (!Directory.Exists(defaultLocation))
                {
                    _ = Directory.CreateDirectory(defaultLocation);
                }

                return defaultLocation;
            }
        }

        public static string FleetVersion
        {
            get
            {
                fleetVersion ??= AssemblyInformation.EntryAssemblyVersion;
                return fleetVersion;
            }
        }

        public static string SardineVersion
        {
            get
            {
                sardineVersion ??= AssemblyInformation.CallingAssemblyVersion;
                return sardineVersion;
            }
        }
    }
}