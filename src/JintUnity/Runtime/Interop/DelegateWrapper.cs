using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JintUnity.Native;
using JintUnity.Native.Function;

namespace JintUnity.Runtime.Interop
{
    /// <summary>
    /// Represents a FunctionInstance wrapper around a CLR method. This is used by user to pass
    /// custom methods to the engine.
    /// </summary>
    public sealed class DelegateWrapper : FunctionInstance
    {
        private readonly Delegate _d;

        public DelegateWrapper(Engine engine, Delegate d) : base(engine, null, null, false)
        {
            _d = d;
            Prototype = engine.Function.PrototypeObject;
        }

        public override JsValue Call(JsValue thisObject, JsValue[] jsArguments)
        {
            var parameterInfos = _d.GetMethodInfo().GetParameters();

            bool delegateContainsParamsArgument = parameterInfos.Any(p => p.HasAttribute<ParamArrayAttribute>());
            int delegateArgumentsCount = parameterInfos.Length;
            int delegateNonParamsArgumentsCount = delegateContainsParamsArgument ? delegateArgumentsCount - 1 : delegateArgumentsCount;

            int jsArgumentsCount = jsArguments.Length;
            int jsArgumentsWithoutParamsCount = Math.Min(jsArgumentsCount, delegateNonParamsArgumentsCount);

            var parameters = new object[delegateArgumentsCount];

            // convert non params parameter to expected types
            for (var i = 0; i < jsArgumentsWithoutParamsCount; i++)
            {
                var parameterType = parameterInfos[i].ParameterType;

                if (parameterType == typeof (JsValue))
                {
                    parameters[i] = jsArguments[i];
                }
                else
                {
                    parameters[i] = Engine.ClrTypeConverter.Convert(
                        jsArguments[i].ToObject(),
                        parameterType,
                        CultureInfo.InvariantCulture);
                }
            }

            // assign null to parameters not provided
            for (var i = jsArgumentsWithoutParamsCount; i < delegateNonParamsArgumentsCount; i++)
            {
                if (parameterInfos[i].ParameterType.IsValueType())
                {
                    parameters[i] = Activator.CreateInstance(parameterInfos[i].ParameterType);
                }
                else
                {
                    parameters[i] = null;
                }
            }

            // assign params to array and converts each objet to expected type
            if(delegateContainsParamsArgument)
            {
                int paramsArgumentIndex = delegateArgumentsCount - 1;
                int paramsCount = Math.Max(0, jsArgumentsCount - delegateNonParamsArgumentsCount);

                Type parameterType = parameterInfos[paramsArgumentIndex].ParameterType;

                var paramsParameterType = parameterType.GetElementType();
                Array paramsParameter = (Array)parameterType.GetConstructor(new Type[] { typeof(int) }).Invoke(new object[] { paramsCount });

                for (var i = paramsArgumentIndex; i < jsArgumentsCount; i++)
                {
                    var paramsIndex = i - paramsArgumentIndex;

                    if (paramsParameterType == typeof(JsValue))
                    {
                        paramsParameter.SetValue(jsArguments[i], paramsIndex);
                    }
                    else
                    {
                        paramsParameter.SetValue(Engine.ClrTypeConverter.Convert(
                            jsArguments[i].ToObject(),
                            paramsParameterType,
                            CultureInfo.InvariantCulture),
                            paramsIndex);
                    }
                }
                parameters[paramsArgumentIndex] = paramsParameter;
            }
            try
            {
                return JsValue.FromObject(Engine, _d.DynamicInvoke(parameters));
            }
            catch (TargetInvocationException exception)
            {
                var meaningfulException = exception.InnerException ?? exception;
                var handler = Engine.Options._ClrExceptionsHandler;

                if (handler != null && handler(meaningfulException))
                {
                    throw new JavaScriptException(Engine.Error, meaningfulException.Message);
                }

                throw meaningfulException;         
            }
        }
    }
}
