namespace Sardine.Core.Metadata
{
    public static class MetadataExtensions
    {
        public static void SetMetadataCollection<THandle>(this Vessel<THandle> vessel, MetadataCollectionHandler<THandle>? handler = null)
        {
            ArgumentNullException.ThrowIfNull(vessel);
            Fleet.Current.Get<MetadataCollectionService>().SetCollectionHandle(vessel, handler);
        }

        public static IDictionary<string, MetadataTable> CollectMetadata()
        {
            return Fleet.Current.Get<MetadataCollectionService>().CollectMetadata();
        }
    }
}