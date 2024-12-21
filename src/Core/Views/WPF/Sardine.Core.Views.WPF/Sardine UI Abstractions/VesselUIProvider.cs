using Sardine.Core.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sardine.Core.Views.WPF
{
    public static class VesselUIProvider
    {
        private static Dictionary<Type, List<Type>>? knownProvidedUIsDictionary;


        private static Dictionary<Type, List<Type>> KnownProvidedUIsDictionary
        {
            get
            {
                if (knownProvidedUIsDictionary is null)
                    InitializeKnownUIsDictionary();

                return knownProvidedUIsDictionary!;
            }
        }


        public static bool CanProvideUI(Vessel vessel)
        {
            ArgumentNullException.ThrowIfNull(vessel);

            return GetRangeOfAllowedTypes(vessel.HandleType).Any((x) => KnownProvidedUIsDictionary.ContainsKey(x));
        }
        
        public static IReadOnlyList<(VesselUserControl VesselUI,Type UIType)> ProvideUI(Vessel vessel, Func<Type, bool>? filter = null)
        {
            ArgumentNullException.ThrowIfNull(vessel);

            List<(VesselUserControl, Type)> _uiList = [];

            foreach (var type in GetRangeOfAllowedTypes(vessel.HandleType))
            {
                if (KnownProvidedUIsDictionary.TryGetValue(type, out List<Type>? typeList))
                {
                    foreach (var uiType in typeList)
                    {
                        if (filter?.Invoke(uiType) ?? true)
                        {
                            object? ui = Activator.CreateInstance(uiType);

                            if (ui is not null)
                            {
                                ((VesselUserControl)ui).LinkedVesselName = vessel.Name;   
                                _uiList.Add(((VesselUserControl)ui, uiType));
                            }
                        }
                    }
                }
            }

            return _uiList.AsReadOnly();
        }

        private static List<Type> GetRangeOfAllowedTypes(Type vesselType)
        {
            List<Type> types = [];

            var baseType = vesselType.GetTypeInfo();

            types.Add(baseType);

            if (baseType.BaseType is not null)
                types.Add(baseType.BaseType);

            types.AddRange(baseType.GetInterfaces());

            return types;
        }

        private static List<Type> ReflectKnownUIs()
        {
            List<Type> knownUIs = [];

            foreach (Type t in AssemblyInformation.KnownTypes)
            {
                if (typeof(VesselUserControl).IsAssignableFrom(t))
                    knownUIs.Add(t);
            }
 
            return knownUIs;
        }

        private static void InitializeKnownUIsDictionary()
        {
            knownProvidedUIsDictionary = [];

            List<Type> knownUITypes = ReflectKnownUIs();

            foreach (Type item in knownUITypes)
            {
                if (!item.IsAbstract)
                {
                    Type[] k = item.GetFullInheritance();

                    foreach (Type t in k.Where((x) => x.BaseType == typeof(VesselUserControl) && x.IsGenericType))
                    {
                        if (!knownProvidedUIsDictionary.ContainsKey(t.GenericTypeArguments[0]))
                            knownProvidedUIsDictionary.Add(t.GenericTypeArguments[0], []);

                        knownProvidedUIsDictionary[t.GenericTypeArguments[0]].Add(item);
                    }
                }
            }
        }
    }
}
