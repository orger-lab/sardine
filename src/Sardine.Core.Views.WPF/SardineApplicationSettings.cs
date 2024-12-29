using System;
using System.Globalization;
using System.IO;

namespace Sardine.Core.Views.WPF
{
    public static class SardineApplicationSettings
    {
        static bool? generateSardineWindow;

        public static bool GenerateSardineWindow
        {
            get
            {
                if (generateSardineWindow is null)
                {
                    string? value = Fleet.Current.SettingsProvider.FetchSetting("Application", "GenerateSardineWindow")?.Value;

                    if (value is not null)
                        generateSardineWindow = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                    else
                        generateSardineWindow = true;
                }

                return (bool)generateSardineWindow;
            }
        }

        public static object BorderBrushColor
        {
            get => Fleet.Current.SettingsProvider.FetchSetting("Application","Loader", "BorderBrush")?.Value ?? "OrangeRed";
        }

        public static object SplashImage
        {
            get
            {
                if (Fleet.Current.SettingsProvider.FetchSetting("Application", "Loader", "SplashImagePath")?.Value is string settingString)
                    return Path.GetFullPath(settingString);
        
                return "";
            }
        }

        public static object SardineWindowBackground
        {
            get => Fleet.Current.SettingsProvider.FetchSetting("Application", "Window", "Background")?.Value ?? "#222222";
        }
    }
}
