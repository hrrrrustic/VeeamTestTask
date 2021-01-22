using System;

namespace Core.Models
{
    public class Configuration
    {
        public WorkMode Mode { get; }
        public String InputFile { get; }
        public String OutputFile { get; }
        public readonly int BlockSize = 1024 * 1024 * 4;

        public Configuration(WorkMode mode, String inputFile, String outputFile)
        {
            Mode = mode;
            InputFile = inputFile;
            OutputFile = outputFile;
        }
    }
}