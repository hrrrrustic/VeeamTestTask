using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using Core.IO;
using Core.Models;
using Core.ProducerConsumer;

namespace Core.CoreLogic
{
    public class GZipExecutor
    {
        private const int ThreeHundredMb = 1024 * 1024 * 300;
        private readonly Configuration _configuration;

        private readonly OperationContext _context;
        public GZipExecutor(Configuration configuration)
        {
            _configuration = configuration;
            _context = new OperationContext();
        }

        public void Execute()
        {
            switch (_configuration.Mode)
            {
                case WorkMode.Decompress:
                    Execute(new Decompressor(), new BlockReader(), new DecompressedBlockWriter());
                    return;
                case WorkMode.Compress:
                    Execute(new Compressor(), new DecompressedBlockReader(_configuration.BlockSize), new BlockWriter());
                    return;
                default:
                    throw new NotSupportedException();
            }
        }

        private void Execute(IFileBlockHandler blockHandler, IBlockReader reader, IBlockWriter writer)
        {
            int itemLimit = ThreeHundredMb / _configuration.BlockSize;
            int processorCount = Environment.ProcessorCount;
            int additionalThreadsCount = Math.Max(1, processorCount - 2);

            using var readOutput = new LimitedBlockingCollection<FileBlock>(itemLimit);
            using var workOutput = new LimitedBlockingCollection<FileBlock>(itemLimit);

            var reading = StartReading(readOutput, reader);
            var workers = StartHandling(additionalThreadsCount, readOutput, workOutput, blockHandler);
            var writing = StartWriting(workOutput, writer);

            reading.Join();
            readOutput.CompleteAdding();

            foreach (var worker in workers)
                worker.Join();

            workOutput.CompleteAdding();

            writing.Join();

            _context.RethrowExceptionIfTerminated();
        }

        private SafeThread StartReading(LimitedBlockingCollection<FileBlock> output, IBlockReader blockReader)
        {
            var reader = new FileReader(_configuration.InputFile, output, blockReader);

            return new SafeThread(() => reader.Read(_context.GetCancellationToken()))
                .WithOnException(_context.Terminate)
                .Start();
        }

        private SafeThread StartWriting(LimitedBlockingCollection<FileBlock> input, IBlockWriter blockWriter)
        {
            var writer = new FileWriter(_configuration.OutputFile, input, blockWriter);

            return new SafeThread(() => writer.Write(_context.GetCancellationToken()))
                .WithOnException(_context.Terminate)
                .Start();
        }

        private SafeThread[] StartHandling(int threadCount, 
            LimitedBlockingCollection<FileBlock> input, 
            LimitedBlockingCollection<FileBlock> output, 
            IFileBlockHandler blockHandler)
        {
            var workers = new SafeThread[threadCount];

            for (int i = 0; i < workers.Length; i++)
            {
                var worker = new BlockWorker(input, output, blockHandler);
                workers[i] = new SafeThread(() => worker.Work(_context.GetCancellationToken()))
                    .WithOnException(_context.Terminate)
                    .Start();
            }

            return workers;
        }
    }
}