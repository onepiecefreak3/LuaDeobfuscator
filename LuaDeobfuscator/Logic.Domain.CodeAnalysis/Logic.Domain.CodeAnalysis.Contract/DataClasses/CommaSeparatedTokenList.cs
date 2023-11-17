using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Logic.Domain.CodeAnalysis.Contract.DataClasses
{
    public class CommaSeparatedTokenList : SyntaxNode
    {
        private SyntaxToken[] _elements;

        public IReadOnlyList<SyntaxToken> Elements => _elements;

        public override Location Location => Elements.Count <= 0 ? new() : new(Elements[0].Location.Position, Elements[^1].Location.EndPosition);

        public CommaSeparatedTokenList(SyntaxToken[]? elements)
        {
            for (var i = 0; i < (elements?.Length ?? 0); i++)
            {
                SyntaxToken parameter = elements![i];
                parameter.Parent = this;

                elements[i] = parameter;
            }

            _elements = elements ?? Array.Empty<SyntaxToken>();

            Root.UpdatePosition();
        }

        public void SetElements(SyntaxToken[]? elements, bool updatePosition = true)
        {
            for (var i = 0; i < (elements?.Length ?? 0); i++)
            {
                SyntaxToken parameter = elements![i];
                parameter.Parent = this;

                elements[i] = parameter;
            }

            _elements = elements ?? Array.Empty<SyntaxToken>();

            if (updatePosition)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            for (var i = 0; i < _elements.Length; i++)
            {
                SyntaxToken parameter = _elements[i];

                position = parameter.UpdatePosition(position);
                position++;

                _elements[i] = parameter;
            }

            return position;
        }
    }
}
