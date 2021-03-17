using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fruberry {
    public class Graph<T> : IStructure<T>{
        //public List<GraphNode<T>> Nodes;
        public Dictionary2<T, List<T>> Nodes; //TODO: Replace with IStructure

        public int Length { get => Nodes?.Count ?? 0; protected set { } } //TODO: Fix when dictionary is replaced

        public int Count => Length;

        int IStructure<T>.Count() { return Length; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        bool ICollection<T>.IsReadOnly => false;

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes };

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        void ICollection<T>.Add(T item) {
            Add(item);
        }

        public Graph<T> Add(T item) {
            if (Nodes == null) Nodes = new Dictionary2<T, List<T>>();

            Nodes[item] = null;

            return this;
        }

        public Graph<T> Add(T item, IList<T> neighbors) {
            if (Nodes == null) Nodes = new Dictionary2<T, List<T>>();

            Nodes[item] = new List<T>(neighbors);

            return this;
        }

        public Graph<T> AddNeighbors(T item, List<T> neighbors) {
            if (Nodes == null) Nodes = new Dictionary2<T, List<T>>();

            if (Nodes.ContainsKey(item) == false) {
                Nodes[item] = neighbors;
            }
            else if (Nodes[item] == null) {
                Nodes[item] = neighbors;
            }
            else {
                Nodes[item].AddRange(neighbors);
            }

            return this;
        }

        public bool ConnectsTo(T origin, T destination) {
            if (Nodes == null) return false;

            if (Nodes.ContainsKey(origin) == false) return false;

            return Nodes[origin].Contains(destination);
        }

        public List<T> GetNeighbors(T item) {
            if (item == null) return new List<T>(0);
            if (Nodes.ContainsKey(item)) {
                return Nodes[item];
            }

            return new List<T>(0);
        }

        public Graph<T> Clear() {
            Nodes = null;

            return this;
        }

        void ICollection<T>.Clear() {
            Clear();
        }

        bool ICollection<T>.Contains(T item) {
            return Nodes?.Any(node => node.Value.Equals(item)) ?? false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            if (Nodes == null) return;

            //for (var i = arrayIndex; i < Nodes.Count; i++) {
            //    array[i] = Nodes[i].Value;
            //}
            
            foreach (var item in Nodes.Keys) {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }

        public bool Remove(T item) {
            return Nodes.Remove(item);
        }

        public bool Contains(T item) {
            return Nodes.ContainsKey(item);
        }

        public T Pop() {
            var result = Nodes.First();

            Nodes.Remove(result.Key);

            return result.Key;
        }

        public T Peek() {
            return Nodes.First().Key;
        }

        public IStructure<T> Enqueue(T item) {
            return Add(item);
        }

        public T Dequeue() {
            return Pop();
        }

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        IStructure<T> IStructure<T>.Clear() {
            return Clear();
        }

        IStructure<T> IStructure<T>.Add(T item) {
            return Add(item);
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public struct Enumerator<T> : IEnumerator<T> {
            //private List<GraphNode<T>> collection;
            private Dictionary2<T, List<T>> collection;

            //private int currentIndex;
            private Dictionary2<T, List<T>>.Enumerator enumerator;

            //private T current = Nodes.Keys.First();
            T IEnumerator<T>.Current => Current; //implements IEnumerable<T>
            object IEnumerator.Current => Current; //implements IEnumerable and used in foreach loop when cast to IEnumerable
            public T Current => enumerator.Current.Key; //used in foreach loop when typed as Chain<T>

            public Enumerator(Graph<T> collection) {
                this.collection = collection.Nodes;
                enumerator = collection.Nodes.GetEnumerator();
            }

            public bool MoveNext() {
                return enumerator.MoveNext();
            }

            public void Reset() {
                enumerator = collection.GetEnumerator();
            }

            public void Dispose() { }
        }
    }

    public class GraphNode<T> {
        public List<GraphNode<T>> Neighbors;

        public T Value;
    }
}
