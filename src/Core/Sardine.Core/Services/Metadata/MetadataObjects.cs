namespace Sardine.Core.Metadata
{
    public abstract class MetadataObject(MetadataType type, object data)
    {
        public MetadataType Type { get; } = type;
        public object Data { get; set; } = data;
    }

    public sealed class MetadataString(string data = "") : MetadataObject(MetadataType.TypeString, data) { }
    public sealed class MetadataInteger(int data = 0) : MetadataObject(MetadataType.TypeInteger, data) { }
    public sealed class MetadataFloat(double data = 0) : MetadataObject(MetadataType.TypeFloat, data) { }
    public sealed class MetadataBool(bool data = false) : MetadataObject(MetadataType.TypeBool, data) { }
    public sealed class MetadataDateTimeLocal(DateTime? data = null) : MetadataObject(MetadataType.TypeDateTimeLocal, data ?? DateTime.Now) { }
    public sealed class MetadataDateTimeOffset(DateTimeOffset? data = null) : MetadataObject(MetadataType.TypeDateTimeOffset, data ?? DateTimeOffset.Now) { }
    public class MetadataArray(IEnumerable<MetadataObject>? data = null) : MetadataObject(MetadataType.TypeArray, data ?? []) { }
    public class MetadataTable(IDictionary<string, MetadataObject>? data = null) : MetadataObject(MetadataType.TypeTable, data ?? new Dictionary<string, MetadataObject>()) { }
}