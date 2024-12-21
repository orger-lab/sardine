using Sardine.Core.Exceptions;
using Sardine.Core.Utils.Text;
using System.Diagnostics;
using System.Xml.Linq;

namespace Sardine.Core.Settings
{
    public sealed class DefaultSettingsProvider : ISettingsProvider
    {
        private const string SARDINE_BASESETTINGS_FILENAME = "SARDINE.xml";
        private const string SARDINE_ROOT_ELEMENT = "SARDINE";


        private XElement Settings { get; } = new XElement(SARDINE_ROOT_ELEMENT);


        public DefaultSettingsProvider(string settingsPath = SARDINE_BASESETTINGS_FILENAME)
        {
            RegenerateBaseSettings(backup: false, settingsPath);

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                XDocument document = XDocument.Load(settingsPath);

                if (document.Root?.Name != SARDINE_ROOT_ELEMENT)
                    throw new SardineException($"Sardine settings file is invalid. Cannot find root element {SARDINE_ROOT_ELEMENT}");

                Settings = document.Root!;
            }
            catch
            {
                Trace.WriteLine("Couldn't properly load base settings file. Reverting to default configurations.");
                RegenerateBaseSettings(backup: true, settingsPath);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }


        public (string Attribute, string Value)? FetchSetting(params string[]? path) => FetchSettings(path).FirstOrDefault();

        public (string Attribute, string Value)[] FetchSettings(params string[]? path)
        {
            path ??= [];

            if (path.Length == 0)
                return [];

            XElement[] searchGroup = [Settings];
            IEnumerable<XElement>? searchGroupFinal;
            IEnumerable<XElement> searchResult;

            for (int i = 0; i < path.Length - 1; i++)
            {
                searchGroupFinal = [];

                foreach (XElement element in searchGroup)
                {
                    searchResult = element.Elements().Where(x => x.Name == path[i]);
                    searchGroupFinal = searchGroupFinal.Concat(searchResult);
                }

                searchGroup = searchGroupFinal.ToArray();

                if (searchGroup.Length == 0)
                    return [];
            }

            return searchGroup.Select(x => x.Attribute(path[^1]))
                              .Where(x => x is not null)
                              .Select(x => (x!.Name.LocalName, x!.Value))
                              .ToArray();
        }

        private static void RegenerateBaseSettings(bool backup, string settingsPath)
        {
            if (backup)
            {
                if (File.Exists(settingsPath))
                {
                    string supportName = $"{settingsPath}.{StringOperations.RandomString(8)}.bak";
                    File.Move(settingsPath, supportName);
                    Console.WriteLine($"Old settings file moved to {supportName}");
                }
            }

            if (!File.Exists(settingsPath))
            {
                using (StreamWriter fs = new(settingsPath))
                {
                    fs.WriteLine($"<{SARDINE_ROOT_ELEMENT}/>");
                }
            }
        }
    }
}
