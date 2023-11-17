using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class QualifiedNameSyntax : NameSyntax
    {
        public NameSyntax Left { get; private set; }
        public SyntaxToken Dot { get; private set; }
        public NameSyntax Right { get; private set; }

        public override Location Location => new(Left.Location.Position, Right.Location.EndPosition);

        public QualifiedNameSyntax(NameSyntax left, SyntaxToken dot, NameSyntax right)
        {
            left.Parent = this;
            dot.Parent = this;
            right.Parent = this;

            Left = left;
            Dot = dot;
            Right = right;
        }

        public void SetLeft(NameSyntax name, bool updatePositions = true)
        {
            name.Parent = this;

            Left = name;

            if (updatePositions)
                Root.UpdatePosition();
        }

        public void SetDot(SyntaxToken dot, bool updatePositions = true)
        {
            dot.Parent = this;

            Dot = dot;

            if (updatePositions)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken dot = Dot;

            position = Left.UpdatePosition(position);
            position = dot.UpdatePosition(position);
            position = Right.UpdatePosition(position);

            Dot = dot;

            return position;
        }
    }
}
