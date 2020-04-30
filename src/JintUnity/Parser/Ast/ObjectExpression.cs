using System.Collections.Generic;

namespace JintUnity.Parser.Ast
{
    public class ObjectExpression : Expression
    {
        public IEnumerable<Property> Properties;
    }
}