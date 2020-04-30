using System;
using System.Collections.Generic;
using JintUnity.Native;
using JintUnity.Parser.Ast;
using JintUnity.Runtime.Environments;

namespace JintUnity.Runtime.Debugger
{
    public class DebugInformation : EventArgs
    {
        public Stack<String> CallStack { get; set; }
        public Statement CurrentStatement { get; set; }
        public Dictionary<string, JsValue> Locals { get; set; }
        public Dictionary<string, JsValue> Globals { get; set; }
    }
}
