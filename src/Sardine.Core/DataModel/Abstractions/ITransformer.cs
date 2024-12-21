namespace Sardine.Core.DataModel.Abstractions
{
    internal interface ITransformer : IDataConsumer, IDataProvider, IEquatable<ITransformer>
    {
        object? Transform(object handle, object data, MessageMetadata metadata);
    }
}
