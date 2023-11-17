using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;

namespace Logic.Domain.CodeAnalysis.Lua
{
    public class LuaLexer : Lexer<LuaTokenKind>
    {
        private readonly StringBuilder _sb;

        public LuaLexer(IBuffer<int> buffer) : base(buffer)
        {
            _sb = new StringBuilder();
        }

        public override LexerToken<LuaTokenKind> Read()
        {
            if (!TryPeekChar(out char character))
                return CreateToken(LuaTokenKind.EndOfFile);

            switch (character)
            {
                case '(':
                    return CreateToken(LuaTokenKind.ParenOpen, $"{ReadChar()}");
                case ')':
                    return CreateToken(LuaTokenKind.ParenClose, $"{ReadChar()}");
                case '{':
                    return CreateToken(LuaTokenKind.CurlyOpen, $"{ReadChar()}");
                case '}':
                    return CreateToken(LuaTokenKind.CurlyClose, $"{ReadChar()}");
                case '[':
                    return CreateToken(LuaTokenKind.BracketOpen, $"{ReadChar()}");
                case ']':
                    return CreateToken(LuaTokenKind.BracketClose, $"{ReadChar()}");

                case '<':
                    if (TryPeekChar(1, out character) && character == '=')
                        return CreateToken(LuaTokenKind.SmallerEquals, $"{ReadChar()}{ReadChar()}");
                    return CreateToken(LuaTokenKind.SmallerThan, $"{ReadChar()}");
                case '>':
                    if (TryPeekChar(1, out character) && character == '=')
                        return CreateToken(LuaTokenKind.GreaterEquals, $"{ReadChar()}{ReadChar()}");
                    return CreateToken(LuaTokenKind.GreaterThan, $"{ReadChar()}");
                case '=':
                    if (TryPeekChar(1, out character) && character == '=')
                        return CreateToken(LuaTokenKind.EqualsEquals, $"{ReadChar()}{ReadChar()}");
                    return CreateToken(LuaTokenKind.Equals, $"{ReadChar()}");

                case '.':
                    if (TryPeekChar(1, out character) && character is >= '0' and <= '9')
                        goto case '0';
                    if (TryPeekChar(1, out character) && character == '.')
                        return CreateToken(LuaTokenKind.DotDot, $"{ReadChar()}{ReadChar()}");
                    return CreateToken(LuaTokenKind.Dot, $"{ReadChar()}");
                case ':':
                    if (TryPeekChar(1, out character) && character == ':')
                        return CreateToken(LuaTokenKind.ColonColon, $"{ReadChar()}{ReadChar()}");
                    return CreateToken(LuaTokenKind.Colon, $"{ReadChar()}");
                case ',':
                    return CreateToken(LuaTokenKind.Comma, $"{ReadChar()}");
                case '#':
                    return CreateToken(LuaTokenKind.Hashtag, $"{ReadChar()}");
                case '+':
                    return CreateToken(LuaTokenKind.Plus, $"{ReadChar()}");
                case '-':
                    if (TryPeekChar(1, out character) && character is >= '0' and <= '9')
                        goto case '0';
                    return CreateToken(LuaTokenKind.Minus, $"{ReadChar()}");
                case '*':
                    return CreateToken(LuaTokenKind.Asterisk, $"{ReadChar()}");
                case '/':
                    return CreateToken(LuaTokenKind.Slash, $"{ReadChar()}");
                case '%':
                    return CreateToken(LuaTokenKind.Percent, $"{ReadChar()}");
                case '~':
                    if (TryPeekChar(1, out character) && character == '=')
                        return CreateToken(LuaTokenKind.NotEquals, $"{ReadChar()}{ReadChar()}");
                    return CreateToken(LuaTokenKind.Tilde, $"{ReadChar()}");

                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    return ReadTrivia();

                case '"':
                    return ReadStringLiteral();

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ReadNumericLiteral();

                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                case '_':
                    return ReadIdentifierOrKeyword();
            }

            throw CreateException("Invalid character.");
        }

        private LexerToken<LuaTokenKind> ReadTrivia()
        {
            int position = Position;
            int line = Line;
            int column = Column;

            _sb.Clear();

            while (TryPeekChar(out char character))
            {
                switch (character)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                        _sb.Append(ReadChar());
                        continue;
                }

                break;
            }

            return CreateToken(LuaTokenKind.Trivia, position, line, column, _sb.ToString());
        }

        private LexerToken<LuaTokenKind> ReadStringLiteral()
        {
            int position = Position;
            int line = Line;
            int column = Column;

            _sb.Clear();

            if (!IsPeekedChar('"'))
                throw CreateException("Invalid string literal start.", "\"");

            _sb.Append(ReadChar());

            while (!IsPeekedChar('"'))
            {
                if (IsPeekedChar('\\'))
                    _sb.Append(ReadChar());

                _sb.Append(ReadChar());
            }

            if (IsEndOfInput)
                throw CreateException("Invalid string literal end.", "\"");

            _sb.Append(ReadChar());

            return CreateToken(LuaTokenKind.StringLiteral, position, line, column, _sb.ToString());
        }

        private LexerToken<LuaTokenKind> ReadNumericLiteral()
        {
            int position = Position;
            int line = Line;
            int column = Column;

            _sb.Clear();

            var hasDot = false;
            int dotColumn = Column;
            var kind = LuaTokenKind.NumericLiteral;

            while (TryPeekChar(out char character))
            {
                switch (character)
                {
                    case '-':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        _sb.Append(ReadChar());
                        continue;

                    case '.':
                        if (hasDot)
                            throw CreateException("Invalid floating numeric literal with multiple dots.");

                        hasDot = true;
                        dotColumn = Column;
                        kind = LuaTokenKind.FloatingNumericLiteral;

                        _sb.Append(ReadChar());
                        continue;
                }

                break;
            }

            if (hasDot && dotColumn == Column - 1)
                throw CreateException("Floating numeric value misses fractional part.");

            return CreateToken(kind, position, line, column, _sb.ToString());
        }

        private LexerToken<LuaTokenKind> ReadIdentifierOrKeyword()
        {
            int position = Position;
            int line = Line;
            int column = Column;

            _sb.Clear();

            var firstChar = true;
            while (TryPeekChar(out char character))
            {
                switch (character)
                {
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                    case '_':
                        firstChar = false;

                        _sb.Append(ReadChar());
                        continue;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (firstChar)
                            throw CreateException("Invalid identifier starting with numbers.");

                        firstChar = false;

                        _sb.Append(ReadChar());
                        continue;
                }

                if (firstChar)
                    throw CreateException("Invalid identifier.");

                break;
            }

            var finalValue = _sb.ToString();
            switch (finalValue)
            {
                case "local":
                    return CreateToken(LuaTokenKind.LocalKeyword, position, line, column, finalValue);

                case "function":
                    return CreateToken(LuaTokenKind.FunctionKeyword, position, line, column, finalValue);

                case "end":
                    return CreateToken(LuaTokenKind.EndKeyword, position, line, column, finalValue);

                case "if":
                    return CreateToken(LuaTokenKind.IfKeyword, position, line, column, finalValue);

                case "elseif":
                    return CreateToken(LuaTokenKind.ElseIfKeyword, position, line, column, finalValue);

                case "then":
                    return CreateToken(LuaTokenKind.ThenKeyword, position, line, column, finalValue);

                case "while":
                    return CreateToken(LuaTokenKind.WhileKeyword, position, line, column, finalValue);

                case "do":
                    return CreateToken(LuaTokenKind.DoKeyword, position, line, column, finalValue);

                case "break":
                    return CreateToken(LuaTokenKind.BreakKeyword, position, line, column, finalValue);

                case "true":
                    return CreateToken(LuaTokenKind.TrueKeyword, position, line, column, finalValue);

                case "false":
                    return CreateToken(LuaTokenKind.FalseKeyword, position, line, column, finalValue);

                case "not":
                    return CreateToken(LuaTokenKind.NotKeyword, position, line, column, finalValue);

                case "and":
                    return CreateToken(LuaTokenKind.AndKeyword, position, line, column, finalValue);

                case "or":
                    return CreateToken(LuaTokenKind.OrKeyword, position, line, column, finalValue);

                case "goto":
                    return CreateToken(LuaTokenKind.GotoKeyword, position, line, column, finalValue);

                case "return":
                    return CreateToken(LuaTokenKind.ReturnKeyword, position, line, column, finalValue);

                case "nil":
                    return CreateToken(LuaTokenKind.NilKeyword, position, line, column, finalValue);

                default:
                    return CreateToken(LuaTokenKind.Identifier, position, line, column, finalValue);
            }
        }
    }
}
