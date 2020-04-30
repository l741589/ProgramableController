using JintUnity.Native.Object;

namespace JintUnity.Native
{
    public interface IConstructor
    {
        JsValue Call(JsValue thisObject, JsValue[] arguments);
        ObjectInstance Construct(JsValue[] arguments);
    }
}
