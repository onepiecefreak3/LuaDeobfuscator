using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.DataClasses
{
    public abstract class SyntaxNode
    {
        public SyntaxNode? Parent { get; internal set; }
        public SyntaxNode Root => Parent?.Root ?? this;

        public abstract Location Location { get; }

        public void Update() => UpdatePosition();
        internal abstract int UpdatePosition(int position = 0);
    }
}
