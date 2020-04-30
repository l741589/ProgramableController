using JintUnity;
using JintUnity.Native;
using JintUnity.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramableControllerBridge.JsRuntime.Core {

    public interface IObjectConverterAdapter {
        IObjectConverterAdapter Init(EngineEx engineEx);
    }

    public abstract class ObjectConverterAdapter<T> : IObjectConverterAdapter,IObjectConverter {
        protected EngineEx _engineEx;
        protected Engine _engine;
        protected bool? _result = null;

        public IObjectConverterAdapter Init(EngineEx engineEx) {
            this._engineEx = engineEx;
            this._engine = engineEx.Engine;
            return this;
        }

        public bool TryConvert(object value, out JsValue result) {
            if (value is T) {
                _engine = _engineEx.Engine;
                result = JsValue.FromObject(_engine, Convert((T)value));
                return _result ?? result != null;
            } else {
                result = null;
                return false;
            }
        }

        public abstract object Convert(T value);
    }
}
