using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramableController.Utils {
    public static class Logger {

        public enum Level {
            VERBOSE = 0,
            DEBUG = 1,
            INFO = 2,
            WARN = 3,
            ERROR = 4
        }

        private static string[] levelNames = Enum.GetNames(typeof(Level));
        public static Level LogLevel { get; set; }
        private static Action<string> log;


        static Logger() {
            if (Environment.CurrentDirectory != null && Environment.CurrentDirectory.Contains("ProgramableControllerTest")) {
                log = Console.WriteLine;
            } else {
                log = ModConsole.Log;
            }
        }


        public static void Log(string s) {
            log(s);
        }


       public static void Log(Level level, object s) {
            if (LogLevel > level) {
                return;
            }
            Log("[ProgramableController][" + levelNames[(int)level] + "] " + s);
        }


        public static void Log(Level level, string fmt, params object[] args) {
            if (LogLevel > level) {
                return;
            }
            if (args == null || args.Length == 0) {
                Log(level, (object)fmt);
            } else {
                Log("[ProgramableController][" + levelNames[(int)level] + "] " + String.Format(fmt, args));
            }
        }

        public static void Debug(object o) {
            Log(Level.DEBUG, o);
        }

        public static void Debug(string fmt,params object[] args) {
            Log(Level.DEBUG, fmt,args);
        }

        public static void Info(object o) {
            Log(Level.INFO, o);
        }

        public static void Info(string fmt, params object[] args) {
            Log(Level.INFO, fmt, args);
        }

        public static void Warn(object o) {
            Log(Level.WARN, o);
        }

        public static void Warn(string fmt, params object[] args) {
            Log(Level.WARN, fmt, args);
        }

        public static void Error(object o) {
            Log(Level.ERROR, o);
        }

        public static void Error(string fmt, params object[] args) {
            Log(Level.ERROR, fmt, args);
        }

    }
}
