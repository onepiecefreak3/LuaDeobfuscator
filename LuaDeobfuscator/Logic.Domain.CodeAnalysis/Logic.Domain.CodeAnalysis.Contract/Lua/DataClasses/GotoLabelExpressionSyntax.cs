using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class GotoLabelExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken StartColon { get; private set; }
        public NameSyntax Name { get; private set; }
        public SyntaxToken EndColon { get; private set; }

        public override Location Location => new(StartColon.FullLocation.Position, EndColon.FullLocation.EndPosition);

        public GotoLabelExpressionSyntax(SyntaxToken startColon, NameSyntax name, SyntaxToken endColon)
        {
            startColon.Parent = this;
            name.Parent = this;
            endColon.Parent = this;

            StartColon = startColon;
            Name = name;
            EndColon = endColon;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken startColon = StartColon;
            SyntaxToken endColon = EndColon;

            position = startColon.UpdatePosition(position);
            position = Name.UpdatePosition(position);
            position = endColon.UpdatePosition(position);

            StartColon = startColon;
            EndColon = endColon;

            return position;
        }
    }
}
