using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using Buffer = Core.Models.Buffer;

namespace Core.IO
{
    public interface IBlockReader
    {
        bool Read(FileStream stream, [NotNullWhen(true)] out Buffer? buffer);
    }

    public class BlockReader : IBlockReader
    {
        public bool Read(FileStream stream, out Buffer? result)
        {
            result = null;
            Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<int>()];
            var readCount = stream.Read(buffer);

            if (readCount <= 0)
                return false;

            var blockSize = BitConverter.ToInt32(buffer);
            result = Buffer.GetBuffer(blockSize);
            readCount = stream.Read(result.ByteBuffer, 0, blockSize);
            result.UpdateActualSize(readCount);

            return true;
        }
    }

    public class DecompressedBlockReader : IBlockReader
    {
        private readonly int _blockSize;
        public DecompressedBlockReader(Int32 blockSize)
        {
            _blockSize = blockSize;
        }

        public bool Read(FileStream stream, out Buffer? result)
        {
            result = Buffer.GetBuffer(_blockSize);
            var readCount = stream.Read(result.ByteBuffer, 0, _blockSize);

            if (readCount <= 0)
            {
                result = null;
                return false;
            }

            result.UpdateActualSize(readCount);

            return true;
        }
    }
}