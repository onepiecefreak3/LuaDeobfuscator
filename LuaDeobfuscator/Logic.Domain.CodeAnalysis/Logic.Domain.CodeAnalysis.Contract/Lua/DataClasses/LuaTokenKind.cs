namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public enum LuaTokenKind
    {
        ParenOpen,
        ParenClose,
        CurlyOpen,
        CurlyClose,
        BracketOpen,
        BracketClose,

        SmallerThan,
        SmallerEquals,
        GreaterThan,
        GreaterEquals,
        Equals,
        EqualsEquals,
        NotEquals,

        Dot,
        DotDot,
        Colon,
        ColonColon,
        Tilde,
        Comma,
        Hashtag,
        Plus,
        Minus,
        Asterisk,
        Slash,
        Percent,

        StringLiteral,
        NumericLiteral,
        FloatingNumericLiteral,
        Identifier,
        Trivia,

        LocalKeyword,
        FunctionKeyword,
        EndKeyword,
        IfKeyword,
        ElseIfKeyword,
        ThenKeyword,
        WhileKeyword,
        DoKeyword,
        BreakKeyword,
        TrueKeyword,
        FalseKeyword,
        NotKeyword,
        AndKeyword,
        OrKeyword,
        GotoKeyword,
        ReturnKeyword,
        NilKeyword,

        EndOfFile
    }
}
