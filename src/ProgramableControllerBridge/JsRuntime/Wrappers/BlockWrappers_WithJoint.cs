using JintUnity.Runtime.Interop;
using Modding.Blocks;
using Modding.Modules.Official;
using ProgramableControllerBridge.JsRuntime.Component;
using ProgramableControllerBridge.JsRuntime.Core;
using ProgramableControllerBridge.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ProgramableControllerBridge.JsRuntime.Wrappers {

    [JsInterface]
    public abstract class JointBlockWrapper<T> : BlockWrapper<T> where T : BlockBehaviour {
        private Joint _joint;
        private Func<Vector3, float> _getAngle;
        public JointBlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
        }

        [JsInterface]
        public virtual float Angle => (_getAngle ?? (_getAngle = GetAngle()))(EulerAngle);

        protected virtual Joint GetJoint() {
            return BB.blockJoint;
        }
        protected virtual Joint Joint => _joint = _joint ?? GetJoint();
        [JsInterface]
        public virtual Vector3 EulerAngle => (Joint as ConfigurableJoint)?.targetRotation.eulerAngles ?? Vector3.zero;
        [JsInterface]
        public float Value => Angle;

        protected abstract Func<Vector3, float> GetAngle();
    }

    public partial class BlockWrappers {

        [JsInterface]
        public class SteeringWheelWrapper : JointBlockWrapper<SteeringWheel> {


            public SteeringWheelWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
               
            }

            protected override Func<Vector3, float> GetAngle() {
                switch (Name) {
                    case "SteeringHinge": return v => v.y;
                    case "SteeringBlock": return v => v.x;
                }
                return v => 0;
            }
        }

        [JsInterface]
        public class CogMotorWrapper : JointBlockWrapper<CogMotorControllerHinge> {

            public CogMotorWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
            }

            protected override Func<Vector3, float> GetAngle() {
                return v => v.x;
            }

            public override float Angle => (Joint as HingeJoint).angle;

            public override Vector3 EulerAngle => new Vector3(Angle,0,0);
        }
    }
}
