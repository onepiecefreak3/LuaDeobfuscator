using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.DataClasses
{
    [DebuggerDisplay("[{FullLocation.Position}..{FullLocation.EndPosition}) {Text}")]
    public struct SyntaxToken
    {
        private int _textPosition;

        public SyntaxNode? Parent { get; internal set; }

        public int RawKind { get; }
        public string Text { get; }

        public SyntaxTokenTrivia? LeadingTrivia { get; private set; }
        public SyntaxTokenTrivia? TrailingTrivia { get; private set; }

        public Location Location => new(_textPosition, _textPosition + Text.Length);
        public Location FullLocation => new(LeadingTrivia?.Location.Position ?? _textPosition, TrailingTrivia?.Location.EndPosition ?? _textPosition + Text.Length);

        public SyntaxToken(string text, int rawKind, SyntaxTokenTrivia? leadingTrivia = null, SyntaxTokenTrivia? trailingTrivia = null)
        {
            RawKind = rawKind;
            Text = text;

            LeadingTrivia = leadingTrivia;
            TrailingTrivia = trailingTrivia;
        }

        public SyntaxToken WithLeadingTrivia(string? trivia)
        {
            LeadingTrivia = trivia == null ? null : new SyntaxTokenTrivia(trivia);

            return this;
        }

        public SyntaxToken WithTrailingTrivia(string? trivia)
        {
            TrailingTrivia = trivia == null ? null : new SyntaxTokenTrivia(trivia);

            return this;
        }

        public SyntaxToken WithNoTrivia()
        {
            LeadingTrivia = null;
            TrailingTrivia = null;

            return this;
        }

        internal int UpdatePosition(int fullPosition)
        {
            if (LeadingTrivia.HasValue)
            {
                LeadingTrivia = new SyntaxTokenTrivia(LeadingTrivia.Value.Text, fullPosition);
                fullPosition += LeadingTrivia.Value.Text.Length;
            }

            _textPosition = fullPosition;
            fullPosition += Text.Length;

            if (TrailingTrivia.HasValue)
            {
                TrailingTrivia = new SyntaxTokenTrivia(TrailingTrivia.Value.Text, fullPosition);
                fullPosition += TrailingTrivia.Value.Text.Length;
            }

            return fullPosition;
        }
    }
}
