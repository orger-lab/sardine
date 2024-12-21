namespace Sardine.Core.DataModel.Abstractions
{
    internal interface IDataProvider : IDataProcessor
    {
        Type[] OutputDataTypes { get; }
    }
}
