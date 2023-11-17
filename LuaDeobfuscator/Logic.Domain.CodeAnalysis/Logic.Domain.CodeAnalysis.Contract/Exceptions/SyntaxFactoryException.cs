using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Exceptions
{
    [Serializable]
    public class SyntaxFactoryException : Exception
    {
        public SyntaxFactoryException()
        {
        }

        public SyntaxFactoryException(string message) : base(message)
        {
        }

        public SyntaxFactoryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SyntaxFactoryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
