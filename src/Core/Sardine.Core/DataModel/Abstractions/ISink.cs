namespace Sardine.Core.DataModel.Abstractions
{
    internal interface ISink : IDataConsumer, IEquatable<ISink>
    {
        void Resolve(object handle, object data, MessageMetadata metadata);
    }
}
