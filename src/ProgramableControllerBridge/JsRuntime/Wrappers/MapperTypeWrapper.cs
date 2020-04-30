using JintUnity.Native;
using JintUnity.Runtime.Interop;
using Modding.Mapper;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime.Core;
using ProgramableControllerBridge.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using static ProgramableControllerBridge.JsRuntime.Wrappers.MapperTypeWrappers;
using Logger = ProgramableController.Utils.Logger;

namespace ProgramableControllerBridge.JsRuntime.Wrappers {

    public class MapperTypeConverter : ObjectConverterAdapter<MapperType> {

        private static TypePair[] wrapperTypes = typeof(MapperTypeWrappers)
                .GetNestedTypes()
                .Select(e => new TypePair(e))
                .Where(e => e.InnerType != typeof(MapperType))
                .ToArray();

        public override object Convert(MapperType mapperType) {
            foreach (var t in wrapperTypes) {
                if (t.InnerType.IsInstanceOfType(mapperType)) {
                    return (MapperTypeWrapper)Activator.CreateInstance(t.WrapperType, _engineEx, mapperType);
                }
            }
            if (typeof(MCustom<>).IsInstanceOfType(mapperType)) {
                return new MCustomWrapper(_engineEx, mapperType);
            }
            return new MNotSupportWrapper(_engineEx, mapperType);
        }
    }

    [JsInterface]
    public class MapperTypeWrapper {
        [JsInterface]
        public override string ToString() {
            var t = GetType();
            Logger.Debug(JintUnity.Utils.Join(", ", t.GetProperties().Select(e=>e.Name)));
            return t.Name.Substring(0, t.Name.Length - 7);
        }
    }

    [JsInterface]
    public abstract class MapperTypeWrapper<T> : MapperTypeWrapper where T : MapperType {

        protected readonly T _mapperType;
        protected readonly EngineEx _engineEx;

        public MapperTypeWrapper(EngineEx _engineEx,T mapperType) {
            this._mapperType = mapperType;
            this._engineEx = _engineEx;
        }
    }


  

}