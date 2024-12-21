using Sardine.Core.Logs;
using System.Collections.Concurrent;

namespace Sardine.Core.DataModel
{
    internal sealed class DataQueue : IDisposable
    {
        internal delegate void DataProcessor(OnSentDataEventArgs dataIn, int queueOccupation);


        private readonly BlockingCollection<OnSentDataEventArgs> queue;
        private readonly Vessel vessel;
        private readonly DataProcessor dataProcessingMethod;
        private readonly object queueStatusLock = new();

        private Thread? queueHandlingThread;
        private CancellationTokenSource? queueHandlingTokenSource;
        private bool disposedValue;


        internal event EventHandler<QueueHandlingFailureEventArgs>? OnQueueHandlingFailure;

        
        internal int Timeout { get; set; } = 5000;
        internal int Length => queue.Count;

        
        internal DataQueue(Vessel vessel, DataProcessor processor, int capacity)
        {
            this.vessel = vessel;
            dataProcessingMethod = processor;
            queue = new BlockingCollection<OnSentDataEventArgs>(capacity);
        }

        
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        internal void Add(OnSentDataEventArgs eventArgs) => queue.TryAdd(eventArgs);

        internal void StartQueueHandling()
        {
            if (queueHandlingThread is not null && queueHandlingThread.IsAlive)
                return;

            lock (queueStatusLock)
            {
                queueHandlingThread ??= new(HandleQueue);
                queueHandlingTokenSource = new CancellationTokenSource();
                queueHandlingThread.Start(queueHandlingTokenSource.Token);
            }
        }

        internal void StopQueueHandling()
        {
            if (queueHandlingThread is null)
                return;

            lock (queueStatusLock)
            {
                if (queueHandlingThread.IsAlive)
                    queueHandlingTokenSource?.Cancel();

                if (!queueHandlingThread.Join(Timeout))
#pragma warning disable CA1031 // Do not catch general exception types
                    try
                    {
                        queueHandlingThread.Interrupt();
                    }
                    catch { }
#pragma warning restore CA1031 // Do not catch general exception types

                while (queue.TryTake(out _)) { }

                queueHandlingThread = null;
            }
        }

        private void HandleQueue(object? ct)
        {
            CancellationToken cancellationToken = (CancellationToken)ct!;

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                if (queue.TryTake(out OnSentDataEventArgs? temp, -1, cancellationToken))
                        dataProcessingMethod(temp, queue.Count);
            }
            catch (OperationCanceledException)
            {
                if (vessel.IsOnline)
                    vessel.Log("Data queue cancelled.", LogLevel.Alert);
            }
            catch (Exception ex)
            {
                OnQueueHandlingFailure?.Invoke(this, new QueueHandlingFailureEventArgs(ex));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopQueueHandling();
                    queue.Dispose();
                    queueHandlingTokenSource?.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}
