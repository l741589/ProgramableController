using JintUnity;
using JintUnity.Runtime.Interop;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ProgramableControllerBridge.JsRuntime.Component {
    public class JsProcess :JsComponent{

        private EngineEx _engineEx;
        [JsInterface]
        public int Pid => _engineEx.Id;
        [JsInterface]
        public string Title => "Cpu";
        [JsInterface]
        public string Platform => Environment.OSVersion.Platform.ToString();

        public override void Register(EngineEx engineEx, Engine engine) {
            _engineEx = engineEx;
            _engineEx.Register("process", this);
        }
    }
}
