namespace JintUnity.Parser.Ast
{
    public class ConditionalExpression : Expression
    {
        public Expression Test;
        public Expression Consequent;
        public Expression Alternate;
    }
}