using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JintUnity.Runtime.Interop {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Field| AttributeTargets.Interface)]
    public class JsInterfaceAttribute : Attribute {
        
    }
}
