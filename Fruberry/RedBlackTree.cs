using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Fruberry {
    public class RedBlackTree<T> : IStructure<T> {
        public class Node<T> {
            public Color Color;
            public Node<T> Left;
            public Node<T> Right;
            public Node<T> Parent;
            public T Value;

            public Node(T data) { this.Value = data; }
            public Node(Color colour) { this.Color = colour; }
            public Node(T data, Color colour) { this.Value = data; this.Color = colour; }

            public override string ToString() {
                return $"{Value} ({Color})";
            }
        }

        public int Length { get; protected set; }

        public int Count => Length;

        int IStructure<T>.Count() { return Length; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        private Node<T> root;

        private int _compare(T left, T right) {
            if (IStructure<T>.Compare != null) return IStructure<T>.Compare(left, right);

            if (typeof(T) is IComparable<T>) return ((IComparable<T>)left).CompareTo(right);

            return 0;
        }

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes };

        public RedBlackTree() { }

        public bool Contains(T key) {
            return Find(key) != null;
        }

        private Node<T> Find(T key) {
            var isFound = false;
            var temp = root;
            while (!isFound) {
                if (temp == null) {
                    break;
                }
                if (_compare(key, temp.Value) < 0) {
                    temp = temp.Left;
                }
                else if (_compare(key, temp.Value) > 0) {
                    temp = temp.Right;
                }
                else if (_compare(key, temp.Value) == 0) {
                    isFound = true;
                }
            }

            return isFound ? temp : null;
        }

        public IStructure<T> Add(T item) {
            var newItem = new Node<T>(item);

            //handle the root specially to avoid a lot of null checking later
            if (root == null) {
                root = newItem;

                root.Color = Color.Black;

                Length++;

                return this;
            }

            Node<T> Y = null;
            var X = root;

            while (X != null) {
                Y = X;
                if (_compare(newItem.Value, X.Value) < 0) {
                    X = X.Left;
                }
                else {
                    X = X.Right;
                }
            }

            newItem.Parent = Y;

            if (Y == null) {
                root = newItem;
            }
            else if (_compare(newItem.Value, Y.Value) < 0) {
                Y.Left = newItem;
            }
            else {
                Y.Right = newItem;
            }

            newItem.Left = null;
            newItem.Right = null;
            newItem.Color = Color.Red;

            RebalanceAdd(newItem);

            Length++;

            return this;
        }

        public bool Remove(T key) {
            var target = Find(key);
            if (target == null) {
                return false;
            }

            var node1 = target.Left == null || target.Right == null ? target : Next(target);

            var node2 = node1.Left ?? node1.Right;

            if (node2 != null) {
                node2.Parent = node1.Parent;
            }

            if (node1.Parent == null) {
                root = node2;
            }
            else if (node1 == node1.Parent.Left) {
                node1.Parent.Left = node2;
            }
            else {
                node1.Parent.Right = node2;
            }

            if (node1 != target) {
                target.Value = node1.Value;
            }

            if (node1.Color == Color.Black) {
                RebalanceRemove(node2);
            }

            Length--;

            return true;
        }

        public T this[int i] {
            get {
                if (i == 0) return root.Value;

                return default;
            }
        }

        public void InOrderTraversal(Node<T> current, Action<Node<T>> action) {
            if (current != null) {
                InOrderTraversal(current.Left, action);

                action(current);

                InOrderTraversal(current.Right, action);
            }
        }

        public Node<T> Minimum(Node<T> node) {
            while (node.Left.Left != null) {
                node = node.Left;
            }

            if (node.Left.Right != null) {
                node = node.Left.Right;
            }

            return node;
        }

        /// <summary>
        /// Display Tree
        /// </summary>
        public void DisplayTree() {
            if (root == null) {
                Console.WriteLine("Nothing in the tree!");

                return;
            }

            if (root != null) {
                InOrderTraversal(root, (node) => Console.WriteLine(node));
            }
        }

        private void LeftRotate(Node<T> node1) {
            var node2 = node1.Right;

            node1.Right = node2.Left; //turn Y's left subtree into X's right subtree

            node2.Left.Parent = node1;

            node2.Parent = node1.Parent;

            if (node1.Parent == null) {
                root = node2;
            }
            else if (node1 == node1.Parent.Left) {
                node1.Parent.Left = node2;
            }
            else {
                node1.Parent.Right = node2;
            }

            node2.Left = node1;

            node1.Parent = node2;
        }

        /// <summary>
        /// Rotate Right
        /// </summary>
        private void RightRotate(Node<T> node1) {
            var node2 = node1.Left;

            node1.Left = node2.Right; //turn Y's left subtree into X's right subtree

            if (node2.Right != null) node2.Right.Parent = node1;

            node2.Parent = node1.Parent;

            if (node1.Parent == null) {
                root = node2;
            }
            else if (node1 == node1.Parent.Right) {
                node1.Parent.Right = node2;
            }
            else {
                node1.Parent.Left = node2;
            }

            node2.Right = node1;

            node1.Parent = node2;
        }

        private void RebalanceAdd(Node<T> redNode) {
            while (redNode.Parent?.Color == Color.Red) {
                if (redNode.Parent == redNode.Parent.Parent.Left) {
                    var temp = redNode.Parent.Parent.Right;

                    if (temp != null && temp.Color == Color.Red) {
                        redNode.Parent.Color = Color.Black;

                        temp.Color = Color.Black;

                        redNode.Parent.Parent.Color = Color.Red;

                        redNode = redNode.Parent.Parent;
                    }
                    else {
                        if (redNode == redNode.Parent.Right) {
                            redNode = redNode.Parent;

                            LeftRotate(redNode);
                        }

                        redNode.Parent.Color = Color.Black;

                        redNode.Parent.Parent.Color = Color.Red;

                        RightRotate(redNode.Parent.Parent);
                    }
                }
                else {
                    var temp = redNode.Parent.Parent.Left;

                    if (temp != null && temp.Color == Color.Red) {
                        redNode.Parent.Color = Color.Black;

                        temp.Color = Color.Black;

                        redNode.Parent.Parent.Color = Color.Red;

                        redNode = redNode.Parent.Parent;
                    }
                    else {
                        if (redNode == redNode.Parent.Left) {
                            redNode = redNode.Parent;

                            RightRotate(redNode);
                        }

                        redNode.Parent.Color = Color.Black;

                        redNode.Parent.Parent.Color = Color.Red;

                        LeftRotate(redNode.Parent.Parent);
                    }
                }

                root.Color = Color.Black;
            }
        }

        private void RebalanceRemove(Node<T> node1) {
            while (node1 != null && node1 != root && node1.Color == Color.Black) {
                if (node1 == node1.Parent.Left) {
                    var node2 = node1.Parent.Right;
                    if (node2.Color == Color.Red) {
                        node2.Color = Color.Black;

                        node1.Parent.Color = Color.Red;

                        LeftRotate(node1.Parent);

                        node2 = node1.Parent.Right;
                    }
                    if (node2.Left.Color == Color.Black && node2.Right.Color == Color.Black) {
                        node2.Color = Color.Red;

                        node1 = node1.Parent;
                    }
                    else if (node2.Right.Color == Color.Black) {
                        node2.Left.Color = Color.Black;

                        node2.Color = Color.Red;

                        RightRotate(node2);
                    }
                    else {
                        node2.Color = node1.Parent.Color;

                        node1.Parent.Color = Color.Black;

                        node2.Right.Color = Color.Black;

                        LeftRotate(node1.Parent);

                        node1 = root;
                    }
                }
                else // mirror code from above with "right" and "left" exchanged
                {
                    var node2 = node1.Parent.Left;
                    if (node2.Color == Color.Red) {
                        node2.Color = Color.Black;

                        node1.Parent.Color = Color.Red;

                        RightRotate(node1.Parent);

                        node2 = node1.Parent.Left;
                    }
                    if (node2.Right.Color == Color.Black && node2.Left.Color == Color.Black) {
                        node2.Color = Color.Black;

                        node1 = node1.Parent;
                    }
                    else if (node2.Left.Color == Color.Black) {
                        node2.Right.Color = Color.Black;

                        node2.Color = Color.Red;

                        LeftRotate(node2);

                        node2 = node1.Parent.Left;
                    }

                    node2.Color = node1.Parent.Color;

                    node1.Parent.Color = Color.Black;

                    node2.Left.Color = Color.Black;

                    RightRotate(node1.Parent);

                    node1 = root;
                }
            }
            if (node1 != null) node1.Color = Color.Black;
        }

        private bool IsConsistent() {
            if (root == null) return true;

            if (root.Color == Color.Red) return false;

            static void checkChildren(Node<T> current) {
                if (current.Color == Color.Red
                && (current.Left != null && current.Left.Color != Color.Black
                    || current.Right != null && current.Right.Color != Color.Black)) {
                    Debugger.Break();
                }
            }

            InOrderTraversal(root, checkChildren);

            return true;
        }

        private Node<T> Next(Node<T> current) {
            if (current.Right != null) {
                return Minimum(current.Right);
            }

            var temp = current.Parent;
            while (temp != null && current == temp.Right) {
                current = temp;

                temp = temp.Parent;
            }

            return temp;
        }

        IStructure<T> IStructure<T>.Add(T item) {
            Add(item);

            return this;
        }

        public IStructure<T> Clear() {
            root = null;

            return this;
        }

        public T Pop() {
            var result = root;

            Remove(root.Value); //TODO: test to find out what is fastest to remove

            return result.Value;
        }

        public T Peek() {
            return root.Value; //Same element as the one returned by Pop()
        }

        public IStructure<T> Enqueue(T item) {
            return Add(item);
        }

        public T Dequeue() {
            return Pop();
        }

        void ICollection<T>.Clear() {
            Clear();
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

        void ICollection<T>.Add(T item) {
            throw new NotImplementedException();
        }
    }
}
