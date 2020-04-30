using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JintUnity {
    public static class Utils {
        public static String Join<T>(String separator, IEnumerable<T> values) {
            if (values == null) {
                return null;
            } else {
                return string.Join(separator, values.Select(e => e.ToString()).ToArray());
            }
        }

        public static void RequiresNotNull(object value, string paramName) {
            Debug.Assert(!string.IsNullOrEmpty(paramName));

            if (value == null) {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void RequiresNotEmpty<T>(ICollection<T> collection, string paramName) {
            RequiresNotNull(collection, paramName);
            if (collection.Count == 0) {
                throw new ArgumentException("NonEmptyCollectionRequired", paramName);
            }
        }
        public static bool AreReferenceAssignable(Type dest, Type src) {
            // WARNING: This actually implements "Is this identity assignable and/or reference assignable?"
            if (AreEquivalent(dest, src)) {
                return true;
            }
            if (!dest.IsValueType && !src.IsValueType && dest.IsAssignableFrom(src)) {
                return true;
            }
            return false;
        }

        public static bool AreEquivalent(Type t1, Type t2) {
            return t1 == t2;
        }
    }
}
