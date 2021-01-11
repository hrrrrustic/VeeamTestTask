using System.IO;
using Core.Models;

namespace Core.CoreLogic
{
    public interface IFileBlockHandler
    {
        FileBlock HandleBlock(FileBlock block);
    }
}