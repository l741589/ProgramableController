using JintUnity.Native;
using JintUnity.Runtime.References;

namespace JintUnity.Runtime.Interop
{
    public interface IReferenceResolver
    {
        bool TryUnresolvableReference(Engine engine, Reference reference, out JsValue value);
        bool TryPropertyReference(Engine engine, Reference reference, ref JsValue value);
        bool TryGetCallable(Engine engine, object callee, out JsValue value);
        bool CheckCoercible(JsValue value);
    }
}