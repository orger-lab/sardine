using Sardine.Core.DataModel;
using Sardine.Core.DataModel.Abstractions;
using Sardine.Core.Logs;

namespace Sardine.Core
{
    public abstract partial class Vessel
    {
        protected void GenerateData(Type? outType = null, double sourceRate = 0)
        {
            if (!IsActive || !IsOnline)
                return;

            if (ObjectHandle is null)
                return;

            foreach (ISource source in Sources)
            {
                if (outType == null || source.OutputDataTypes.Contains(outType))
                    new Task(() => GenerateAndSendData(sourceRate, source)).Start();
            }
        }

        protected void SendData(object data, Type[] dataTypes, double sourceRate, int sourceSignature, long sourceID)
        {
            DatalineStatus.MostRecentIDOut = sourceID;
            DatalineStatus.MostRecentDataTypesOut = dataTypes;
            OnSentData?.Invoke(this, new OnSentDataEventArgs(data, dataTypes, Signature, Name, sourceSignature, sourceRate, sourceID));
        }

        protected void ProcessData(OnSentDataEventArgs? eventArgs, int queueOccupation)
        {
            if (eventArgs is null)
                return;

            if (!IsActive || !IsOnline)
                return;

            if (ObjectHandle is null)
                return;

            QueueLength = queueOccupation;
            DatalineStatus.MostRecentIDIn = eventArgs.Metadata.SourceID;
            DatalineStatus.MostRecentDataTypesIn = [.. eventArgs.Metadata.DataTypes];

            if (eventArgs.Data is null)
                return;

            // TODO make this better
            foreach (Type dataType in eventArgs.Metadata.DataTypes)
            {
                if (Sinks.TryGetValue(dataType, out List<ISink>? sinkList))
                {
                    foreach (ISink sink in sinkList)
                    {
#pragma warning disable CA1031 // Do not catch general exception types
                        try
                        {
                            sink.Resolve(ObjectHandle, eventArgs.Data, eventArgs.Metadata);
                        }
                        catch (Exception ex)
                        {
                            this.Log($"Failure on Sink of type {dataType.Name}: {ex.Message}", LogLevel.Warning);
                        }
#pragma warning restore CA1031 // Do not catch general exception types
                    }
                }

                if (Transformers.TryGetValue(dataType, out List<ITransformer>? transformersList))
                {
                    foreach (ITransformer processor in transformersList)
                    {

#pragma warning disable CA1031 // Do not catch general exception types
                        try
                        {
                            object? result = processor.Transform(ObjectHandle, eventArgs.Data, eventArgs.Metadata);

                            if (result is not null)
                                SendData(result, processor.OutputDataTypes, eventArgs.Metadata.SourceRate, eventArgs.Metadata.OriginalSender, eventArgs.Metadata.SourceID);
                        }
                        catch (Exception ex)
                        {
                            this.Log($"Failure on Transformer of type {dataType.Name}: {ex.Message}", LogLevel.Warning);
                        }
#pragma warning restore CA1031 // Do not catch general exception types
                    }

                }
            }
        }

        private void ReceiveData(object? sender, OnSentDataEventArgs eventArgs)
        {
            switch (DataHandlingStrategy)
            {
                case DataHandlingStrategy.QueueToInbox:
                    QueueData(eventArgs);
                    break;
                case DataHandlingStrategy.ResolveImmediatly:
                    ProcessData(eventArgs, 0);
                    break;
            }
        }

        private void QueueData(OnSentDataEventArgs eventArgs)
        {
            if (!IsActive)
                return;

            queue?.Add(eventArgs);
        }

        private void GenerateAndSendData(double sourceRate, ISource source)
        {
            bool lockTaken = false;
            Monitor.TryEnter(SourceLocks[source], ref lockTaken);

            if (!lockTaken)
                return;
            
            try
            {
                bool hasMore = true;
                while (hasMore)
                {
                    object? result = null;

                    if (ObjectHandle is not null)
                        result = source.Generate(ObjectHandle, out hasMore);

                    if (result is null)
                        hasMore = false;

                    if (result is not null)
                        SendData(result, source.OutputDataTypes, sourceRate, Signature, SentDataCounter[source]++);
                }
            }
            finally
            {
                Monitor.Exit(SourceLocks[source]);
            }
        }
    }
}
