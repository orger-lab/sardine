namespace Sardine.Core.DataModel.Abstractions
{
    internal interface IDataConsumer : IDataProcessor
    {
        Type InputDataType { get; }
    }
}
