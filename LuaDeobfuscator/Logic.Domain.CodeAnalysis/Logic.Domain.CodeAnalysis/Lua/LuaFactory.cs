using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;

namespace Logic.Domain.CodeAnalysis.Lua
{
    internal class LuaFactory : ITokenFactory<LexerToken<LuaTokenKind>>
    {
        private readonly ICoCoKernel _kernel;
        private readonly IBufferFactory _bufferFactory;

        public LuaFactory(ICoCoKernel kernel, IBufferFactory bufferFactory)
        {
            _kernel = kernel;
            _bufferFactory = bufferFactory;
        }

        public ILexer<LexerToken<LuaTokenKind>> CreateLexer(string text)
        {
            IBuffer<int> buffer = _bufferFactory.CreateStringBuffer(text);
            return _kernel.Get<ILexer<LexerToken<LuaTokenKind>>>(
                new ConstructorParameter("buffer", buffer));
        }

        public IBuffer<LexerToken<LuaTokenKind>> CreateTokenBuffer(ILexer<LexerToken<LuaTokenKind>> lexer)
        {
            return _kernel.Get<IBuffer<LexerToken<LuaTokenKind>>>(new ConstructorParameter("lexer", lexer));
        }
    }
}
