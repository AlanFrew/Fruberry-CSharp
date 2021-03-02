using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fruberry {
    public class Graph<T> : ICollection<T>{
        //public List<GraphNode<T>> Nodes;
        public Dictionary<T, List<T>> Nodes;

        int ICollection<T>.Count => Nodes?.Count ?? 0;

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item) {
            if (Nodes == null) {
                //Nodes = new List<GraphNode<T>>();
                Nodes = new Dictionary<T, List<T>>();
            }

            //Nodes.Add(new GraphNode<T> {
            //    Value = item
            //});
            Nodes[item] = null;

            //Nodes[item].Add(item);
        }

        public Graph<T> Add(T item) {
            if (Nodes == null) Nodes = new Dictionary<T, List<T>>();

            Nodes[item] = null;

            return this;
        }

        public Graph<T> Add(T item, List<T> neighbors) {
            if (Nodes == null) Nodes = new Dictionary<T, List<T>>();

            Nodes[item] = neighbors;

            return this;
        }

        public Graph<T> AddNeighbors(T item, List<T> neighbors) {
            if (Nodes == null) Nodes = new Dictionary<T, List<T>>();

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
            if (Nodes.ContainsKey(item)) {
                return Nodes[item];
            }

            return null;
        }

        void ICollection<T>.Clear() {
            Nodes = null;
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

        bool ICollection<T>.Remove(T item) {
            if (Nodes == null) return false;

            var size = Nodes.Count;
            Nodes = null;// Nodes.Where(node => !node.Value.Equals(item)).ToDictionary(_ => _.Key, _ => _.Value);

            return size != Nodes.Count;
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public struct Enumerator<T> : IEnumerator<T> {
            //private List<GraphNode<T>> collection;
            private Dictionary<T, List<T>> collection;

            //private int currentIndex;
            private Dictionary<T, List<T>>.Enumerator enumerator;

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
