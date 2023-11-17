using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.EventBrokerage;
using CrossCutting.Core.Contract.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;
using Logic.Domain.CodeAnalysis.Lua;

namespace Logic.Domain.CodeAnalysis
{
    public class CodeAnalysisActivator : IComponentActivator
    {
        public void Activating()
        {
        }

        public void Activated()
        {
        }

        public void Deactivating()
        {
        }

        public void Deactivated()
        {
        }

        public void Register(ICoCoKernel kernel)
        {
            kernel.Register<ITokenFactory<LexerToken<LuaTokenKind>>, LuaFactory>(ActivationScope.Unique);
            kernel.Register<ILexer<LexerToken<LuaTokenKind>>, LuaLexer>();
            kernel.Register<IBuffer<LexerToken<LuaTokenKind>>, TokenBuffer<LexerToken<LuaTokenKind>>>();

            kernel.Register<ILuaParser, LuaParser>(ActivationScope.Unique);
            kernel.Register<ILuaComposer, LuaComposer>(ActivationScope.Unique);

            kernel.Register<ILuaSyntaxFactory, LuaSyntaxFactory>();

            kernel.Register<IBufferFactory, BufferFactory>(ActivationScope.Unique);
            kernel.Register<IBuffer<int>, StringBuffer>();

            kernel.RegisterConfiguration<CodeAnalysisConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
