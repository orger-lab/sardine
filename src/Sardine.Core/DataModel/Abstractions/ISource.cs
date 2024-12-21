namespace Sardine.Core.DataModel.Abstractions
{
    internal interface ISource : IDataProvider, IEquatable<ISource>
    {
        object? Generate(object handle, out bool hasMore);
    }
}
