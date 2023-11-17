using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Exceptions
{
    [Serializable]
    public class BufferFactoryException : Exception
    {
        public BufferFactoryException()
        {
        }

        public BufferFactoryException(string message) : base(message)
        {
        }

        public BufferFactoryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BufferFactoryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
