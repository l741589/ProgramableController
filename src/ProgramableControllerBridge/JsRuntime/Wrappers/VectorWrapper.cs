using JintUnity;
using JintUnity.Native;
using JintUnity.Native.Array;
using JintUnity.Runtime.Interop;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramableControllerBridge.JsRuntime.Wrappers {

    public class Vector3Converter : ObjectConverterAdapter<Vector3> {
        public override object Convert(Vector3 value) {
            return new VectorWrapper(_engine, value);
        }
    }

    [JsInterface]
    public class VectorWrapper {
        private Vector3 _vector;

        public VectorWrapper(Engine engine, Vector3 vector) {
            this._vector = vector;
        }
        [JsInterface]
        public float X { get => _vector.x; set => _vector.x = value; }
        [JsInterface]
        public float Y { get => _vector.y; set => _vector.y = value; }
        [JsInterface]
        public float Z { get => _vector.z; set => _vector.z = value; }
        [JsInterface]
        public float this[int index] {
            get => _vector[index];
            set => _vector[index] = value;
        }
        [JsInterface]
        public int Length => 3;

        [JsInterface]
        public override string ToString() {
            return _vector.ToString();
        }

    }
}
