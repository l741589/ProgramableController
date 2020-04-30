using JintUnity;
using JintUnity.Native;
using JintUnity.Runtime;
using JintUnity.Runtime.Interop;
using Modding.Blocks;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramableControllerBridge.JsRuntime.Component {

    public delegate void OnStrageItemChangeHandler(string key, object newValue, object oldValue);
    public class JsStorageFactory : JsComponentFactory {

        public readonly Dictionary<string, object> Data = new Dictionary<string, object>();
        public readonly Dictionary<string, OnStrageItemChangeHandler> Events = new Dictionary<string, OnStrageItemChangeHandler>();

        public override JsComponent Create() {
            return new JsStorage(this);
        }

        public override void Start(PlayerMachine machine) {
            Data.Clear();
            Events.Clear();
        }

        public override void Stop(PlayerMachine machine) {
            Data.Clear();
            Events.Clear();
        }
    }

    [JsInterface]
    public class JsStorage : JsComponent {

        private EngineEx _engineEx;
        private Engine _engine;
        private JsStorageFactory _factory;
        private readonly Dictionary<string, object> _data;
        public readonly Dictionary<string, OnStrageItemChangeHandler> _events;

        public JsStorage(JsStorageFactory factory) {
            this._factory = factory;
            this._data = factory.Data;
            this._events = factory.Events;
        }

        public override void Register(EngineEx engineEx, Engine engine) {
            engineEx.Register("storage", this);
            this._engineEx = engineEx;
            this._engine = engine;
        }

        [JsInterface]
        public object GetItem(string key) {
            return _data.GetOrDefault(key, _data.ContainsKey(key) ? JsValue.Null : JsValue.Undefined);
        }

        [JsInterface]
        public void SetItem(string key, object val) {
            if (ReferenceEquals(val, JsValue.Undefined)) {
                return;
            }
            var old = _data.GetOrDefault(key, JsValue.Undefined);
            _data[key] = val;
            _engineEx.NextFrameAction += () => {
                if (key != "") {
                    _events.GetOrDefault(key)?.Invoke(key, val, old);
                }
                _events.GetOrDefault("")?.Invoke(key, val, old);
            };
        }

        [JsInterface]
        public void RemoveItem(string key) {
            var old = _data.GetOrDefault(key, JsValue.Undefined);
            if (_data.Remove(key) && ReferenceEquals(old, JsValue.Undefined)) {
                _engineEx.NextFrameAction += () => {
                    if (key != "") {
                        _events.GetOrDefault(key)?.Invoke(key, JsValue.Undefined, old);
                    }
                    _events.GetOrDefault("")?.Invoke(key, JsValue.Undefined, old);
                };
            }            
        }

        [JsInterface]
        public void Clear() {
            var copy = new Dictionary<string, object>(_data);
            _data.Clear();
            var defaultListeners = _events.GetOrDefault("");
            foreach (KeyValuePair<string, OnStrageItemChangeHandler> p in _events) {
                if (p.Key == "") {
                    continue;
                }
                var old = copy.GetOrDefault(p.Key);
                if (old != null) {
                    _engineEx.NextFrameAction += () => {
                        p.Value?.Invoke(p.Key, JsValue.Undefined, old);
                        defaultListeners?.Invoke(p.Key, JsValue.Undefined, old);
                    };
                }
            }
        }

        [JsInterface]
        public object this[string key] {
            get => GetItem(key);
            set => SetItem(key, value);
        }

        [JsInterface]
        public int Length => _data.Count();

        [JsInterface]
        public string Key(int index) {
            return _data.Keys.Skip(index).First();
        }

        [JsInterface]
        public void Subscribe(string key, OnStrageItemChangeHandler cb) {
            if (cb == null) {
                return;
            }
            if (!_events.TryGetValue(key, out OnStrageItemChangeHandler c)) {
                _events[key] = (k, n, o) => _engine.ResetTimeoutTicks();
            }
            _events[key] += cb;
        }

        [JsInterface]
        public void Subscribe(OnStrageItemChangeHandler cb) {
            Subscribe("", cb);
        }

        [JsInterface]
        public void Unsubscribe(string key) {
            _events.Remove(key);
        }

        [JsInterface]
        public void Unsubscribe() {
            _events.Clear();
        }
    }

}
