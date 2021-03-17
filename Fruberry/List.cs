namespace Fruberry {
    using System;
    using System.Diagnostics;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;

    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class List<T> : IList<T>, IList {
        protected static T[] _emptyArray = new T[0];
        protected static int _defaultCapacity = 1;
        protected static float CapacityMultiplier = 1.5f;
        protected T[] _items;
        private int Length;

        public List() {
            _items = _emptyArray;
        }

        public List(int capacity) {
            _items = capacity == 0 ? _emptyArray : (new T[capacity]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Size and capacity are preserved</remarks>
        public List(IEnumerable<T> source) {
            if (source is ICollection<T> c) {
                var count = c.Count;
                if (count == 0) {
                    _items = _emptyArray;
                }
                else {
                    _items = new T[count];

                    c.CopyTo(_items, 0);

                    Length = count;
                }
            }
            else {
                Length = 0;
                _items = _emptyArray;

                using (IEnumerator<T> en = source.GetEnumerator()) {
                    while (en.MoveNext()) {
                        Add(en.Current);
                    }
                }
            }
        }

        public int Capacity {
            get => _items.Length;
            set {
                if (value == _items.Length) return;

                if (value > 0) {
                    T[] newItems = new T[value];

                    if (Length > 0) {
                        Array.Copy(_items, 0, newItems, 0, Math.Min(Length, value));
                    }
                    _items = newItems;
                }
                else {
                    _items = _emptyArray;
                }
            }
        }

        public int Count => Length;

        bool IList.IsFixedSize => false;

        bool ICollection<T>.IsReadOnly => false;

        bool IList.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        public List<T> ToList(IEnumerable<T> source) {
            if (source is List<T>) {
                return (List<T>)source;
            }

            return new List<T>(source.ToList()); //TODO: replace call to ToList() to avoid double copying
        }

        public T this[int index] {
            get => _items[index];
            set => _items[index] = value;
        }

        object IList.this[int index] {
            get => this[index];
            set {
                this[index] = (T)value;
            }
        }

        public void Add(T item) {
            if (Length == _items.Length) AdjustCapacity(Length + 1);

            _items[Length++] = item;
        }

        int IList.Add(object item) {
            Add((T)item);
           
            return Count - 1;
        }

        public void AddRange(IEnumerable<T> collection) {
            InsertRange(Length, collection);
        }

        public ReadOnlyCollection<T> AsReadOnly() {
            return new ReadOnlyCollection<T>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Apply bitwise complement (~) to a negative result to produce the index of the first element that is larger than the given search value. This is also the index at which
        /// the search value should be inserted into the list in order for the list
        /// to remain sorted.</remarks>
        public int BinarySearch(T item, IComparer<T> comparer = null, int index = 0, int count = -1) {
            if (count < 0) count = Length - index;

            return Array.BinarySearch(_items, index, count, item, comparer);
        }

        public void Clear() {
            if (Length <= 0) return;

            Array.Clear(_items, 0, Length);

            Length = 0;
        }

        public bool Contains(T item) {
            if (item == null) {
                for (var i = 0; i < Length; i++) {
                    if (_items[i] == null) return true;
                }
                    
                return false;
            }
            else {
                var comparer = EqualityComparer<T>.Default;

                for (var i = 0; i < Length; i++) {
                    if (comparer.Equals(_items[i], item)) return true;
                }

                return false;
            }
        }

        bool IList.Contains(object item) {
            return IsCompatibleObject(item) && Contains((T)item);
        }

        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter) {
            var list = new List<TOutput>(Length);

            for (var i = 0; i < Length; i++) {
                list._items[i] = converter(_items[i]);
            }

            list.Length = Length;

            return list;
        }

        void ICollection.CopyTo(Array array, int arrayIndex) {
            Array.Copy(_items, 0, array, arrayIndex, Length);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            Array.Copy(_items, 0, array, arrayIndex, Length);
        }

        protected int MaxArrayLength = 0X7FEFFFFF;

        private void AdjustCapacity(int min) {
            if (_items.Length < min) {
                int newCapacity = _items.Length == 0 ? _defaultCapacity : (int)(_items.Length * CapacityMultiplier);

                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if ((uint)newCapacity > MaxArrayLength) newCapacity = MaxArrayLength;

                if (newCapacity < min) newCapacity = min;

                Capacity = newCapacity;
            }
        }

        public bool Exists(Predicate<T> match) {
            return FindIndex(match) != -1;
        }

        public T Find(Predicate<T> match) {
            for (var i = 0; i < Length; i++) {
                if (match(_items[i])) return _items[i];
            }

            return default;
        }

        public List<T> FindAll(Predicate<T> match) {
            var list = new List<T>();

            for (var i = 0; i < Length; i++) {
                if (match(_items[i])) {
                    list.Add(_items[i]);
                }
            }

            return list;
        }

        public int FindIndex(Predicate<T> match, int startIndex = 0, int count = -1) {
            if (count < 0) count = Length - startIndex;

            int endIndex = startIndex + count;

            for (var i = startIndex; i < endIndex; i++) {
                if (match(_items[i])) return i;
            }

            return -1;
        }

        public T FindLast(Predicate<T> match) {
            for (var i = Length - 1; i >= 0; i--) {
                if (match(_items[i])) {
                    return _items[i];
                }
            }

            return default;
        }

        public int FindLastIndex(Predicate<T> match, int startIndex = -1, int count = -1) {
            if (startIndex < 0) startIndex = Length - 1;
            if (count < 0) count = startIndex + 1;

            // this also handles when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
            int endIndex = startIndex - count;

            for (var i = startIndex; i > endIndex; i--) {
                if (match(_items[i])) {
                    return i;
                }
            }

            return -1;
        }

        public void ForEach(Action<T> action) {
            for (var i = 0; i < Length; i++) {
                action(_items[i]);
            }
        }

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(this);
        }

        public List<T> GetRange(int index, int count) {
            var list = new List<T>(count);

            Array.Copy(_items, index, list._items, 0, count);

            list.Length = count;

            return list;
        }

        public int IndexOf(T item) {
            return Array.IndexOf(_items, item, 0, Length);
        }

        int IList.IndexOf(object item) {
            if (IsCompatibleObject(item)) {
                return IndexOf((T)item);
            }

            return -1;
        }

        public int IndexOf(T item, int index = 0, int count = -1) {
            if (count < 0) count = Length - index;

            return Array.IndexOf(_items, item, index, count);
        }

        public void Insert(int index, T item) {
            if (Length == _items.Length) {
                AdjustCapacity(Length + 1);
            }

            if (index < Length) {
                Array.Copy(_items, index, _items, index + 1, Length - index);
            }

            _items[index] = item;

            Length++;
        }

        void IList.Insert(int index, object item) {
            Insert(index, (T)item);
        }

        public void InsertRange(int index, IEnumerable<T> source) {
            if (source is ICollection<T> collection) {
                var count = collection.Count;
                if (count > 0) {
                    AdjustCapacity(Length + count);

                    if (index < Length) {
                        Array.Copy(_items, index, _items, index + count, Length - index);
                    }

                    // handle inserting the list into itself
                    if (this == collection) {
                        Array.Copy(_items, 0, _items, index, index);

                        Array.Copy(_items, index + count, _items, index * 2, Length - index);
                    }
                    else {
                        var itemsToInsert = new T[count];

                        collection.CopyTo(itemsToInsert, 0);

                        itemsToInsert.CopyTo(_items, index);
                    }

                    Length += count;
                }
            }
            else {
                using IEnumerator<T> en = source.GetEnumerator();

                while (en.MoveNext()) {
                    Insert(index++, en.Current);
                }
            }
        }

        public int LastIndexOf(T item, int index = -1, int count = -1) {
            if (index < 0) index = _items.Length - 1;
            if (count < 0) count = _items.Length;

            if (Length == 0) return -1;

            return Array.LastIndexOf(_items, item, index, count);
        }

        public bool Remove(T item) {
            int index = IndexOf(item);
            if (index >= 0) {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        void IList.Remove(object item) {
            if (IsCompatibleObject(item)) {
                Remove((T)item);
            }
        }
  
        public int RemoveAll(Predicate<T> match) {
            var freeIndex = 0;

            while (freeIndex < Length && !match(_items[freeIndex])) freeIndex++;

            if (freeIndex >= Length) return 0;

            var current = freeIndex + 1;
            while (current < Length) {
                while (current < Length && match(_items[current])) current++;

                if (current < Length) {
                    _items[freeIndex++] = _items[current++];
                }
            }

            Array.Clear(_items, freeIndex, Length - freeIndex);

            var result = Length - freeIndex;

            Length = freeIndex;

            return result;
        }

        public void RemoveAt(int index) {
            Length--;

            if (index < Length) {
                Array.Copy(_items, index + 1, _items, index, Length - index);
            }

            _items[Length] = default;
        }

        public void RemoveRange(int index, int count) {
            if (count <= 0) return;

            Length -= count;

            if (index < Length) {
                Array.Copy(_items, index + count, _items, index, Length - index);
            }

            Array.Clear(_items, Length, count);
        }

        public void Reverse(int index = 0, int count = -1) {
            if (count < 0) count = Length;

            Array.Reverse(_items, index, count);
        }

        public void Sort() {
            Sort(0, Length, null);
        }

        public void Sort(IComparer<T> comparer) {
            Sort(0, Length, comparer);
        }

        public void Sort(int index = 0, int count = -1, IComparer<T> comparer = null) {
            if (count < 0) count = Length;

            Array.Sort(_items, index, count, comparer);
        }

        //public void Sort(Comparison<T> comparison) {
        //    Contract.EndContractBlock();

        //    if (_size > 0) {
        //        IComparer<T> comparer = new Array.FunctorComparer<T>(comparison);
        //        Array.Sort(_items, 0, _size, comparer);
        //    }
        //}

        public T[] ToArray() {
            var array = new T[Length];

            Array.Copy(_items, 0, array, 0, Length);

            return array;
        }

        public void TrimExcess() {
            var threshold = (int)(_items.Length * 0.9);

            if (Length < threshold) {
                Capacity = Length;
            }
        }

        public bool TrueForAll(Predicate<T> match) {
            for (var i = 0; i < Length; i++) {
                if (!match(_items[i])) return false;
            }

            return true;
        }

        protected bool IsCompatibleObject(object value) {        
            return (value is T) || (value == null && default(T) == null); // only accept nulls if T is a class or Nullable<U>.
        }

        //internal static IList<T> Synchronized(List<T> list) {
        //    return new SynchronizedList(list);
        //}

        [Serializable]
        public struct Enumerator : IEnumerator<T>, IEnumerator {
            public List<T> List;
            public int Index;
            private T current;

            public Enumerator(List<T> list) {
                List = list;
                Index = 0;
                current = default;
            }

            public void Dispose() { }

            public bool MoveNext() {
                var localList = List;

                if ((uint)Index < (uint)localList.Length) {
                    current = localList._items[Index];
                    Index++;
                    return true;
                }

                Index = List.Length + 1;
                current = default;

                return false;
                //if (Index < List.Length) {
                //    Index++;

                //    return true;
                //}

                //return false;
            }

            //public T Current => Index < List.Length ? List[Index] : default;
            public T Current => current;

            object IEnumerator.Current => Current;

            void IEnumerator.Reset() {
                Index = 0;
            }
        }

        [Serializable()]
        internal class SynchronizedList : IList<T> {
            private List<T> _list;
            private object _root;

            internal SynchronizedList(List<T> list) {
                _list = list;
                _root = ((ICollection)list).SyncRoot;
            }

            public int Count {
                get {
                    lock (_root) {
                        return _list.Count;
                    }
                }
            }

            public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

            public void Add(T item) {
                lock (_root) {
                    _list.Add(item);
                }
            }

            public void Clear() {
                lock (_root) {
                    _list.Clear();
                }
            }

            public bool Contains(T item) {
                lock (_root) {
                    return _list.Contains(item);
                }
            }

            public void CopyTo(T[] array, int arrayIndex) {
                lock (_root) {
                    ((ICollection)_list).CopyTo(array, arrayIndex);
                }
            }

            public bool Remove(T item) {
                lock (_root) {
                    return _list.Remove(item);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() {
                lock (_root) {
                    return _list.GetEnumerator();
                }
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator() {
                lock (_root) {
                    return ((IEnumerable<T>)_list).GetEnumerator();
                }
            }

            public T this[int index] {
                get {
                    lock (_root) {
                        return _list[index];
                    }
                }
                set {
                    lock (_root) {
                        _list[index] = value;
                    }
                }
            }

            public int IndexOf(T item) {
                lock (_root) {
                    return _list.IndexOf(item);
                }
            }

            public void Insert(int index, T item) {
                lock (_root) {
                    _list.Insert(index, item);
                }
            }

            public void RemoveAt(int index) {
                lock (_root) {
                    _list.RemoveAt(index);
                }
            }
        }
    }
}
