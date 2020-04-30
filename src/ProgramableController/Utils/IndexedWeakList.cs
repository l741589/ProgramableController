using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProgramableController.Utils {
    public class IndexedWeakList<K,T> : IList<T> {

        public readonly List<WeakReference> list = new List<WeakReference>();
        private Func<T, K> keySelector;
        private Dictionary<K, T> index;
        private bool dirty = true;

        public IndexedWeakList(Func<T, K> keySelector) {
            this.keySelector = keySelector;
        }

        public Dictionary<K, T> Index {
            get {
                refresh();
                return this.index;
            }
        }

        private List<WeakReference> refresh() {
            if (dirty) {
                list.RemoveAll(e => !e.IsAlive);
                this.index = list.Select(e => (T)e.Target).ToDictionary(keySelector, e => e);
                dirty = false;
            }
            return list;
        }

        public T this[int index] {
            get {
                WeakReference wr = list[index];
                if (wr == null) {
                    return default(T);
                } else if (wr.IsAlive) {
                    return (T)wr.Target;
                } else {
                    list.Remove(wr);
                    return default(T);
                }
            }
            set {
                list[index] = new WeakReference(value);
                dirty = true;
            }
        }

        public int Count => refresh().Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item) {
            list.Add(new WeakReference(item));
            dirty = true;
        }

        public void Clear() {
            list.Clear();
            dirty = true;
        }

        public bool Contains(T item) {
            return item != null && Index.ContainsKey(keySelector(item));
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() {
            return list.Where(e => e.IsAlive).Select(e => (T)e.Target).GetEnumerator();
        }

        public int IndexOf(T item) {
            return refresh().FindIndex(e => Object.ReferenceEquals(item, e.Target));
        }

        public void Insert(int index, T item) {
            throw new NotImplementedException();
        }

        public bool Remove(T item) {
            WeakReference wr = list.Find(e => Object.ReferenceEquals(item, e.Target));
            if (wr!= null) {
                bool ret = list.Remove(wr);
                if (ret) {
                    dirty = true;
                }
                return ret;
            } else {
                return false;
            }
        }

        public void RemoveAt(int index) {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return list.Where(e => e.IsAlive).Select(e => e.Target).GetEnumerator();
        }
    }
}
