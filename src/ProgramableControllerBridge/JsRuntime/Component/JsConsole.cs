using JintUnity.Native;
using ProgramableController.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JintUnity;
using ProgramableControllerBridge.JsRuntime.Core;
using JintUnity.Runtime.Interop;
using JintUnity.Runtime;

namespace ProgramableControllerBridge.JsRuntime.Component {
    public class JsConsole :JsComponent{

        private Engine _engine;

        [JsInterface]
        public void Log(JsValue value, params JsValue[] values) {
            if (values == null || values.Length == 0) {
                if (value == null || value.IsNull()) {
                    Logger.Log("null");
                    return;
                } else if (value.IsUndefined()){
                    Logger.Log("undefined");
                }
            }
            Logger.Log(value + " " + JintUnity.Utils.Join(" ", values));
        }


        [JsInterface]
        public void Clear() {
            Command("clear");
        }

        [JsInterface]
        public void Command(string cmd) {
            ConsoleView.Instance.Controller.RunCommandString(cmd);
        }

        [JsInterface]
        public void Dir(object obj) {
            if (obj == null) {
                Logger.Log("null");
            } else if (obj.GetType().IsPrimitive || obj is string) {
                Logger.Log(obj.ToString());
            } else if (obj is JsValue) {
                var s = TypeConverter.ToString(_engine.Json.Stringify(obj as JsValue, new JsValue[] { JsValue.Undefined, 2 }));
                Logger.Log(s);
            } else {
                Logger.Info("==================  DIR  ==================");
                var t = obj.GetType();
                Logger.Info(t.FullName);
                foreach (var p in t.GetProperties()) {
                    Logger.Info("P: " + p);
                }

                foreach (var f in t.GetFields()) {
                    Logger.Info("F: " + f);
                }

                foreach (var m in t.GetMethods()) {
                    Logger.Info("M: " + m);
                }

                Logger.Info("===========================================");
            }
        }

        [JsInterface]
        public void Log(JsValue value) {
            Logger.Log(value.ToString());
        }
        [JsInterface]
        public override void Register(EngineEx engineEx, Engine engine) {
            engineEx.Register("console", this);
            _engine = engine;
        }
    }
}
