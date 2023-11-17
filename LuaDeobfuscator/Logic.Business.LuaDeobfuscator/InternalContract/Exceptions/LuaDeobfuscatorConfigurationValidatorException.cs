using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Business.LuaDeobfuscator.InternalContract.Exceptions
{
    public class LuaDeobfuscatorConfigurationValidatorException : Exception
    {
        public LuaDeobfuscatorConfigurationValidatorException()
        {
        }

        public LuaDeobfuscatorConfigurationValidatorException(string message) : base(message)
        {
        }

        public LuaDeobfuscatorConfigurationValidatorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected LuaDeobfuscatorConfigurationValidatorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
