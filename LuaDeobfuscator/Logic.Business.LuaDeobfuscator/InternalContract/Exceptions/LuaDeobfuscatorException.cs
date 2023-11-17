using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Business.LuaDeobfuscator.InternalContract.Exceptions
{
    internal class LuaDeobfuscatorException : Exception
    {
        public LuaDeobfuscatorException()
        {
        }

        public LuaDeobfuscatorException(string message) : base(message)
        {
        }

        public LuaDeobfuscatorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected LuaDeobfuscatorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
