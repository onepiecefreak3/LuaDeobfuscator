using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class VariableIdentifiersExpressionSyntax : ExpressionSyntax
    {
        public CommaSeparatedSyntaxList<NameSyntax> Variables { get; private set; }

        public override Location Location => Variables.Location;

        public VariableIdentifiersExpressionSyntax(CommaSeparatedSyntaxList<NameSyntax> variables)
        {
            variables.Parent = this;

            Variables = variables;
        }

        internal override int UpdatePosition(int position = 0)
        {
            position = Variables.UpdatePosition(position);

            return position;
        }
    }
}
