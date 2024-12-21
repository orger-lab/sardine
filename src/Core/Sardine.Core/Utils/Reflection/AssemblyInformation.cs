using System.Reflection;
using System.Runtime.InteropServices;

namespace Sardine.Core.Utils.Reflection
{
    public static class AssemblyInformation
    {
        private static IList<Type>? knownTypes;

        public static string CallingAssemblyVersion => Assembly.GetCallingAssembly().GetName().Version!.ToString();
        public static AssemblyName EntryAssembly => Assembly.GetEntryAssembly()!.GetName();
        public static string EntryAssemblyName => EntryAssembly!.Name!;
        public static string EntryAssemblyVersion => EntryAssembly.Version!.ToString();
        public static IList<Type> KnownTypes
        {
            get
            {
                if (knownTypes is null)
                {
                    ReloadAssemblies();
                }

                return knownTypes ?? new List<Type>();
            }
            private set => knownTypes = value;
        }
        public static IList<string> AssemblyLoadPaths { get; } = new List<string>() { AppDomain.CurrentDomain.BaseDirectory };

        public static IList<T> GetInterfaceImplementations<T>()
        {
            if (!typeof(T).IsInterface)
            {
                throw new InvalidOperationException();
            }

            List<T> knownInterfaces = [];

            foreach (Type t in KnownTypes)
            {
                if (t.GetInterface(typeof(T).Name) is not null && t.GetConstructor([]) is not null)
                {
                    T? k = (T?)Activator.CreateInstance(t);
                    if (k is not null)
                    {
                        knownInterfaces.Add(k);
                    }
                }
            }

            return knownInterfaces;
        }
        private static void ReloadAssemblies()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotImplementedException();
            }

            List<Assembly> loadedAssemblies = [];
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                loadedAssemblies.Add(assembly);
            }

            string[] loadedPaths = loadedAssemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

            foreach (string assemblyLoadPath in AssemblyLoadPaths)
            {
                string[] referencedPaths = Directory.GetFiles(assemblyLoadPath, "*.dll");
                List<string> toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
                toLoad.ForEach(path =>
                {
                    try
                    {
                        loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path)));
                    }
                    catch (Exception ex) when (ex is BadImageFormatException or FileNotFoundException) { }
                });
            }

            KnownTypes = GetAllKnownTypes();
        }
        private static List<Type> GetAllKnownTypes()
        {
            List<Type> knownTypes = [];
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    Type[] types = a.GetTypes();
                    foreach (Type t in types)
                    {
                        knownTypes.Add(t);
                    }
                }
                catch (ReflectionTypeLoadException) { continue; }
            }

            return knownTypes;
        }
    }
}
