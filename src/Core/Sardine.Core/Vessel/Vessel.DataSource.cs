using Sardine.Core.DataModel.Abstractions;
using Sardine.Core.Exceptions;
using Sardine.Core.Utils.Reflection;

namespace Sardine.Core
{
    public sealed partial class Vessel<THandle>
    {
        private sealed class DataSource<TOut> : ISource
        {
            private readonly Source<TOut> sourceDelegate;


            public Type[] OutputDataTypes { get; }


            internal DataSource(Source<TOut> source)
            {
                sourceDelegate = source;
                OutputDataTypes = typeof(TOut).GetFullInheritance();
            }


            public object Generate(object handle, out bool hasMore) => sourceDelegate((THandle)handle, out hasMore)!;
            public bool Equals(ISource? other) => other is not null && other.Generate == this.Generate;
        }


        public void AddSource<TOut>(Source<TOut> source)
        {
            if (IsLinked)
                throw new VesselException(LINKED_VESSEL_ERROR_MESSAGE);

            DataSource<TOut> dataSource = new(source);

            Sources.Add(dataSource);
            SourceLocks.Add(dataSource, new object());
            
            foreach (Type outputType in dataSource.OutputDataTypes)
                    OutTypes.Add(outputType);
        }

        public void RemoveSource<TOut>(Source<TOut> source)
        {
            if (IsLinked)
                throw new VesselException(LINKED_VESSEL_ERROR_MESSAGE);

            DataSource<TOut> dataSource = new(source);

            Sources.Remove(dataSource);
            OutTypes.Remove(typeof(TOut));
            SentDataCounter.Reset(dataSource);
        }
    }
}
