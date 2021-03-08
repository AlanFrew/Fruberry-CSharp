using System;
using System.Collections;
using System.Collections.Generic;

namespace Fruberry {
    public class Pool<T> : IStructure<T>{
        protected int PoolSize;
        protected Queue<T> Reservoir;
        protected List<T> ActiveItems;
        protected Func<T> Instantiator;

        public int Length { get; protected set; }

        public int Count => Length;

        int IStructure<T>.Count() { return Length; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes, Prefer.NoCompare };

        public Pool(int poolSize, Func<T> instantiator, int startingSize = 0) {
            PoolSize = poolSize;
            Reservoir = new Queue<T>(poolSize);
            ActiveItems = new List<T>();
            Instantiator = instantiator;

            FillReservoir(startingSize);
        }

        public virtual Pool<T> FillReservoir(int size) {
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

        public virtual T SurfaceItem() {
            if (ActiveItems.Count == PoolSize) return default;

            if (Reservoir.None()) FillReservoir(1);

            var cachedObj = Reservoir.Dequeue();

            ActiveItems.Add(cachedObj);

            return cachedObj;
        }

        public virtual void Drown() {
            while (ActiveItems.Count > 0) {
                SinkItem(ActiveItems[0]);
            }
        }

        public override string ToString() {
            return $"Pool (Total: {PoolSize} Active: {ActiveItems.Count} Reserved: {Reservoir.Count})";
        }

        public IStructure<T> Add(T item) {
            if (Reservoir.Count < PoolSize) Reservoir.Enqueue(item);

            return this;
        }

        public bool Remove(T item) {
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
            return Add(item);
        }

        public T Dequeue() {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item) {
            Add(item);
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
