using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Core.Models;
using Core.ProducerConsumer;

namespace Core.IO
{
    public class FileWriter
    {
        private ICompletableConsumerProducer<FileBlock> Input { get; }
        private readonly string _path;
        private int _nextBlockForWriting = 1;
        private readonly IBlockWriter _blockWriter;
        //Из комперссора/декомпрессора кажется могут прилететь блоки чуть-чуть(!) перемешано.
        private readonly Dictionary<int, FileBlock> _unsortedItems = new Dictionary<int, FileBlock>(Environment.ProcessorCount);
        public FileWriter(String path, ICompletableConsumerProducer<FileBlock> input, IBlockWriter blockWriter)
        {
            Input = input;
            _blockWriter = blockWriter;
            _path = path;
        }

        public void Write(CancellationToken token)
        {
            foreach (var fileBlock in Input.GetConsumingEnumerable(token))
            {
                token.ThrowIfCancellationRequested();
                if (fileBlock.BlockNumber == _nextBlockForWriting)
                {
                    using (fileBlock)
                    {
                        WriteBlock(fileBlock);
                    }
                    continue;
                }

                _unsortedItems.Add(fileBlock.BlockNumber, fileBlock);
                WriteSkippedBlocks();
            }

            WriteSkippedBlocks();
        }

        private void WriteSkippedBlocks()
        {
            while (_unsortedItems.ContainsKey(_nextBlockForWriting))
            {
                using (var skippedBlock = _unsortedItems[_nextBlockForWriting])
                {
                    WriteBlock(skippedBlock);
                    _unsortedItems.Remove(skippedBlock.BlockNumber);
                }
            }
        }
        private void WriteBlock(FileBlock block)
        {
            using (FileStream writeStream = new FileStream(_path, FileMode.Append))
            {
                _blockWriter.Write(writeStream, block);
            }

            _nextBlockForWriting++;
        }
    }
}