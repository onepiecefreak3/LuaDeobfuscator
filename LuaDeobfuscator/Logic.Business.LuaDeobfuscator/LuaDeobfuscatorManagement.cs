using Logic.Business.LuaDeobfuscator.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Business.LuaDeobfuscator.InternalContract;
using Logic.Domain.CodeAnalysis.Contract.Lua;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;

namespace Logic.Business.LuaDeobfuscator
{
    internal class LuaDeobfuscatorManagement : ILuaDeobfuscatorManagement
    {
        private readonly LuaDeobfuscatorConfiguration _config;
        private readonly ILuaDeobfuscatorConfigurationValidator _validator;
        private readonly ILuaParser _parser;
        private readonly ILuaDeobfuscator _deobfuscator;
        private readonly ILuaComposer _composer;

        public LuaDeobfuscatorManagement(LuaDeobfuscatorConfiguration config, ILuaDeobfuscatorConfigurationValidator validator,
            ILuaParser parser, ILuaDeobfuscator deobfuscator, ILuaComposer composer)
        {
            _config = config;
            _validator = validator;
            _parser = parser;
            _deobfuscator = deobfuscator;
            _composer = composer;
        }

        public int Execute()
        {
            _validator.Validate(_config);

            // Collect files to extract
            string[] files = Directory.Exists(_config.FilePath) ?
                Directory.GetFiles(_config.FilePath, "*.lua", SearchOption.AllDirectories) :
                new[] { _config.FilePath };

            // Extract files
            foreach (string file in files)
            {
                Console.Write($"Deobfuscate {file}: ");
                try
                {
                    DeobfuscateScript(file);
                    Console.WriteLine("Ok");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error ({e.Message})");
                }
            }

            return 0;
        }

        private void DeobfuscateScript(string filePath)
        {
            string text = File.ReadAllText(filePath);
            CodeUnitSyntax syntax = _parser.ParseCodeUnit(text);

            _deobfuscator.Deobfuscate(syntax);
            text = _composer.ComposeCodeUnit(syntax);

            File.WriteAllText(filePath + ".deob.lua", text);
        }
    }
}
