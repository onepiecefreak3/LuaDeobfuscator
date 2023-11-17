using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.Exceptions
{
    public class LuaParserException : Exception
    {
        public LuaParserException()
        {
        }

        public LuaParserException(string message) : base(message)
        {
        }

        public LuaParserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected LuaParserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
