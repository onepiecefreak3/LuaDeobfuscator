using System.Runtime.Serialization;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.Exceptions
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
