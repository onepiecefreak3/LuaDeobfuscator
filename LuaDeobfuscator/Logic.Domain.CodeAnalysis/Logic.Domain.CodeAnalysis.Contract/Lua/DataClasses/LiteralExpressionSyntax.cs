using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class LiteralExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken Literal { get; private set; }

        public override Location Location => Literal.FullLocation;

        public LiteralExpressionSyntax(SyntaxToken literal)
        {
            literal.Parent = this;

            Literal = literal;
        }

        public void SetLiteral(SyntaxToken literal, bool updatePositions = true)
        {
            literal.Parent = this;

            Literal = literal;

            if (updatePositions)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken literal = Literal;

            position = literal.UpdatePosition(position);

            Literal = literal;

            return position;
        }
    }
}
