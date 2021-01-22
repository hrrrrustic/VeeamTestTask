using System;
using System.IO;
using System.IO.Compression;
using Core.Models;
using Buffer = Core.Models.Buffer;

namespace Core.CoreLogic
{
    public class Decompressor : IFileBlockHandler
    {
        public FileBlock HandleBlock(FileBlock block)
        {
            using (MemoryStream resultStream = new MemoryStream())
            {
                using (MemoryStream blockForDecompressing = new MemoryStream(block.Data.ByteBuffer, 0, block.Data.ActualSize))
                {
                    using (GZipStream gZipStream = new GZipStream(blockForDecompressing, CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(resultStream);
                    }
                }

                return new FileBlock(block.BlockNumber, new Buffer(resultStream.ToArray()));
            }
        }
    }
}