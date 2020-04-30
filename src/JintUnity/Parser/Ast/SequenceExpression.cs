using System.Collections.Generic;

namespace JintUnity.Parser.Ast
{
    public class SequenceExpression : Expression
    {
        public IList<Expression> Expressions;
    }
}