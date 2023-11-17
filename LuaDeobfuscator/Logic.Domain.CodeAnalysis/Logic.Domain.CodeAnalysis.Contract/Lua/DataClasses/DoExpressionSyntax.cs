using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class DoExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken DoToken { get; private set; }
        public BlockExpressionSyntax Body { get; private set; }

        public override Location Location => new(DoToken.FullLocation.Position, Body.Location.EndPosition);

        public DoExpressionSyntax(SyntaxToken doTokenToken, BlockExpressionSyntax body)
        {
            doTokenToken.Parent = this;
            body.Parent = this;
            
            DoToken = doTokenToken;
            Body = body;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken doToken = DoToken;

            position = doToken.UpdatePosition(position);
            position = Body.UpdatePosition(position);

            DoToken = doToken;

            return position;
        }
    }
}
