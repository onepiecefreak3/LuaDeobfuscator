using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class BlockExpressionSyntax : ExpressionSyntax
    {
        public IReadOnlyList<StatementSyntax> Statements { get; private set; }

        public override Location Location => new(Statements.Count <= 0 ? 0 : Statements[0].Location.Position,
            Statements.Count <= 0 ? 0 : Statements[^1].Location.Position);

        public BlockExpressionSyntax(IReadOnlyList<StatementSyntax>? statements)
        {
            Statements = statements ?? Array.Empty<StatementSyntax>();
            foreach (var statement in Statements)
                statement.Parent = this;
        }

        public void SetStatements(IReadOnlyList<StatementSyntax>? statements, bool updatePositions = true)
        {
            Statements = statements ?? Array.Empty<StatementSyntax>();
            foreach (var statement in Statements)
                statement.Parent = this;

            if (updatePositions)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            foreach (var statement in Statements)
                position = statement.UpdatePosition(position);

            return position;
        }
    }
}
