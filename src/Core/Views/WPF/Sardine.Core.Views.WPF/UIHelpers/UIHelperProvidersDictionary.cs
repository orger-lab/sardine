using Sardine.Core.Utils.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sardine.Core.Views.WPF.UIHelpers
{
    public class UIHelperProvidersDictionary : IReadOnlyDictionary<string, IUIHelperProvider>
    {
        private IReadOnlyDictionary<string, IUIHelperProvider>? helperProviders;


        private IReadOnlyDictionary<string, IUIHelperProvider> HelperProviders
        {
            get
            {
                helperProviders ??= AssemblyInformation.KnownTypes.Where(x => x.InheritsFrom(typeof(IUIHelperProvider)) && !x.IsAbstract && x.GetConstructor([]) is not null)
                                                          .ToDictionary(x => x.FullName!, x => (IUIHelperProvider)Activator.CreateInstance(x)!);

                return helperProviders;
            }
        }

        public IEnumerable<string> Keys => HelperProviders.Keys;
        public IEnumerable<IUIHelperProvider> Values => HelperProviders.Values;
        public int Count => HelperProviders.Count;

        public IUIHelperProvider this[string key]
        {
            get
            {
                if (HelperProviders.TryGetValue(key, out IUIHelperProvider? helper))
                    return helper;

                throw new KeyNotFoundException(key);
            }
        }


        public bool ContainsKey(string key) => HelperProviders.ContainsKey(key);
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out IUIHelperProvider value) => HelperProviders.TryGetValue(key, out value);
        public IEnumerator<KeyValuePair<string, IUIHelperProvider>> GetEnumerator() => HelperProviders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)HelperProviders).GetEnumerator();
    }
}
