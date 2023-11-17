using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.CodeAnalysis.Contract.Exceptions;

namespace Logic.Domain.CodeAnalysis.Contract
{
    [MapException(typeof(BufferFactoryException))]
    public interface IBufferFactory
    {
        IBuffer<int> CreateStringBuffer(string input);
    }
}
