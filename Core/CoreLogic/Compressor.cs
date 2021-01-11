using System;
using System.IO;
using System.IO.Compression;
using Core.Models;
using Buffer = Core.Models.Buffer;

namespace Core.CoreLogic
{
    public class Compressor : IFileBlockHandler
    {
        public FileBlock HandleBlock(FileBlock block)
        {
            using (MemoryStream compressedBlock = new MemoryStream(block.Data.ActualSize))
            {
                using (GZipStream gZipStream = new GZipStream(compressedBlock, CompressionMode.Compress))
                {
                    gZipStream.Write(block.Data.ByteBuffer, 0, block.Data.ActualSize);
                }

                var result = compressedBlock.ToArray();
                Buffer buffer = new Buffer(result, result.Length, false);
                return new FileBlock(block.BlockNumber, buffer);
            }
        }
    }
}