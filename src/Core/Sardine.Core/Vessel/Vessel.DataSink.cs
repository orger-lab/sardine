using Sardine.Core.DataModel;
using Sardine.Core.DataModel.Abstractions;
using Sardine.Core.Exceptions;

namespace Sardine.Core
{
    public sealed partial class Vessel<THandle>
    {
        private sealed class DataSink<TIn> : ISink
        {
            private readonly Sink<TIn> sinkDelegate;


            public Type InputDataType { get; }


            internal DataSink(Sink<TIn> sink)
            {
                sinkDelegate = sink;
                InputDataType = typeof(TIn);
            }


            public void Resolve(object handle, object data, MessageMetadata metadata) => sinkDelegate((THandle)handle, (TIn)data, metadata);
            public bool Equals(ISink? other) => other is not null && other.Resolve == this.Resolve;
        }


        public void AddSink<TIn>(Sink<TIn> sink, IEnumerable<Vessel>? sourceFilter = null)
        {
            if (IsLinked)
                throw new VesselException(LINKED_VESSEL_ERROR_MESSAGE);

            DataSink<TIn> dataSink = new(sink);

            Sinks.TryAdd(dataSink.InputDataType, []);

            Sinks[dataSink.InputDataType].Add(dataSink);
            ReceiverFiltersDictionary.Add(dataSink, sourceFilter?.ToList());
        }

        public void RemoveSink<TIn>(Sink<TIn> sink)
        {
            if (IsLinked)
                throw new VesselException(LINKED_VESSEL_ERROR_MESSAGE);

            DataSink<TIn> dataSink = new(sink);

            ReceiverFiltersDictionary.Remove(dataSink);
            Sinks[dataSink.InputDataType].Remove(dataSink);
        }
    }
}
