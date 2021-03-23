namespace Fruberry {
    using System;
    using System.Diagnostics;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Collections;

    internal sealed class CollectionDebugView<T> {
        private ICollection<T> collection;

        public CollectionDebugView(ICollection<T> collection) {
            this.collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items {
            get {
                T[] items = new T[collection.Count];
                collection.CopyTo(items, 0);
                return items;
            }
        }
    }

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [Serializable]
    public class List<T> : IList<T>, IList, IStructure<T> {
        protected static T[] _emptyArray = new T[0];
        protected static int _defaultCapacity = 2;
        protected static float CapacityMultiplier = 1.5f;
        public static ExceptionBehavior ExceptionBehavior = ExceptionBehavior.BestEffort;
        protected T[] _items;

        public int Length { get; protected set; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        public List() {
            _items = _emptyArray;
        }

        public List(int capacity) {
            if (capacity < 0) {
                capacity = ExceptionBehavior switch {
                    ExceptionBehavior.Throw => throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Capacity must be nonnegative"),
                    _ => 0,
                };
            }

            _items = capacity == 0 ? _emptyArray : (new T[capacity]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Size and capacity are preserved</remarks>
        public List(IEnumerable<T> source) {
            if (source == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException(nameof(source));
                    default: Capacity = 0; return;
                }
            }

            if (source is ICollection<T> collection) {
                var count = collection.Count;
                if (count == 0) {
                    _items = _emptyArray;
                }
                else {
                    _items = new T[count];

                    collection.CopyTo(_items, 0);

                    Length = count;
                }
            }
            else {
                Length = 0;
                _items = _emptyArray;

                using (var enumerator = source.GetEnumerator()) {
                    while (enumerator.MoveNext()) {
                        Add(enumerator.Current);
                    }
                }
            }
        }

        public int Capacity {
            get => _items.Length;
            set {
                if (value < Length) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(Capacity), Capacity, $"Capacity must be nonnegative and no less than the current Length of the list ({Length})");
                        default: return;
                    }
                }

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

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes, Prefer.Add };

        //public List<T> ToList(IEnumerable<T> source) {
        //    if (source == null) {
        //        switch (ExceptionBehavior) {
        //            case ExceptionBehavior.Throw: throw new ArgumentNullException(nameof(source));
        //            default: return this;
        //        }
        //    }

        //    if (source is List<T> list) return list;

        //    return new List<T>(source.ToList()); //TODO: replace call to ToList() to avoid double copying
        //}

        public T this[int index] {
            get {
                if (index < 0) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be nonnegative");
                        case ExceptionBehavior.Abort: return default;
                        default: index = 0; break;
                    }
                }
                if (index >= Length) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be less than Length");
                        case ExceptionBehavior.Abort: return default;
                        default: 
                            if (index == 0) return default;
                            index = Length - 1;
                            break;
                    }
                }
                    
                return _items[index];
            }
            set {
                if (index < 0) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be nonnegative");
                        case ExceptionBehavior.Abort: return;
                        default: index = 0; break;
                    }
                }
                if (index >= Length) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be less than Length");
                        case ExceptionBehavior.Abort: return;
                        default: index = Length - 1; break;
                    }
                }

                _items[index] = value;
            }
        }

        object IList.this[int index] {
            get => this[index];
            set {
                this[index] = (T)value;
            }
        }

        public void Add(T item) {
            if (Length == _items.Length) AdjustCapacity(Length + 1);

            _items[Length] = item;

            Length++;
        }

        int IList.Add(object item) {
            if (!item.GetType().IsAssignableFrom(typeof(T))) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(item), item, $"Item must be of type {typeof(T)}");
                    default: return -1;
                }
            }
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
        /// Searches the List for the given value
        /// </summary>
        /// <remarks>The given section of the List must be sorted. Apply bitwise complement (~) to a negative result to produce the index of the first element that is larger than the given search value. This is also the index at which the search value should be inserted into the list in order for the list to remain sorted.</remarks>
        /// <returns>The index of an arbitrary matching value if there is one, or a negative integer otherwise</returns>
        public int BinarySearch(T item, IComparer<T> comparer = null, int index = 0, int count = -1) {
            if (index < 0 || index >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Index must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: count = Length - index; break;
                }
            }

            if (count < 0 || count + index >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: count = Length - index; break;
                }
            }

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
            if (converter == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException(nameof(converter));
                    default: return new List<TOutput>();
                }
            }

            var list = new List<TOutput>(Length);

            for (var i = 0; i < Length; i++) {
                list._items[i] = converter(_items[i]);
            }

            list.Length = Length;

            return list;
        }

        public void CopyTo(Array array, int arrayIndex) {
            if (array == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException(nameof(array), $"Array must be nonnull");
                    case ExceptionBehavior.Abort: return;
                    default: array = new Array[Length]; break;
                }
            }

            if (arrayIndex < 0 || arrayIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, $"ArrayIndex must be nonnegative and less than target array Length ({array.Length})");
                    case ExceptionBehavior.Abort: return;
                    default: arrayIndex = Math.Max(array.Length - Length, 0); break;
                }
            }

            var copyLength = arrayIndex + Length;
            if (copyLength > array.Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new InvalidOperationException($"ArrayIndex plus Length of this structure must be no greater than the Length of the target array ({array.Length})");
                    case ExceptionBehavior.Abort: break;
                    default:
                        copyLength = array.Length;
                        break;
                }
            }

            try {
                Array.Copy(_items, 0, array, arrayIndex, copyLength);
            }
            catch(Exception ex) {
                switch(ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new Exception("Failed to copy to the target array. See InnerException for details", ex);
                    default: break;
                }
            }
        }

        public void CopyTo(T[] array, int arrayIndex) {
           CopyTo((Array)array, arrayIndex);
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
            if (match == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException("Match must be nonnull");
                    default: return default;
                }
            }

            for (var i = 0; i < Length; i++) {
                if (match(_items[i])) return _items[i];
            }

            return default;
        }

        public List<T> FindAll(Predicate<T> match) {
            if (match == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException("Match must be nonnull");
                    default: return default;
                }
            }

            var list = new List<T>();

            for (var i = 0; i < Length; i++) {
                if (match(_items[i])) {
                    list.Add(_items[i]);
                }
            }

            return list;
        }

        public int FindIndex(Predicate<T> match, int startIndex = 0, int count = -1) {
            if (match == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException("Match must be nonnull");
                    default: return default;
                }
            }

            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: startIndex = 0; break;
                }
            }

            if (count < 0 || count + startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: count = Length - startIndex; break;
                }
            }

            for (var i = startIndex; i < startIndex + count; i++) {
                if (match(_items[i])) return i;
            }

            return -1;
        }

        public T FindLast(Predicate<T> match) {
            if (match == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException("Match must be nonnull");
                    default: return default;
                }
            }

            for (var i = Length - 1; i >= 0; i--) {
                if (match(_items[i])) {
                    return _items[i];
                }
            }

            return default;
        }

        public int FindLastIndex(Predicate<T> match, int startIndex = -1, int count = -1) {
            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: startIndex = 0; break;
                }
            }

            if (count < 0 || count + startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and StartIndex + Count must be less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: count = Length - startIndex; break;
                }
            }

            for (var i = startIndex; i > startIndex - count; i--) {
                if (match(_items[i])) {
                    return i;
                }
            }

            return -1;
        }

        public void ForEach(Action<T> action) {
            if (action == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException("Match must be nonnull");
                    default: return;
                }
            }

            for (var i = 0; i < Length; i++) {
                action(_items[i]);
            }
        }

        protected virtual bool Validate() {
            if (Capacity != _items.Length) return false;
            if (Length > Capacity) return false;
            if (Length < 0 || Capacity < 0) return false;

            var comparer = EqualityComparer<T>.Default;
            var index = 0;

            foreach (var item in this) {
                if (comparer.Equals(item)) return false;
                if (!this[index].Equals(item)) return false;

                index++;
            }

            return true;
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

        public List<T> GetRange(int startIndex, int count) {
            var list = new List<T>(count);

            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return list;
                    default: startIndex = 0; break;
                }
            }

            if (count < 0 || count + startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and StartIndex + Count must be less than Length ({Length})");
                    case ExceptionBehavior.Abort: return list;
                    default: count = Length - startIndex; break;
                }
            }

            Array.Copy(_items, startIndex, list._items, 0, count);

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

        public int IndexOf(T item, int startIndex = 0, int count = -1) {
            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: startIndex = 0; break;
                }
            }

            if (count < 0 || count + startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and StartIndex + Count must be less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: count = Length - startIndex; break;
                }
            }

            return Array.IndexOf(_items, item, startIndex, count);
        }

        public void Insert(int index, T item) {
            if (index < 0 || index >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(index), index, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: index = 0; break;
                }
            }

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

        public void InsertRange(int startIndex, IEnumerable<T> source) {
            if (source == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException("Match must be nonnull");
                    default: return;
                }
            }

            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: startIndex = 0; break;
                }
            }

            if (source is ICollection<T> collection) {
                var count = collection.Count;
                if (count > 0) {
                    AdjustCapacity(Length + count);

                    if (startIndex < Length) {
                        Array.Copy(_items, startIndex, _items, startIndex + count, Length - startIndex);
                    }

                    // handle inserting the list into itself
                    if (this == collection) {
                        Array.Copy(_items, 0, _items, startIndex, startIndex);

                        Array.Copy(_items, startIndex + count, _items, startIndex * 2, Length - startIndex);
                    }
                    else {
                        var itemsToInsert = new T[count];

                        collection.CopyTo(itemsToInsert, 0);

                        itemsToInsert.CopyTo(_items, startIndex);
                    }

                    Length += count;
                }
            }
            else {
                using IEnumerator<T> en = source.GetEnumerator();

                while (en.MoveNext()) {
                    Insert(startIndex++, en.Current);
                }
            }
        }

        public int LastIndexOf(T item, int startIndex = -1, int count = -1) {
            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: startIndex = 0; break;
                }
            }

            if (count < 0 || count + startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and StartIndex + Count must be less than Length ({Length})");
                    case ExceptionBehavior.Abort: return int.MinValue;
                    default: count = Length - startIndex; break;
                }
            }

            //if (Length == 0) return -1;

            return Array.LastIndexOf(_items, item, startIndex, count);
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
            if (match == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException("Match must be nonnull");
                    default: return 0;
                }
            }

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
            if (index < 0 || index >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: index = 0; break;
                }
            }

            Length--;

            if (index < Length) {
                Array.Copy(_items, index + 1, _items, index, Length - index); //move subsequent forward to fill the gap, but don't change capacity
            }

            _items[Length] = default;
        }

        public void RemoveRange(int startIndex, int count) {
            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: startIndex = 0; break;
                }
            }

            if (count < 0 || count + startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and StartIndex + Count must be less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: count = Length - startIndex; break;
                }
            }

            Length -= count;

            if (startIndex < Length) {
                Array.Copy(_items, startIndex + count, _items, startIndex, Length - startIndex);
            }

            Array.Clear(_items, Length, count);
        }

        public void Reverse(int startIndex = 0, int count = -1) {
            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: startIndex = 0; break;
                }
            }

            if (count < 0 || count + startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and StartIndex + Count must be less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: count = Length - startIndex; break;
                }
            }

            Array.Reverse(_items, startIndex, count);
        }

        public void Sort() {
            Sort(0, Length, null);
        }

        public void Sort(IComparer<T> comparer) {
            Sort(0, Length, comparer);
        }

        public void Sort(int startIndex = 0, int count = -1, IComparer<T> comparer = null) {
            if (startIndex < 0 || startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, $"StartIndex must be nonnegative and less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: startIndex = 0; break;
                }
            }

            if (count < 0 || count + startIndex >= Length) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(count), count, $"Count must be nonnegative and StartIndex + Count must be less than Length ({Length})");
                    case ExceptionBehavior.Abort: return;
                    default: count = Length - startIndex; break;
                }
            }

            Array.Sort(_items, startIndex, count, comparer);
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
            if (match == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException("Match must be nonnull");
                    default: return false;
                }
            }

            for (var i = 0; i < Length; i++) {
                if (!match(_items[i])) return false;
            }

            return true;
        }

        protected bool IsCompatibleObject(object value) {        
            return (value is T) || (value == null && default(T) == null); // only accept nulls if T is a class or Nullable<U>.
        }

        IStructure<T> IStructure<T>.Add(T item) {
            Add(item);

            return this;
        }

        int IStructure<T>.Count() {
            return Length;
        }

        IStructure<T> IStructure<T>.Clear() {
            Clear();

            return this;
        }

        public T Pop() {
            var result = this[Length - 1];

            RemoveAt(Length - 1);

            return result;
        }

        public T Peek() {
            if (Length == 0) {
                switch(ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new InvalidOperationException("Peek must be called on a nonempty structure");
                    default: return default;
                }
            }

            return this[Length - 1];
        }

        public IStructure<T> Enqueue(T item) {
            Add(item);

            return this;
        }

        public T Dequeue() {
            return Pop();
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
