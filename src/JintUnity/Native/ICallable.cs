namespace JintUnity.Native
{
    public interface ICallable
    {
        JsValue Call(JsValue thisObject, JsValue[] arguments);
    }
}