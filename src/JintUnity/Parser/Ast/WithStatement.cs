namespace JintUnity.Parser.Ast
{
    public class WithStatement : Statement
    {
        public Expression Object;
        public Statement Body;
    }
}