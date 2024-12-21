using System.Diagnostics.CodeAnalysis;

namespace Sardine.Core.Utils.Reflection
{
    public static class TypeExtensions
    {
        // https://stackoverflow.com/questions/8868119/find-all-parent-types-both-base-classes-and-interfaces
        public static bool InheritsFrom(this Type? type, Type? baseType)
        {
            if (type is null)
            {
                return false;
            }

            if (baseType is null)
            {
                return type.IsInterface || type == typeof(object);
            }

            if (baseType.IsInterface)
            {
                return type.GetInterfaces().Contains(baseType);
            }

            Type? currentType = type;
            while (currentType is not null)
            {
                if (currentType.BaseType == baseType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }

        public static Type[] GetFullInheritance(this Type? type)
        {
            if (type is null)
            {
                return [];
            }

            List<Type> types =
            [
                type,
                .. type.GetInterfaces()
            ];

            if (type.BaseType is not null)
            {
                foreach (Type t in type.BaseType.GetFullInheritance())
                {
                    types.Add(t);
                }
            }

            return types.Distinct().ToArray();
        }

        public static bool FindRelatedTypeInList(this Type t, List<Type> typeSet, [MaybeNullWhen(false)] out Type type)
        {
            type = null;

            if (typeSet.Contains(t))
            {
                type = t;
                return true;
            }

            if (t.IsGenericType)
            {
                foreach (Type genType in t.GenericTypeArguments)
                {
                    if (typeSet.Contains(genType))
                    {
                        type = genType;
                        return true;
                    }
                }
            }

            if (t.BaseType is not null)
            {
                if (typeSet.Contains(t.BaseType))
                {
                    type = t.BaseType;
                    return true;
                }

                if (t.BaseType.IsGenericType)
                {
                    foreach (Type genType in t.BaseType.GenericTypeArguments)
                    {
                        if (typeSet.Contains(genType))
                        {
                            type = genType;
                            return true;
                        }
                    }
                }

            }

            return false;
        }
    }
}
