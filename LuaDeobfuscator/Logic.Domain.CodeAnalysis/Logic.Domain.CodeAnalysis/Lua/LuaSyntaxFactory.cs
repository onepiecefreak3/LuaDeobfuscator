using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;

namespace Logic.Domain.CodeAnalysis.Lua
{
    internal class LuaSyntaxFactory : ILuaSyntaxFactory
    {
        public SyntaxToken Create(string text, LuaTokenKind kind, SyntaxTokenTrivia? leadingTrivia = null, SyntaxTokenTrivia? trailingTrivia = null)
        {
            return new SyntaxToken(text, (int)kind, leadingTrivia, trailingTrivia);
        }

        public SyntaxToken Token(LuaTokenKind kind)
        {
            switch (kind)
            {
                case LuaTokenKind.ParenOpen: return new("(", (int)kind);
                case LuaTokenKind.ParenClose: return new(")", (int)kind);
                case LuaTokenKind.Equals: return new("=", (int)kind);
                case LuaTokenKind.Dot: return new(".", (int)kind);
                case LuaTokenKind.Comma: return new(",", (int)kind);
                case LuaTokenKind.LocalKeyword: return new("local", (int)kind);
                case LuaTokenKind.FunctionKeyword: return new("function", (int)kind);
                case LuaTokenKind.EndKeyword: return new("end", (int)kind);
                case LuaTokenKind.IfKeyword: return new("if", (int)kind);
                case LuaTokenKind.ThenKeyword: return new("then", (int)kind);
                case LuaTokenKind.WhileKeyword: return new("while", (int)kind);
                case LuaTokenKind.DoKeyword: return new("do", (int)kind);
                case LuaTokenKind.BreakKeyword: return new("break", (int)kind);
                case LuaTokenKind.TrueKeyword: return new("true", (int)kind);
                case LuaTokenKind.FalseKeyword: return new("false", (int)kind);
                default: throw new InvalidOperationException($"Cannot create simple token from kind {kind}. Use other methods instead.");
            }
        }
    }
}
