using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class IfThenElseStatementSyntax : StatementSyntax
    {
        public IfExpressionSyntax If { get; private set; }
        public ThenExpressionSyntax Then { get; private set; }
        public IReadOnlyList<ElseIfThenStatementSyntax> ElseIfs { get; private set; }
        public SyntaxToken End { get; private set; }

        public override Location Location => new(If.Location.Position, End.FullLocation.EndPosition);

        public IfThenElseStatementSyntax(IfExpressionSyntax ifExpression, ThenExpressionSyntax thenExpression, IReadOnlyList<ElseIfThenStatementSyntax>? elseIfs, SyntaxToken end)
        {
            ifExpression.Parent = this;
            thenExpression.Parent = this;
            if (elseIfs != null)
                foreach (var elseIf in elseIfs)
                    elseIf.Parent = this;
            end.Parent = this;

            If = ifExpression;
            Then = thenExpression;
            ElseIfs = elseIfs ?? Array.Empty<ElseIfThenStatementSyntax>();
            End = end;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken end = End;

            position = If.UpdatePosition(position);
            position = Then.UpdatePosition(position);
            foreach (var elseIf in ElseIfs)
                position = elseIf.UpdatePosition(position);
            position = end.UpdatePosition(position);

            return position;
        }
    }
}
