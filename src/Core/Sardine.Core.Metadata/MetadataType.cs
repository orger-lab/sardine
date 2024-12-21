[assembly: CLSCompliant(true)]
namespace Sardine.Core.Metadata
{
    public enum MetadataType
    {
        TypeString,
        TypeInteger,
        TypeFloat,
        TypeBool,
        TypeDateTimeLocal,
        TypeDateTimeOffset,
        TypeArray,
        TypeTable,
    }
}