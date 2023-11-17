using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis
{
    public abstract class Parser<TKind>
        where TKind : struct
    {
        private readonly ITokenFactory<LexerToken<TKind>> _scriptFactory;
        private readonly ISyntaxFactory<TKind> _syntaxFactory;

        protected abstract TKind TriviaKind { get; }

        public Parser(ITokenFactory<LexerToken<TKind>> scriptFactory, ISyntaxFactory<TKind> syntaxFactory)
        {
            _scriptFactory = scriptFactory;
            _syntaxFactory = syntaxFactory;
        }

        protected SyntaxToken CreateToken(IBuffer<LexerToken<TKind>> buffer, TKind expectedKind)
        {
            SyntaxTokenTrivia? leadingTrivia = ReadTrivia(buffer);

            if (!buffer.Peek().Kind.Equals(expectedKind))
                throw CreateException(buffer, $"Unexpected token {buffer.Peek().Kind}.", expectedKind);
            LexerToken<TKind> content = buffer.Read();

            SyntaxTokenTrivia? trailingTrivia = ReadTrivia(buffer);

            return _syntaxFactory.Create(content.Text, expectedKind, leadingTrivia, trailingTrivia);
        }

        private SyntaxTokenTrivia? ReadTrivia(IBuffer<LexerToken<TKind>> buffer)
        {
            if (buffer.Peek().Kind.Equals(TriviaKind))
            {
                LexerToken<TKind> token = buffer.Read();
                return new SyntaxTokenTrivia(token.Text);
            }

            return null;
        }

        protected void SkipTokenKind(IBuffer<LexerToken<TKind>> buffer, TKind expectedKind)
        {
            int toSkip = 1;

            LexerToken<TKind> peekedToken = buffer.Peek();
            if (peekedToken.Kind.Equals(TriviaKind))
            {
                peekedToken = buffer.Peek(1);
                toSkip++;
            }

            if (!peekedToken.Kind.Equals(expectedKind))
                throw CreateException(buffer, $"Unexpected token {peekedToken.Kind}.", expectedKind);

            for (var i = 0; i < toSkip; i++)
                buffer.Read();
        }

        protected bool HasTokenKind(IBuffer<LexerToken<TKind>> buffer, TKind expectedKind)
        {
            return HasTokenKind(buffer, 0, expectedKind);
        }

        protected bool HasTokenKind(IBuffer<LexerToken<TKind>> buffer, int position, TKind expectedKind)
        {
            var toPeek = 0;
            LexerToken<TKind> peekedToken = buffer.Peek(toPeek);

            position = Math.Max(0, position);
            for (var i = 0; i < position + 1; i++)
            {
                peekedToken = buffer.Peek(toPeek++);
                if (peekedToken.Kind.Equals(TriviaKind))
                    peekedToken = buffer.Peek(toPeek++);
            }

            return peekedToken.Kind.Equals(expectedKind);
        }

        protected IBuffer<LexerToken<TKind>> CreateTokenBuffer(string text)
        {
            ILexer<LexerToken<TKind>> lexer = _scriptFactory.CreateLexer(text);
            return _scriptFactory.CreateTokenBuffer(lexer);
        }

        protected Exception CreateException(IBuffer<LexerToken<TKind>> buffer, string message,
            params TKind[] expected)
        {
            (int line, int column) = GetCurrentLineAndColumn(buffer);
            return CreateException(message, line, column, expected);
        }

        protected Exception CreateException(string message, int line, int column, params TKind[] expected)
        {
            message = $"{message} (Line {line}, Column {column})";

            if (expected.Length > 0)
            {
                message = expected.Length == 1
                    ? $"{message} (Expected {expected[0]})"
                    : $"{message} (Expected any of {string.Join(',', expected)})";
            }

            throw new InvalidOperationException(message);
        }

        private (int, int) GetCurrentLineAndColumn(IBuffer<LexerToken<TKind>> buffer)
        {
            var toPeek = 0;

            if (buffer.Peek().Kind.Equals(TriviaKind))
                toPeek++;

            LexerToken<TKind> token = buffer.Peek(toPeek);
            return (token.Line, token.Column);
        }
    }
}
