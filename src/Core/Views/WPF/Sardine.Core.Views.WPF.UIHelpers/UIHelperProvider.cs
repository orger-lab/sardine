using Sardine.Utils.Reflection;
[assembly: CLSCompliant(true)]

namespace Sardine.Core.Helpers
{
    public class UIHelperProvider
    {
        IReadOnlyDictionary<string, IUIHelper>? helpers;


        public IReadOnlyDictionary<string, IUIHelper> Helpers
        {
            get
            {
                helpers ??= AssemblyInformation.KnownTypes.Where(x => x.InheritsFrom(typeof(IUIHelper)) && !x.IsAbstract && x.GetConstructor([]) is not null)
                                                          .ToDictionary(x => x.FullName!, x => (IUIHelper)Activator.CreateInstance(x)!);

                return helpers;
            }
        }

        public IUIHelper? this[string key]
        {
            get
            {
                Helpers.TryGetValue(key, out IUIHelper? helper);
                return helper;
            }
        }
    }
}
