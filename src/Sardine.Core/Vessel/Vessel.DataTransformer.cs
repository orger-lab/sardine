using Sardine.Core.DataModel;
using Sardine.Core.DataModel.Abstractions;
using Sardine.Core.Exceptions;
using Sardine.Core.Utils.Reflection;

namespace Sardine.Core
{
    public sealed partial class Vessel<THandle>
    {
        private sealed class DataTransformer<TIn, TOut> : ITransformer
        {
            private readonly Transformer<TIn, TOut> transformerDelegate;


            public Type InputDataType { get; }
            public Type[] OutputDataTypes { get; }


            internal DataTransformer(Transformer<TIn, TOut> transformer)
            {
                transformerDelegate = transformer;
                InputDataType = typeof(TIn);
                OutputDataTypes = typeof(TOut).GetFullInheritance();
            }


            public object Transform(object handle, object data, MessageMetadata metadata) => transformerDelegate((THandle)handle, (TIn)data, metadata)!;
            public bool Equals(ITransformer? other) => other is not null && other.Transform == this.Transform;
        }


        public void AddTransformer<TIn, TOut>(Transformer<TIn, TOut> transformer, IEnumerable<Vessel>? sourceFilter = null)
        {
            if (IsLinked)
                throw new VesselException(LINKED_VESSEL_ERROR_MESSAGE);

            DataTransformer<TIn, TOut> dataTransformer = new(transformer);

            if (!Transformers.TryGetValue(dataTransformer.InputDataType, out List<ITransformer>? value))
            {
                value = [];
                Transformers.Add(dataTransformer.InputDataType, value);
            }

            foreach (Type outputType in dataTransformer.OutputDataTypes)
                OutTypes.Add(outputType);

            value.Add(dataTransformer);
            ReceiverFiltersDictionary.Add(dataTransformer, sourceFilter?.ToList());
        }

        public void RemoveTransformer<TIn, TOut>(Transformer<TIn, TOut> transformer)
        {
            if (IsLinked)
                throw new VesselException(LINKED_VESSEL_ERROR_MESSAGE);

            DataTransformer<TIn, TOut> dataTransformer = new(transformer);
            ReceiverFiltersDictionary.Remove(dataTransformer);
            Transformers[dataTransformer.InputDataType].Remove(dataTransformer);
            OutTypes.Remove(typeof(TOut));
        }
    }
}
