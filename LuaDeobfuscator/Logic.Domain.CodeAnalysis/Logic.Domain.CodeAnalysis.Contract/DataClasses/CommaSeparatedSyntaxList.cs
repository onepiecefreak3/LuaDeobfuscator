using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.DataClasses
{
    public class CommaSeparatedSyntaxList<TNode> : SyntaxNode
        where TNode : SyntaxNode
    {
        public IReadOnlyList<TNode> Elements { get; private set; }

        public override Location Location => Elements.Count <= 0 ? new() : new(Elements[0].Location.Position, Elements[^1].Location.EndPosition);

        public CommaSeparatedSyntaxList(IReadOnlyList<TNode>? elements)
        {
            Elements = elements ?? Array.Empty<TNode>();

            foreach (TNode parameter in Elements)
                parameter.Parent = this;

            Root.UpdatePosition();
        }

        public void SetElements(IReadOnlyList<TNode>? elements, bool updatePosition = true)
        {
            Elements = elements ?? Array.Empty<TNode>();

            foreach (TNode parameter in Elements)
                parameter.Parent = this;

            if (updatePosition)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            foreach (TNode parameter in Elements)
            {
                position = parameter.UpdatePosition(position);
                position++;
            }

            return position;
        }
    }
}
