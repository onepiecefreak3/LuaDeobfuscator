using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.Aspects;
using Logic.Business.LuaDeobfuscator.InternalContract.Exceptions;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;

namespace Logic.Business.LuaDeobfuscator.InternalContract
{
    [MapException(typeof(LuaDeobfuscatorException))]
    public interface ILuaDeobfuscator
    {
        void Deobfuscate(CodeUnitSyntax syntax);
    }
}
