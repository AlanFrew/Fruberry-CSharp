using System;
using System.Collections;
using System.Collections.Generic;

namespace Fruberry {
    public class Pool<T> : IStructure<T>{
        private int PoolSize;
        private Queue<T> Reservoir;
        private RedBlackTree<T> ActiveItems;
        private Func<T> Instantiator;

        int ICollection<T>.Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        int ICollection.Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes, Prefer.NoCompare };

        int IStructure<T>.Length { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Pool(int poolSize, Func<T> instantiator, int startingSize = 0) {
            PoolSize = poolSize;
            Reservoir = new Queue<T>(poolSize);
            ActiveItems = new RedBlackTree<T>();
            Instantiator = instantiator;

            FillReservoir(startingSize);
        }

        public Pool<T> FillReservoir(int size) {
            var startingCount = ActiveItems.Count + Reservoir.Count;

            for (var i = 0; i < size && startingCount + i < PoolSize; i++) {
                SinkItem(Instantiator());
            }

            return this;
        }

        public Pool<T> SinkItem(T item) {
            ActiveItems.Remove(item);
            if (!Reservoir.Contains(item))
                Reservoir.Enqueue(item);

            return this;
        }

        public T SurfaceItem() {
            if (ActiveItems.Count == PoolSize) return default;

            if (Reservoir.None()) FillReservoir(1);

            var cachedObj = Reservoir.Dequeue();

            ActiveItems.Add(cachedObj);

            return cachedObj;
        }

        public void Drown() {
            while (ActiveItems.Count > 0) {
                SinkItem(ActiveItems[0]);
            }
        }

        public override string ToString() {
            return $"Pool (Total: {PoolSize} Active: {ActiveItems.Count} Reserved: {Reservoir.Count})";
        }

        public IStructure<T> Add(T item) {
            throw new NotImplementedException();
        }

        public bool Remove(T item) {
            throw new NotImplementedException();
        }

        public int Count() {
            throw new NotImplementedException();
        }

        public bool Contains(T item) {
            throw new NotImplementedException();
        }

        public IStructure<T> Clear() {
            throw new NotImplementedException();
        }

        public T Pop() {
            throw new NotImplementedException();
        }

        public T Peek() {
            throw new NotImplementedException();
        }

        public IStructure<T> Enqueue(T item) {
            throw new NotImplementedException();
        }

        public T Dequeue() {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item) {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear() {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
