using Modding;
using Modding.Blocks;
using ProgramableController.Bridges;
using ProgramableController.Script;
using ProgramableController.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;
using Logger = ProgramableController.Utils.Logger;

namespace ProgramableController {
    public class CpuController : BlockScript {

        public int CpuId;
        public MToggle StrictMode;
        public MSlider timeout;
        public TextArea script;
        public CpuBridge CpuBridge { get; private set; }

        public override void SafeAwake() {            
            //Logger.Debug("SafeAwake " + this.GetHashCode());
            if (CpuBridge == null) {
                CpuBridge = Mod.Instance.Bridge.CreateCpuBridge(this);
            }
            AddCustom(this.script = new TextArea("script", "script", ""));
            StrictMode = AddToggle("strictMode", "strictMode", false);
            timeout = AddSliderUnclamped("timeout", "timeout", 1f, 0.01f, 60f);
            CpuBridge.SafeAwake();
        }

        public override bool EmulatesAnyKeys => true;

        public override void OnSimulateStart() {
            base.OnSimulateStart();
            CpuBridge?.OnSimulateStart();
        }

        public override void OnSimulateStop() {
            base.OnSimulateStop();
            CpuBridge?.OnSimulateStop();
        }

        public override void SimulateUpdateHost() {
            base.SimulateUpdateHost();
            CpuBridge?.Update();
        }

        public override void SimulateFixedUpdateHost() {
            base.SimulateFixedUpdateHost();
            CpuBridge?.FixedUpdate();
        }

        public override void KeyEmulationUpdate() {
            base.KeyEmulationUpdate();
            CpuBridge?.KeyEmulationUpdate();
        }
    }
}
