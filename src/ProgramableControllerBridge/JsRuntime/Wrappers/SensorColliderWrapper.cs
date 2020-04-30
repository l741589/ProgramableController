using JintUnity.Runtime.Interop;
using Modding.Levels;
using ProgramableControllerBridge.JsRuntime.Core;
using System.Text;
using UnityEngine;

namespace ProgramableControllerBridge.JsRuntime.Wrappers {

	[JsInterface]
	public class SensorColliderWrapper {
		private Collider _collider;
		private SensorBlock _sensor;
		private EngineEx _engineEx;
		private Transform _transform;
		private BlockBehaviour _bb;
		private Rigidbody _attachedRigidbody;
		private Vector3 _targetPos;
		private Vector3 _forward;
		private Vector3 _startPos;

		public SensorColliderWrapper(EngineEx engineEx, BlockWrappers.SensorBlockWrapper sensorWrapper, Collider collider) {
			this._engineEx = engineEx;
			this._sensor = sensorWrapper.BB;
			this._collider = collider;
			this._forward = sensorWrapper.Forward;
			this._startPos = _sensor.sensorPos.position;
			this._targetPos = _collider.ClosestPointOnBounds(this._startPos) - this._startPos;
		}

		private Transform Transform => _transform ?? (_transform = _collider.transform);
		private BlockBehaviour BB => _bb ?? (_bb = Transform.GetComponentInParent<BlockBehaviour>());
		private Rigidbody AttachedRigidbody => _attachedRigidbody ?? (_attachedRigidbody = _collider.attachedRigidbody);

		[JsInterface]
		public Vector3 Forward => _forward;
		[JsInterface]
		public Vector3 Target => _targetPos;
		//[JsInterface]
		public Vector3 Start => _startPos;
		[JsInterface]
		public float SqrDistance => Target.sqrMagnitude;
		[JsInterface]
		public float Distance => Target.magnitude;
		[JsInterface]
		public Vector3 TargetForward => Vector3.Project(this._targetPos, this._forward);
		[JsInterface]
		public float SqrDistanceForward => TargetForward.sqrMagnitude;
		[JsInterface]
		public float DistanceForward => TargetForward.magnitude;

		
		[JsInterface]
		public string Name => BlockName ?? (HasAI ? "Creature" : "Entity");
		[JsInterface]
		public bool IsBlock => BB != null;
		[JsInterface]
		public string BlockName => BB?.Prefab?.name;
		[JsInterface]
		public bool IsProjectile => AttachedRigidbody?.GetComponentInParent<ProjectileScript>() != null;
		[JsInterface]
		public bool IsCreature => AttachedRigidbody?.GetComponentInParent<InjuryController>() != null || HasAI;
		[JsInterface]
		public bool HasAI => Transform.GetComponentInParent<EntityAI>() != null || Transform.GetComponentInParent<EnemyAISimple>() != null;
		[JsInterface]
		public bool IsDead => (Transform.GetComponentInParent<EntityAI>()?.isDead ?? false) || (Transform.GetComponentInParent<EnemyAISimple>()?.isDead ?? false);
		[JsInterface]
		public bool IsStatic => BlockWrappers.IsStatic(_collider.attachedRigidbody);
		[JsInterface]
		public bool CanBreak => 
			AttachedRigidbody?.GetComponentInParent<SimBehaviour>() is IExplosionEffect ||
			AttachedRigidbody?.GetComponent<PhysNodeTile>() != null ||
			AttachedRigidbody?.GetComponent<StructuralPhysTile>() != null;

		[JsInterface]
		public string PropertiesMask {
			get {
				StringBuilder ps = new StringBuilder();
				if (IsBlock) ps.Append("B");
				if (IsProjectile) ps.Append("P");
				if (IsCreature) ps.Append("C");
				if (IsDead) ps.Append("D");
				if (HasAI) ps.Append("A");
				if (CanBreak) ps.Append("R");
				if (IsStatic) ps.Append("S");
				return ps.ToString();
			}
		}

		[JsInterface]
		public override string ToString() {
			return string.Format("{{{4}[{3}] {{ x:{0}, y:{1}, z:{2} }}", Target.x, Target.y, Target.z, PropertiesMask, Name);
		}
	}
}
