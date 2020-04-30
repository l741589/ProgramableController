using JintUnity;
using JintUnity.Runtime.Interop;
using Modding.Blocks;
using ProgramableController;
using ProgramableController.Script;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime.Wrappers;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

namespace ProgramableControllerBridge.JsRuntime.Component {

    public class BlockIndex {
        private Dictionary<string, List<Block>> _data = new Dictionary<string, List<Block>>();

        public void Add(Block block, params string[] keys) {
            if (keys == null) {
                return;
            }
            foreach (var key in keys) {
                if (key == null) {
                    continue;
                }
                List<Block> list;
                if (!_data.TryGetValue(key.ToLower(), out list)) {
                    _data[key.ToLower()] = list = new List<Block>();
                }
                //Logger.Debug("Add: " + key);
                list.Add(block);
            }
        }

        public List<Block> Gets(string key) {
            List<Block> ret;
            if (key!= null && _data.TryGetValue(key.ToLower(), out ret)) {
                if (ret != null) {
                    return ret;
                }
            }
            return new List<Block>();
        }

        public Block Get(string key) {
            return Gets(key).FirstOrDefault();
        }

        public void Clear() {
            _data.Clear();
        }
    }

    public class JsMachineFactory : JsComponentFactory {

        public readonly BlockIndex NameIndex = new BlockIndex();
        public readonly BlockIndex GuidIndex = new BlockIndex();
        public readonly BlockIndex ClassNameIndex = new BlockIndex();
        public KeyInputController KeyInputController { get; private set; }

        public override void Start(PlayerMachine machine) {
            KeyInputController = machine.InternalObject.GetComponent<KeyInputController>();
            foreach (var block in machine.SimulationBlocks) {
                NameIndex.Add(block, block.Prefab.Name);
                GuidIndex.Add(block, block.Guid.ToString());
                ClassNameIndex.Add(block, BlockWrapper.GetClassNames(block));
            }
        }

        public override void Stop(PlayerMachine machine) {
            NameIndex.Clear();
            GuidIndex.Clear();
            ClassNameIndex.Clear();
        }

        public override JsComponent Create() {
            return new JsMachine(this);
        }
    }

    public partial class JsMachine : JsComponent {

        private JsMachineFactory _factory;
        private EngineEx _engineEx;
        private Engine _engine;

        public JsMachine(JsMachineFactory factory) {
            _factory = factory;
        }

        public override void Register(EngineEx engineEx, Engine engine) {
            _engineEx = engineEx;
            _engine = engine;
            _engineEx.Register("machine", this);
            keyInputController = _factory.KeyInputController;
            emulateKey = (code, down) => engineEx.Cpu.EmulateKeys(new MKey[0], new MKey("", "__emulated", code, true), down);
        }

        [JsInterface]
        public Block GetBlockByName(string name) {
            return _factory.NameIndex.Get(name?.ToLower());
        }

        [JsInterface]
        public Block[] GetBlocksByName(string name) {
            return _factory.NameIndex.Gets(name?.ToLower()).ToArray();
        }

        [JsInterface]
        public Block GetBlockByGuid(string guid) {
            return _factory.GuidIndex.Get(guid?.ToLower());
        }

       /* [JsInterface]
        public Block[] GetBlocksByGuid(string guid) {
            return _factory.GuidIndex.Gets(guid?.ToLower()).ToArray();
        }
*/
        [JsInterface]
        public Block GetBlockByClassName(string className) {
            return _factory.ClassNameIndex.Get(className?.ToLower());
        }

        [JsInterface]
        public Block[] GetBlocksByClassName(string className) {
            return _factory.ClassNameIndex.Gets(className?.ToLower()).ToArray();
        }
    }

}
