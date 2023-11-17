using Logic.Business.LuaDeobfuscator.InternalContract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;
using System.Xml.Linq;

namespace Logic.Business.LuaDeobfuscator
{
    internal class LuaDeobfuscator : ILuaDeobfuscator
    {
        private readonly ILuaSyntaxFactory _syntaxFactory;
        private readonly IBufferFactory _bufferFactory;

        public LuaDeobfuscator(ILuaSyntaxFactory syntaxFactory, IBufferFactory bufferFactory)
        {
            _syntaxFactory = syntaxFactory;
            _bufferFactory = bufferFactory;
        }

        public void Deobfuscate(CodeUnitSyntax syntax)
        {
            IReadOnlyList<StatementSyntax> statements = DeobfuscateStatements(syntax.Statements);
            syntax.SetStatements(statements, false);

            syntax.Update();
        }

        private IReadOnlyList<StatementSyntax> DeobfuscateStatements(IReadOnlyList<StatementSyntax> statements)
        {
            List<StatementSyntax> localStatements = statements.ToList();

            for (var i = 0; i < localStatements.Count; i++)
            {
                switch (localStatements[i])
                {
                    case FunctionDeclarationStatementSyntax functionDeclarationStatement:
                        DeobfuscateFunctionDeclarationStatement(functionDeclarationStatement, localStatements, i);
                        break;

                    case FunctionInvocationExpressionSyntax functionInvocationExpression:
                        DeobfuscateFunctionInvocationExpression(functionInvocationExpression, localStatements, ref i);
                        break;

                    default:
                        DeobfuscateStatement(localStatements[i], localStatements, ref i);
                        break;
                }
            }

            return localStatements;
        }

        private void DeobfuscateFunctionDeclarationStatement(FunctionDeclarationStatementSyntax functionDeclarationStatement,
            IList<StatementSyntax> statements, int declarationIndex)
        {
            SyntaxToken functionIdentifier = functionDeclarationStatement.Identifier;

            for (int j = declarationIndex + 1; j < statements.Count; j++)
            {
                if (statements[j] is not AssignmentExpressionSyntax { Right: SimpleNameSyntax rightName } assignment)
                    continue;

                if (rightName.Identifier.Text != functionIdentifier.Text)
                    continue;

                if (assignment.Left is not SimpleNameSyntax leftName)
                    continue;

                SyntaxToken newIdentifier = _syntaxFactory.Create(leftName.Identifier.Text, (LuaTokenKind)functionIdentifier.RawKind,
                    functionIdentifier.LeadingTrivia, functionIdentifier.TrailingTrivia);
                functionDeclarationStatement.SetIdentifier(newIdentifier, false);

                statements.Remove(assignment);

                break;
            }

            DeobfuscateBlockExpression(functionDeclarationStatement.Body);
        }

        private void DeobfuscateFunctionInvocationExpression(FunctionInvocationExpressionSyntax functionInvocation,
            IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateFunctionInvocationName(functionInvocation, statements, ref statementIndex);

            var localParameters = functionInvocation.ParameterList.Parameters.Elements.ToArray();
            for (var i = 0; i < localParameters.Length; i++)
                DeobfuscateFunctionInvocationParameter(localParameters, i, statements, ref statementIndex);

            functionInvocation.ParameterList.Parameters.SetElements(localParameters, false);
        }

        private void DeobfuscateFunctionInvocationName(FunctionInvocationExpressionSyntax functionInvocation,
            IList<StatementSyntax> statements, ref int statementIndex)
        {
            NameSyntax finalName = functionInvocation.Name;
            if (finalName is not SimpleNameSyntax)
                return;

            for (int i = statementIndex - 1; i >= 0; i--)
            {
                if (statements[i] is not AssignmentExpressionSyntax { Left: SimpleNameSyntax simpleLeftName, Right: NameSyntax rightName } assignment)
                    continue;

                switch (finalName)
                {
                    case SimpleNameSyntax simpleFinalName:
                        if (simpleFinalName.Identifier.Text != simpleLeftName.Identifier.Text)
                            continue;

                        switch (rightName)
                        {
                            case SimpleNameSyntax simpleRightName:
                                RemoveNameTrivia(simpleRightName);
                                functionInvocation.SetName(simpleRightName, false);

                                statements.RemoveAt(i);
                                statementIndex--;

                                return;

                            case QualifiedNameSyntax qualifiedRightName:
                                RemoveNameTrivia(qualifiedRightName);
                                finalName = qualifiedRightName;

                                statements.RemoveAt(i);
                                statementIndex--;

                                break;
                        }

                        break;

                    case QualifiedNameSyntax qualifiedFinalName:
                        if (GetFirstNamePart(qualifiedFinalName).Text != simpleLeftName.Identifier.Text)
                            continue;

                        RemoveNameTrivia(rightName);
                        SetFirstNamePart(qualifiedFinalName, rightName);

                        statements.RemoveAt(i);
                        statementIndex--;

                        break;
                }
            }

            functionInvocation.SetName(finalName, false);
        }

        private void DeobfuscateFunctionInvocationParameter(ExpressionSyntax[] functionInvocationParameters, int parameterIndex,
            IList<StatementSyntax> statements, ref int statementIndex)
        {
            if (functionInvocationParameters[parameterIndex] is not SimpleNameSyntax simpleParameterName)
                return;

            for (int i = statementIndex - 1; i >= 0; i--)
            {
                if (statements[i] is not AssignmentExpressionSyntax { Left: SimpleNameSyntax simpleLeftName } assignment)
                    continue;

                if (simpleParameterName.Identifier.Text != simpleLeftName.Identifier.Text)
                    continue;

                switch (assignment.Right)
                {
                    case LiteralExpressionSyntax literalExpression:
                        literalExpression.SetLiteral(literalExpression.Literal.WithNoTrivia(), false);
                        if (parameterIndex > 0)
                            literalExpression.SetLiteral(literalExpression.Literal.WithLeadingTrivia(" "), false);
                        functionInvocationParameters[parameterIndex] = literalExpression;
                        break;

                    case SimpleNameSyntax simpleName:
                        simpleName.SetIdentifier(simpleName.Identifier.WithNoTrivia(), false);
                        if (parameterIndex > 0)
                            simpleName.SetIdentifier(simpleName.Identifier.WithLeadingTrivia(" "), false);
                        functionInvocationParameters[parameterIndex] = simpleName;
                        break;
                }

                statements.RemoveAt(i);
                statementIndex--;
            }
        }

        private SyntaxToken GetFirstNamePart(NameSyntax name)
        {
            switch (name)
            {
                case SimpleNameSyntax simpleName:
                    return simpleName.Identifier;

                case QualifiedNameSyntax qualifiedName:
                    return GetFirstNamePart(qualifiedName.Left);
            }

            throw new InvalidOperationException("Unknown name syntax.");
        }

        private void RemoveNameTrivia(NameSyntax name)
        {
            switch (name)
            {
                case SimpleNameSyntax simpleName:
                    simpleName.SetIdentifier(simpleName.Identifier.WithNoTrivia(), false);
                    break;

                case QualifiedNameSyntax qualifiedName:
                    RemoveNameTrivia(qualifiedName.Left);
                    qualifiedName.SetDot(qualifiedName.Dot.WithNoTrivia(), false);
                    RemoveNameTrivia(qualifiedName.Right);
                    break;
            }
        }

        private void SetFirstNamePart(QualifiedNameSyntax qualifiedName, NameSyntax replace)
        {
            switch (qualifiedName.Left)
            {
                case SimpleNameSyntax:
                    qualifiedName.SetLeft(replace, false);
                    break;

                case QualifiedNameSyntax leftQualifiedName:
                    SetFirstNamePart(leftQualifiedName, replace);
                    break;
            }
        }

        private void DeobfuscateStatement(StatementSyntax statement, IList<StatementSyntax> statements, ref int statementIndex)
        {
            switch (statement)
            {
                case FunctionDeclarationStatementSyntax functionDeclaration:
                    DeobfuscateFunctionDeclarationStatement(functionDeclaration);
                    break;

                case DoStatementSyntax doStatement:
                    DeobfuscateDoStatement(doStatement);
                    break;

                case IfThenElseStatementSyntax ifStatement:
                    DeobfuscateIfStatement(ifStatement, statements, ref statementIndex);
                    break;

                case ElseIfThenStatementSyntax elseStatement:
                    DeobfuscateElseStatement(elseStatement, statements, ref statementIndex);
                    break;

                case ReturnStatementSyntax returnStatement:
                    DeobfuscateReturnStatement(returnStatement, statements, ref statementIndex);
                    break;

                case WhileDoStatementSyntax whileStatement:
                    DeobfuscateWhileStatement(whileStatement, statements, ref statementIndex);
                    break;

                case ExpressionSyntax expression:
                    DeobfuscateExpression(expression, statements, ref statementIndex);
                    break;
            }
        }

        private void DeobfuscateFunctionDeclarationStatement(FunctionDeclarationStatementSyntax functionDeclaration)
        {
            DeobfuscateBlockExpression(functionDeclaration.Body);
        }

        private void DeobfuscateDoStatement(DoStatementSyntax doStatement)
        {
            DeobfuscateDoExpression(doStatement.Do);
        }

        private void DeobfuscateIfStatement(IfThenElseStatementSyntax ifStatement, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateIfExpression(ifStatement.If, statements, ref statementIndex);
            DeobfuscateThenExpression(ifStatement.Then);
            foreach (ElseIfThenStatementSyntax elseIf in ifStatement.ElseIfs)
                DeobfuscateElseStatement(elseIf, statements, ref statementIndex);
        }

        private void DeobfuscateElseStatement(ElseIfThenStatementSyntax elseStatement, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateThenExpression(elseStatement.Then);
            DeobfuscateElseIfExpression(elseStatement.ElseIf, statements, ref statementIndex);
        }

        private void DeobfuscateWhileStatement(WhileDoStatementSyntax whileStatement, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateWhileExpression(whileStatement.While, statements, ref statementIndex);
            DeobfuscateDoStatement(whileStatement.Do);
        }

        private void DeobfuscateReturnStatement(ReturnStatementSyntax returnStatement, IList<StatementSyntax> statements, ref int statementIndex)
        {
            if (returnStatement.Expression != null)
                DeobfuscateExpression(returnStatement.Expression, statements, ref statementIndex);
        }

        private void DeobfuscateDoExpression(DoExpressionSyntax doExpression)
        {
            DeobfuscateBlockExpression(doExpression.Body);
        }

        private void DeobfuscateIfExpression(IfExpressionSyntax ifExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(ifExpression.CompareExpression, statements, ref statementIndex);
        }

        private void DeobfuscateThenExpression(ThenExpressionSyntax thenExpression)
        {
            DeobfuscateBlockExpression(thenExpression.Body);
        }

        private void DeobfuscateElseIfExpression(ElseIfExpressionSyntax elseIfExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(elseIfExpression.CompareExpression, statements, ref statementIndex);
        }

        private void DeobfuscateWhileExpression(WhileExpressionSyntax whileExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(whileExpression.CompareExpression, statements, ref statementIndex);
        }

        private void DeobfuscateExpression(ExpressionSyntax expression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            switch (expression)
            {
                case AssignmentExpressionSyntax assignment:
                    DeobfuscateAssignmentExpression(assignment, statements, ref statementIndex);
                    break;

                case BinaryExpressionSyntax binaryExpression:
                    DeobfuscateBinaryExpression(binaryExpression, statements, ref statementIndex);
                    break;

                case BlockExpressionSyntax blockExpression:
                    DeobfuscateBlockExpression(blockExpression);
                    break;

                case FunctionInvocationExpressionSyntax functionInvocationExpression:
                    DeobfuscateFunctionInvocationExpression(functionInvocationExpression, statements, ref statementIndex);
                    break;

                case LiteralExpressionSyntax literal:
                    DeobfuscateLiteralExpression(literal);
                    break;

                case LogicalExpressionSyntax logicalExpression:
                    DeobfuscateLogicalExpression(logicalExpression, statements, ref statementIndex);
                    break;

                case ParenthesizedExpressionSyntax parenthesizedExpression:
                    DeobfuscateParenthesizedExpression(parenthesizedExpression, statements, ref statementIndex);
                    break;

                case TableAccessExpressionSyntax tableAccessExpression:
                    DeobfuscateTableAccessExpression(tableAccessExpression, statements, ref statementIndex);
                    break;

                case UnaryExpressionSyntax unaryExpression:
                    DeobfuscateUnaryExpression(unaryExpression, statements, ref statementIndex);
                    break;
            }
        }

        private void DeobfuscateAssignmentExpression(AssignmentExpressionSyntax assignment, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(assignment.Left, statements, ref statementIndex);
            DeobfuscateExpression(assignment.Right, statements, ref statementIndex);
        }

        private void DeobfuscateBinaryExpression(BinaryExpressionSyntax binaryExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(binaryExpression.Left, statements, ref statementIndex);
            DeobfuscateExpression(binaryExpression.Right, statements, ref statementIndex);
        }

        private void DeobfuscateBlockExpression(BlockExpressionSyntax blockExpression)
        {
            IReadOnlyList<StatementSyntax> statements = DeobfuscateStatements(blockExpression.Statements);
            blockExpression.SetStatements(statements, false);
        }

        private void DeobfuscateLiteralExpression(LiteralExpressionSyntax literal)
        {
            if (literal.Literal.RawKind != (int)LuaTokenKind.StringLiteral)
                return;

            DeobfuscateStringLiteralExpression(literal);
        }

        private void DeobfuscateLogicalExpression(LogicalExpressionSyntax logicalExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(logicalExpression.Left, statements, ref statementIndex);
            DeobfuscateExpression(logicalExpression.Right, statements, ref statementIndex);
        }

        private void DeobfuscateParenthesizedExpression(ParenthesizedExpressionSyntax parenthesizedExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(parenthesizedExpression.Expression, statements, ref statementIndex);
        }

        private void DeobfuscateTableAccessExpression(TableAccessExpressionSyntax tableAccessExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateTableAccessExpression(tableAccessExpression.Indexer, statements, ref statementIndex);
        }

        private void DeobfuscateTableAccessExpression(TableAccessIndexerExpressionSyntax tableIndexerExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(tableIndexerExpression.IndexExpression, statements, ref statementIndex);
        }

        private void DeobfuscateUnaryExpression(UnaryExpressionSyntax unaryExpression, IList<StatementSyntax> statements, ref int statementIndex)
        {
            DeobfuscateExpression(unaryExpression.Expression, statements, ref statementIndex);
        }

        private void DeobfuscateStringLiteralExpression(LiteralExpressionSyntax stringLiteral)
        {
            string deobfuscatedString = DeobfuscateString(stringLiteral.Literal.Text);

            SyntaxToken oldLiteral = stringLiteral.Literal;
            SyntaxToken newLiteral = _syntaxFactory.Create(deobfuscatedString, (LuaTokenKind)oldLiteral.RawKind,
                oldLiteral.LeadingTrivia, oldLiteral.TrailingTrivia);

            stringLiteral.SetLiteral(newLiteral, false);
        }

        private string DeobfuscateString(string input)
        {
            IBuffer<int> buffer = _bufferFactory.CreateStringBuffer(input);

            var bytes = new byte[input.Length];
            var byteCount = 0;

            StringBuilder sb = new();
            while (!buffer.IsEndOfInput)
            {
                switch (buffer.Peek())
                {
                    case '\\':
                        var backSlash = (byte)buffer.Read();

                        if (buffer.Peek() is >= '0' and <= '9')
                        {
                            sb.Clear();

                            var digit1 = (char)buffer.Read();
                            sb.Append(digit1);

                            if (buffer.Peek() is >= '0' and <= '9')
                            {
                                var digit2 = (char)buffer.Read();
                                sb.Append(digit2);

                                if (buffer.Peek() is >= '0' and <= '9')
                                {
                                    var digit3 = (char)buffer.Read();
                                    sb.Append(digit3);

                                    bytes[byteCount++] = byte.Parse(sb.ToString());

                                    break;
                                }

                                bytes[byteCount++] = (byte)buffer.Read();
                                bytes[byteCount++] = (byte)digit2;
                                bytes[byteCount++] = (byte)digit1;
                                bytes[byteCount++] = backSlash;

                                break;
                            }

                            bytes[byteCount++] = (byte)buffer.Read();
                            bytes[byteCount++] = (byte)digit1;
                            bytes[byteCount++] = backSlash;
                        }

                        bytes[byteCount++] = (byte)buffer.Read();
                        bytes[byteCount++] = backSlash;

                        break;

                    default:
                        bytes[byteCount++] = (byte)buffer.Read();
                        break;
                }
            }

            return Encoding.UTF8.GetString(bytes, 0, byteCount);
        }
    }
}
