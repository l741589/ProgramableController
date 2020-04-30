using JintUnity;
using JintUnity.Native;
using JintUnity.Native.Object;
using JintUnity.Runtime;
using JintUnity.Runtime.Interop;
using Modding;
using Modding.Blocks;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime.Component;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ProgramableControllerBridge.JsRuntime.Wrappers {

    public class BlockConverter : ObjectConverterAdapter<Block> {

        private static TypePair[] _wrapperTypes = typeof(BlockWrappers)
              .GetNestedTypes()
              .Select(e => new TypePair(e))
              .ToArray();

        private static Dictionary<Type, TypePair> _found = new Dictionary<Type, TypePair>();

        public override object Convert(Block block) {
            if (_found.TryGetValue(block.InternalObject.GetType(), out TypePair _t)) {
                return (BlockWrapper)Activator.CreateInstance(_t.WrapperType, _engineEx, block);
            } else {
                foreach (var t in _wrapperTypes) {
                    var ib = block.InternalObject;
                    if (
                        t.InnerType.IsInstanceOfType(ib) ||
                        ib.GetComponent(t.InnerType)||
                        ib.gameObject.GetComponent(t.InnerType)
                    ) {
                        _found[ib.GetType()] = t;
                        return (BlockWrapper)Activator.CreateInstance(t.WrapperType, _engineEx, block);
                    }
                }
            }
            return new BlockWrapper(_engineEx, block);

        }
    }

    [JsInterface]
    public class BlockWrapper {

        internal readonly Block _block;
        internal readonly EngineEx _engineEx;

        internal BlockWrapper(EngineEx engineEx,Block block) {
            this._block = block;
            this._engineEx = engineEx;
        }

        [JsInterface]
        public string[] ClassNames => GetClassNames(_block);

        internal static string[] GetClassNames(Block block) {
            return Regex.Split((block.InternalObject.GetMapperType("bmt-" + Const.KEY_BLOCK_ADDITION_TAG) as MText)?.Value ?? "", "[,\\s]+")
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(e => e.Trim().ToLower()).ToArray();
        }

        [JsInterface]
        public virtual string Name => _block.Prefab.Name;
        [JsInterface]
        public virtual string Comp => _block.InternalObject?.GetType().Name;
        [JsInterface]
        public virtual string Guid => _block.Guid.ToString();

        private MapperType GetMapperType(string key) {
            return _block.InternalObject.GetMapperType("bmt-"+key) ?? _block.InternalObject.GetMapperType(key);
        }

        [JsInterface]
        public MapperType GetAttribute(string key) {
            return GetMapperType(key);
        }

        [JsInterface]
        public MapperType this[string key] => GetAttribute(key);

        [JsInterface]
        public void Print() {
            ModConsole.Log("===============");
            ModConsole.Log("Name = " + Name);
            ModConsole.Log("Comp = " + Comp);
            ModConsole.Log("Guid = " + Guid);
            ModConsole.Log("---------------");
            ModConsole.Log("Type".PadRight(16)+ " | " + "Key".PadRight(32) + " | " + "DisplayName");
            foreach (var mt in _block.InternalObject.MapperTypes) {
                ModConsole.Log(mt.GetType().Name.PadRight(16)+" | "+mt.Key.PadRight(32) + " | "+mt.DisplayName);
            }
            ModConsole.Log("===============");
        }

        [JsInterface]
        public override string ToString() {
            return _block.ToString();
        }
    }

    public abstract class BlockWrapper<T> : BlockWrapper where T : class {
        private T _bb;

        public BlockWrapper(EngineEx engineEx, Block block) : base(engineEx, block) {
        }

        public T BB => _bb ?? (_bb = (
            (_block.InternalObject as T) ?? 
            _block.InternalObject.GetComponent<T>() ?? 
            _block.InternalObject.gameObject.GetComponent<T>()
            ));

        [JsInterface]
        public override string Comp => BB?.GetType().Name ?? "UNKNOWN";
    }

}
