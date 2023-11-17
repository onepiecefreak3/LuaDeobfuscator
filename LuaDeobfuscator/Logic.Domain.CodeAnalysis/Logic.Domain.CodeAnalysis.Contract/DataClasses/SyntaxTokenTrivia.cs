using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.DataClasses
{
    public readonly struct SyntaxTokenTrivia
    {
        private readonly int _position;

        public string Text { get; }
        public Location Location => new(_position, _position + Text.Length);

        internal SyntaxTokenTrivia(string text, int position) : this(text)
        {
            _position = position;
        }

        public SyntaxTokenTrivia(string text)
        {
            Text = text;
        }
    }
}
