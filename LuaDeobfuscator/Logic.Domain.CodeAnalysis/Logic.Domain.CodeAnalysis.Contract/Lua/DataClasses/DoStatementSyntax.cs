using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class DoStatementSyntax : StatementSyntax
    {
        public DoExpressionSyntax Do { get; private set; }
        public SyntaxToken End { get; private set; }

        public override Location Location => new(Do.Location.Position, End.FullLocation.EndPosition);

        public DoStatementSyntax(DoExpressionSyntax doExpression, SyntaxToken end)
        {
            doExpression.Parent = this;
            end.Parent = this;

            Do = doExpression;
            End = end;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken end = End;

            position = Do.UpdatePosition(position);
            position = end.UpdatePosition(position);

            End = end;

            return position;
        }
    }
}
