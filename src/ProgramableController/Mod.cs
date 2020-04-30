using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Modding;
using Modding.Blocks;
using Modding.Mapper;
using Mono.Cecil;
using ProgramableController.Bridges;
using ProgramableController.Script;
using ProgramableController.Utils;
using UnityEngine;
using Logger = ProgramableController.Utils.Logger;

namespace ProgramableController {
	public class Mod : ModEntryPoint {
		public static Mod Instance { get; set; }

		public ModBridge Bridge { get; set; }

		public Mod() {
			Instance = this;
		}

		public override void OnLoad() {
			Const.Init();
			CustomMapperTypes.AddMapperType<string, TextArea, TextAreaSelector>();
			AppDomain.CurrentDomain.ExecuteAssembly(Util.FindBridge());
			Bridge.OnLoad();
	
		}
	}
	
}
