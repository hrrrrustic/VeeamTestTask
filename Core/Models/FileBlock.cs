using System;

namespace Core.Models
{
    public class FileBlock : IDisposable
    {
        public int BlockNumber { get; }
        public Buffer Data { get; }

        public FileBlock(Int32 blockNumber, Buffer data)
        {
            BlockNumber = blockNumber;
            Data = data;    
        }

        public void Dispose()
        {
            Data?.Dispose();
        }
    }
}