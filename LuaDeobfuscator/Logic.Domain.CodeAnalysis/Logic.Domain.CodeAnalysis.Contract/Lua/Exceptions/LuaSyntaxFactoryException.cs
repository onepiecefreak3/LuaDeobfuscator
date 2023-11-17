using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.Exceptions
{
    [Serializable]
    public class LuaSyntaxFactoryException : Exception
    {
        public LuaSyntaxFactoryException()
        {
        }

        public LuaSyntaxFactoryException(string message) : base(message)
        {
        }

        public LuaSyntaxFactoryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected LuaSyntaxFactoryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
