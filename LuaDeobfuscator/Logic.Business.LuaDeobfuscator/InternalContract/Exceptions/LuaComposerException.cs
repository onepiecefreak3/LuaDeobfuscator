using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Business.LuaDeobfuscator.InternalContract.Exceptions
{
    [Serializable]
    public class LuaComposerException:Exception
    {
        public LuaComposerException()
        {
        }

        public LuaComposerException(string message) : base(message)
        {
        }

        public LuaComposerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected LuaComposerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
