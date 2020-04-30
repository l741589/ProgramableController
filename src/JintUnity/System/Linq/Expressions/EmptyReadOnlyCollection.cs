using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace JintUnity.SystemCompat.Linq.Expressions {
    static class EmptyReadOnlyCollection<T> {
        internal static ReadOnlyCollection<T> Instance = new ReadOnlyCollection<T>(new T[0]);
    }
}
