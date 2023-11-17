using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken ParenOpen { get; private set; }
        public ExpressionSyntax Expression { get; private set; }
        public SyntaxToken ParenClose { get; private set; }

        public override Location Location => new(ParenOpen.FullLocation.Position, ParenClose.FullLocation.EndPosition);

        public ParenthesizedExpressionSyntax(SyntaxToken parenOpen, ExpressionSyntax expression, SyntaxToken parenClose)
        {
            parenOpen.Parent = this;
            expression.Parent = this;
            parenClose.Parent = this;

            ParenOpen = parenOpen;
            Expression = expression;
            ParenClose = parenClose;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken parenOpen = ParenOpen;
            SyntaxToken parenClose = ParenClose;

            position = parenOpen.UpdatePosition(position);
            position = Expression.UpdatePosition(position);
            position = parenClose.UpdatePosition(position);

            ParenOpen = parenOpen;
            ParenClose = parenClose;

            return position;
        }
    }
}
