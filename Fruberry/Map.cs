using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Fruberry {
    public class Map<TKey, TValue> :IDictionary<TKey, TValue> {
        private Dictionary2<TKey, TValue> Internal = new Dictionary2<TKey, TValue>();

        TValue IDictionary<TKey, TValue>.this[TKey key] {
            get {
                if (Internal.ContainsKey(key)) {
                    return Internal[key];
                }

                return default;
            }
            set {
                Internal[key] = value;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Internal.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Internal.Values;

        int ICollection<KeyValuePair<TKey, TValue>>.Count => Internal.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) {
            if (key == null) return;

            Internal.Add(key, value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            throw new NotImplementedException();
        }

        bool IDictionary<TKey, TValue>.ContainsKey(TKey key) {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key) {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotImplementedException();
        }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) {
            throw new NotImplementedException();
        }
    }
}
