using JintUnity;
using JintUnity.Native;
using JintUnity.Runtime;
using JintUnity.Runtime.Interop;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgramableControllerBridge.JsRuntime.Component {

    public class JsScheduler : JsComponent{

        private class Item {
            private static int _id = 1;

            public readonly int id;
            public readonly ICallable callback;
            public readonly JsValue[] callbackParameters;
            public readonly long startTime;
            public readonly long timeout;
            public readonly bool repeat;
            public long invokeTime;
            public bool disabled = false;


            public Item(ICallable callback, long timeout, JsValue[] args, bool repeat) {
                this.id = Interlocked.Increment(ref _id);
                this.callback = callback;
                this.callbackParameters = args;
                this.startTime = DateTime.UtcNow.Ticks / 10000;
                this.timeout = timeout;
                this.invokeTime = this.startTime + timeout;
                this.repeat = repeat;
            }
        }

        private SortedList<long, List<Item>> _scheduler = new SortedList<long, List<Item>>();
        private Dictionary<int, Item> _itemMap = new Dictionary<int, Item>();
        private EngineEx _engineEx;
        private Engine _engine;

        public override void FixedUpdate() {
            long t = DateTime.UtcNow.Ticks/ 10000;
            List<Item> reschedule = new List<Item>();
            while (_scheduler.Count > 0 && t >= _scheduler.Keys[0]) {
                List<Item> items = PopItem(_scheduler.Keys[0]);
                foreach (var item in items) {
                    if (item.disabled) {
                        _itemMap.Remove(item.id);
                    }else if (item.repeat) {
                        reschedule.Add(item);
                    } else {
                        _itemMap.Remove(item.id);
                    }
                }
                foreach (var item in items) {
                    if (!item.disabled) {
                        _engine.ResetTimeoutTicks();
                        item.callback.Call(JsValue.Undefined, item.callbackParameters);
                    }
                }
            }
            foreach(var item in reschedule) {
                lock (_scheduler) {
                    item.invokeTime += item.timeout;
                    Schedule(item);
                }
            }
        }

        private List<Item> PopItem(long key) {
            lock (_scheduler) {
                List<Item> items = _scheduler[key];
                _scheduler.Remove(key);
               return items;
            }
        }

        private int Schedule(Item item) {
            lock (_scheduler) {
                List<Item> list;
                if (!_scheduler.TryGetValue(item.invokeTime, out list)) {
                    list = _scheduler[item.invokeTime] = new List<Item>();
                }
                list.Add(item);
            }
            _itemMap[item.id] = item;
            return item.id;
        }

        public JsValue ScheduleTimeout(params JsValue[] args) {
            return ScheduleTimeout(args, false);
        }

        public JsValue ScheduleInterval(params JsValue[] args) {
            return ScheduleTimeout(args, true);
        }

        private JsValue ScheduleTimeout(JsValue[] args, bool repeat) {
            if (args.Length == 0) {
                return JsValue.Undefined;
            }
            JsValue vCb = args[0];
            JsValue vTimeout = args.Length > 1 ? args[1] : new JsValue(0);
            JsValue[] cbArgs = args.Length > 2 ? args.Skip(2).ToArray() : new JsValue[0];

            ICallable cb;
            if (vCb.IsString()) {
                cb = new DelegateWrapper(_engine, new Action(() => _engine.Eval.Call(JsValue.Undefined, cbArgs)));
            } else {
                cb = vCb.TryCast<ICallable>();
            }
            if (cb == null) {
                throw new JavaScriptException(_engine.TypeError, "argument 1 requires a function or a string");
            }
           
            int timeout = TypeConverter.ToInt32(vTimeout);
            //Logger.Debug("timeout {0}", timeout);
            //Logger.Debug("val {0}", vTimeout);
            Item item = new Item(cb, timeout, cbArgs, repeat);
            return Schedule(item);
        }

        internal JsValue Schedule(Func<JsValue,JsValue[],JsValue> action, long timeout, JsValue[] args, bool repeat) {
            return Schedule(new Item(new DelegateWrapper(_engine,action), timeout, args, repeat));
        }


        public override bool IsFinish =>_scheduler.Values.Select(e => e.Where(f => !f.disabled).Count()).Sum() == 0;

        private bool Unschedule(int id) {
            Item item;
            if (_itemMap.TryGetValue(id,out item)) {
                _itemMap.Remove(id);
                item.disabled = true;
                lock(_scheduler) {
                    List<Item> items;
                    if (_scheduler.TryGetValue(item.invokeTime,out items)) {
                        items.Remove(item);
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Register(EngineEx engineEx, Engine engine) {
            _engineEx = engineEx;
            _engine = engine;
            engineEx.Register("setTimeout", new Func<JsValue[], JsValue>(ScheduleTimeout));
            engineEx.Register("setInterval", new Func<JsValue[], JsValue>(ScheduleInterval));
            engineEx.Register("clearTimeout", new Func<int, bool>(Unschedule));
            engineEx.Register("clearInterval", new Func<int, bool>(Unschedule));
        }

        public override void Stop() {
            _scheduler.Clear();
            _itemMap.Clear();
        }
    }
}
