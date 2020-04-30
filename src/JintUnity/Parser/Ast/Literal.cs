using JintUnity.Native;

namespace JintUnity.Parser.Ast
{
    public class Literal : Expression, IPropertyKeyExpression
    {
        public object Value;
        public string Raw;

        public bool Cached;
        public JsValue CachedValue; 

        public string GetKey()
        {
            return Value.ToString();
        }

    }
}