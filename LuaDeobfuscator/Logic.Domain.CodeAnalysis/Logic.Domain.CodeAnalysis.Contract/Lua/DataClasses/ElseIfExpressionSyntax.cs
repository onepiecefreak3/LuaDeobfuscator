using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class ElseIfExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken ElseIf { get; private set; }
        public ExpressionSyntax CompareExpression { get; private set; }

        public override Location Location => new(ElseIf.FullLocation.Position, CompareExpression.Location.EndPosition);

        public ElseIfExpressionSyntax(SyntaxToken elseIf, ExpressionSyntax compare)
        {
            elseIf.Parent = this;
            compare.Parent = this;

            ElseIf = elseIf;
            CompareExpression = compare;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken elseIf = ElseIf;

            position = elseIf.UpdatePosition(position);
            position = CompareExpression.UpdatePosition(position);

            ElseIf = elseIf;

            return position;
        }
    }
}
