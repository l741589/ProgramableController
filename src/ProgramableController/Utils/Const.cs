using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ProgramableController.Utils {
    public static class Const {

        public static String BESIEGE_GAME_ASSENBLIES_DIR;
        public static int STEAM_GAME_ID = 346010;
        public static int STEAM_MOD_ID = 1473122210;
        public static string BRIDGE_PATH;
        public static Dictionary<string, KeyCode> KEY_CODES;
        public static string KEY_BLOCK_ADDITION_TAG = "progamablecontroller-tag";

        public static void Init() {
            BESIEGE_GAME_ASSENBLIES_DIR = Environment.GetEnvironmentVariable("BESIEGE_GAME_ASSEMBLIES");
            BRIDGE_PATH = Util.FindBridge();

            KEY_CODES = new Dictionary<string, KeyCode>();
            foreach (KeyCode e in Enum.GetValues(typeof(KeyCode))) {
                KEY_CODES[e.ToString()] = e;
                KEY_CODES[e.ToString().ToLower()] = e;
                KEY_CODES[e.ToString().ToUpper()] = e;
                KEY_CODES[Regex.Replace(e.ToString(), "^[a-z]", m => m.Value.ToLower())] = e;
                KEY_CODES[Regex.Replace(e.ToString(), "(\\w)([A-Z0-9])", m => m.Groups[0] + "_" + m.Groups[1].Value).ToUpper()] = e;
                KEY_CODES[Regex.Replace(e.ToString(), "(\\w)([A-Z0-9])", m => m.Groups[0] + "_" + m.Groups[1].Value).ToLower()] = e;
            }



        }

     
    }
}
