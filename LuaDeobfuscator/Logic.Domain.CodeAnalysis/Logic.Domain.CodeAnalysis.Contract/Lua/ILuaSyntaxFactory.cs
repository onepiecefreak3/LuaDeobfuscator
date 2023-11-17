using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua.Exceptions;

namespace Logic.Domain.CodeAnalysis.Contract.Lua
{
    [MapException(typeof(LuaSyntaxFactoryException))]
    public interface ILuaSyntaxFactory : ISyntaxFactory<LuaTokenKind>
    {
    }
}
