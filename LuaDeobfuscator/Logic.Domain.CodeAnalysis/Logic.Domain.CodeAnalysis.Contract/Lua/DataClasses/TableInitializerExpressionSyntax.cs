using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class TableInitializerExpressionSyntax : ExpressionSyntax
    {
        public SyntaxToken CurlyOpen { get; private set; }
        public SyntaxToken CurlyClose { get; private set; }

        public override Location Location => new(CurlyOpen.FullLocation.Position, CurlyClose.FullLocation.EndPosition);

        public TableInitializerExpressionSyntax(SyntaxToken curlyOpen, SyntaxToken curlyClose)
        {
            curlyOpen.Parent = this;
            curlyClose.Parent = this;

            CurlyOpen = curlyOpen;
            CurlyClose = curlyClose;
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken curlyOpen = CurlyOpen;
            SyntaxToken curlyClose = CurlyClose;

            position = curlyOpen.UpdatePosition(position);
            position = curlyClose.UpdatePosition(position);

            CurlyOpen = curlyOpen;
            CurlyClose = curlyClose;

            return position;
        }
    }
}
