using System;
using System.Threading;
using Core.Models;
using Core.ProducerConsumer;

namespace Core.CoreLogic
{
    public class BlockWorker
    {
        public ICompletableConsumerProducer<FileBlock> Input { get; }
        public ICompletableConsumerProducer<FileBlock> Output { get; }
        private readonly IFileBlockHandler _handler;

        public BlockWorker(ICompletableConsumerProducer<FileBlock> input, ICompletableConsumerProducer<FileBlock> output, IFileBlockHandler handler)
        {
            Input = input;
            Output = output;
            _handler = handler;
        }

        public void Work(CancellationToken token)
        {
            foreach (var fileBlock in Input.GetConsumingEnumerable(token))
            {
                using (fileBlock)
                {
                    token.ThrowIfCancellationRequested();
                    FileBlock handledBlock = _handler.HandleBlock(fileBlock);
                    Output.AddItem(handledBlock);
                }
            }
        }
    }
}