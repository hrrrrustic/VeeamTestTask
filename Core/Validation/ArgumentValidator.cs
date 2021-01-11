using System.IO;
using Core.Exceptions;
using Core.Models;

namespace Core.Validation
{
    public class ArgumentValidator : IValidator<Configuration>
    {
        public void Validate(Configuration config)
        {
            if(!File.Exists(config.InputFile))
                throw new ValidationException("Input file doesn't exist");

        }
    }
}