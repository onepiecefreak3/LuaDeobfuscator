using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class BreakStatement : StatementSyntax
    {
        public SyntaxToken Break { get; private set; }

        public override Location Location => Break.FullLocation;

        public BreakStatement(SyntaxToken breakToken)
        {
            breakToken.Parent = this;

            Break = breakToken;
        }

        internal override int UpdatePosition(int position = 0)
        {
            var breakToken = Break;

            position= breakToken.UpdatePosition(position);

            Break = breakToken;

            return position;
        }
    }
}
