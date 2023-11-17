using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class ThenExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken Then { get; private set; }
        public BlockExpressionSyntax Body { get; private set; }

        public override Location Location => new(Then.FullLocation.Position, Body.Location.EndPosition);

        public ThenExpressionSyntax(SyntaxToken thenToken, BlockExpressionSyntax body)
        {
            thenToken.Parent = this;
            body.Parent = this;

            Then = thenToken;
            Body = body;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken thenToken = Then;

            position = thenToken.UpdatePosition(position);
            position = Body.UpdatePosition(position);

            Then = thenToken;

            return position;
        }
    }
}
