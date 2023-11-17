using CrossCutting.Core.Contract.Bootstrapping;
using CrossCutting.Core.Contract.Configuration;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using CrossCutting.Core.Contract.EventBrokerage;
using Logic.Business.LuaDeobfuscator.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Business.LuaDeobfuscator.InternalContract;

namespace Logic.Business.LuaDeobfuscator
{
    public class LuaDeobfuscatorActivator : IComponentActivator
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
            kernel.Register<ILuaDeobfuscatorManagement, LuaDeobfuscatorManagement>(ActivationScope.Unique);
            kernel.Register<ILuaDeobfuscatorConfigurationValidator, LuaDeobfuscatorConfigurationValidator>(ActivationScope.Unique);
            kernel.Register<ILuaDeobfuscator, LuaDeobfuscator>(ActivationScope.Unique);

            kernel.RegisterConfiguration<LuaDeobfuscatorConfiguration>();
        }

        public void AddMessageSubscriptions(IEventBroker broker)
        {
        }

        public void Configure(IConfigurator configurator)
        {
        }
    }
}
