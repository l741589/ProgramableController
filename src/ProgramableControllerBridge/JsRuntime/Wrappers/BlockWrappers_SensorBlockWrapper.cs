using JintUnity.Runtime.Interop;
using Modding.Blocks;
using ProgramableControllerBridge.JsRuntime.Core;
using ProgramableControllerBridge.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProgramableControllerBridge.JsRuntime.Wrappers {
	public static partial class BlockWrappers {
        public class SensorBlockWrapper : ActivableBlockWrapper<SensorBlock> {

			public SensorBlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {

			}
		
			[JsInterface]
			public override bool IsActive => BB.Inverted.IsActive ^ (Count > 0);

			[JsInterface]
			public int Count => (int)Util2.ReadPrivateField(BB, "overlapCount");

			[JsInterface]
			public Vector3 Forward => -BB.transform.up;

			[JsInterface]
			public SensorColliderWrapper[] Value {
				get {
					List<SensorColliderWrapper> list = new List<SensorColliderWrapper>();
					float radius = BB.RadiusSllider.Value;
					float num = BB.DistanceSlider.Value - radius * 2f;
					if (num < 0f) {
						num = 0f;
					}
					Vector3 point = BB.sensorPos.position + Forward * radius;
					Vector3 point2 = BB.sensorPos.position + Forward * (num + radius);
					Collider[] array = Physics.OverlapCapsule(point, point2, radius, BB.sensorMask);
					foreach (Collider collider in array) {
						if (IsTrgger(collider)) {
							continue;
						}
						if (IsSingleOrDestroyed(collider) || collider.transform != BB.transform && !(BB.IgnoreStatic.IsActive && IsStatic(collider.attachedRigidbody))) {
							list.Add(new SensorColliderWrapper(_engineEx, this, collider));
						}
					}
					return list.OrderBy(e=>e.SqrDistance).ToArray();
				}
			}


			private bool IsTrgger(Collider collider) {
				if (collider.isTrigger) {
					if (collider.transform.root != SingleInstanceFindOnly<AddPiece>.Instance.PhysicsGoalObject.root && collider.transform.root != ReferenceMaster.physicsGoalInstance) {
						return true;
					} else if (StatMaster.isMP) {
						if (collider.gameObject.GetComponent<InsigniaTriggerObject>()) {
							return true;
						}
					} else if (collider.gameObject.GetComponentInParent<FinishLine>()) {
						return true;
					}
				}
				return false;
			}

			private bool IsSingleOrDestroyed(Collider collider) {
				Transform transform = collider.transform;
				if (BB.ParentMachine.finishedPhysics) {
					return false;
				}
				BlockBehaviour componentInParent = transform.GetComponentInParent<BlockBehaviour>();
				if (componentInParent == null) {
					return true;
				}
				if (componentInParent == BB) {
					return false;
				}
				if (BB.IgnoreStatic.IsActive && IsStatic(componentInParent.Rigidbody)) {
					return false;
				}
				for (int j = 0; j < componentInParent.DestroyOnSimulate.Length; j++) {
					GameObject gameObject = componentInParent.DestroyOnSimulate[j];
					if (gameObject != null && gameObject.transform == transform) {
						return true;
					}
				}

				return false;
			}
		}


		public static bool IsStatic(Rigidbody attachedRigidbody) {
			return attachedRigidbody == null || attachedRigidbody.isKinematic;
		}
	}
}
