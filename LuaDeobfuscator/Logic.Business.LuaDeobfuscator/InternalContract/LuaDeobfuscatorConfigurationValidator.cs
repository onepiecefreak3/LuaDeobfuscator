using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.Aspects;
using Logic.Business.LuaDeobfuscator.InternalContract.Exceptions;

namespace Logic.Business.LuaDeobfuscator.InternalContract
{
    [MapException(typeof(LuaDeobfuscatorConfigurationValidatorException))]
    public interface ILuaDeobfuscatorConfigurationValidator
    {
        void Validate(LuaDeobfuscatorConfiguration config);
    }
}
