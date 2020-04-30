using JintUnity.Native;
using System.Collections.Generic;

namespace JintUnity.Parser.Ast
{
    public class CallExpression : Expression
    {
        public Expression Callee;
        public IList<Expression> Arguments;

        public bool Cached;
        public bool CanBeCached = true;
        public JsValue[] CachedArguments;
    }
}