using System;
using System.Collections;
using System.Collections.Generic;

namespace Fruberry {
    public class WaitList<T> : IStructure<T> {
        private List<T> _data;

        public bool IsReadOnly => false;

        public Func<T, T, int> Compare;

        public int Length { get => _data.Count; protected set { } }

        public int Count => Length;

        int IStructure<T>.Count() { return Length; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        private int _compare(T left, T right) {
            if (Compare != null) return Compare(left, right);

            if (typeof(T) is IComparable<T>) return ((IComparable<T>)left).CompareTo(right);

            return 0;
        }

        public WaitList(Func<T, T, int> compare = null) {
            _data = new List<T>();
            Compare = compare;
        }

        public void Enqueue(T item) {
            _data.Add(item);

            var child = _data.Count - 1;
            while (child > 0) {
                var parent = (child - 1) / 2;

                if (_compare(_data[child], _data[parent]) >= 0) break;

                var tmp = _data[child]; _data[child] = _data[parent]; _data[parent] = tmp;

                child = parent;
            }
        }

        public T Dequeue() {
            var lastIndex = _data.Count - 1;
            var frontItem = _data[0];

            _data[0] = _data[lastIndex];

            _data.RemoveAt(lastIndex);

            lastIndex--;

            var parent = 0;
            while (true) {
                var leftChild = parent * 2 + 1;

                if (leftChild > lastIndex) break;

                var rightChild = leftChild + 1;

                if (rightChild <= lastIndex && _compare(_data[rightChild], _data[leftChild]) < 0) {
                    leftChild = rightChild;
                }

                if (_compare(_data[parent], _data[leftChild]) <= 0) break;

                var tmp = _data[parent];
                _data[parent] = _data[leftChild];
                _data[leftChild] = tmp;

                parent = leftChild;
            }

            return frontItem;
        }

        public T First => _data[0];

        public T Peek() => _data[0];

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes };

        public T this[int index] => _data[index];

        public override string ToString() {
            var result = "";
            foreach (var item in _data) {
                result += item.ToString() + " ";
            }

            result += "count = " + _data.Count;

            return result.TrimEnd();
        }

        /// <summary>
        /// Check that heap property is still true for all data
        /// </summary>
        /// <remarks>For testing purposes</remarks>
        public bool IsConsistent() {
            if (_data.Count == 0) return true;

            var lastIndex = _data.Count - 1;

            for (var parentIndex = 0; parentIndex < _data.Count; ++parentIndex)
            {
                var leftChildIndex = 2 * parentIndex + 1;
                var rightChildIndex = 2 * parentIndex + 2;

                if (leftChildIndex <= lastIndex && _compare(_data[parentIndex], _data[leftChildIndex]) > 0) return false;

                if (rightChildIndex <= lastIndex && _compare(_data[parentIndex], _data[rightChildIndex]) > 0) return false;
            }

            return true;
        }

        public WaitList<T> Add(T item) {
            Enqueue(item);

            return this;
        }

        void ICollection<T>.Add(T item) {
            Enqueue(item);
        }

        public void Clear() {
            _data.Clear();
        }

        public bool Contains(T item) {
            return _data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            foreach(var item in this) {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        public bool Remove(T item) {
            return _data.Remove(item);
        }

        public IEnumerator<T> GetEnumerator() {
            return new Enumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator<T>(this);
        }

        IStructure<T> IStructure<T>.Add(T item) {
            throw new NotImplementedException();
        }

        IStructure<T> IStructure<T>.Clear() {
            throw new NotImplementedException();
        }

        public T Pop() {
            throw new NotImplementedException();
        }

        IStructure<T> IStructure<T>.Enqueue(T item) {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public struct Enumerator<T> : IEnumerator<T> {
            private WaitList<T> _collection;

            T IEnumerator<T>.Current => Current; //implements IEnumerable<T>
            object IEnumerator.Current => Current; //implements IEnumerable and used in foreach loop when cast to IEnumerable
            public T Current { //used in foreach loop when typed as WaitList<T>
                get {
                    return _collection[_currentIndex];
                }} 

            private int _currentIndex { get; set; }

            public Enumerator(WaitList<T> collection) {
                _collection = collection;
                _currentIndex = -1;
            }

            public bool MoveNext() {
                if (_currentIndex < _collection.Count - 1) {
                    _currentIndex++;

                    return true;
                }

                return false;
            }

            public void Reset() {
                _currentIndex = -1;
            }

            public void Dispose() { }
        }
    }
}
