using System.Collections.Generic;

namespace JintUnity.Parser.Ast
{
    public class SwitchStatement : Statement
    {
        public Expression Discriminant;
        public IEnumerable<SwitchCase> Cases;
    }
}