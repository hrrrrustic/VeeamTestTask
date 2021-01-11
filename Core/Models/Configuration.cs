using System;

namespace Core.Models
{
    public class Configuration
    {
        public WorkMode Mode { get; }
        public String InputFile { get; }
        public String OutputFile { get; }
        public readonly int BlockSize = 512;

        public Configuration(WorkMode mode, String inputFile, String outputFile)
        {
            Mode = mode;
            InputFile = inputFile;
            OutputFile = outputFile;
        }
    }
}