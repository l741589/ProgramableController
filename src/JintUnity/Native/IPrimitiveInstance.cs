using JintUnity.Runtime;

namespace JintUnity.Native
{
    public interface IPrimitiveInstance
    {
        Types Type { get; } 
        JsValue PrimitiveValue { get; }
    }
}
