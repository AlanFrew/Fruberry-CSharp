using System;
using System.Collections;
using System.Collections.Generic;

namespace Fruberry {
    public abstract class Structure<T> : IStructure<T> {
        public IList<Prefer> Constraints => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        int ICollection<T>.Count => throw new NotImplementedException();

        int ICollection.Count => throw new NotImplementedException();

        int IStructure<T>.Length { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IStructure<T> Add(T item) {
            var foo = new System.Collections.Generic.List<T>();
            throw new NotImplementedException();
        }

        public IStructure<T> Clear() {
            throw new NotImplementedException();
        }

        public bool Contains(T item) {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        public int Count() {
            throw new NotImplementedException();
        }

        public T Dequeue() {
            throw new NotImplementedException();
        }

        public IStructure<T> Enqueue(T item) {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() {
            throw new NotImplementedException();
        }

        public T Peek() {
            throw new NotImplementedException();
        }

        public T Pop() {
            throw new NotImplementedException();
        }

        public bool Remove(T item) {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item) {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
