using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class WhileExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken While { get; private set; }
        public ExpressionSyntax CompareExpression { get; private set; }

        public override Location Location => new(While.FullLocation.Position, CompareExpression.Location.EndPosition);

        public WhileExpressionSyntax(SyntaxToken whileToken, ExpressionSyntax compare)
        {
            whileToken.Parent = this;
            compare.Parent = this;

            While = whileToken;
            CompareExpression = compare;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken whileToken = While;

            position = whileToken.UpdatePosition(position);
            position = CompareExpression.UpdatePosition(position);

            While = whileToken;

            return position;
        }
    }
}
