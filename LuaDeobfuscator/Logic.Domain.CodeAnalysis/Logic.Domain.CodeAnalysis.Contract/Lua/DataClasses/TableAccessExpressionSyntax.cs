using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class TableAccessExpressionSyntax : ExpressionSyntax
    {
        public NameSyntax Name { get; private set; }
        public TableAccessIndexerExpressionSyntax Indexer { get; private set; }

        public override Location Location => new(Name.Location.Position, Indexer.Location.EndPosition);

        public TableAccessExpressionSyntax(NameSyntax name, TableAccessIndexerExpressionSyntax indexer)
        {
            name.Parent = this;
            indexer.Parent = this;

            Name = name;
            Indexer = indexer;
        }

        internal override int UpdatePosition(int position = 0)
        {
            position = Name.UpdatePosition(position);
            position = Indexer.UpdatePosition(position);

            return position;
        }
    }
}
