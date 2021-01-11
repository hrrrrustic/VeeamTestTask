using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Core.ProducerConsumer
{
    public sealed class LimitedBlockingCollection<T> : ICompletableConsumerProducer<T>
    {
        private readonly ManualResetEventSlim _writeResetEvent = new ManualResetEventSlim(true);
        private readonly ManualResetEventSlim _insertResetEvent = new ManualResetEventSlim(true);
        private readonly object _locker = new Object();
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly int _itemLimit;
        private bool _addingCompleted;

        public LimitedBlockingCollection(Int32 itemLimit)
        {
            if (itemLimit <= 0)
                throw new ArgumentException($"{nameof(itemLimit)} must be greater than 0");

            _itemLimit = itemLimit;
        }

        public void AddItem(T item)
        {
            if (_addingCompleted)
                throw new InvalidOperationException("Queue marked as completed, inserting is impossible");
            
            if (_queue.Count == _itemLimit)
                _writeResetEvent.Wait();

            lock (_locker)
            {
                _queue.Enqueue(item);
                _insertResetEvent.Set();

                if(_queue.Count == _itemLimit)
                    _writeResetEvent.Reset();
            }
        }

        public IEnumerable<T> GetConsumingEnumerable(CancellationToken token)
        {
            while (!_addingCompleted || _queue.Count != 0)
                if (TryGet(token, out T item))
                    yield return item;
        }

        public void CompleteAdding()
        {
            if (_addingCompleted)
                return;

            _addingCompleted = true;
            _insertResetEvent.Set();
        }

        public bool TryGet(CancellationToken token, out T item)
        {
            item = default!;

            _insertResetEvent.Wait(token);
            token.ThrowIfCancellationRequested();

            if (_queue.Count == 0)
                return false;

            lock (_locker)
            {
                if (_queue.Count == 0)
                    return false;

                item = _queue.Dequeue();
                if (_queue.Count == 0)
                    _insertResetEvent.Reset();

                _writeResetEvent.Set();
            }

            return true;
        }

        public void Dispose()
        {
            _writeResetEvent?.Dispose();
            _insertResetEvent?.Dispose();
        }
    }
}