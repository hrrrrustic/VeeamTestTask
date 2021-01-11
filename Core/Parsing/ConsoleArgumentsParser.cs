using System;
using Core.Exceptions;
using Core.Models;

namespace Core.Parsing
{
    public class ConsoleArgumentsParser : IParser<Configuration>
    {
        public Configuration Parse(string[] values)
        {
            if(values.Length != 3)
                throw new ArgumentParsingException($"Invalid argument count : {values.Length}. Expected : 3");

            var result = Enum.TryParse(values[0], true, out WorkMode mode);
            if (!result)
                throw new ArgumentParsingException($"Invalid mode chosen : {values[0]}. Expected : compress or decompress");

            return new Configuration(mode, values[1], values[2]);
        }
    }
}