using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.ProducerConsumer
{
    public interface ICompletableConsumerProducer<T> : IDisposable
    {
        void CompleteAdding();
        void AddItem(T item);
        IEnumerable<T> GetConsumingEnumerable(CancellationToken token);
    }
}