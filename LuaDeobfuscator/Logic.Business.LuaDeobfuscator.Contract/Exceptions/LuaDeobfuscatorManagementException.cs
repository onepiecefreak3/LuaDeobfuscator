using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Business.LuaDeobfuscator.Contract.Exceptions
{
    [Serializable]
    public class LuaDeobfuscatorManagementException : Exception
    {
        public LuaDeobfuscatorManagementException()
        {
        }

        public LuaDeobfuscatorManagementException(string message) : base(message)
        {
        }

        public LuaDeobfuscatorManagementException(string message, Exception inner) : base(message, inner)
        {
        }

        protected LuaDeobfuscatorManagementException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
