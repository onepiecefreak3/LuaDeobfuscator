using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;

namespace Logic.Domain.CodeAnalysis.Lua
{
    public class LuaParser : Parser<LuaTokenKind>, ILuaParser
    {
        private readonly ILuaSyntaxFactory _syntaxFactory;

        protected override LuaTokenKind TriviaKind => LuaTokenKind.Trivia;

        public LuaParser(ITokenFactory<LexerToken<LuaTokenKind>> scriptFactory, ILuaSyntaxFactory syntaxFactory) :
            base(scriptFactory, syntaxFactory)
        {
            _syntaxFactory = syntaxFactory;
        }

        public CodeUnitSyntax ParseCodeUnit(string text)
        {
            IBuffer<LexerToken<LuaTokenKind>> buffer = CreateTokenBuffer(text);

            return ParseCodeUnit(buffer);
        }

        private CodeUnitSyntax ParseCodeUnit(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            var statements = ParseStatements(buffer, LuaTokenKind.EndOfFile);

            return new CodeUnitSyntax(statements);
        }

        private IReadOnlyList<StatementSyntax> ParseStatements(IBuffer<LexerToken<LuaTokenKind>> buffer, params LuaTokenKind[] stopTokens)
        {
            var result = new List<StatementSyntax>();

            while (!stopTokens.Contains(buffer.Peek().Kind))
                result.Add(ParseStatement(buffer));

            return result;
        }

        private StatementSyntax ParseStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            if (HasTokenKind(buffer, LuaTokenKind.ReturnKeyword))
                return ParseReturnStatement(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.LocalKeyword))
                return ParseVariableDeclaratorStatement(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.FunctionKeyword))
                return ParseFunctionDeclarationStatement(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.IfKeyword))
                return ParseIfThenElseStatement(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.WhileKeyword))
                return ParseWhileDoStatement(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.DoKeyword))
                return ParseDoStatement(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.BreakKeyword))
                return ParseBreakStatement(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.GotoKeyword))
                return ParseGotoStatement(buffer);

            return ParseExpression(buffer);
        }

        private ReturnStatementSyntax ParseReturnStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken returnToken = ParseReturnKeywordToken(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.EndKeyword))
                return new ReturnStatementSyntax(returnToken);

            var expression = ParseExpression(buffer);

            return new ReturnStatementSyntax(returnToken, expression);
        }

        private VariableDeclaratorStatementSyntax ParseVariableDeclaratorStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken localToken = ParseLocalKeywordToken(buffer);
            var identifiers = ParseCommaSeparatedVariables(buffer);
            if (identifiers == null)
                throw CreateException(buffer, "No identifiers for variable declaration.");

            return new VariableDeclaratorStatementSyntax(localToken, identifiers);
        }

        private CommaSeparatedSyntaxList<NameSyntax>? ParseCommaSeparatedVariables(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            if (!HasTokenKind(buffer, LuaTokenKind.Identifier))
                return null;

            var result = new List<NameSyntax>();

            NameSyntax expression = ParseName(buffer);
            result.Add(expression);

            while (HasTokenKind(buffer, LuaTokenKind.Comma))
            {
                SkipTokenKind(buffer, LuaTokenKind.Comma);

                if (!HasTokenKind(buffer, LuaTokenKind.Identifier))
                    throw CreateException(buffer, "Invalid end of variable identifiers.", LuaTokenKind.Identifier);

                result.Add(ParseName(buffer));
            }

            return new CommaSeparatedSyntaxList<NameSyntax>(result.ToArray());
        }

        private FunctionDeclarationStatementSyntax ParseFunctionDeclarationStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken functionToken = ParseFunctionKeywordToken(buffer);
            SyntaxToken identifier = ParseIdentifierToken(buffer);
            var functionParameters = ParseFunctionParameters(buffer);
            var functionBody = ParseBlockExpression(buffer, LuaTokenKind.EndKeyword);
            SyntaxToken endToken = ParseEndKeywordToken(buffer);

            return new FunctionDeclarationStatementSyntax(functionToken, identifier, functionParameters, functionBody, endToken);
        }

        private FunctionParametersSyntax ParseFunctionParameters(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken parenOpen = ParseParenOpenToken(buffer);
            if (HasTokenKind(buffer, LuaTokenKind.ParenClose))
                return new FunctionParametersSyntax(parenOpen, null, ParseParenCloseToken(buffer));

            var parameterList = ParseCommaSeparatedExpressions(buffer);
            SyntaxToken parenClose = ParseParenCloseToken(buffer);

            return new FunctionParametersSyntax(parenOpen, parameterList, parenClose);
        }

        private CommaSeparatedSyntaxList<ExpressionSyntax> ParseCommaSeparatedExpressions(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            var result = new List<ExpressionSyntax>();

            ExpressionSyntax expression = ParseExpression(buffer, false);
            result.Add(expression);

            while (HasTokenKind(buffer, LuaTokenKind.Comma))
            {
                SkipTokenKind(buffer, LuaTokenKind.Comma);

                if (!HasTokenKind(buffer, LuaTokenKind.Identifier))
                    throw CreateException(buffer, "Invalid end of variable identifiers.", LuaTokenKind.Identifier);

                result.Add(ParseExpression(buffer, false));
            }

            return new CommaSeparatedSyntaxList<ExpressionSyntax>(result.ToArray());
        }

        private BlockExpressionSyntax ParseBlockExpression(IBuffer<LexerToken<LuaTokenKind>> buffer, params LuaTokenKind[] stopTokens)
        {
            var statements = ParseStatements(buffer, stopTokens);

            return new BlockExpressionSyntax(statements);
        }

        private IfThenElseStatementSyntax ParseIfThenElseStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            var ifExpression = ParseIfExpression(buffer);
            var thenExpression = ParseThenExpression(buffer);
            var elseStatements = ParseElseIfThenStatements(buffer);
            SyntaxToken endToken = ParseEndKeywordToken(buffer);

            return new IfThenElseStatementSyntax(ifExpression, thenExpression, elseStatements, endToken);
        }

        private IfExpressionSyntax ParseIfExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken ifToken = ParseIfKeywordToken(buffer);
            var comparison = ParseExpression(buffer);

            return new IfExpressionSyntax(ifToken, comparison);
        }

        private ThenExpressionSyntax ParseThenExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken thenToken = ParseThenKeywordToken(buffer);
            var block = ParseBlockExpression(buffer, LuaTokenKind.EndKeyword, LuaTokenKind.ElseIfKeyword);

            return new ThenExpressionSyntax(thenToken, block);
        }

        private IReadOnlyList<ElseIfThenStatementSyntax>? ParseElseIfThenStatements(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            if (HasTokenKind(buffer, LuaTokenKind.EndKeyword))
                return null;

            var result = new List<ElseIfThenStatementSyntax>();

            while (HasTokenKind(buffer, LuaTokenKind.ElseIfKeyword))
            {
                var elseIf = ParseElseIfThenStatement(buffer);
                result.Add(elseIf);
            }

            return result;
        }

        private ElseIfThenStatementSyntax ParseElseIfThenStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            var elseIf = ParseElseIfExpression(buffer);
            var then = ParseThenExpression(buffer);

            return new ElseIfThenStatementSyntax(elseIf, then);
        }

        private ElseIfExpressionSyntax ParseElseIfExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken elseIfToken = ParseElseIfKeyword(buffer);
            var comparison = ParseExpression(buffer);

            return new ElseIfExpressionSyntax(elseIfToken, comparison);
        }

        private WhileDoStatementSyntax ParseWhileDoStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            var whileExpression = ParseWhileExpression(buffer);
            var doStatement = ParseDoStatement(buffer);

            return new WhileDoStatementSyntax(whileExpression, doStatement);
        }

        private WhileExpressionSyntax ParseWhileExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken whileToken = ParseWhileKeywordToken(buffer);
            var comparison = ParseExpression(buffer);

            return new WhileExpressionSyntax(whileToken, comparison);
        }

        private DoStatementSyntax ParseDoStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            var doExpression = ParseDoExpression(buffer);
            var endToken = ParseEndKeywordToken(buffer);

            return new DoStatementSyntax(doExpression, endToken);
        }

        private DoExpressionSyntax ParseDoExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken doToken = ParseDoKeywordToken(buffer);
            var block = ParseBlockExpression(buffer, LuaTokenKind.EndKeyword);

            return new DoExpressionSyntax(doToken, block);
        }

        private BreakStatement ParseBreakStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken breakToken = ParseBreakKeywordToken(buffer);

            return new BreakStatement(breakToken);
        }

        private GotoStatementSyntax ParseGotoStatement(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken gotoToken = ParseGotoKeywordToken(buffer);
            NameSyntax name = ParseName(buffer);

            return new GotoStatementSyntax(gotoToken, name);
        }

        private FunctionInvocationExpressionSyntax ParseFunctionInvocationExpression(IBuffer<LexerToken<LuaTokenKind>> buffer, NameSyntax name)
        {
            var parameters = ParseFunctionParameters(buffer);

            return new FunctionInvocationExpressionSyntax(name, parameters);
        }

        private bool IsLogicalExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return HasTokenKind(buffer, LuaTokenKind.AndKeyword) ||
                   HasTokenKind(buffer, LuaTokenKind.OrKeyword);
        }

        private LogicalExpressionSyntax ParseLogicalExpression(IBuffer<LexerToken<LuaTokenKind>> buffer, ExpressionSyntax left)
        {
            if (HasTokenKind(buffer, LuaTokenKind.AndKeyword))
                return new LogicalExpressionSyntax(left, ParseAndKeywordToken(buffer), ParseExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.OrKeyword))
                return new LogicalExpressionSyntax(left, ParseOrKeywordToken(buffer), ParseExpression(buffer));

            throw CreateException(buffer, "Unknown logical expression.", LuaTokenKind.AndKeyword, LuaTokenKind.OrKeyword);
        }

        private bool IsAssignmentExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return HasTokenKind(buffer, LuaTokenKind.Equals);
        }

        private AssignmentExpressionSyntax ParseAssignmentExpression(IBuffer<LexerToken<LuaTokenKind>> buffer, ExpressionSyntax left)
        {
            if (HasTokenKind(buffer, LuaTokenKind.Equals))
                return new AssignmentExpressionSyntax(left, ParseEqualsToken(buffer), ParseExpression(buffer));

            throw CreateException(buffer, "Invalid assignment.", LuaTokenKind.Equals);
        }

        private ParenthesizedExpressionSyntax ParseParenthesizedExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken parenOpen = ParseParenOpenToken(buffer);
            var expression = ParseExpression(buffer);
            SyntaxToken parenClose = ParseParenCloseToken(buffer);

            return new ParenthesizedExpressionSyntax(parenOpen, expression, parenClose);
        }

        private bool IsUnaryOperator(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return HasTokenKind(buffer, LuaTokenKind.NotKeyword) ||
                   HasTokenKind(buffer, LuaTokenKind.Hashtag);
        }

        private UnaryExpressionSyntax ParseUnaryExpressionSyntax(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            if (HasTokenKind(buffer, LuaTokenKind.NotKeyword))
                return new UnaryExpressionSyntax(ParseNotKeywordToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.Hashtag))
                return new UnaryExpressionSyntax(ParseHashtagToken(buffer), ParseAtomicExpression(buffer));

            throw CreateException(buffer, "Unknown unary expression.", LuaTokenKind.NotKeyword);
        }

        private bool IsBinaryExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return HasTokenKind(buffer, LuaTokenKind.SmallerThan) ||
                   HasTokenKind(buffer, LuaTokenKind.GreaterThan) ||
                   HasTokenKind(buffer, LuaTokenKind.SmallerEquals) ||
                   HasTokenKind(buffer, LuaTokenKind.GreaterEquals) ||
                   HasTokenKind(buffer, LuaTokenKind.EqualsEquals) ||
                   HasTokenKind(buffer, LuaTokenKind.Plus) ||
                   HasTokenKind(buffer, LuaTokenKind.Minus) ||
                   HasTokenKind(buffer, LuaTokenKind.Asterisk) ||
                   HasTokenKind(buffer, LuaTokenKind.Slash) ||
                   HasTokenKind(buffer, LuaTokenKind.Percent) ||
                   HasTokenKind(buffer, LuaTokenKind.DotDot) ||
                   HasTokenKind(buffer, LuaTokenKind.NotEquals);
        }

        private BinaryExpressionSyntax ParseBinaryExpression(IBuffer<LexerToken<LuaTokenKind>> buffer, ExpressionSyntax left)
        {
            if (HasTokenKind(buffer, LuaTokenKind.SmallerThan))
                return new BinaryExpressionSyntax(left, ParseSmallerThanToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.GreaterThan))
                return new BinaryExpressionSyntax(left, ParseGreaterThanToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.SmallerEquals))
                return new BinaryExpressionSyntax(left, ParseSmallerEqualsToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.GreaterEquals))
                return new BinaryExpressionSyntax(left, ParseGreaterEqualsToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.EqualsEquals))
                return new BinaryExpressionSyntax(left, ParseEqualsEqualsToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.Plus))
                return new BinaryExpressionSyntax(left, ParsePlusToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.Minus))
                return new BinaryExpressionSyntax(left, ParseMinusToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.Asterisk))
                return new BinaryExpressionSyntax(left, ParseAsteriskToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.Slash))
                return new BinaryExpressionSyntax(left, ParseSlashToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.Percent))
                return new BinaryExpressionSyntax(left, ParsePercentToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.DotDot))
                return new BinaryExpressionSyntax(left, ParseDotDotToken(buffer), ParseAtomicExpression(buffer));

            if (HasTokenKind(buffer, LuaTokenKind.NotEquals))
                return new BinaryExpressionSyntax(left, ParseNotEqualsToken(buffer), ParseAtomicExpression(buffer));

            throw CreateException(buffer, "Unknown binary expression.", LuaTokenKind.SmallerThan, LuaTokenKind.GreaterThan, LuaTokenKind.SmallerEquals, LuaTokenKind.GreaterEquals, LuaTokenKind.EqualsEquals);
        }

        private GotoLabelExpressionSyntax ParseGotoLabelExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken startColon = ParseColonColonToken(buffer);
            NameSyntax name = ParseName(buffer);
            SyntaxToken endColon = ParseColonColonToken(buffer);

            return new GotoLabelExpressionSyntax(startColon, name, endColon);
        }

        private ExpressionSyntax ParseExpression(IBuffer<LexerToken<LuaTokenKind>> buffer, bool allowCommaSeparated = true)
        {
            ExpressionSyntax left = ParseAtomicExpression(buffer, allowCommaSeparated);

            if (!IsLogicalExpression(buffer))
                return left;

            return ParseLogicalExpression(buffer, left);
        }

        private ExpressionSyntax ParseAtomicExpression(IBuffer<LexerToken<LuaTokenKind>> buffer, bool allowCommaSeparated = true)
        {
            if (HasTokenKind(buffer, LuaTokenKind.CurlyOpen))
                return ParseTableInitializerExpression(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.ColonColon))
                return ParseGotoLabelExpression(buffer);

            if (HasTokenKind(buffer, LuaTokenKind.ParenOpen))
                return ParseParenthesizedExpression(buffer);

            if (IsUnaryOperator(buffer))
                return ParseUnaryExpressionSyntax(buffer);

            ExpressionSyntax? left = null;

            if (HasTokenKind(buffer, LuaTokenKind.TrueKeyword))
                left = ParseTrueLiteralExpression(buffer);

            else if (HasTokenKind(buffer, LuaTokenKind.FalseKeyword))
                left = ParseFalseLiteralExpression(buffer);

            else if (HasTokenKind(buffer, LuaTokenKind.StringLiteral))
                left = ParseStringLiteralExpression(buffer);

            else if (HasTokenKind(buffer, LuaTokenKind.NumericLiteral))
                left = ParseNumericLiteralExpression(buffer);

            else if (HasTokenKind(buffer, LuaTokenKind.FloatingNumericLiteral))
                left = ParseFloatingNumericLiteralExpression(buffer);

            else if (HasTokenKind(buffer, LuaTokenKind.NilKeyword))
                left = ParseNilLiteralExpression(buffer);

            else if (HasTokenKind(buffer, LuaTokenKind.Identifier))
            {
                if (allowCommaSeparated && HasTokenKind(buffer, 1, LuaTokenKind.Comma))
                    left = ParseVariableIdentifiersExpression(buffer);

                else
                {
                    left = ParseName(buffer);

                    if (HasTokenKind(buffer, LuaTokenKind.BracketOpen))
                        left = ParseTableAccessExpression(buffer, (NameSyntax)left);

                    else if (HasTokenKind(buffer, LuaTokenKind.ParenOpen))
                        return ParseFunctionInvocationExpression(buffer, (NameSyntax)left);
                }
            }

            if (left == null)
                throw CreateException(buffer, "Unknown expression.", LuaTokenKind.TrueKeyword,
                    LuaTokenKind.FalseKeyword, LuaTokenKind.StringLiteral, LuaTokenKind.NumericLiteral,
                    LuaTokenKind.FloatingNumericLiteral, LuaTokenKind.Identifier);

            if (IsAssignmentExpression(buffer))
                return ParseAssignmentExpression(buffer, left);

            if (IsBinaryExpression(buffer))
                return ParseBinaryExpression(buffer, left);

            return left;
        }

        private TableInitializerExpressionSyntax ParseTableInitializerExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken curlyOpen = ParseCurlyOpenToken(buffer);
            SyntaxToken curlyClose = ParseCurlyCloseToken(buffer);

            return new TableInitializerExpressionSyntax(curlyOpen, curlyClose);
        }

        private VariableIdentifiersExpressionSyntax ParseVariableIdentifiersExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            var identifiers = ParseCommaSeparatedVariables(buffer);
            if (identifiers == null)
                throw CreateException(buffer, "Invalid variable identifiers.");

            return new VariableIdentifiersExpressionSyntax(identifiers);
        }

        private LiteralExpressionSyntax ParseTrueLiteralExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken literal = ParseTrueKeywordToken(buffer);

            return new LiteralExpressionSyntax(literal);
        }

        private LiteralExpressionSyntax ParseFalseLiteralExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken literal = ParseFalseKeywordToken(buffer);

            return new LiteralExpressionSyntax(literal);
        }

        private LiteralExpressionSyntax ParseStringLiteralExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken literal = ParseStringLiteralToken(buffer);

            return new LiteralExpressionSyntax(literal);
        }

        private LiteralExpressionSyntax ParseNumericLiteralExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken literal = ParseNumericLiteralToken(buffer);

            return new LiteralExpressionSyntax(literal);
        }

        private LiteralExpressionSyntax ParseFloatingNumericLiteralExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken literal = ParseFloatingNumericLiteralToken(buffer);

            return new LiteralExpressionSyntax(literal);
        }

        private LiteralExpressionSyntax ParseNilLiteralExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken literal = ParseNilKeywordToken(buffer);

            return new LiteralExpressionSyntax(literal);
        }

        private TableAccessExpressionSyntax ParseTableAccessExpression(IBuffer<LexerToken<LuaTokenKind>> buffer, NameSyntax name)
        {
            var indexer = ParseTableAccessIndexerExpression(buffer);

            return new TableAccessExpressionSyntax(name, indexer);
        }

        private TableAccessIndexerExpressionSyntax ParseTableAccessIndexerExpression(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            SyntaxToken bracketOpen = ParseBracketOpenToken(buffer);
            var index = ParseExpression(buffer);
            SyntaxToken bracketClose = ParseBracketCloseToken(buffer);

            return new TableAccessIndexerExpressionSyntax(bracketOpen, index, bracketClose);
        }

        private NameSyntax ParseName(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            if (!HasTokenKind(buffer, LuaTokenKind.Identifier))
                throw CreateException(buffer, "Invalid name syntax.", LuaTokenKind.Identifier);

            NameSyntax left = new SimpleNameSyntax(ParseIdentifierToken(buffer));
            if (!HasTokenKind(buffer, LuaTokenKind.Dot))
                return left;

            SyntaxToken dot = ParseDotToken(buffer);

            return new QualifiedNameSyntax(left, dot, ParseName(buffer));
        }

        private SyntaxToken ParseParenOpenToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.ParenOpen);
        }

        private SyntaxToken ParseParenCloseToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.ParenClose);
        }

        private SyntaxToken ParseCurlyOpenToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.CurlyOpen);
        }

        private SyntaxToken ParseCurlyCloseToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.CurlyClose);
        }

        private SyntaxToken ParseBracketOpenToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.BracketOpen);
        }

        private SyntaxToken ParseBracketCloseToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.BracketClose);
        }

        private SyntaxToken ParseSmallerThanToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.SmallerThan);
        }

        private SyntaxToken ParseGreaterThanToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.GreaterThan);
        }

        private SyntaxToken ParseSmallerEqualsToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.SmallerEquals);
        }

        private SyntaxToken ParseGreaterEqualsToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.GreaterEquals);
        }

        private SyntaxToken ParseEqualsEqualsToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.EqualsEquals);
        }

        private SyntaxToken ParseEqualsToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Equals);
        }

        private SyntaxToken ParseNotEqualsToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.NotEquals);
        }

        private SyntaxToken ParseDotToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Dot);
        }

        private SyntaxToken ParseDotDotToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.DotDot);
        }

        private SyntaxToken ParseColonColonToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.ColonColon);
        }

        private SyntaxToken ParseHashtagToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Hashtag);
        }

        private SyntaxToken ParsePlusToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Plus);
        }

        private SyntaxToken ParseMinusToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Minus);
        }

        private SyntaxToken ParseAsteriskToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Asterisk);
        }

        private SyntaxToken ParseSlashToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Slash);
        }

        private SyntaxToken ParsePercentToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Percent);
        }

        private SyntaxToken ParseStringLiteralToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.StringLiteral);
        }

        private SyntaxToken ParseNumericLiteralToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.NumericLiteral);
        }

        private SyntaxToken ParseFloatingNumericLiteralToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.FloatingNumericLiteral);
        }

        private SyntaxToken ParseNilKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.NilKeyword);
        }

        private SyntaxToken ParseIdentifierToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.Identifier);
        }

        private SyntaxToken ParseLocalKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.LocalKeyword);
        }

        private SyntaxToken ParseFunctionKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.FunctionKeyword);
        }

        private SyntaxToken ParseEndKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.EndKeyword);
        }

        private SyntaxToken ParseIfKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.IfKeyword);
        }

        private SyntaxToken ParseElseIfKeyword(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.ElseIfKeyword);
        }

        private SyntaxToken ParseThenKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.ThenKeyword);
        }

        private SyntaxToken ParseWhileKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.WhileKeyword);
        }

        private SyntaxToken ParseDoKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.DoKeyword);
        }

        private SyntaxToken ParseBreakKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.BreakKeyword);
        }

        private SyntaxToken ParseTrueKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.TrueKeyword);
        }

        private SyntaxToken ParseFalseKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.FalseKeyword);
        }

        private SyntaxToken ParseNotKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.NotKeyword);
        }

        private SyntaxToken ParseAndKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.AndKeyword);
        }

        private SyntaxToken ParseOrKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.OrKeyword);
        }

        private SyntaxToken ParseGotoKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.GotoKeyword);
        }

        private SyntaxToken ParseReturnKeywordToken(IBuffer<LexerToken<LuaTokenKind>> buffer)
        {
            return CreateToken(buffer, LuaTokenKind.ReturnKeyword);
        }
    }
}
