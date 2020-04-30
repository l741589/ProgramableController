using System.Collections.Generic;

namespace JintUnity.Parser.Ast
{
    public class BlockStatement : Statement
    {
        public IEnumerable<Statement> Body;
    }
}