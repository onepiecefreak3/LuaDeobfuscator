using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class ReturnStatementSyntax : StatementSyntax
    {
        public SyntaxToken ReturnToken { get; private set; }
        public ExpressionSyntax? Expression { get; private set; }

        public override Location Location => new(ReturnToken.FullLocation.Position, Expression?.Location.EndPosition ?? ReturnToken.FullLocation.EndPosition);

        public ReturnStatementSyntax(SyntaxToken returnToken, ExpressionSyntax? expression = null)
        {
            returnToken.Parent = this;
            if (expression != null)
                expression.Parent = this;

            ReturnToken = returnToken;
            Expression = expression;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken returnToken = ReturnToken;

            position = returnToken.UpdatePosition(position);
            if (Expression != null)
                position = Expression.UpdatePosition(position);

            ReturnToken = returnToken;

            return position;
        }
    }
}
