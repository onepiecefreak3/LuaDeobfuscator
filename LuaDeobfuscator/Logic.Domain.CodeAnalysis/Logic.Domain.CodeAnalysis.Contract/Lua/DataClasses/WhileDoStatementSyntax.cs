using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class WhileDoStatementSyntax : StatementSyntax
    {
        public WhileExpressionSyntax While { get; private set; }
        public DoStatementSyntax Do { get; private set; }

        public override Location Location => new(While.Location.Position, Do.Location.EndPosition);

        public WhileDoStatementSyntax(WhileExpressionSyntax whileExpression, DoStatementSyntax doExpression)
        {
            whileExpression.Parent = this;
            doExpression.Parent = this;

            While = whileExpression;
            Do = doExpression;
        }

        internal override int UpdatePosition(int position = 0)
        {
            position = While.UpdatePosition(position);
            position = Do.UpdatePosition(position);

            return position;
        }
    }
}
