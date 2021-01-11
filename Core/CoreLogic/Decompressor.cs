using System;
using System.IO;
using System.IO.Compression;
using Core.Models;
using Buffer = Core.Models.Buffer;

namespace Core.CoreLogic
{
    public class Decompressor : IFileBlockHandler
    {
        private readonly int _blockSize;
        public Decompressor(Int32 blockSize)
        {
            _blockSize = blockSize;
        }

        public FileBlock HandleBlock(FileBlock block)
        {
            Buffer buffer = Buffer.GetBuffer(_blockSize);

            using (MemoryStream blockForDecompressing = new MemoryStream(block.Data.ByteBuffer, 0, block.Data.ActualSize))
            {
                using (GZipStream gZipStream = new GZipStream(blockForDecompressing, CompressionMode.Decompress))
                {
                    var decompressedSize = gZipStream.Read(buffer.ByteBuffer);
                    buffer.UpdateActualSize(decompressedSize);
                }
            }

            return new FileBlock(block.BlockNumber, buffer);
        }
    }
}