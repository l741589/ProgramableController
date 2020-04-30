using JintUnity;
using JintUnity.Native;
using JintUnity.Runtime.Interop;
using Modding.Blocks;
using ProgramableController.Utils;
using ProgramableControllerBridge.JsRuntime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgramableControllerBridge.JsRuntime.Component {
    [JsInterface]
    public class JsMessageContent {
        [JsInterface]
        public DateTime Timestamp = new DateTime();
        [JsInterface]
        public int Id;
        [JsInterface]
        public int ReplyTo = -1;
        [JsInterface]
        public int Sender;
        [JsInterface]
        public int Target;
        [JsInterface]
        public object Body;
    }

    public delegate void JsMessageConsumer(JsMessageContent cx);

    public class JsMessageFactory : JsComponentFactory {

        private int _idCount = 0;
        public Dictionary<int, JsMessageConsumer> Consumers = new Dictionary<int, JsMessageConsumer>();
        public int NewId() {
            return Interlocked.Increment(ref _idCount);
        }

        public override JsComponent Create() {
            return new JsMessage(this);
        }

        public override void Start(PlayerMachine machine) {
            Consumers.Clear();
        }

        public override void Stop(PlayerMachine machine) {
            Consumers.Clear();
        }
    }

    public class JsMessage : JsComponent {
        private JsMessageFactory _factory;
        private int _id;
        private Engine _engine;
        private EngineEx _engineEx;

        public JsMessage(JsMessageFactory factory) {
            this._factory = factory;
        }

        public override void Register(EngineEx engineEx, Engine engine) {
            this._id = engineEx.Id;
            this._engine = engine;
            this._engineEx = engineEx;
            engineEx.Register("message", this);
        }

        private JsMessageContent CreateMessageContent(object body, int target) {
            JsMessageContent content = new JsMessageContent();
            content.Id = _factory.NewId();
            content.Target = target;
            content.Sender = _id;
            content.Body = body;
            return content;
        }

        [JsInterface]
        public void Broadcast(object body) {
            JsMessageContent content = CreateMessageContent(body, -1);
            _engineEx.NextFrameAction += () => {
                foreach (var c in _factory.Consumers.Values) {
                    c?.Invoke(content);
                }
            };
        }

        [JsInterface]
        public void Send(int target, object body) {
            JsMessageContent content = CreateMessageContent(body, target);
            _engineEx.NextFrameAction += () => _factory.Consumers.GetOrDefault(target)?.Invoke(content);
        }

        [JsInterface]
        public void Subscribe(JsMessageConsumer consumer) {
            if (!_factory.Consumers.TryGetValue(_id, out JsMessageConsumer c)) {
                _factory.Consumers[_id] = cx => _engine.ResetTimeoutTicks();
            }
            _factory.Consumers[_id] += consumer;
        }

        [JsInterface]
        public void Unsubscribe() {
            _factory.Consumers.Remove(_id);
        }

        public override bool IsFinish => !_factory.Consumers.ContainsKey(_id);
    }
}
