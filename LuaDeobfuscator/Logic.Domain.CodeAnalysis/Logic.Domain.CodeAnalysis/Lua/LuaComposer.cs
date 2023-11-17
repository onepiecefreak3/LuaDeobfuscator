using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;
using Logic.Domain.CodeAnalysis.Contract.Lua;
using Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses;

namespace Logic.Domain.CodeAnalysis.Lua
{
    internal class LuaComposer : ILuaComposer
    {
        private readonly ILuaSyntaxFactory _syntaxFactory;

        public LuaComposer(ILuaSyntaxFactory syntaxFactory)
        {
            _syntaxFactory = syntaxFactory;
        }

        public string ComposeCodeUnit(CodeUnitSyntax codeUnit)
        {
            StringBuilder sb = new();
            ComposeCodeUnit(codeUnit, sb);

            return sb.ToString();
        }

        private void ComposeCodeUnit(CodeUnitSyntax codeUnit, StringBuilder sb)
        {
            ComposeStatements(codeUnit.Statements, sb);
        }

        private void ComposeStatements(IReadOnlyList<StatementSyntax> statements, StringBuilder sb)
        {
            foreach (StatementSyntax statement in statements)
                ComposeStatement(statement, sb);
        }

        private void ComposeStatement(StatementSyntax statement, StringBuilder sb)
        {
            switch (statement)
            {
                case BreakStatement breakStatement:
                    ComposeBreakStatement(breakStatement, sb);
                    break;

                case DoStatementSyntax doStatement:
                    ComposeDoStatement(doStatement, sb);
                    break;

                case ElseIfThenStatementSyntax elseStatement:
                    ComposeElseStatement(elseStatement, sb);
                    break;

                case ExpressionSyntax expression:
                    ComposeExpression(expression, sb);
                    break;

                case GotoStatementSyntax gotoStatement:
                    ComposeGotoStatement(gotoStatement, sb);
                    break;

                case FunctionDeclarationStatementSyntax functionDeclaration:
                    ComposeFunctionDeclarationStatement(functionDeclaration, sb);
                    break;

                case IfThenElseStatementSyntax ifStatement:
                    ComposeIfStatement(ifStatement, sb);
                    break;

                case ReturnStatementSyntax returnStatement:
                    ComposeReturnStatement(returnStatement, sb);
                    break;

                case VariableDeclaratorStatementSyntax variableDeclaratorStatement:
                    ComposeVariableDeclaratorStatement(variableDeclaratorStatement, sb);
                    break;

                case WhileDoStatementSyntax whileStatement:
                    ComposeWhileStatement(whileStatement, sb);
                    break;
            }
        }

        private void ComposeBreakStatement(BreakStatement breakStatement, StringBuilder sb)
        {
            ComposeSyntaxToken(breakStatement.Break, sb);
        }

        private void ComposeDoStatement(DoStatementSyntax doStatement, StringBuilder sb)
        {
            ComposeDoExpression(doStatement.Do, sb);
            ComposeSyntaxToken(doStatement.End, sb);
        }

        private void ComposeElseStatement(ElseIfThenStatementSyntax elseStatement, StringBuilder sb)
        {
            ComposeElseIfExpression(elseStatement.ElseIf, sb);
            ComposeThenExpression(elseStatement.Then, sb);
        }

        private void ComposeGotoStatement(GotoStatementSyntax gotoStatement, StringBuilder sb)
        {
            ComposeSyntaxToken(gotoStatement.Goto, sb);
            ComposeName(gotoStatement.Label, sb);
        }

        private void ComposeFunctionDeclarationStatement(FunctionDeclarationStatementSyntax functionDeclaration, StringBuilder sb)
        {
            ComposeSyntaxToken(functionDeclaration.FunctionKeyword, sb);
            ComposeSyntaxToken(functionDeclaration.Identifier, sb);
            ComposeFunctionParameterSyntax(functionDeclaration.Parameters, sb);
            ComposeBlockExpression(functionDeclaration.Body, sb);
            ComposeSyntaxToken(functionDeclaration.End, sb);
        }

        private void ComposeFunctionParameterSyntax(FunctionParametersSyntax functionParameters, StringBuilder sb)
        {
            ComposeSyntaxToken(functionParameters.ParenOpen, sb);
            ComposeCommaSeparatedFunctionParameters(functionParameters.Parameters, sb);
            ComposeSyntaxToken(functionParameters.ParenClose, sb);
        }

        private void ComposeCommaSeparatedFunctionParameters(CommaSeparatedSyntaxList<ExpressionSyntax> parameters, StringBuilder sb)
        {
            if (parameters.Elements.Count <= 0)
                return;

            for (var i = 0; i < parameters.Elements.Count - 1; i++)
            {
                ComposeExpression(parameters.Elements[i], sb);
                ComposeSyntaxToken(_syntaxFactory.Token(LuaTokenKind.Comma), sb);
            }

            ComposeExpression(parameters.Elements[^1], sb);
        }

        private void ComposeCommaSeparatedVariables(CommaSeparatedSyntaxList<NameSyntax> variables, StringBuilder sb)
        {
            if (variables.Elements.Count <= 0)
                return;

            for (var i = 0; i < variables.Elements.Count - 1; i++)
            {
                ComposeName(variables.Elements[i], sb);
                ComposeSyntaxToken(_syntaxFactory.Token(LuaTokenKind.Comma), sb);
            }

            ComposeName(variables.Elements[^1], sb);
        }

        private void ComposeIfStatement(IfThenElseStatementSyntax ifStatement, StringBuilder sb)
        {
            ComposeIfExpression(ifStatement.If, sb);
            ComposeThenExpression(ifStatement.Then, sb);
            foreach (ElseIfThenStatementSyntax elseIf in ifStatement.ElseIfs)
                ComposeElseStatement(elseIf, sb);
            ComposeSyntaxToken(ifStatement.End, sb);
        }

        private void ComposeWhileStatement(WhileDoStatementSyntax whileStatement, StringBuilder sb)
        {
            ComposeWhileExpression(whileStatement.While, sb);
            ComposeDoStatement(whileStatement.Do, sb);
        }

        private void ComposeReturnStatement(ReturnStatementSyntax returnStatement, StringBuilder sb)
        {
            ComposeSyntaxToken(returnStatement.ReturnToken, sb);
            if (returnStatement.Expression != null)
                ComposeExpression(returnStatement.Expression, sb);
        }

        private void ComposeVariableDeclaratorStatement(VariableDeclaratorStatementSyntax variableDeclaratorStatement, StringBuilder sb)
        {
            ComposeSyntaxToken(variableDeclaratorStatement.LocalKeyword, sb);
            ComposeCommaSeparatedVariables(variableDeclaratorStatement.Variables, sb);
        }

        private void ComposeDoExpression(DoExpressionSyntax doExpression, StringBuilder sb)
        {
            ComposeSyntaxToken(doExpression.DoToken, sb);
            ComposeBlockExpression(doExpression.Body, sb);
        }

        private void ComposeIfExpression(IfExpressionSyntax ifExpression, StringBuilder sb)
        {
            ComposeSyntaxToken(ifExpression.If, sb);
            ComposeExpression(ifExpression.CompareExpression, sb);
        }

        private void ComposeThenExpression(ThenExpressionSyntax thenExpression, StringBuilder sb)
        {
            ComposeSyntaxToken(thenExpression.Then, sb);
            ComposeBlockExpression(thenExpression.Body, sb);
        }

        private void ComposeElseIfExpression(ElseIfExpressionSyntax elseIfExpression, StringBuilder sb)
        {
            ComposeSyntaxToken(elseIfExpression.ElseIf, sb);
            ComposeExpression(elseIfExpression.CompareExpression, sb);
        }

        private void ComposeWhileExpression(WhileExpressionSyntax whileExpression, StringBuilder sb)
        {
            ComposeSyntaxToken(whileExpression.While, sb);
            ComposeExpression(whileExpression.CompareExpression, sb);
        }

        private void ComposeExpression(ExpressionSyntax expression, StringBuilder sb)
        {
            switch (expression)
            {
                case AssignmentExpressionSyntax assignment:
                    ComposeAssignmentExpression(assignment, sb);
                    break;

                case BinaryExpressionSyntax binaryExpression:
                    ComposeBinaryExpression(binaryExpression, sb);
                    break;

                case BlockExpressionSyntax blockExpression:
                    ComposeBlockExpression(blockExpression, sb);
                    break;

                case FunctionInvocationExpressionSyntax functionInvocation:
                    ComposeFunctionInvocationExpression(functionInvocation, sb);
                    break;

                case GotoLabelExpressionSyntax gotoLabel:
                    ComposeGotoLabelExpression(gotoLabel, sb);
                    break;

                case LiteralExpressionSyntax stringLiteral:
                    ComposeLiteralExpression(stringLiteral, sb);
                    break;

                case LogicalExpressionSyntax logicalExpression:
                    ComposeLogicalExpression(logicalExpression, sb);
                    break;

                case NameSyntax name:
                    ComposeName(name, sb);
                    break;

                case ParenthesizedExpressionSyntax parenthesizedExpression:
                    ComposeParenthesizedExpression(parenthesizedExpression, sb);
                    break;

                case TableAccessExpressionSyntax tableAccessExpression:
                    ComposeTableAccessExpression(tableAccessExpression, sb);
                    break;

                case TableInitializerExpressionSyntax tableInit:
                    ComposeTableInitializerExpression(tableInit, sb);
                    break;

                case UnaryExpressionSyntax unaryExpression:
                    ComposeUnaryExpression(unaryExpression, sb);
                    break;

                case VariableIdentifiersExpressionSyntax variableIdentifiers:
                    ComposeVariableIdentifiersExpression(variableIdentifiers, sb);
                    break;
            }
        }

        private void ComposeAssignmentExpression(AssignmentExpressionSyntax assignment, StringBuilder sb)
        {
            ComposeExpression(assignment.Left, sb);
            ComposeSyntaxToken(assignment.Operation, sb);
            ComposeExpression(assignment.Right, sb);
        }

        private void ComposeBinaryExpression(BinaryExpressionSyntax binaryExpression, StringBuilder sb)
        {
            ComposeExpression(binaryExpression.Left, sb);
            ComposeSyntaxToken(binaryExpression.Operation, sb);
            ComposeExpression(binaryExpression.Right, sb);
        }

        private void ComposeBlockExpression(BlockExpressionSyntax blockExpression, StringBuilder sb)
        {
            ComposeStatements(blockExpression.Statements, sb);
        }

        private void ComposeFunctionInvocationExpression(FunctionInvocationExpressionSyntax functionInvocation, StringBuilder sb)
        {
            ComposeName(functionInvocation.Name, sb);
            ComposeFunctionParameterSyntax(functionInvocation.ParameterList, sb);
        }

        private void ComposeGotoLabelExpression(GotoLabelExpressionSyntax gotoLabel, StringBuilder sb)
        {
            ComposeSyntaxToken(gotoLabel.StartColon, sb);
            ComposeName(gotoLabel.Name, sb);
            ComposeSyntaxToken(gotoLabel.EndColon, sb);
        }

        private void ComposeLiteralExpression(LiteralExpressionSyntax literal, StringBuilder sb)
        {
            ComposeSyntaxToken(literal.Literal, sb);
        }

        private void ComposeLogicalExpression(LogicalExpressionSyntax logicalExpression, StringBuilder sb)
        {
            ComposeExpression(logicalExpression.Left, sb);
            ComposeSyntaxToken(logicalExpression.Operation, sb);
            ComposeExpression(logicalExpression.Right, sb);
        }

        private void ComposeName(NameSyntax name, StringBuilder sb)
        {
            switch (name)
            {
                case SimpleNameSyntax simpleName:
                    ComposeSyntaxToken(simpleName.Identifier, sb);
                    break;

                case QualifiedNameSyntax qualifiedName:
                    ComposeName(qualifiedName.Left, sb);
                    ComposeSyntaxToken(qualifiedName.Dot, sb);
                    ComposeName(qualifiedName.Right, sb);
                    break;
            }
        }

        private void ComposeParenthesizedExpression(ParenthesizedExpressionSyntax parenthesizedExpression, StringBuilder sb)
        {
            ComposeSyntaxToken(parenthesizedExpression.ParenOpen, sb);
            ComposeExpression(parenthesizedExpression.Expression, sb);
            ComposeSyntaxToken(parenthesizedExpression.ParenClose, sb);
        }

        private void ComposeTableAccessExpression(TableAccessExpressionSyntax tableAccessExpression, StringBuilder sb)
        {
            ComposeName(tableAccessExpression.Name, sb);
            ComposeTableAccessExpression(tableAccessExpression.Indexer, sb);
        }

        private void ComposeTableAccessExpression(TableAccessIndexerExpressionSyntax tableIndexerExpression, StringBuilder sb)
        {
            ComposeSyntaxToken(tableIndexerExpression.BracketOpen, sb);
            ComposeExpression(tableIndexerExpression.IndexExpression, sb);
            ComposeSyntaxToken(tableIndexerExpression.BracketClose, sb);
        }

        private void ComposeTableInitializerExpression(TableInitializerExpressionSyntax tableInit, StringBuilder sb)
        {
            ComposeSyntaxToken(tableInit.CurlyOpen, sb);
            ComposeSyntaxToken(tableInit.CurlyClose, sb);
        }

        private void ComposeUnaryExpression(UnaryExpressionSyntax unaryExpression, StringBuilder sb)
        {
            ComposeSyntaxToken(unaryExpression.Operation, sb);
            ComposeExpression(unaryExpression.Expression, sb);
        }

        private void ComposeVariableIdentifiersExpression(VariableIdentifiersExpressionSyntax variableIdentifiers, StringBuilder sb)
        {
            ComposeCommaSeparatedVariables(variableIdentifiers.Variables, sb);
        }

        private void ComposeSyntaxToken(SyntaxToken token, StringBuilder sb)
        {
            if (token.LeadingTrivia.HasValue)
                sb.Append(token.LeadingTrivia.Value.Text);

            sb.Append(token.Text);

            if (token.TrailingTrivia.HasValue)
                sb.Append(token.TrailingTrivia.Value.Text);
        }
    }
}
