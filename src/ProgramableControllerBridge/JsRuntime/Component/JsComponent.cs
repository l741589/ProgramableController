using JintUnity;
using JintUnity.Runtime.Interop;
using Modding.Blocks;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramableControllerBridge.JsRuntime.Component {


    public abstract class JsComponentFactory {

        public abstract JsComponent Create();

        public virtual void Start(PlayerMachine machine) {

        }
        public virtual void Stop(PlayerMachine machine) {

        }
    }

    public interface IJsComponent {
        bool IsFinish { get; }
        void FixedUpdate();
        void KeyEmulationUpdate();
        void Register(EngineEx engineEx, Engine engine);
        void Stop();
        void Update();
    }

    [JsInterface]
    public abstract class JsComponent : IJsComponent {

        public abstract void Register(EngineEx engineEx, Engine engine);

        public virtual void FixedUpdate() {

        }

        public virtual void KeyEmulationUpdate() {

        }

        public virtual void Stop() {

        }

        public virtual void Update() {

        }

        public virtual bool IsFinish => true;

        public override string ToString() {
            return GetType().Name.Substring(2) + "{}";
        }
    }
}
