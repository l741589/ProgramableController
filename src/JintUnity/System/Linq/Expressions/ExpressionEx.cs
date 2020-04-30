using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace JintUnity.SystemCompat.Linq.Expressions {
    static partial class ExpressionEx {
        public static T ReturnObject<T>(object collectionOrT) where T : class {
            T t = collectionOrT as T;
            if (t != null) {
                return t;
            }

            return ((ReadOnlyCollection<T>)collectionOrT)[0];
        }

        public static ReadOnlyCollection<T> ReturnReadOnly<T>(ref IList<T> collection) {
            IList<T> value = collection;

            // if it's already read-only just return it.
            ReadOnlyCollection<T> res = value as ReadOnlyCollection<T>;
            if (res != null) {
                return res;
            }

            // otherwise make sure only readonly collection every gets exposed
            Interlocked.CompareExchange<IList<T>>(
                ref collection,
                new ReadOnlyCollection<T>(value),
                value
            );

            // and return it
            return (ReadOnlyCollection<T>)collection;
        }
        // Checks that all variables are non-null, not byref, and unique.
        public static void ValidateVariables(IList<ParameterExpression> varList, string collectionName) {
            if (varList.Count == 0) {
                return;
            }

            int count = varList.Count;
            var set = new HashSet<ParameterExpression>();
            for (int i = 0; i < count; i++) {
                ParameterExpression v = varList[i];
                if (v == null) {
                    throw new ArgumentNullException(string.Format("{0}[{1}]", collectionName, set.Count));
                }
                /*if (v.IsByRef) {
                    throw Error.VariableMustNotBeByRef(v, v.Type);
                }*/
                if (set.Contains(v)) {
                    throw new Exception("DuplicateVariable");
                }
                set.Add(v);
            }
        }

        public static void RequiresCanRead(Expression expression, string paramName) {
            if (expression == null) {
                throw new ArgumentNullException(paramName);
            }

            // validate that we can read the node
            switch (expression.NodeType) {
                /*case ExpressionType.Index:
                    IndexExpression index = (IndexExpression)expression;
                    if (index.Indexer != null && !index.Indexer.CanRead) {
                        throw new ArgumentException(Strings.ExpressionMustBeReadable, paramName);
                    }
                    break;*/
                case ExpressionType.MemberAccess:
                    MemberExpression member = (MemberExpression)expression;
                    MemberInfo memberInfo = member.Member;
                    if (memberInfo.MemberType == MemberTypes.Property) {
                        PropertyInfo prop = (PropertyInfo)memberInfo;
                        if (!prop.CanRead) {
                            throw new ArgumentException("ExpressionMustBeReadable", paramName);
                        }
                    }
                    break;
            }
        }

        private static void RequiresCanRead(IEnumerable<Expression> items, string paramName) {
            if (items != null) {
                // this is called a lot, avoid allocating an enumerator if we can...
                IList<Expression> listItems = items as IList<Expression>;
                if (listItems != null) {
                    for (int i = 0; i < listItems.Count; i++) {
                        RequiresCanRead(listItems[i], paramName);
                    }
                    return;
                }

                foreach (var i in items) {
                    RequiresCanRead(i, paramName);
                }
            }
        }
    }
}
