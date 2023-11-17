using CrossCutting.Core.Contract.Aspects;
using Logic.Business.LuaDeobfuscator.Contract.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Business.LuaDeobfuscator.Contract
{
    [MapException(typeof(LuaDeobfuscatorManagementException))]
    public interface ILuaDeobfuscatorManagement
    {
        int Execute();
    }
}
