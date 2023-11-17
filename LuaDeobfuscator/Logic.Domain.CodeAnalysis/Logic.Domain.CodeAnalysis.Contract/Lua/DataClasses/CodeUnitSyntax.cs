using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class CodeUnitSyntax : SyntaxNode
    {
        public IReadOnlyList<StatementSyntax> Statements { get; private set; }

        public override Location Location => new(Statements.Count > 0 ? Statements[0].Location.Position : 0,
            Statements.Count > 0 ? Statements[^1].Location.EndPosition : 0);

        public CodeUnitSyntax(IReadOnlyList<StatementSyntax>? statements)
        {
            if (statements != null)
                foreach (StatementSyntax statement in statements)
                    statement.Parent = this;

            Statements = statements ?? Array.Empty<StatementSyntax>();
        }

        public void SetStatements(IReadOnlyList<StatementSyntax>? statements, bool updatePosition = true)
        {
            Statements = statements ?? Array.Empty<StatementSyntax>();
            foreach (StatementSyntax statement in Statements)
                statement.Parent = this;

            if (updatePosition)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            foreach (StatementSyntax statement in Statements)
                position = statement.UpdatePosition(position);

            return position;
        }
    }
}
