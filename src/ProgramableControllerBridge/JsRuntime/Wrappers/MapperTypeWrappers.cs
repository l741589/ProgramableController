using JintUnity.Native;
using JintUnity.Runtime.Interop;
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
    using Logger = ProgramableController.Utils.Logger;
    public static class MapperTypeWrappers {
        public class MTextWrapper : MapperTypeWrapper<MText> {

            public MTextWrapper(EngineEx blockWrapper, MText mapperType) : base(blockWrapper, mapperType) {
            }

            [JsInterface]
            public string Value {
                get => _mapperType.Value;
                set => _mapperType.SetValue(value);
            }
        }

        public class MKeyWrapper : MapperTypeWrapper<MKey> {
            public MKeyWrapper(EngineEx blockWrapper, MKey mapperType) : base(blockWrapper, mapperType) {
                for (int i = 0; i < mapperType.KeysCount; ++i) {
                    mapperType.GetKey(i);
                }
            }

            [JsInterface]
            public string[] Value {
                get => Enumerable.Range(0, _mapperType.KeysCount).Select(_mapperType.GetKey).Select(kc => kc.ToString()).ToArray();
                set {
                    var vc = value.Length;
                    for (int i = 0; i < vc; ++i) {
                        _mapperType.AddOrReplaceKey(i, Util2.AsKeyCode(value[i]));
                    }
                    var mc = _mapperType.KeysCount;
                    for (int i = vc; i < mc; ++i) {
                        _mapperType.RemoveKey(i);
                    }
                    _mapperType.RemoveRedundant();
                }
            }


            [JsInterface]
            public bool IsUp => _mapperType.IsReleased;
            [JsInterface]
            public bool IsPress => _mapperType.IsPressed;
            [JsInterface]
            public bool IsDown => _mapperType.IsHeld;
            [JsInterface]
            public bool Emulating => _mapperType.Emulating;
            [JsInterface]
            public float EmulationValue => _mapperType.EmulationValue();

    
            [JsInterface]
            public void UpdateEmulation(bool emulated) {
                _mapperType.UpdateEmulation(emulated);
            }

            [JsInterface]
            public void KeyDown() {
                UpdateEmulation(true);
            }

            [JsInterface]
            public void KeyUp() {
                UpdateEmulation(false);
            }

            [JsInterface]
            public void KeyPress(long timeout = 0) {
                KeyDown();
                if (timeout > 0) {
                    _engineEx.GetComponent<JsScheduler>()?.Schedule((_1, _2) => { KeyUp(); return JsValue.Undefined; }, timeout, new JsValue[0], false);
                } else {
                    _engineEx.NextFrameAction += () => _engineEx.NextFrameAction += () => KeyUp();
                }
            }
        }

        public class MColourSliderWrapper : MapperTypeWrapper<MColourSlider> {

            public MColourSliderWrapper(EngineEx blockWrapper, MColourSlider mapperType) : base(blockWrapper, mapperType) {
            }
            [JsInterface]
            public string Value {
                get => _mapperType.Value.a == 1 ? ColorUtility.ToHtmlStringRGB(_mapperType.Value) : ColorUtility.ToHtmlStringRGBA(_mapperType.Value);
                set {
                    if (ColorUtility.TryParseHtmlString(value, out Color color)) {
                        _mapperType.Value = color;
                    }
                }
            }
        }

        public class MHealthTypeWrapper : MapperTypeWrapper<MHealthType> {
            public MHealthTypeWrapper(EngineEx blockWrapper, MHealthType mapperType) : base(blockWrapper, mapperType) {
            }

            [JsInterface]
            public int Value {
                get => (int)_mapperType.HealthRange;
                set => _mapperType.HealthRange = (HealthRange)value;
            }
        }

        public class MLimitsWrapper : MapperTypeWrapper<MLimits> {
            public MLimitsWrapper(EngineEx blockWrapper, MLimits mapperType) : base(blockWrapper, mapperType) {
            }
            [JsInterface]
            public bool IsActive { get => _mapperType.UseLimitsToggle.IsActive; set => _mapperType.UseLimitsToggle.IsActive = value; }
            [JsInterface]
            public float MaxValue { get => _mapperType.MaxValue; set => _mapperType.MaxValue = value; }
            [JsInterface]
            public float Value { get => _mapperType.MaxValue; set => _mapperType.MaxValue = value; }
            [JsInterface]
            public float Max { get => _mapperType.Max; set => _mapperType.Max = value; }
            [JsInterface]
            public float Min { get => _mapperType.Min; set => _mapperType.Min = value; }
        }
        
        public class MNotSupportWrapper : MapperTypeWrapper<MapperType> {
            public MNotSupportWrapper(EngineEx blockWrapper, MapperType mapperType) : base(blockWrapper, mapperType) {
            }

            [JsInterface]
            public int Value {
                get {
                    Logger.Warn("EntityLogic is too complex to support, maybe implements it with your own code in CPU is easier");
                    return 0;
                }
                set {
                    Logger.Warn("EntityLogic is too complex to support, maybe implements it with your own code in CPU is easier");
                }
            }
        }


        public class MMenuWrapper : MapperTypeWrapper<MMenu> {
            public MMenuWrapper(EngineEx blockWrapper, MMenu mapperType) : base(blockWrapper, mapperType) {
            }


            [JsInterface]
            public string[] Items { get => _mapperType.Items?.ToArray(); set => _mapperType.Items = value?.ToList(); }
            [JsInterface]
            public int Value { get => _mapperType.Value; set => _mapperType.Value = value; }
            [JsInterface]
            public string Selection {
                get => _mapperType.Selection; set {
                    var index = _mapperType.Items.IndexOf(value);
                    if (index >= 0) {
                        _mapperType.Value = index;
                    }
                }
            }
        }

        public class MSliderWrapper : MapperTypeWrapper<MSlider> {
            public MSliderWrapper(EngineEx blockWrapper, MSlider mapperType) : base(blockWrapper, mapperType) {
            }
            [JsInterface]
            public float Value { get => _mapperType.Value; set => _mapperType.Value = value; }
        }

        public class MTeamWrapper : MapperTypeWrapper<MTeam> {
            public MTeamWrapper(EngineEx blockWrapper, MTeam mapperType) : base(blockWrapper, mapperType) {
            }
            [JsInterface]
            public int Value { get => (int)_mapperType.Team; set => _mapperType.Team = (MPTeam)value; }
            [JsInterface]
            public int Team { get => (int)_mapperType.Team; set => _mapperType.Team = (MPTeam)value; }
        }

        public class MToggleWrapper : MapperTypeWrapper<MToggle> {
            public MToggleWrapper(EngineEx blockWrapper, MToggle mapperType) : base(blockWrapper, mapperType) {
            }

            [JsInterface]
            public bool IsActive { get => _mapperType.IsActive; set => _mapperType.IsActive = value; }
            [JsInterface]
            public bool Value { get => _mapperType.IsActive; set => _mapperType.IsActive = value; }
        }

        public class MValueWrapper : MapperTypeWrapper<MValue> {
            public MValueWrapper(EngineEx blockWrapper, MValue mapperType) : base(blockWrapper, mapperType) {
            }
            [JsInterface]
            public float Value { get => _mapperType.Value; set => _mapperType.Value = value; }
        }

        public class MCustomWrapper : MapperTypeWrapper<MapperType> {

            private PropertyInfo valueProps;

            public MCustomWrapper(EngineEx blockWrapper, MapperType mapperType) : base(blockWrapper, mapperType) {
                valueProps = mapperType.GetType().GetProperty("Value");
            }
            [JsInterface]
            public object Value { get => valueProps.GetValue(_mapperType, new object[0]); set => valueProps.SetValue(_mapperType, value, new object[0]); }
        }

    }
}
