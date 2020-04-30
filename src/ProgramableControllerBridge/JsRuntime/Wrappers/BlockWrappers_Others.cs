using JintUnity.Runtime.Interop;
using Modding.Blocks;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProgramableControllerBridge.JsRuntime.Wrappers {
    public partial class BlockWrappers {

        public class SpringCodeBlockWrapper : BlockWrapper<SpringCode> {
            private FieldInfo _lengthField;
            private FieldInfo _maxLengthField;
            public SpringCodeBlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
                _lengthField = typeof(SpringCode).GetField("currentLength", BindingFlags.Instance | BindingFlags.NonPublic);
                _maxLengthField = typeof(SpringCode).GetField("maxMagnitude", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            [JsInterface]
            public float Length => (float)_lengthField.GetValue(BB);
            [JsInterface]
            public float MaxLength => (float)_maxLengthField.GetValue(BB);

            [JsInterface]
            public float Value => Length;
        }

    }
}
