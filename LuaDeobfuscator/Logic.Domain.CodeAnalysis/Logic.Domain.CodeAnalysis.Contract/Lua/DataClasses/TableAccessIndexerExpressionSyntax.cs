using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class TableAccessIndexerExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken BracketOpen { get; private set; }
        public ExpressionSyntax IndexExpression { get; private set; }
        public SyntaxToken BracketClose { get; private set; }

        public override Location Location => new(BracketOpen.FullLocation.Position, BracketClose.FullLocation.EndPosition);

        public TableAccessIndexerExpressionSyntax(SyntaxToken bracketOpen, ExpressionSyntax index, SyntaxToken bracketClose)
        {
            bracketOpen.Parent = this;
            index.Parent = this;
            bracketClose.Parent = this;

            BracketOpen = bracketOpen;
            IndexExpression = index;
            BracketClose = bracketClose;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken bracketOpen = BracketOpen;
            SyntaxToken bracketClose = BracketClose;

            position = bracketOpen.UpdatePosition(position);
            position = IndexExpression.UpdatePosition(position);
            position = bracketClose.UpdatePosition(position);

            BracketOpen = bracketOpen;
            BracketClose = bracketClose;

            return position;
        }
    }
}
