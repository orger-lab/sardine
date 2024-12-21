namespace Sardine.Core.Utils.Reflection
{
    public class QualifiedClassName(string name, string fullName)
    {
        public string Name { get; } = name;
        public string FullName { get; } = fullName;
    }

}
