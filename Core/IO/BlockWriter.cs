using System;
using System.IO;
using Core.Models;

namespace Core.IO
{
    public interface IBlockWriter
    {
        void Write(FileStream stream, FileBlock block);
    }

    public class BlockWriter : IBlockWriter
    {
        public void Write(FileStream stream, FileBlock block)
        {
            stream.Write(BitConverter.GetBytes(block.Data.ActualSize));
            stream.Write(block.Data.ByteBuffer, 0, block.Data.ActualSize);
        }
    }

    public class DecompressedBlockWriter : IBlockWriter
    {
        public void Write(FileStream stream, FileBlock block)
        {
            stream.Write(block.Data.ByteBuffer, 0, block.Data.ActualSize);
        }
    }
}