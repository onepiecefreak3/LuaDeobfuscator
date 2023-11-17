using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class FunctionDeclarationStatementSyntax : StatementSyntax
    {
        public SyntaxToken FunctionKeyword { get; private set; }
        public SyntaxToken Identifier { get; private set; }
        public FunctionParametersSyntax Parameters { get; private set; }
        public BlockExpressionSyntax Body { get; private set; }
        public SyntaxToken End { get; private set; }

        public override Location Location => new(FunctionKeyword.FullLocation.Position, End.FullLocation.EndPosition);

        public FunctionDeclarationStatementSyntax(SyntaxToken functionToken, SyntaxToken identifier, FunctionParametersSyntax parameters, BlockExpressionSyntax body, SyntaxToken end)
        {
            functionToken.Parent = this;
            identifier.Parent = this;
            parameters.Parent = this;
            body.Parent = this;
            end.Parent = this;

            FunctionKeyword = functionToken;
            Identifier = identifier;
            Parameters = parameters;
            Body = body;
            End = end;
        }

        public void SetIdentifier(SyntaxToken identifier, bool updatePositions = true)
        {
            identifier.Parent = this;

            Identifier = identifier;

            if (updatePositions)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            SyntaxToken functionToken = FunctionKeyword;
            SyntaxToken identifier = Identifier;
            SyntaxToken endToken = End;

            position = functionToken.UpdatePosition(position);
            position = identifier.UpdatePosition(position);
            position = Parameters.UpdatePosition(position);
            position = Body.UpdatePosition(position);
            position= endToken.UpdatePosition(position);

            FunctionKeyword = functionToken;
            Identifier = identifier;
            End = endToken;

            return position;
        }
    }
}
