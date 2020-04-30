using JintUnity;
using Modding;
using Modding.Blocks;
using ProgramableController;
using ProgramableController.Bridges;
using ProgramableController.Script;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime;
using ProgramableControllerBridge.JsRuntime.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ProgramableControllerBridge {


	public class ModBridgeImpl : ModBridge {

		public static readonly ModBridgeImpl Instance = new ModBridgeImpl();
		public readonly List<JsComponentFactory> ComponentFactories = new List<JsComponentFactory>();

		public CpuBridge CreateCpuBridge(CpuController cpu) {
			return new CpuBridgeImpl(cpu);
		}

		public void OnLoad() {
			ComponentFactories.Add(new JsMachineFactory());
			ComponentFactories.Add(new JsStorageFactory());
			ComponentFactories.Add(new JsMessageFactory());
			Events.OnBlockInit += Events_OnBlockInit;
			Events.OnBlockRemoved += Events_OnBlockRemoved;
			Events.OnMachineSimulationToggle += Events_OnMachineSimulationToggle;
		}


		private void Events_OnMachineSimulationToggle(PlayerMachine machine, bool sim) {
			if (sim) {
				int id = 0;
				foreach(var e in machine.SimulationBlocks) {
					if (e.BlockScript is CpuController) {
						(e.BlockScript as CpuController).CpuId = ++id;
					}
				}
				ComponentFactories.ForEach(e => e.Start(machine));
			} else {
				ComponentFactories.ForEach(e => e.Stop(machine));
			}
		}

		private void Events_OnBlockInit(Block obj) {
			if (obj.GameObject.GetComponent<BlockTagAddon>() == null) {
				obj.GameObject.AddComponent<BlockTagAddon>();
			}
		}


		private void Events_OnBlockRemoved(Block obj) {

		}
	}

    public class Program {

        public static void Main() {
            Mod.Instance.Bridge = ModBridgeImpl.Instance;
        }
    }
}
