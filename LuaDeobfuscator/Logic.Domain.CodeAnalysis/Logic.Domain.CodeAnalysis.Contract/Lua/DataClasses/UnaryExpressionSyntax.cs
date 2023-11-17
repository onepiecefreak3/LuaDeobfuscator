using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class UnaryExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken Operation { get; private set; }
        public ExpressionSyntax Expression { get; private set; }

        public override Location Location => new(Operation.FullLocation.Position, Expression.Location.EndPosition);

        public UnaryExpressionSyntax(SyntaxToken operation, ExpressionSyntax expression)
        {
            operation.Parent = this;
            expression.Parent = this;
            
            Operation = operation;
            Expression = expression;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken operation = Operation;
            
            position = operation.UpdatePosition(position);
            position = Expression.UpdatePosition(position);

            Operation = operation;

            return position;
        }
    }
}
