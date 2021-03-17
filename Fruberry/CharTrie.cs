using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fruberry {
    public class CharTrie : IStructure<char> {
        public class Node {
            public bool IsTerminal { get; set; }
            public char Value { get; set; }

            public Dictionary2<char, Node> Children { get; set; }

            public Node(char value) {
                Value = value;
                Children = new Dictionary2<char, Node>();
            }

            public Node FindChild(char c) {
                return Children.ContainsKey(c) ? Children[c] : null;
            }

            public override string ToString() {
                return Value == default(char) ? "<no value>" : Value.ToString();
            }
        }

        private readonly Node _root; //Contains no real data

        public IList<Prefer> Constraints => new[] { Prefer.NoDupes, Prefer.Find };

        public int Length { get; protected set; }

        int IStructure<char>.Length { get => Length; set => Length = value; }

        int ICollection<char>.Count => Length;

        public bool IsReadOnly => false;

        int System.Collections.ICollection.Count => Length;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public CharTrie() {
            _root = new Node(default);
        }

        public Node Prefix(string s) {
            var currentNode = _root;
            var result = currentNode;

            foreach (var c in s) {
                currentNode = currentNode.FindChild(c);
                if (currentNode == null) break;
                result = currentNode;
            }

            return result;
        }

        public List<Node> PrefixPath(string s) {
            var currentNode = _root;
            var foo = new List<Node>();

            var result = currentNode;

            foreach (var c in s) {
                currentNode = currentNode.FindChild(c);
                if (currentNode == null) break;
                else foo.Add(currentNode);

                result = currentNode;
            }

            return foo;
        }

        public bool ContainsExact(string s) {
            var currentNode = _root;
            Node nextNode = null;

            foreach(var ch in s) {
                nextNode = currentNode.FindChild(ch);

                if (nextNode == null) {
                    return false;
                }

                currentNode = nextNode;
            }

            return nextNode.IsTerminal;
        }

        public List<string> GetMatches(string s) {
            var matches = new List<List<Node>>();

            var prefixPath = PrefixPath(s);

            var stack = new Stack<Node>(prefixPath);

            foreach (var _ in prefixPath.Last().Children) {
                GetMatchesInner(stack, matches);
            }

            var result = new List<string>();

            foreach (var match in matches) {
                var bar = new StringBuilder();

                for (int i = match.Count - 1; i >= 0; i--) {
                    var letter = match[i];
                    bar.Append(letter);
                }

                result.Add(bar.ToString());
            }

            if (prefixPath.Last().Children.None()) {
                var prefixMatch = new StringBuilder();

                foreach (var node in prefixPath) {
                    prefixMatch.Append(node.Value);
                }

                result.Add(prefixMatch.ToString());
            }

            return result;
        }

        private List<List<Node>> GetMatchesInner(Stack<Node> stack, List<List<Node>> results) {
            if (stack.Peek().IsTerminal) {
                results.Add(new List<Node>(stack));
            }

            foreach (var child in stack.Peek().Children) {
                stack.Push(child.Value);

                GetMatchesInner(stack, results);

                stack.Pop();
            }

            return results;
        }

        public void AddRange(IEnumerable<string> items) {
            foreach (var item in items) Add(item);
        }

        public CharTrie Add(string s) {
            var current = _root;
            for (var i = 0; i < s.Length; i++) {
                if (current.FindChild(s[i]) == null) {
                    current.Children.Add(s[i], new Node(s[i]));
                }

                current = current.FindChild(s[i]);
            }

            current.IsTerminal = true;

            return this;
        }

        public void Delete(string s) {
            var possibleDeletions = new Stack<Node>();
            possibleDeletions.Push(_root);

            var currentNode = _root;

            foreach (var ch in s) {
                var nextNode = currentNode.FindChild(ch);

                if (nextNode != null) {
                    possibleDeletions.Push(nextNode);
                }
                else break;

                currentNode = nextNode;
            }

            possibleDeletions.Peek().IsTerminal = false;

            while (possibleDeletions.Count > 1) {
                var nodeToDelete = possibleDeletions.Pop();

                if (nodeToDelete.IsTerminal == false && nodeToDelete.Children.None()) {
                    possibleDeletions.Peek().Children.Remove(nodeToDelete.Value);
                }
                else return;
            }
        }

        public IStructure<char> Add(char item) {
            throw new System.NotImplementedException();
        }

        public bool Remove(char item) {
            throw new System.NotImplementedException();
        }

        public int Count() {
            throw new System.NotImplementedException();
        }

        public bool Contains(char item) {
            throw new System.NotImplementedException();
        }

        public IStructure<char> Clear() {
            throw new System.NotImplementedException();
        }

        public char Pop() {
            throw new System.NotImplementedException();
        }

        public char Peek() {
            throw new System.NotImplementedException();
        }

        public IStructure<char> Enqueue(char item) {
            throw new System.NotImplementedException();
        }

        public char Dequeue() {
            throw new System.NotImplementedException();
        }

        void ICollection<char>.Add(char item) {
            throw new System.NotImplementedException();
        }

        void ICollection<char>.Clear() {
            throw new System.NotImplementedException();
        }

        public void CopyTo(char[] array, int arrayIndex) {
            throw new System.NotImplementedException();
        }

        public IEnumerator<char> GetEnumerator() {
            throw new System.NotImplementedException();
        }

        public void CopyTo(System.Array array, int index) {
            throw new System.NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            throw new System.NotImplementedException();
        }
    }
}