using System.Collections.Generic;

namespace JintUnity.Parser.Ast
{
    public class NewExpression : Expression
    {
        public Expression Callee;
        public IEnumerable<Expression> Arguments;
    }
}