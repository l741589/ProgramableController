using System.Collections.Generic;

namespace JintUnity.Parser.Ast
{
    public class SwitchCase : SyntaxNode
    {
        public Expression Test;
        public IEnumerable<Statement> Consequent;
    }
}