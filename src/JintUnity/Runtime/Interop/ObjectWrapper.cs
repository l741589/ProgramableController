using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JintUnity.Native;
using JintUnity.Native.Object;
using JintUnity.Runtime.Descriptors;
using JintUnity.Runtime.Descriptors.Specialized;

namespace JintUnity.Runtime.Interop
{
	/// <summary>
	/// Wraps a CLR instance
	/// </summary>
	public sealed class ObjectWrapper : ObjectInstance, IObjectWrapper
    {
        public Object Target { get; set; }
        private Type type;
        private bool isInterface;

        public ObjectWrapper(Engine engine, Object obj)
            : base(engine)
        {
            Target = obj;
            type = Target.GetType();
            isInterface = IsJsInterface(type);
            if (!isInterface) {
                throw new JavaScriptException(engine.TypeError, "type "+type+" is not support for javascript");
            }
        }

        public override void Put(string propertyName, JsValue value, bool throwOnError)
        {
            if (!CanPut(propertyName))
            {
                if (throwOnError)
                {
                    throw new JavaScriptException(Engine.TypeError);
                }

                return;
            }

            var ownDesc = GetOwnProperty(propertyName);

            if (ownDesc == null)
            {
                if (throwOnError)
                {
                    throw new JavaScriptException(Engine.TypeError, "Unknown member: " + propertyName);
                }
                else
                {
                    return;
                }
            }

            ownDesc.Value = value;
        }

        private bool IsJsInterface(MemberInfo memberInfo) {
            return memberInfo?.GetCustomAttributes(true)?.Any(e => e is JsInterfaceAttribute) ?? false;
        }        

        public override PropertyDescriptor GetOwnProperty(string propertyName)
        {
            PropertyDescriptor x;
            if (Properties.TryGetValue(propertyName, out x))
                return x;
           
            // look for a property
            var property = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
                .Where(p => EqualsIgnoreCasing(p.Name, propertyName))
                .Where(p => !isInterface || IsJsInterface(p))
                .FirstOrDefault();
            if (property != null)
            {
                var descriptor = new PropertyInfoDescriptor(Engine, property, Target);
                Properties.Add(propertyName, descriptor);
                return descriptor;
            }

            // look for a field
            var field = type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
                .Where(f => EqualsIgnoreCasing(f.Name, propertyName))
                .Where(f => !isInterface || IsJsInterface(f))
                .FirstOrDefault();
            if (field != null)
            {
                var descriptor = new FieldInfoDescriptor(Engine, field, Target);
                Properties.Add(propertyName, descriptor);
                return descriptor;
            }

            // if no properties were found then look for a method 
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
                .Where(m => EqualsIgnoreCasing(m.Name, propertyName))
                .Where(m => !isInterface || IsJsInterface(m))
                .ToArray();

            if (methods.Any())
            {
                var descriptor = new PropertyDescriptor(new MethodInfoFunctionInstance(Engine, methods), false, true, false);
                Properties.Add(propertyName, descriptor);
                return descriptor;
            }



            // if no methods are found check if target implemented indexing
            var indexes = type.GetProperties().Where(p => p.GetIndexParameters().Length == 1).Where(p => !isInterface || IsJsInterface(p));
            if (indexes.FirstOrDefault() != null)
            {
                var descriptor = IndexDescriptor.Create(Engine, propertyName, Target);
                if (descriptor != null) {
                    Properties.Add(propertyName, descriptor);
                    return descriptor;
                }
            }

            var interfaces = type.GetInterfaces();

            // try to find a single explicit property implementation
            var explicitProperties = (from iface in interfaces
                                      from iprop in iface.GetProperties()
                                      where EqualsIgnoreCasing(iprop.Name, propertyName)
                                      where !isInterface || IsJsInterface(iprop)
                                      select iprop).ToArray();

            if (explicitProperties.Length == 1)
            {
                var descriptor = new PropertyInfoDescriptor(Engine, explicitProperties[0], Target);
                Properties.Add(propertyName, descriptor);
                return descriptor;
            }

            // try to find explicit method implementations
            var explicitMethods = (from iface in interfaces
                                   from imethod in iface.GetMethods()
                                   where EqualsIgnoreCasing(imethod.Name, propertyName)
                                   where !isInterface || IsJsInterface(imethod)
                                   select imethod).ToArray();

            if (explicitMethods.Length > 0)
            {
                var descriptor = new PropertyDescriptor(new MethodInfoFunctionInstance(Engine, explicitMethods), false, true, false);
                Properties.Add(propertyName, descriptor);
                return descriptor;
            }

            // try to find explicit indexer implementations
            var explicitIndexers =
                (from iface in interfaces
                 from iprop in iface.GetProperties()
                 where iprop.GetIndexParameters().Length != 0
                 where !isInterface || IsJsInterface(iprop)
                 select iprop).ToArray();

            if (explicitIndexers.Length == 1)
            {
                var descriptor =  IndexDescriptor.Create(Engine, explicitIndexers[0].DeclaringType, propertyName, Target);
                if (descriptor != null) {
                    Properties.Add(propertyName, descriptor);
                    return descriptor;
                }
            }

            return PropertyDescriptor.Undefined;
        }

        private bool EqualsIgnoreCasing(string s1, string s2)
        {
            bool equals = false;
            if (s1.Length == s2.Length)
            {
                if (s1.Length > 0 && s2.Length > 0) 
                {
                    equals = (s1.ToLower()[0] == s2.ToLower()[0]);
                }
                if (s1.Length > 1 && s2.Length > 1) 
                {
                    equals = equals && (s1.Substring(1) == s2.Substring(1));
                }
            }
            return equals;
        }

        public override IEnumerable<KeyValuePair<string, PropertyDescriptor>> GetOwnProperties() {
            var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !isInterface || IsJsInterface(p));

            foreach (var property in properties) {
                string name = char.ToLower(property.Name[0]) + property.Name.Substring(1);
                var desc = GetOwnProperty(name);
                yield return new KeyValuePair<string, PropertyDescriptor>(name, desc);
            }

            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
                .Where(f => !isInterface || IsJsInterface(f));

            foreach (var field in fields) {
                string name = char.ToLower(field.Name[0]) + field.Name.Substring(1);
                var desc = GetOwnProperty(name);
                yield return new KeyValuePair<string, PropertyDescriptor>(name, desc);
            }

            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public)
                 .Where(m => !isInterface || IsJsInterface(m));

            foreach (var method in methods) {
                string name = char.ToLower(method.Name[0]) + method.Name.Substring(1);
                var desc = GetOwnProperty(name);
                yield return new KeyValuePair<string, PropertyDescriptor>(name, desc);
            }

        }
    }
}
