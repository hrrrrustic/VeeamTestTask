using System;
using Core;
using Core.CoreLogic;
using Core.Exceptions;
using Core.Models;
using Core.Validation;

namespace ConsoleClient
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                //var config = new ConsoleArgumentsParser().Parse(args);
                var config = Decompress();
                new ArgumentValidator().Validate(config);
                new GZipExecutor(config).Execute();
                return 0;
            }
            catch (ArgumentParsingException ex)
            {
                Console.WriteLine("Invalid arguments");
                Console.WriteLine(ex.Message);
                return 1;
            }
            catch (ValidationException ex)
            {
                Console.WriteLine("Wrong arguments");
                Console.WriteLine(ex.Message);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
        }

        private static Configuration Compress()
        {
            return new Configuration(WorkMode.Compress, "D:\\Development\\TestSanbox\\test.png", "D:\\Development\\TestSanbox\\testResult.gz");
        }

        private static Configuration Decompress()
        {
            return new Configuration(WorkMode.Decompress, "D:\\Development\\TestSanbox\\testResult.gz", "D:\\Development\\TestSanbox\\test2.png");
        }
    }
}