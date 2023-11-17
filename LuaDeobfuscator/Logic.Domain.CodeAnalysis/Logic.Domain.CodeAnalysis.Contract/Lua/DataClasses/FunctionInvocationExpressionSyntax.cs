using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Domain.CodeAnalysis.Contract.DataClasses;

namespace Logic.Domain.CodeAnalysis.Contract.Lua.DataClasses
{
    public class FunctionInvocationExpressionSyntax : ExpressionSyntax
    {
        public NameSyntax Name { get; private set; }
        public FunctionParametersSyntax ParameterList { get; private set; }

        public override Location Location => new(Name.Location.Position, ParameterList.Location.EndPosition);

        public FunctionInvocationExpressionSyntax(NameSyntax name, FunctionParametersSyntax parameterList)
        {
            name.Parent = this;
            parameterList.Parent = this;

            Name = name;
            ParameterList = parameterList;
        }

        public void SetName(NameSyntax name, bool updatePositions = true)
        {
            name.Parent = this;

            Name = name;

            if (updatePositions)
                Root.UpdatePosition();
        }

        internal override int UpdatePosition(int position = 0)
        {
            position = Name.UpdatePosition(position);
            position = ParameterList.UpdatePosition(position);

            return position;
        }
    }
}
