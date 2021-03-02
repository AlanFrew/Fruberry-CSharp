using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fruberry {
    public class CharTrieNode {
        public bool IsTerminal { get; set; }
        public char Value { get; set; }
        public Dictionary<char, CharTrieNode> Children { get; set; }

        public CharTrieNode(char value) {
            Value = value;
            Children = new Dictionary<char, CharTrieNode>();
        }

        public CharTrieNode FindChildNode(char c) {
            return Children.ContainsKey(c) ? Children[c] : null;
        }

        public override string ToString() {
            return Value == default(char) ? "<no value>" : Value.ToString();
        }
    }

    public class CharTrie {
        private readonly CharTrieNode _root;

        public CharTrie() {
            _root = new CharTrieNode(default);
        }

        public CharTrieNode Prefix(string s) {
            var currentNode = _root;
            var result = currentNode;

            foreach (var c in s) {
                currentNode = currentNode.FindChildNode(c);
                if (currentNode == null) break;
                result = currentNode;
            }

            return result;
        }

        public List<CharTrieNode> PrefixPath(string s) {
            var currentNode = _root;
            var foo = new List<CharTrieNode>();

            var result = currentNode;

            foreach (var c in s) {
                currentNode = currentNode.FindChildNode(c);
                if (currentNode == null) break;
                else foo.Add(currentNode);

                result = currentNode;
            }

            return foo;
        }

        public bool ContainsExact(string s) {
            var currentNode = _root;
            CharTrieNode nextNode = null;

            foreach(var ch in s) {
                nextNode = currentNode.FindChildNode(ch);

                if (nextNode == null) {
                    return false;
                }

                currentNode = nextNode;
            }

            return nextNode.IsTerminal;
        }

        public List<string> GetMatches(string s) {
            var matches = new List<List<CharTrieNode>>();

            var prefixPath = PrefixPath(s);

            var stack = new Stack<CharTrieNode>(prefixPath);

            foreach (var _ in prefixPath.Last().Children) {
                matches.AddRange(GetMatchesInner(stack, matches));
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

        private List<List<CharTrieNode>> GetMatchesInner(Stack<CharTrieNode> stack, List<List<CharTrieNode>> results) {
            if (stack.Peek().IsTerminal) {
                results.Add(new List<CharTrieNode>(stack));
            }

            foreach (var child in stack.Peek().Children) {
                stack.Push(child.Value);

                results.AddRange(GetMatchesInner(stack, results));

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
                if (current.FindChildNode(s[i]) == null) {
                    current.Children.Add(s[i], new CharTrieNode(s[i]));
                }

                current = current.FindChildNode(s[i]);
            }

            current.IsTerminal = true;

            return this;
        }

        public void Delete(string s) {
            var possibleDeletions = new Stack<CharTrieNode>();
            possibleDeletions.Push(_root);

            var currentNode = _root;

            foreach (var ch in s) {
                var nextNode = currentNode.FindChildNode(ch);

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

    }
}