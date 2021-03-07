using System.Collections.Generic;
using System.Linq;

namespace Fruberry {
    public class TrieNode<T> {
        public bool IsMatch { get; set; }
        public T Value { get; set; }
        public Dictionary2<T, TrieNode<T>> Children { get; set; }

        public TrieNode(T value) {
            Value = value;
        }

        public TrieNode<T> GetChild(T c) {
            return Children != null && Children.ContainsKey(c) ? Children[c] : null;
        }

        public override string ToString() {
            return Value == null || Value.Equals(default(T)) ? "<no value>" : Value.ToString();
        }
    }

    public class Trie<T> : ICollection<IEnumerable<T>> {
        private readonly TrieNode<T> _root;
        private int _count;

        public int Count => _count;

        public bool IsReadOnly => false;

        public Trie() {
            _root = new TrieNode<T>(default);
        }

        public TrieNode<T> Prefix(IEnumerable<T> s) {
            var currentNode = _root;
            var result = currentNode;

            foreach (var c in s) {
                currentNode = currentNode.GetChild(c);
                if (currentNode == null) break;
                result = currentNode;
            }

            return result;
        }

        public List<TrieNode<T>> PrefixPath(IEnumerable<T> s) {
            if (s.Count() == 0) return new List<TrieNode<T>> { _root };

            var currentNode = _root;
            var result = new List<TrieNode<T>>();

            foreach (var c in s) {
                currentNode = currentNode.GetChild(c);
                if (currentNode == null) break;
                else result.Add(currentNode);
            }

            return result;
        }

        public bool ContainsExact(IEnumerable<T> s) {
            var currentNode = _root;
            TrieNode<T> nextNode = null;

            foreach(var ch in s) {
                nextNode = currentNode.GetChild(ch);

                if (nextNode == null) {
                    return false;
                }

                currentNode = nextNode;
            }

            return nextNode.IsMatch;
        }

        public IEnumerable<IEnumerable<T>> GetMatches(IEnumerable<T> s) {
            var prefixPath = PrefixPath(s);

            if (prefixPath.None()) return new List<IEnumerable<T>>();

            var matches = new List<IEnumerable<T>>();

            var children = prefixPath[^1].Children;

            if (children != null) {

                foreach (var _ in children) {
                    GetMatchesInner(matches, prefixPath, prefixPath.Count - 1, prefixPath[0] == _root);
                }
            }
            else {
                var result = new T[prefixPath.Count];

                for (int i = 0; i < prefixPath.Count; i++) {
                    result[i] = prefixPath[i].Value;
                }

                matches.Add(Join(prefixPath));
            }

            return matches;
        }

        private List<IEnumerable<T>> GetMatchesInner(List<IEnumerable<T>> results, List<TrieNode<T>> stack, int stackIndex, bool containsRoot) {
            if (stack[stackIndex].IsMatch) {
                if (containsRoot) {
                    var result = new T[stackIndex];

                    for (int i = 1; i < stackIndex + 1; i++) {
                        result[i - 1] = stack[i].Value;
                    }

                    results.Add(result);
                }
                else {
                    var result = new T[stackIndex + 1];

                    for (int i = 0; i < stackIndex + 1; i++) {
                        result[i] = stack[i].Value;
                    }

                    results.Add(result);
                }
            }

            var children = stack[stackIndex].Children;

            if (children != null) {
                foreach (var child in children) {
                    stackIndex++;

                    if (stack.Count > stackIndex) stack[stackIndex] = child.Value;
                    else stack.Add(child.Value);


                    GetMatchesInner(results, stack, stackIndex, containsRoot);

                    stackIndex--;
                }
            }

            return results;
        }

        public void AddRange(IEnumerable<IEnumerable<T>> items) {
            foreach (var item in items) Add(item);
        }

        public Trie<T> Add(IEnumerable<T> s) {
            var current = _root;

            foreach (var item in s) {
                if (current.GetChild(item) == null) {
                    if (current.Children == null) current.Children = new Dictionary2<T, TrieNode<T>>();

                    current.Children.Add(item, new TrieNode<T>(item));
                }

                current = current.GetChild(item);
            }

            if (current.IsMatch == false) _count++;

            current.IsMatch = true;

            return this;
        }

        public bool Remove(IEnumerable<T> s) {
            var possibleDeletions = new Stack<TrieNode<T>>();
            possibleDeletions.Push(_root);

            var currentNode = _root;

            foreach (var ch in s) {
                var nextNode = currentNode.GetChild(ch);

                if (nextNode != null) {
                    possibleDeletions.Push(nextNode);
                }
                else break;

                currentNode = nextNode;
            }

            possibleDeletions.Peek().IsMatch = false;

            while (possibleDeletions.Count > 1) {
                var nodeToDelete = possibleDeletions.Pop();

                if (nodeToDelete.IsMatch == false && nodeToDelete.Children.None()) {
                    possibleDeletions.Peek().Children.Remove(nodeToDelete.Value);
                    _count--;

                    return true;
                }
                else return false;
            }

            return false; //unreachable
        }

        public Trie<T> Clear() {
            _root.Children = null;

            return this;
        }

        public static IEnumerable<T> Join(IEnumerable<TrieNode<T>> source) {
            var result = new T[source.Count()];

            int index = 0;
            foreach (var node in source) {
                result[index] = node.Value;

                index++;
            }

            return result;
        }

        public IEnumerable<IEnumerable<T>> this[IEnumerable<T> stem] {
            get {
                return GetMatches(stem);
            }
        }

        void ICollection<IEnumerable<T>>.Add(IEnumerable<T> item) {
            Add(item);
        }

        void ICollection<IEnumerable<T>>.Clear() {
            Clear();
        }

        bool ICollection<IEnumerable<T>>.Contains(IEnumerable<T> item) {
            return ContainsExact(item);
        }

        public void CopyTo(IEnumerable<T>[] array, int arrayIndex) {
            var items = GetMatches(Enumerable.Empty<T>());

            int increment = 0;
            foreach(var item in items) {
                array[arrayIndex + increment] = item;
            }
        }

        //bool ICollection<IEnumerable<T>>.Remove(IEnumerable<T> item) {
        //    return Remove(item);
        //}

        IEnumerator<IEnumerable<T>> IEnumerable<IEnumerable<T>>.GetEnumerator() {
            throw new System.NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new System.NotImplementedException();
        }
    }
}