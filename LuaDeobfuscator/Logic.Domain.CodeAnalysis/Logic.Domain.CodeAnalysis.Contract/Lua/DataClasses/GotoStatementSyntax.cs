using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class GotoStatementSyntax : StatementSyntax
    {
        public SyntaxToken Goto { get; private set; }
        public NameSyntax Label { get; private set; }

        public override Location Location => new(Goto.FullLocation.Position, Label.Location.EndPosition);

        public GotoStatementSyntax(SyntaxToken gotoToken, NameSyntax label)
        {
            gotoToken.Parent = this;
            label.Parent = this;

            Goto = gotoToken;
            Label = label;
        }

        internal override int UpdatePosition(int position = 0)
        {
            var gotoToken = Goto;

            position = gotoToken.UpdatePosition(position);
            position = Label.UpdatePosition(position);

            Goto = gotoToken;

            return position;
        }
    }
}
