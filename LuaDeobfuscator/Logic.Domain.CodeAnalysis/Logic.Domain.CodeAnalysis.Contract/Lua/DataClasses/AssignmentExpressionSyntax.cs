using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Left { get; protected set; }
        public SyntaxToken Operation { get; private set; }
        public ExpressionSyntax Right { get; protected set; }

        public override Location Location => new(Left.Location.Position, Right.Location.EndPosition);

        public AssignmentExpressionSyntax(ExpressionSyntax left, SyntaxToken operation, ExpressionSyntax right)
        {
            left.Parent = this;
            operation.Parent = this;
            right.Parent = this;

            Left = left;
            Operation = operation;
            Right = right;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken operation = Operation;

            position = Left.UpdatePosition(position);
            position=operation.UpdatePosition(position);
            position = Right.UpdatePosition(position);

            Operation = operation;

            return position;
        }
    }
}
