using JintUnity.Runtime;
using Modding;
using ProgramableController;
using ProgramableController.Bridges;
using ProgramableControllerBridge.JsRuntime;
using ProgramableControllerBridge.JsRuntime.Component;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramableControllerBridge {
   
    class CpuBridgeImpl : CpuBridge {
       
        private CpuController cpuBlock;
        private EngineEx engineEx;

        public CpuBridgeImpl(CpuController cpuBlock) {
            this.cpuBlock = cpuBlock;
        }

        public void SafeAwake() {
            
        }



        public void Update() {
            if (engineEx != null) {
                engineEx.Update();
            }
        }


        public void FixedUpdate() {
            if (engineEx != null) {
                engineEx.FixedUpdate();
            }
        }

        public void KeyEmulationUpdate() {
            if (engineEx != null) {
                engineEx.KeyEmulationUpdate();
            }
        }

        public void OnSimulateStart() {
            try {
                if (engineEx != null) {
                    engineEx.Stop();
                }
                engineEx = new EngineEx(cpuBlock,o => o
                    .Strict(cpuBlock.StrictMode.IsActive)
                    .TimeoutInterval(TimeSpan.FromSeconds(cpuBlock.timeout.Value))
                );
                engineEx.Execute(cpuBlock.script.Value);
            } catch (JavaScriptException e) {
                var msg = e.Message;
                if (string.IsNullOrEmpty(msg)) msg = e.ToString();
                if (string.IsNullOrEmpty(msg)) msg = e.GetType().ToString();
                ProgramableController.Utils.Logger.Error(msg);
                ProgramableController.Utils.Logger.Error(e.StackTrace);
                ProgramableController.Utils.Logger.Error(e.CallStack);
            } catch (Exception e) {
                ProgramableController.Utils.Logger.Error(e);
                ProgramableController.Utils.Logger.Error(e.StackTrace);
            }
        }

        public void OnSimulateStop() {
            if (engineEx != null) {
                engineEx.Stop();
                engineEx = null;
            }
        }

        public bool IsFInished() {
            return engineEx?.IsFinish ?? true;
        }
    }
}
