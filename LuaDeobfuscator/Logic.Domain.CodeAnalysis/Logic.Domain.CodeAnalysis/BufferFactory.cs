using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.DependencyInjection;
using CrossCutting.Core.Contract.DependencyInjection.DataClasses;
using Logic.Domain.CodeAnalysis.Contract;

namespace Logic.Domain.CodeAnalysis
{
    internal class BufferFactory : IBufferFactory
    {
        private readonly ICoCoKernel _kernel;

        public BufferFactory(ICoCoKernel kernel)
        {
            _kernel = kernel;
        }

        public IBuffer<int> CreateStringBuffer(string input)
        {
            return _kernel.Get<IBuffer<int>>(new ConstructorParameter("text", input));
        }
    }
}
