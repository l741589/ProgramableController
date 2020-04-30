using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProgramableController.Script {
    public class CpuOptions {
        public bool Strict { get; set; }
        public float Timeout { get; set; }
        public string Title { get; set; }
        public Action<KeyCode> KeyDown { get; set; }
        public Action<KeyCode> KeyUp { get; set; }
        public Action<KeyCode> KeyPress { get; set; }
        public Action<KeyCode, Action> SubscribeKeyDown;
        public Action<KeyCode, Action> SubscribeKeyUp;
        public Action<KeyCode, Action> SubscribeKeyPress;
        public Action<KeyCode> UnsubscribeKeyDown;
        public Action<KeyCode> UnsubscribeKeyUp;
        public Action<KeyCode> UnsubscribeKeyPress;

    }
}
