using System.Collections.Generic;
using System.Collections;
using System;
using System.Diagnostics;
using System.Text;

namespace Fruberry {
    public class Chainlink<T> {
        public Chainlink<T> Previous;
        public Chainlink<T> Next;
        public T Value;

        public Chainlink() { }

        public Chainlink(T value) {
            Value = value;
        }

        public override bool Equals(object obj) {
            if (obj == null || !(obj is Chainlink<T>)) return false;

            return Value.Equals(((Chainlink<T>)obj).Value);
        }

        public override int GetHashCode() {
            if (Value != null) return Value.GetHashCode();

            return base.GetHashCode();
        }

        public override string ToString() {
            return Value?.ToString() ?? "";
        }
    }

    //TODO: Implement interfaces of list, linkedlist, dictionary, hashset
    //TODO: Throw exceptions flag
    /// <summary>
    /// A data structure with fast addtion and removal that can be modified while enumerating
    /// </summary>
    /// <typeparam name="T">The base type of object to store</typeparam>
    /// <remarks>Implemented as a doubly linked list</remarks>
    public class Chain<T> : IStructure<T>, IList, IList<T> {
        public Chainlink<T> Head;
        public Chainlink<T> Tail;

        public T First => Head == null ? default : Head.Value;
        public T Last => Tail == null ? default : Tail.Value;

        public int Length { get; protected set; }

        public int Count => Length;

        int IStructure<T>.Count() { return Length; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        protected static Func<T, T, bool> comparer;
        public static Func<T, T, bool> Comparer {
            get {
                if (comparer != null) return comparer;

                if (typeof(IComparable).IsAssignableFrom(typeof(T))) {
                    return (left, right) => {
                        if (left == null && right == null) return true;

                        if (left == null ^ right == null) return false;

                        return (left as IComparable).CompareTo(right) <= 0;
                    };
                }

                return null;
            }
            set => comparer = value;
        }

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => null;

        bool ICollection<T>.IsReadOnly => false;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        object IList.this[int index] { get => this[index]; set => this[index] = (T)value; }

        T IList<T>.this[int index] { get => this[index]; set => this[index] = value; }

        //public IEnumerable<Chainlink<T>> Chainlinks() {
        //    var result = new 
        //}

        public Chain(Func<T, T, bool> comparer = null) {
            if (comparer != null) Comparer = comparer;
        }

        public Chain(IEnumerable<T> source, Func<T, T, bool> comparer = null) {
            if (comparer != null) Comparer = comparer;

            foreach (var element in source) {
                var chainlink = new Chainlink<T>(element);
                AddLast(chainlink);
            }
        }

        public string Print() {
            var builder = new StringBuilder();

            if (typeof(T) == typeof(string)) {
                foreach (var chainLink in this) {
                    builder.Append("\"");
                    builder.Append((string)(object)chainLink.Value);
                    builder.Append("\", ");
                }
            }
            else {
                foreach (var chainLink in this) {
                    builder.Append(chainLink);
                    builder.Append(", ");
                }
            }

            builder.Remove(builder.Length - 3, 2);

            return builder.ToString();
        }

        /// <summary>
        /// Adds the given value to the end of the chain
        /// </summary>
        /// <param name="chainlink">The value to add</param>
        /// <returns>A reference to the chain</returns>
        public Chain<T> Add(T value) {
            return AddLast(value);
        }

        /// <summary>
        /// Adds the given value to the end of the chain
        /// </summary>
        /// <param name="chainlink">The value to add</param>
        /// <returns>A reference to the chain</returns>
        public Chain<T> AddLast(T value) {
            var link = new Chainlink<T>(value);

            return AddLast(link);
        }

        /// <summary>
        /// Adds the given chainlink to the end of the chain, making it the Tail
        /// </summary>
        /// <param name="chainlink">The chainlink to add</param>
        /// <returns>A reference to the chain</returns>
        public Chain<T> AddLast(Chainlink<T> chainlink) {
            chainlink.Previous = Tail;

            if (Head == null) {
                Head = chainlink;
                Tail = chainlink;
            }
            else {
                Tail.Next = chainlink;
                Tail = chainlink;
            }

            Length++;
#if DEBUG
            foreach (var chainlink2 in this) {
                if (chainlink2.Previous != null && chainlink2.Previous.Next != chainlink2) Debugger.Break();
                if (chainlink2.Next != null && chainlink2.Next.Previous != chainlink2) Debugger.Break();
            }
#endif
            return this;
        }

        /// <summary>
        /// Adds the given value to the beginning of the chain
        /// </summary>
        /// <param name="chainlink">The value to add</param>
        /// <returns>A reference to the chain</returns>
        public Chain<T> AddFirst(T value) {
            var link = new Chainlink<T>(value);

            return AddFirst(link);
        }

        /// <summary>
        /// Adds the given chainlink to the beginning of the chain, making it the Head
        /// </summary>
        /// <param name="chainlink">The chainlink to add</param>
        /// <returns>A reference to the chain</returns>
        public Chain<T> AddFirst(Chainlink<T> chainlink) {
            chainlink.Next = Head;

            if (Head == null) {
                Head = chainlink;
                Tail = chainlink;
            }
            else {
                Head.Previous = chainlink;
                Head = chainlink;
            }

            Length++;
#if DEBUG
            foreach (var chainlink2 in this) {
                if (chainlink2.Previous != null && chainlink2.Previous.Next != chainlink2) Debugger.Break();
                if (chainlink2.Next != null && chainlink2.Next.Previous != chainlink2) Debugger.Break();
            }
#endif
            return this;
        }

        public bool Remove(T valueToRemove) {
            foreach (var chainlink in this) {
                if (chainlink.Value.Equals(valueToRemove)) {
                    Remove(chainlink);
#if DEBUG
                    foreach (var link in this) {
                        if (link.Previous == null && link != Head) Debugger.Break();
                        if (link.Next == null && link != Tail) Debugger.Break();
                        if (link.Previous != null && link.Previous.Next != link) Debugger.Break();
                        if (link.Next != null && link.Next.Previous != link) Debugger.Break();
                    }
#endif
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removed the given chainlink and its value, if the chainlink is in the list
        /// </summary>
        /// <param name="chainlink">The chainlink to remove</param>
        /// <returns>True if the chainlink is valid and was in the list, false otherwise</returns>
        public bool Remove(Chainlink<T> chainlink) {
            if (chainlink == null || chainlink.Next != null && chainlink.Next.Previous != chainlink && chainlink != Tail || chainlink.Previous != null && chainlink.Previous.Next != chainlink && chainlink != Head) {
#if DEBUG
                Debugger.Break();
#endif
                return false;
            }

            if (chainlink == Head) Head = chainlink.Next;
            else chainlink.Previous.Next = chainlink.Next;

            if (chainlink == Tail) Tail = chainlink.Previous;
            else chainlink.Next.Previous = chainlink.Previous;

            Length--;

#if DEBUG
            foreach (var link in this) {
                if (link.Previous == null && link != Head) Debugger.Break();
                if (link.Next == null && link != Tail) Debugger.Break();
                if (link.Previous != null && link.Previous.Next != link) Debugger.Break();
                if (link.Next != null && link.Next.Previous != link) Debugger.Break();
            }
#endif
            return true;
        }

        //public Chain<T> Sort2() {
        //    var result = this.OrderBy<Chainlink<T>, string>(chainlink => chainlink.Value.);
        //}

        /// <summary>
        /// Sort the chain using the Comparer
        /// </summary>
        /// <returns>A reference to the chain</returns>
        /// <remarks>Uses the Comparer static property to compare elements if present, otherwise tries to use the elements' IComparable interface</remarks>
        public Chain<T> Sort() {
            if (Comparer != null && Length >= 2) {
                var (head, tail) = SortInner(Head, Tail, Length);
                Head = head;
                Tail = tail;
                Head.Previous = null;
                Tail.Next = null;
            }
#if DEBUG
            foreach (var chainlink in this) {
                if (chainlink.Previous != null && chainlink.Previous.Next != chainlink) Debugger.Break();
                if (chainlink.Next != null && chainlink.Next.Previous != chainlink) Debugger.Break();
                if (chainlink.Previous == null && chainlink != Head) Debugger.Break();
                if (chainlink.Next == null && chainlink != Tail) Debugger.Break();
            }
#endif
            return this;
        }

        protected (Chainlink<T> Head, Chainlink<T> Tail) SortInner(Chainlink<T> head, Chainlink<T> tail, int count) {
            if (count == 2) {
                if (Comparer(head.Value, tail.Value)) return (head, tail);

                head.Previous = tail;
                tail.Next = head;

                return (tail, head);
            }

            var current = head;
            var i = 0;
            for (; i < (count - 1) / 2; i++) {
                current = current.Next;
            }

            var next = current.Next;
            var (frontHead, frontTail) = SortInner(head, current, (count + 1) / 2);
            var (backHead, backTail) = count > 3 ? SortInner(next, tail, count - (count + 1) / 2) : (Head: next, Tail: tail);

            if (Comparer(frontTail.Value, backHead.Value)) {
                frontTail.Next = backHead;
                backHead.Previous = frontTail;

                return (frontHead, backTail);
            }

            frontTail.Next = null;
            backHead.Previous = null;
            backTail.Next = null;

            return Merge(frontHead, frontTail, backHead, backTail);
        }

        protected (Chainlink<T> front, Chainlink<T> back) Merge(Chainlink<T> left, Chainlink<T> leftTail, Chainlink<T> right, Chainlink<T> rightTail) {
            var tail = Comparer(leftTail.Value, rightTail.Value) ? rightTail : leftTail;

            Chainlink<T> head;
            if (Comparer(left.Value, right.Value)) {
                head = left;
                left = left.Next;
            }
            else {
                head = right;
                right = right.Next;
            }

            var current = head;

            while (true) {
                if (left == null) {
                    current.Next = right;
                    right.Previous = current;

                    return (head, tail);
                }

                if (right == null) {
                    current.Next = left;
                    left.Previous = current;

                    return (head, tail);
                }

                if (Comparer(left.Value, right.Value)) {
                    current.Next = left;
                    left.Previous = current;
                    left = left.Next;
                }
                else {
                    current.Next = right;
                    right.Previous = current;
                    right = right.Next;
                }

                current = current.Next;
            }
        }

        /// <summary>
        /// Removes all but the first instance of any equal elements that appear consecutively
        /// </summary>
        /// <returns>A reference to the chain</returns>
        /// <remarks>Ensures that all elements are distinct if the Chain is in sorted order</remarks>
        public Chain<T> Dedupe() {
            if (Length <= 1) return this;

            var current = Head;
            if (typeof(IComparable).IsAssignableFrom(typeof(T))) {
                while (current.Next != null) {
                    if (((IComparable)current.Value).CompareTo((IComparable)current.Next.Value) == 0) {
                        Remove(current.Next);
                    }
                    else {
                        current = current.Next;
                    }
                }
            }
            else {
                var count = Length - 1;
                while (count >= 0 && current.Next != null) {
                    if (current.Value.Equals(current.Next.Value)) {
                        Remove(current.Next);
                    }
                    else {
                        current = current.Next;
                    }

                    count--;
                }

                if (current.Next != null) {
#if DEBUG
                    Debugger.Break();
#else
                    Head = null;
                    Tail = null;
                    Count = 0;
#endif
                }
            }

            return this;
        }

        public Chain<T> Dedupe2() {
            if (Length <= 1) return this;

            var foundValues = new HashSet<T>() { Head.Value };
            var current = Head.Next;

            while (current != null) {
                if (foundValues.Contains(current.Value)) {
                    Remove(current);
                }
                current = current.Next;
            }

            return this;
        }

        public T this[int index] { //TODO: maybe implement this as a Map?
            get {
                var result = Head;

                for (var i = 0; i < index; i++) {
                    result = result.Next;
                }

                return result.Value;
            }
            set {
                var currentIndex = 0;
                foreach (var chainlink in this) {
                    if (currentIndex == index) {
                        chainlink.Value = value;
                        break;
                    }

                    currentIndex++;
                }
            }
        }

        public Enumerator<T> GetEnumerator() {
            return new Enumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator<T>(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        #region ICollection
        void ICollection.CopyTo(Array array, int index) {
            var currentIndex = index;

            foreach (var chainlink in this) {
                array.SetValue(chainlink.Value, currentIndex);
                currentIndex++;
            }
        }
        #endregion

        #region ICollection<T>
        bool ICollection<T>.Contains(T item) {
            foreach (var chainlink in this) {
                if (chainlink.Value.Equals(item)) {
                    return true;
                }
            }

            return false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            ((ICollection)this).CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item) {
            return Remove(item);
        }

        void ICollection<T>.Add(T item) {
            AddLast(item);
        }
        #endregion

        #region IList<T>
        int IList<T>.IndexOf(T item) {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item) {
            throw new NotImplementedException();
        }

        void IList<T>.RemoveAt(int index) {
            throw new NotImplementedException();
        }
        #endregion

        #region IList
        int IList.Add(object value) {
            throw new NotImplementedException();
        }

        void IList.Clear() {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value) {
            return ((ICollection<T>)this).Contains((T)value);
        }

        int IList.IndexOf(object value) {
            var currentIndex = 0;

            foreach (var chainlink in this) {
                if (chainlink.Value.Equals(value)) {
                    return currentIndex;
                }

                currentIndex++;
            }

            return -1;
        }

        void IList.Insert(int index, object value) {
            throw new NotImplementedException();
        }

        void IList.Remove(object value) {
            Remove((T)value);
        }

        void IList.RemoveAt(int index) {
            var currentIndex = 0;

            foreach (var chainlink in this) {
                if (currentIndex == index) {
                    Remove(chainlink);
                    break;
                }

                currentIndex++;
            }
        }

        public bool Contains(T target) {
            foreach(var item in this) {
                if (item.Equals(target)) return true;
            }

            return false;
        }

        public IStructure<T> Clear() {
            Head = null;
            Tail = null;
            Length = 0;

            return this;
        }

        public T Pop() {
            var result = Head.Value;

            Remove(Head);

            return result;
        }

        public T Peek() {
            return Head.Value;
        }

        public IStructure<T> Enqueue(T item) {
            return Add(item);
        }

        public T Dequeue() {
            return Pop();
        }

        IStructure<T> IStructure<T>.Add(T item) {
            return Add(item);
        }

        void ICollection<T>.Clear() {
            Clear();
        }

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes, Prefer.NoCompare };
        #endregion

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public struct Enumerator<T> : IEnumerator<T> {
            private Chain<T> collection;

            T IEnumerator<T>.Current => Current.Value; //implements IEnumerable<T>
            object IEnumerator.Current => Current.Value; //implements IEnumerable and used in foreach loop when cast to IEnumerable
            public Chainlink<T> Current { get; private set; } //used in foreach loop when typed as Chain<T>

            public Enumerator(Chain<T> collection) {
                this.collection = collection;
                Current = null;
            }

            public bool MoveNext() {
                if (Current == null) {
                    Current = collection.Head;
                    return Current != null;
                }

                if (Current?.Next == null) return false;

                Current = Current.Next;
                return true;
            }

            public void Reset() {
                Current = collection.Head;
            }

            public void Dispose() { }
        }
    }
}
