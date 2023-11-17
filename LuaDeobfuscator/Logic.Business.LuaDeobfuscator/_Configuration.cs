using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.Configuration.DataClasses;

namespace Logic.Business.LuaDeobfuscator
{
    public class LuaDeobfuscatorConfiguration
    {
        [ConfigMap("CommandLine", new[] { "f", "file" })]
        public virtual string FilePath { get; set; }
    }
}