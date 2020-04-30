using JintUnity;
using JintUnity.Native;
using JintUnity.Native.Object;
using JintUnity.Runtime.Descriptors;
using JintUnity.Runtime.Interop;
using ProgramableController.Script;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime.Wrappers;
using ProgramableControllerBridge.JsRuntime.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ProgramableController;

namespace ProgramableControllerBridge.JsRuntime.Core {
    public class EngineEx {
        public int Id => Cpu.CpuId;
        public Engine Engine { get; private set; }
        public JsScheduler TimeoutScheduler { get; private set; }
        public JsConsole Console { get; private set; }
        public JsProcess Process { get; private set; }
        public CpuController Cpu { get; private set; }
        public event Action NextFrameAction;
        public List<IJsComponent> Components = new List<IJsComponent>();
        private int UpdateCount = 0;
        private int FixedUpdateCount = 0;
        
        private IObjectConverter[] _conveters = {
            new Vector3Converter(),
            new BlockConverter(),
            new MapperTypeConverter(),
        };

        public EngineEx(CpuController cpu, Action<Options> options) {
            this.Cpu = cpu;
            this.Engine = new Engine(o => {
                foreach(var e in _conveters) {
                    if (e is IObjectConverterAdapter) {
                        (e as IObjectConverterAdapter).Init(this);
                    }
                    o.AddObjectConverter(e);
                }
                
                options?.Invoke(o);
            });
            this.Engine.SetValue("__dirname", Environment.CurrentDirectory);
            this.Engine.SetValue("__filename", Environment.CurrentDirectory + "/Besiege.exe");
            this.Register(new JsScheduler()).Register(new JsConsole()).Register(new JsProcess());
            ModBridgeImpl.Instance.ComponentFactories.ForEach(e => Register(e));

        }

        public bool IsFinish => Components.All(e => e.IsFinish);

        public EngineEx Register(JsComponentFactory factory) {
            Register(factory.Create());
            return this;
        }

        public EngineEx Register(IJsComponent comp) {
            if (Components.Contains(comp)) {
                return this;
            }
            Components = Components.Concat(new IJsComponent[] { comp }).ToList();
            comp.Register(this, Engine);
            return this;
        }

        public EngineEx Unregister(IJsComponent comp) {
            if (!Components.Contains(comp)) {
                return this;
            }
            Components = Components.Where(e => e != comp).ToList();
            return this;
        }

        public EngineEx Register(string name, Delegate @delegate) {
            Engine.SetValue(name, @delegate);
            return this;
        }

        public EngineEx Register(string name, object obj) {
            Engine.SetValue(name, obj);
            return this;
        }


        public void Update() {
            ++UpdateCount;
            Components.ForEach(e => e?.Update());
        }


        public void FixedUpdate() {
            ++FixedUpdateCount;
            var ev = NextFrameAction;
            NextFrameAction = null;
            ev?.Invoke();
            Components.ForEach(e => e?.FixedUpdate());
        }

        public void KeyEmulationUpdate() {
            Components.ForEach(e => e?.KeyEmulationUpdate());
        }

        public void Stop() {
            Engine?.Interrupt();
            Components.ForEach(e => e?.Stop());
            NextFrameAction = null;
        }

        public void Execute(string code) {
            Engine?.Execute(code);
        }

        public void PrintFrameInfo() {
            Logger.Info("U={0}  FU={1}", UpdateCount, FixedUpdateCount);
        }

        public T GetComponent<T>() where T : JsComponent {
            return Components.Find(e => e is T) as T;
        }
    }
}
