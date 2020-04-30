using System;
using System.Globalization;
using System.Reflection;
using JintUnity.Native;

namespace JintUnity.Runtime.Descriptors.Specialized
{
    public sealed class IndexDescriptor : PropertyDescriptor
    {
        private readonly Engine _engine;
        private readonly object _key;
        private readonly object _item;
        private readonly PropertyInfo _indexer;
        private readonly MethodInfo _containsKey;

        public static IndexDescriptor Create(Engine engine, Type targetType, string key, object item) {
            // get all instance indexers with exactly 1 argument
            var indexers = targetType.GetProperties();
            object _key;
            // try to find first indexer having either public getter or setter with matching argument type
            foreach (var indexer in indexers) {
                if (indexer.GetIndexParameters().Length != 1) continue;
                if (indexer.GetGetMethod() != null || indexer.GetSetMethod() != null) {
                    var paramType = indexer.GetIndexParameters()[0].ParameterType;
                    if (engine.ClrTypeConverter.TryConvert(key, paramType, CultureInfo.InvariantCulture, out _key)) {
                        return new IndexDescriptor(engine, targetType, indexer, _key, paramType, item);
                    }
                }
            }

            return null;
        }

    
        public static IndexDescriptor Create(Engine engine, string key, object item) {
            return Create(engine, item.GetType(), key, item);
        }

        public IndexDescriptor(Engine engine, Type targetType, PropertyInfo indexer, object key, Type paramType, object item) {
            _engine = engine;
            _item = item;
            _key = key;
            _indexer = indexer;
            _containsKey = targetType.GetMethod("ContainsKey", new Type[] { paramType });
            Writable = true;
        }



        public override JsValue Value
        {
            get
            {
                var getter = _indexer.GetGetMethod();

                if (getter == null) 
                {
                    throw new InvalidOperationException("Indexer has no public getter.");
                }

                object[] parameters = { _key };

                if (_containsKey != null)
                {
                    if ((_containsKey.Invoke(_item, parameters) as bool?) != true)
                    {
                        return JsValue.Undefined;
                    }
                }

                try
                {
                    return JsValue.FromObject(_engine, getter.Invoke(_item, parameters));
                }
                catch
                {
                    return JsValue.Undefined;
                }
            }

            set
            {
                var setter = _indexer.GetSetMethod();
                if (setter == null)
                {
                    throw new InvalidOperationException("Indexer has no public setter.");
                }

                object[] parameters = { _key, value != null ? value.ToObject() : null };
                setter.Invoke(_item, parameters);
            }
        }
    }
}