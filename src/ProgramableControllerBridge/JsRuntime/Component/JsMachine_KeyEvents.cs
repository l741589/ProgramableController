using JintUnity;
using JintUnity.Native;
using JintUnity.Runtime;
using JintUnity.Runtime.Interop;
using Modding;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime.Core;
using ProgramableControllerBridge.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Logger = ProgramableController.Utils.Logger;

namespace ProgramableControllerBridge.JsRuntime.Component {

    using KeyEvents = Dictionary<KeyCode, List<Action>>;

    public partial class JsMachine : JsComponent{

        private Action<KeyCode, bool> emulateKey;
        private KeyInputController keyInputController;
        private KeyEvents keyDownEvent = new KeyEvents();
        private KeyEvents keyUpEvent = new KeyEvents();
        private KeyEvents keyPressEvent = new KeyEvents();

        [JsInterface]
        public void SubscribeKeyDown(JsValue keyCode,JsValue callback) {
            Subscribe(ref keyDownEvent, keyCode, callback);
        }

        [JsInterface]
        public void SubscribeKeyUp(JsValue keyCode, JsValue callback) {
            Subscribe(ref keyUpEvent, keyCode, callback);
        }

        [JsInterface]
        public void SubscribeKeyPress(JsValue keyCode, JsValue callback) {
            Subscribe(ref keyPressEvent, keyCode, callback);
        }

        [JsInterface]
        public void UnsubscribeKeyDown(JsValue keyCode) {
            Unsubscribe(ref keyDownEvent, keyCode);
        }
        [JsInterface]
        public void UnsubscribeKeyUp(JsValue keyCode) {
            Unsubscribe(ref keyUpEvent, keyCode);
        }
        [JsInterface]
        public void UnsubscribeKeyPress(JsValue keyCode) {
            Unsubscribe(ref keyPressEvent, keyCode);
        }

        [JsInterface]
        public void KeyDown(JsValue value) {
            emulateKey(Util2.AsKeyCode(value), true);
        }
        [JsInterface]
        public void KeyUp(JsValue value) {
            emulateKey(Util2.AsKeyCode(value), false);
        }

        [JsInterface]
        public void KeyPress(JsValue value) {
            var code = Util2.AsKeyCode(value);
            //Logger.Debug("Down:" + code);
            emulateKey(code, true);
            _engineEx.NextFrameAction += () => _engineEx.NextFrameAction += () => emulateKey(code, false);
        }

        [JsInterface]
        public void KeyPress(JsValue value, JsValue timeout) {
            var code = Util2.AsKeyCode(value);
            //Logger.Debug("Down:" + code);
            emulateKey(code,true);
            _engineEx.GetComponent<JsScheduler>().Schedule((_1, _2) => {
                //Logger.Debug("Up:" + code);
                emulateKey(code,false);
                return JsValue.Undefined;
            }, TypeConverter.ToInt32(timeout), new JsValue[0], false);
        }

        public override void KeyEmulationUpdate() {
            InvokeKeyEvent(keyDownEvent, keyInputController.IsHeld);
            InvokeKeyEvent(keyPressEvent, keyInputController.IsPressed);
            InvokeKeyEvent(keyUpEvent, keyInputController.IsReleased);
        }

        private void InvokeKeyEvent(KeyEvents events, Predicate<KeyCode> isActive) {
            foreach (KeyValuePair<KeyCode, List<Action>> p in events) {
                if (isActive(p.Key)) {
                    foreach (Action a in p.Value) {
                        _engine.ResetTimeoutTicks();
                        a();
                    }
                }
            }
        }

        private void Subscribe(ref KeyEvents events, JsValue keyCodeVal, JsValue action) {
            List<Action> actions;
            var keyCode = Util2.AsKeyCode(keyCodeVal);
            if (events.TryGetValue(keyCode, out actions)) {
                events = new KeyEvents(events);
                events[keyCode] = actions = new List<Action>(actions);
            } else {
                events = new KeyEvents(events);
                events[keyCode] = actions = new List<Action>();
            }
            //Logger.Debug("subscribe " + keyCode);
            actions.Add(()=>action.Invoke((int)keyCode));
            if (keyInputController != null) {
                keyInputController.Add(keyCode);
            }
        }

        private void Unsubscribe(ref KeyEvents events, JsValue keyCodeVal) {
            var keyCode = Util2.AsKeyCode(keyCodeVal);
            //Logger.Debug("Unsubscribe " + keyCode);
            events = new KeyEvents(events);
            events.Remove(keyCode);
        }

        public override void Stop() {
            this.keyDownEvent.Clear();
            this.keyPressEvent.Clear();
            this.keyUpEvent.Clear();

        }

        public override bool IsFinish => keyDownEvent.Count() == 0 && keyPressEvent.Count() == 0 && keyUpEvent.Count() == 0;
    }
}
