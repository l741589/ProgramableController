using JintUnity.Native;
using JintUnity.Runtime;
using ProgramableController.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Logger = ProgramableController.Utils.Logger;

namespace ProgramableControllerBridge.Utils {
    public static class Util2 {
		public static KeyCode AsKeyCode(JsValue value) {
			if (value == null) {
				Logger.Warn("invalid keycode: " + value);
				return KeyCode.None;
			} else if (value.IsNumber()) {
				return (KeyCode)TypeConverter.ToInt32(value);
			} else if (value.IsString()) {
				return AsKeyCode(TypeConverter.ToString(value));
			}
			Logger.Warn("invalid keycode: " + value);
			return KeyCode.None;
		}

		public static KeyCode AsKeyCode(string value) {
			if (int.TryParse(value, out int num)) {
				return (KeyCode)num;
			} else if (Const.KEY_CODES.TryGetValue(value, out KeyCode code)) {
				return code;
			}
			return KeyCode.None;
		}

		public static object ReadPrivateField(object obj,string field) {
			return obj.GetType().InvokeMember(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, obj, null);
		}
	}
}
