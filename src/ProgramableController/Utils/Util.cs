using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramableController.Utils {
    public static class Util {
		private static void PrintEnvVar() {
			foreach (DictionaryEntry e in Environment.GetEnvironmentVariables()) {
				Logger.Info(e.Key + " = " + e.Value);
			}
		}

		public static bool IsExist(String path) {
			try {
				//ModConsole.Log("Find: " + path);
				if (ModIO.ExistsFile(path)) {
					return true;
				}
			} catch {
				//ModConsole.Log("" + e);
			}
			return false;
		}

		public static String FindBridge() {
			var dir = Const.BESIEGE_GAME_ASSENBLIES_DIR;
			if (dir == null) {
				Logger.Error("BESIEGE_GAME_ASSENBLIES_DIR is null");
				return null;
			}
			dir = dir.Replace('\\', '/');
			while (dir[dir.Length - 1] == '/') {
				dir = dir.Substring(0, dir.Length - 1);
			}
			var path1 = dir.Substring(0, dir.Length - "common/Besiege/Besiege_Data/Managed".Length) + "workshop/content/" + Const.STEAM_GAME_ID + "/" + Const.STEAM_MOD_ID + "/NoBounds/NoBounds.dll";
			if (IsExist(path1)) {
				return path1;
			}

			var path2 = dir.Substring(0, dir.Length - "Managed".Length) + "Mods/ProgramableController Project/ProgramableController/ProgramableControllerBridge.exe";
			if (IsExist(path2)) {
				return path2;
			}
			return null;
		}

		public static V GetOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultValue = default) {
			V val;
			if (dict.TryGetValue(key, out val)) {
				return val;
			} else {
				return defaultValue;
			}
		}


		
	}
}
