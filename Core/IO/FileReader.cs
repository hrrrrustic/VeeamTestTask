using System;
using System.IO;
using System.Threading;
using Core.Models;
using Core.ProducerConsumer;

namespace Core.IO
{
    public class FileReader
    {
        private readonly String _path;
        private readonly IBlockReader _reader;
        public ICompletableConsumerProducer<FileBlock> Output { get; }

        public FileReader(String path, ICompletableConsumerProducer<FileBlock> destination, IBlockReader reader)
        {
            _path = path;
            Output = destination;
            _reader = reader;
        }

        public void Read(CancellationToken token)
        {
            using FileStream fileStream = File.OpenRead(_path);

            Int32 blockCount = 1;

            while (_reader.Read(fileStream, out var result))
            {
                FileBlock block = new FileBlock(blockCount, result);
                blockCount++;
                Output.AddItem(block);
                token.ThrowIfCancellationRequested();
            }
        }
    }
}