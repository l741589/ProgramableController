using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JintUnity.Runtime {
    using SystemCompat;

    namespace Jint.Runtime {
        public class ExecutionInterruptedException : Exception {
            public ExecutionInterruptedException() : base("The execution is interrupted.") {
            }
        }
    }

}
