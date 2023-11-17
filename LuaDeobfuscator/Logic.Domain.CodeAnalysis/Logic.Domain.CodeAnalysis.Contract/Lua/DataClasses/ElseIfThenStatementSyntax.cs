using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class ElseIfThenStatementSyntax : StatementSyntax
    {
        public ElseIfExpressionSyntax ElseIf { get; private set; }
        public ThenExpressionSyntax Then { get; private set; }

        public override Location Location => new(ElseIf.Location.Position, Then.Location.EndPosition);

        public ElseIfThenStatementSyntax(ElseIfExpressionSyntax elseIf, ThenExpressionSyntax thenExpression)
        {
            elseIf.Parent = this;
            thenExpression.Parent = this;

            ElseIf = elseIf;
            Then = thenExpression;
        }

        internal override int UpdatePosition(int position = 0)
        {
            position = ElseIf.UpdatePosition(position);
            position = Then.UpdatePosition(position);

            return position;
        }
    }
}
