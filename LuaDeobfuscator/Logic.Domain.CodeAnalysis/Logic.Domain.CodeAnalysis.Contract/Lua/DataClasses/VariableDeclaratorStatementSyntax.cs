using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class VariableDeclaratorStatementSyntax : StatementSyntax
    {
        public SyntaxToken LocalKeyword { get; private set; }
        public CommaSeparatedSyntaxList<NameSyntax> Variables { get; private set; }

        public override Location Location => new(LocalKeyword.FullLocation.Position, Variables.Location.EndPosition);

        public VariableDeclaratorStatementSyntax(SyntaxToken localToken, CommaSeparatedSyntaxList<NameSyntax> variables)
        {
            localToken.Parent = this;
            variables.Parent = this;

            LocalKeyword = localToken;
            Variables = variables;
        }

        public void SetLocalKeyword(SyntaxToken localToken, bool updatePosition = true)
        {
            localToken.Parent = this;

            LocalKeyword = localToken;

            if (updatePosition)
                Root.UpdatePosition();
        }

        public void SetIdentifiers(CommaSeparatedSyntaxList<NameSyntax> variables, bool updatePosition = true)
        {
            variables.Parent = this;

            Variables = variables;

            if (updatePosition)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken localKeyword = LocalKeyword;

            position = localKeyword.UpdatePosition(position);
            position = Variables.UpdatePosition(position);

            LocalKeyword = localKeyword;

            return position;
        }
    }
}
