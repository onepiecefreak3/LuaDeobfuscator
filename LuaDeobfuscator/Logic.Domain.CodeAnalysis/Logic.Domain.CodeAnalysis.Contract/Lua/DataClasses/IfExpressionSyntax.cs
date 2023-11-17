using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class IfExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken If { get; private set; }
        public ExpressionSyntax CompareExpression { get; private set; }

        public override Location Location => new(If.FullLocation.Position, CompareExpression.Location.EndPosition);

        public IfExpressionSyntax(SyntaxToken ifToken, ExpressionSyntax compare)
        {
            ifToken.Parent = this;
            compare.Parent = this;

            If = ifToken;
            CompareExpression = compare;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken ifToken = If;

            position = ifToken.UpdatePosition(position);
            position = CompareExpression.UpdatePosition(position);

            If = ifToken;

            return position;
        }
    }
}
