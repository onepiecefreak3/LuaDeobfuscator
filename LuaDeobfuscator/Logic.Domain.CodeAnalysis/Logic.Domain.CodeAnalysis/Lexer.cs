using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis
{
    public abstract class Lexer<TKind> : ILexer<LexerToken<TKind>>
        where TKind : struct
    {
        private readonly IBuffer<int> _buffer;

        public bool IsEndOfInput => _buffer.IsEndOfInput;

        protected int Line { get; set; } = 1;
        protected int Column { get; set; } = 1;
        protected int Position { get; set; }

        public Lexer(IBuffer<int> buffer)
        {
            _buffer = buffer;
        }

        public abstract LexerToken<TKind> Read();

        protected LexerToken<TKind> CreateToken(TKind kind, string? text = null)
        {
            return new LexerToken<TKind>(kind, Position, Line, Column, text);
        }

        protected LexerToken<TKind> CreateToken(TKind kind, int position, int line, int column, string? text = null)
        {
            return new LexerToken<TKind>(kind, position, line, column, text);
        }

        protected bool IsPeekedChar(char expected)
        {
            return IsPeekedChar(0, expected);
        }

        protected bool IsPeekedChar(int position, char expected)
        {
            return TryPeekChar(position, out char character) && character == expected;
        }

        protected bool TryPeekChar(out char character)
        {
            return TryPeekChar(0, out character);
        }

        protected bool TryPeekChar(int position, out char character)
        {
            character = default;

            int result = _buffer.Peek(position);
            if (result < 0)
                return false;

            character = (char)result;
            return true;
        }

        protected char ReadChar()
        {
            int result = _buffer.Read();
            if (result < 0)
                throw CreateException("Could not read character.");

            if (result == '\n')
            {
                Line++;
                Column = 0;
            }

            if (result == '\t')
                Column += 3;

            Column++;
            Position++;

            return (char)result;
        }

        protected Exception CreateException(string message, string? expected = null)
        {
            message = $"{message} (Line {Line}, Column {Column})";

            if (!string.IsNullOrEmpty(expected))
                message = $"{message} (Expected \"{expected}\")";

            throw new InvalidOperationException(message);
        }
    }
}
