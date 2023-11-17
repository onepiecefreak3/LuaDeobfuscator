using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Core.Contract.Aspects;
using Logic.Domain.CodeAnalysis.Contract.Exceptions;

namespace Logic.Domain.CodeAnalysis.Contract
{
    [MapException(typeof(SyntaxFactoryException))]
    public interface ISyntaxFactory<in TKind>
        where TKind : struct
    {
        SyntaxToken Create(string text, TKind kind, SyntaxTokenTrivia? leadingTrivia = null, SyntaxTokenTrivia? trailingTrivia = null);

        SyntaxToken Token(TKind kind);
    }
}
