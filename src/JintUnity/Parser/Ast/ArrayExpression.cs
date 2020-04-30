using System.Collections.Generic;

namespace JintUnity.Parser.Ast
{
    public class ArrayExpression : Expression
    {
        public IEnumerable<Expression> Elements;
    }
}