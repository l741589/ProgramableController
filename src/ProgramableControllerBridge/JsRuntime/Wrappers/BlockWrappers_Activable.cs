using JintUnity;
using JintUnity.Native;
using JintUnity.Runtime;
using JintUnity.Runtime.Interop;
using Modding;
using Modding.Blocks;
using ProgramableController;
using ProgramableControllerBridge.JsRuntime.Component;
using ProgramableControllerBridge.JsRuntime.Core;
using ProgramableControllerBridge.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static LogicGate;

namespace ProgramableControllerBridge.JsRuntime.Wrappers {
	public abstract class ActivableBlockWrapper<T> : BlockWrapper<T>, IJsComponent where T : class {

		private event Action<bool> _eventHandler;
		private bool _prevActive;

		public ActivableBlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {

		}

		[JsInterface]
		public abstract bool IsActive { get; }

		public bool IsFinish => _eventHandler == null;

		[JsInterface]
		public void Subscribe(JsValue callback) {
			_prevActive = IsActive;
			_engineEx.Register(this);
			ICallable call = callback?.TryCast<ICallable>();
			if (call == null) {
				throw new JavaScriptException(_engineEx.Engine.TypeError, "requires function");
			}
			_eventHandler += b => {
				_engineEx.Engine.ResetTimeoutTicks();
				call.Call(JsValue.Undefined, new JsValue[] { new JsValue(b) });
			};
		}

		[JsInterface]
		public void Unsubscrive() {
			_eventHandler = null;
			_engineEx.Unregister(this);
		}

		public void FixedUpdate() {
			if (_eventHandler == null) {
				return;
			}
			var isActive = IsActive;
			if (_prevActive != isActive) {
				_prevActive = isActive;
				_eventHandler.Invoke(isActive);
			}
		}

		public void KeyEmulationUpdate() {

		}

		public void Register(EngineEx engineEx, Engine engine) {
		}

		public void Stop() {
			_engineEx.Unregister(this);
		}

		public void Update() {
		}
	}


	public static partial class BlockWrappers {



		public class AltimeterBlockWrapper : ActivableBlockWrapper<AltimeterBlock> {
			public AltimeterBlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
			}
			[JsInterface]
			public float Value => Height;
			[JsInterface]
			public float Height => BB.Height;
			[JsInterface]
			public override bool IsActive => BB.Inverted.IsActive ^ BB.Height > BB.HeightSlider.Value;

		}

		public class AnglometerBlockWrapper : ActivableBlockWrapper<AnglometerBlock> {

			private Vector3 _targetDir;
			private MethodInfo _correctedAngle;
			private MethodInfo _isBetweenLimits;

			public AnglometerBlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
				_targetDir = (Vector3)Util2.ReadPrivateField(BB, "targetDir");
				var t = typeof(AnglometerBlock);
				_correctedAngle = t.GetMethod("CorrectedAngle", BindingFlags.Instance | BindingFlags.NonPublic);
				_isBetweenLimits = t.GetMethod("IsBetweenLimits", BindingFlags.Instance | BindingFlags.NonPublic);

			}
			[JsInterface]
			public override bool IsActive => BB.Inverted.IsActive ^ (bool)_isBetweenLimits.Invoke(BB, new object[] { _correctedAngle.Invoke(BB, new object[] { _targetDir }) });

			[JsInterface]
			public Vector3 TargetDir => _targetDir;
			[JsInterface]
			public float Value => Angle;
			[JsInterface]
			public float Angle => BB.CurrentAngle(TargetDir);
		}

		public class SpeedometerBlockWrapper : ActivableBlockWrapper<SpeedometerBlock> {

			private MSlider _speedSlider;
			public SpeedometerBlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
				_speedSlider = (MSlider)Util2.ReadPrivateField(BB, "speedSlider");

			}
			[JsInterface]
			public float Value => Speed;
			[JsInterface]
			public float SqrValue => SqrSpeed;
			[JsInterface]
			public float Speed => Mathf.Sqrt(BB.SqrSpeed);
			[JsInterface]
			public float SqrSpeed => BB.SqrSpeed;
			[JsInterface]
			public override bool IsActive => BB.Inverted.IsActive ^ SqrValue > _speedSlider.Value * _speedSlider.Value;

		}

		public class TimerBlockWrapper : ActivableBlockWrapper<TimerBlock> {
			public TimerBlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
			}

			[JsInterface]
			public override bool IsActive {
				get {
					var phase = (int)Util2.ReadPrivateField(BB, "phase");
					return phase == 3;
				}
			}
		}

		public class LogicGateWrapper : ActivableBlockWrapper<LogicGate> {
			private FieldInfo _a;
			private FieldInfo _b;
			public LogicGateWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
				_a = typeof(LogicGate).GetField("A", BindingFlags.Instance | BindingFlags.NonPublic);
				_b = typeof(LogicGate).GetField("A", BindingFlags.Instance | BindingFlags.NonPublic);

			}
			[JsInterface]
			public bool A => (bool)_a.GetValue(BB);
			[JsInterface]
			public bool B => (bool)_b.GetValue(BB);
			[JsInterface]
			public string Operator => BB.Type.ToString();
			[JsInterface]
			public override bool IsActive {
				get {
					switch (BB.Type) {
						case LogicGate.GateType.NOT: return !A;
						case LogicGate.GateType.AND: return A && B;
						case LogicGate.GateType.NAND: return !(A && B);
						case LogicGate.GateType.OR: return A || B;
						case LogicGate.GateType.NOR: return !(A || B);
						case LogicGate.GateType.XOR: return A ^ B;
						case LogicGate.GateType.XNOR: return A == B;
					}
					return false;
				}
			}
			[JsInterface]
			public bool Value => IsActive;
		}

		public class GrabberWrapper : ActivableBlockWrapper<GrabberBlock> {
			public GrabberWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
			}
			[JsInterface]
			public override bool IsActive => BB.joinOnTriggerBlock.isJoined;
		}


		public class SliderCompressWrapper : ActivableBlockWrapper<SliderCompress> {
			public SliderCompressWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
			}
			[JsInterface]
			public override bool IsActive => BB.posToBe == BB.newLimit;
		}


		public class CpuWrapper : ActivableBlockWrapper<CpuController> {
			public CpuWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
			}
			[JsInterface]
			public override string Name => "Cpu";
			[JsInterface]
			public override bool IsActive => !BB.CpuBridge.IsFInished();
			[JsInterface]
			public int CpuId => _engineEx.Id;
		}
	}
}