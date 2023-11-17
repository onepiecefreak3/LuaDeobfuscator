using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua.Exceptions;

namespace Logic.Domain.CodeAnalysis.Contract.Lua
{
    [MapException(typeof(LuaComposerException))]
    public interface ILuaComposer
    {
        string ComposeCodeUnit(CodeUnitSyntax codeUnit);
    }
}
