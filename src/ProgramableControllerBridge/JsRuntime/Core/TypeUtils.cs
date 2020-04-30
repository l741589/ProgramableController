using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProgramableControllerBridge.JsRuntime.Core {
    public static class TypeUtils {
        public delegate void Action0();
        public delegate void Action1<T1>();
        public delegate void Action2<T1, T2>();
        public delegate void Action3<T1, T2, T3>();
        public delegate void Action4<T1, T2, T3, T4>();
        public delegate void Action5<T1, T2, T3, T4, T5>();
        public delegate void Action6<T1, T2, T3, T4, T5, T6>();
        
        public delegate R Func0<R>();
        public delegate R Func1<T1,R>();
        public delegate R Func2<T1, T2,R>();
        public delegate R Func3<T1, T2, T3,R>();
        public delegate R Func4<T1, T2, T3, T4,R>();
        public delegate R Func5<T1, T2, T3, T4, T5,R>();
        public delegate R Func6<T1, T2, T3, T4, T5, T6,R>();


        private static Type[] _actions = { typeof(Action0), typeof(Action1<>), typeof(Action2<,>), typeof(Action3<,,>), typeof(Action4<,,,>), typeof(Action5<,,,,>), typeof(Action6<,,,,,>) };
        private static Type[] _funcs = { typeof(Func0<>), typeof(Func1<,>), typeof(Func2<,,>), typeof(Func3<,,,>), typeof(Func4<,,,,>), typeof(Func5<,,,,,>), typeof(Func6<,,,,,,>) };

        public static Type GetMethodDelegateType(MethodInfo m) {
            if (m.ReturnType == typeof(void)) {
                var ps = m.GetParameters();
                return _actions[ps.Length].MakeGenericType(ps.Select(p => p.ParameterType).ToArray());
            } else {
                var ps = m.GetParameters();
                return _funcs[ps.Length].MakeGenericType(ps.Select(p => p.ParameterType).Concat(new Type[] { m.ReturnType }).ToArray());
            }
        }

        public static Delegate AsDelegate(MethodInfo m) {
            return Delegate.CreateDelegate(GetMethodDelegateType(m), m);
        }

        public static Delegate AsDelegate(object @this,MethodInfo m) {
            return Delegate.CreateDelegate(GetMethodDelegateType(m), @this,m.Name);
        }
    }
}
