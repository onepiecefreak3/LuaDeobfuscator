using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class SimpleNameSyntax : NameSyntax
    {
        public SyntaxToken Identifier { get; private set; }

        public override Location Location => Identifier.FullLocation;

        public SimpleNameSyntax(SyntaxToken identifier)
        {
            identifier.Parent = this;

            Identifier = identifier;
        }

        public void SetIdentifier(SyntaxToken identifier, bool updatePositions = true)
        {
            identifier.Parent = this;

            Identifier = identifier;

            if (updatePositions)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken identifier = Identifier;

            position = identifier.UpdatePosition(position);

            Identifier = identifier;

            return position;
        }
    }
}
