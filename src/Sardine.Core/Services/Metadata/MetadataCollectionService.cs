namespace Sardine.Core.Metadata
{
    public delegate MetadataTable? MetadataCollectionHandler<T>(T handle);


    public sealed class MetadataCollectionService
    {
        readonly Dictionary<Vessel, Delegate> registeredHandles = [];


        public IDictionary<string, MetadataTable> CollectMetadata()
        {
            Dictionary<string, MetadataTable> metadataCollection = [];

            foreach (var kvp in registeredHandles)
            {
#pragma warning disable CA1031 // Do not catch general exception types
                try
                {
                    MetadataTable? metadata = (MetadataTable)kvp.Value.DynamicInvoke(kvp.Key.ObjectHandle)!;

                    if (metadata is not null)
                        metadataCollection.Add(kvp.Key.Name, metadata);
                }
                catch
                {
                    continue;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return metadataCollection;
        }

        public void SetCollectionHandle<T>(Vessel<T> vessel, MetadataCollectionHandler<T>? metadataCollectionHandler = null)
        {
            if (metadataCollectionHandler is null)
                    registeredHandles.Remove(vessel);

            registeredHandles[vessel] = metadataCollectionHandler!;
        }
    }
}
