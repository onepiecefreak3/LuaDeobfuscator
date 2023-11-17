using Logic.Business.LuaDeobfuscator.InternalContract;
using Logic.Business.LuaDeobfuscator.InternalContract.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Business.LuaDeobfuscator
{
    internal class LuaDeobfuscatorConfigurationValidator : ILuaDeobfuscatorConfigurationValidator
    {
        public void Validate(LuaDeobfuscatorConfiguration config)
        {
            ValidateFilePath(config);
        }

        private void ValidateFilePath(LuaDeobfuscatorConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config.FilePath))
                throw new LuaDeobfuscatorConfigurationValidatorException("No file to process was specified. Specify a file by using the -f argument.");

            if (!File.Exists(config.FilePath) && !Directory.Exists(config.FilePath))
                throw new LuaDeobfuscatorConfigurationValidatorException($"File or directory '{Path.GetFullPath(config.FilePath)}' was not found.");
        }
    }
}
