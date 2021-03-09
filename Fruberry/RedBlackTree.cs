using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fruberry {
    public class RedBlackTree<T> : IStructure<T> {
        public class Node<T> {
            public bool Color;
            public Node<T> Left;
            public Node<T> Right;
            public Node<T> Parent;
            public T Value;

            public Node() { Color = BLACK; }
            public Node(T data) { Value = data; }
            public Node(Node<T> parent) { Parent = parent; Color = BLACK; }

            public override string ToString() {
                return $"{(Value == null ? "Nil" : Value.ToString())} ({(Color ? "Black" : "Red")})";
            }
        }

        protected static bool BLACK = true;
        protected static bool RED = false;
        protected Node<T> Nil = new Node<T>();

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

            if (typeof(T).GetInterface(nameof(IComparable<T>)) != null) return ((IComparable<T>)left).CompareTo(right);

            return 0;
        }

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes };

        public RedBlackTree() {
            Nil.Left = Nil.Right = Nil;

            root = Nil;
        }

        public bool Contains(T key) {
            var result = Find(key) != null;

            //if (result == false) Debugger.Break();

            return result;
        }

        private Node<T> Find(T key) {
            var isFound = false;
            var temp = root;
            while (!isFound) {
                if (temp == Nil) {
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
            var Y = Nil;
            var newItem = new Node<T>(item) { Left = Nil, Right = Nil };
            var X = root;
            //handle the root specially to avoid a lot of null checking later
            //if (root == null) {
            //    root = newItem;

            //    root.Color = BLACK;

            //    Length++;

            //    return this;
            //}

            //Node<T> Y = null;
            //var X = root;

            while (X != Nil) {
                Y = X;
                if (_compare(newItem.Value, X.Value) < 0) {
                    X = X.Left;
                }
                else {
                    X = X.Right;
                }
            }

            newItem.Parent = Y;

            if (Y == Nil) {
                root = newItem;
            }
            else if (_compare(newItem.Value, Y.Value) < 0) {
                Y.Left = newItem;
            }
            else {
                Y.Right = newItem;
            }

            newItem.Color = RED;

            RebalanceAdd(newItem);

            Length++;

            return this;
        }

        //public bool Remove(T key) {
        //    var z = Find(key);
        //    if (z == null) {
        //        return false;
        //    }

        //    var y = z;
        //    var yOriginalColor = y.Color;

        //    Node<T> x;

        //    if (z.Left == null) {
        //        x = z.Right;
        //        rbTransplant(z, z.Left);
        //    }
        //    else if (z.Right == null) {
        //        x = z.Left;
        //    }
        //    else {
        //        y = Minimum(z.Right);
        //        yOriginalColor = y.Color;
        //        x = y.Right;
        //        if (y.Parent == z) {
        //            x.Parent = y;
        //        }
        //        else {
        //            rbTransplant(y, y.Right);
        //            y.Right = z.Right;
        //            y.Right.Parent = y;
        //        }

        //        rbTransplant(z, y);
        //        y.Left = z.Left;
        //        y.Left.Parent = y;
        //        y.Color = z.Color;
        //    }

        //    if (yOriginalColor == BLACK) {
        //        RebalanceRemove(x);
        //    }

        //    Length--;

        //    return true;
        //}

        public bool Remove(T key) {
            var target = Find(key);
            if (target == null) {
                return false;
            }

            Node<T> x;
            Node<T> y;

            if (target.Left == Nil || target.Right == Nil)
                y = target;
            else {
                //y = target.Right;
                //while (y.Left != Nil)
                //    y = y.Left;
                y = Next(target);
            }

            if (y.Left != Nil)
                x = y.Left;
            else
                x = y.Right;

            x.Parent = y.Parent;
            if (y.Parent != Nil)
                if (y == y.Parent.Left)
                    y.Parent.Left = x;
                else
                    y.Parent.Right = x;
            else
                root = x;

            if (y != target) {
                target.Value = y.Value;
            }

            if (y.Color == BLACK)
                RebalanceRemove(x);

            //lastNodeFound = sentinelNode;

            //var node1 = (target.Left == Nil || target.Right == Nil) ? target : Next(target);

            //Node<T> fake = null;

            //var node2 = node1.Left != Nil ? node1.Left : node1.Right;

            //node2.Parent = node1.Parent;

            //if (node1.Parent == Nil) {
            //    root = node2;
            //}
            //else if (node1 == node1.Parent.Left) {
            //    node1.Parent.Left = node2;
            //}
            //else {
            //    node1.Parent.Right = node2;
            //}

            //if (node1 != target) {
            //    target.Value = node1.Value;
            //}

            //if (node1.Color == BLACK) {
            //    if (node2.Color != BLACK) Debugger.Break();

            //    RebalanceRemove(node2, fake);
            //}

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
            while (node.Left != Nil) {
                node = node.Left;
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

            if (node2.Left != Nil) node2.Left.Parent = node1;

            node2.Parent = node1.Parent;

            if (node1.Parent == Nil) {
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

            if (node2.Right != Nil) node2.Right.Parent = node1;

            node2.Parent = node1.Parent;

            if (node1.Parent == Nil) {
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
            while (redNode.Parent.Color == RED) {
                if (redNode.Parent == redNode.Parent.Parent.Left) {
                    var temp = redNode.Parent.Parent.Right;

                    if (temp != null && temp.Color == RED) {
                        redNode.Parent.Color = BLACK;

                        temp.Color = BLACK;

                        redNode.Parent.Parent.Color = RED;

                        redNode = redNode.Parent.Parent;
                    }
                    else {
                        if (redNode == redNode.Parent.Right) {
                            redNode = redNode.Parent;

                            LeftRotate(redNode);
                        }

                        redNode.Parent.Color = BLACK;

                        redNode.Parent.Parent.Color = RED;

                        RightRotate(redNode.Parent.Parent);
                    }
                }
                else {
                    var temp = redNode.Parent.Parent.Left;

                    if (temp != null && temp.Color == RED) {
                        redNode.Parent.Color = BLACK;

                        temp.Color = BLACK;

                        redNode.Parent.Parent.Color = RED;

                        redNode = redNode.Parent.Parent;
                    }
                    else {
                        if (redNode == redNode.Parent.Left) {
                            redNode = redNode.Parent;

                            RightRotate(redNode);
                        }

                        redNode.Parent.Color = BLACK;

                        redNode.Parent.Parent.Color = RED;

                        LeftRotate(redNode.Parent.Parent);
                    }
                }
            }

            root.Color = BLACK;
        }

        //private void RebalanceRemove(Node<T> x) {
        //    Node<T> s;
        //    while (x != root && x.Color == BLACK) {
        //        if (x == x.Parent.Left) {
        //            s = x.Parent.Right;
        //            if (s.Color == RED) {
        //                // case 3.1
        //                s.Color = BLACK;
        //                x.Parent.Color = RED;
        //                LeftRotate(x.Parent);
        //                s = x.Parent.Right;
        //            }

        //            if (s.Left.Color == BLACK && s.Right.Color == BLACK) {
        //                // case 3.2
        //                s.Color = RED;
        //                x = x.Parent;
        //            }
        //            else {
        //                if (s.Right.Color == BLACK) {
        //                    // case 3.3
        //                    s.Left.Color = BLACK;
        //                    s.Color = RED;
        //                    RightRotate(s);
        //                    s = x.Parent.Right;
        //                }

        //                // case 3.4
        //                s.Color = x.Parent.Color;
        //                x.Parent.Color = BLACK;
        //                s.Right.Color = BLACK;
        //               LeftRotate(x.Parent);
        //                x = root;
        //            }
        //        }
        //        else {
        //            s = x.Parent.Left;
        //            if (s.Color == RED) {
        //                // case 3.1
        //                s.Color = BLACK;
        //                x.Parent.Color = RED;
        //                RightRotate(x.Parent);
        //                s = x.Parent.Left;
        //            }

        //            if (s.Right.Color == BLACK && s.Right.Color == BLACK) {
        //                // case 3.2
        //                s.Color = RED;
        //                x = x.Parent;
        //            }
        //            else {
        //                if (s.Left.Color == BLACK) {
        //                    // case 3.3
        //                    s.Right.Color = BLACK;
        //                    s.Color = RED;
        //                    LeftRotate(s);
        //                    s = x.Parent.Left;
        //                }

        //                // case 3.4
        //                s.Color = x.Parent.Color;
        //                x.Parent.Color = BLACK;
        //                s.Left.Color = BLACK;
        //                RightRotate(x.Parent);
        //                x = root;
        //            }
        //        }
        //    }
        //    x.Color = BLACK;
        //}

        protected void RebalanceRemove(Node<T> x) {
            while (x != root && x.Color == BLACK) {
                if (x == x.Parent.Left) {
                    var sibling = x.Parent.Right;
                    if (sibling.Color == RED) {
                        sibling.Color = BLACK;

                        x.Parent.Color = RED;

                        LeftRotate(x.Parent);

                        sibling = x.Parent.Right;
                    }
                    if (sibling.Left.Color == BLACK && sibling.Right.Color == BLACK) {
                        sibling.Color = RED;

                        x = x.Parent;
                    }
                    else {
                        if (sibling.Right.Color == BLACK) {
                            sibling.Left.Color = BLACK;

                            sibling.Color = RED;

                            RightRotate(sibling);

                            sibling = x.Parent.Right;
                        }

                        sibling.Color = x.Parent.Color;

                        x.Parent.Color = BLACK;

                        sibling.Right.Color = BLACK;

                        LeftRotate(x.Parent);

                        x = root;
                    }
                }
                else // mirror code from above with "right" and "left" exchanged
                {
                    var y = x.Parent.Left;
                    if (y?.Color == RED) {
                        y.Color = BLACK;

                        x.Parent.Color = RED;

                        RightRotate(x.Parent);

                        y = x.Parent.Left;
                    }
                    if (y.Right.Color == BLACK && y.Left.Color == BLACK) {
                        y.Color = RED;

                        x = x.Parent;
                    }
                    else {
                        if (y.Left.Color == BLACK) {
                            y.Right.Color = BLACK;

                            y.Color = RED;

                            LeftRotate(y);

                            y = x.Parent.Left;
                        }

                        y.Color = x.Parent.Color;

                        x.Parent.Color = BLACK;

                        y.Left.Color = BLACK;

                        RightRotate(x.Parent);

                        x = root;
                    }
                }
            }

            x.Color = BLACK;

            //if (fake != null) {
            //    if (fake.Parent.Left == fake) fake.Parent.Left = null;
            //    else fake.Parent.Right = null;
            //}
        }

        protected bool IsConsistent() {
            if (root == null) return true;

            if (root.Color == RED) return false;

            static void checkChildren(Node<T> current) {
                if (current.Color == RED
                && (current.Left != null && current.Left.Color != BLACK
                    || current.Right != null && current.Right.Color != BLACK)) {
                    Debugger.Break();
                }
            }

            InOrderTraversal(root, checkChildren);

            return true;
        }

        protected bool Validate() {
            if (root == Nil) return Length == 0;

            if (root.Parent != Nil) return false;

            if (root.Color == RED) return false;

            return Validate(root).isValid;
        }

        protected (bool isValid, int blackCount) Validate(Node<T> subroot) {
            if (subroot == Nil) return (true, 1);

            int blackCount;
            if (subroot.Color == RED) {
                blackCount = 0;

                if (subroot.Left != null && subroot.Left.Color == RED
                || subroot.Right != null && subroot.Right.Color == RED) {
                    return (false, -1);
                }
            }
            else {
                blackCount = 1;
            }

            (var leftValid, var leftCount) = Validate(subroot.Left);
            (var rightValid, var rightCount) = Validate(subroot.Right);

            if (!leftValid || !rightValid || leftCount != rightCount) Debugger.Break();

            return (leftValid && rightValid && leftCount == rightCount, leftCount + blackCount);
        }

        protected Node<T> Next(Node<T> current) {
            if (current.Right != Nil) {
                return Minimum(current.Right);
            }

            var temp = current.Parent;
            while (temp != Nil && current == temp.Right) {
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
            if (root == null) return default;

            var result = root.Value;

            Remove(root.Value); //TODO: test to find out what node is fastest to remove

            return result;
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

        public class RedBlackEnumerator<T> {
            private Stack<RedBlackTree<T>.Node<T>> stack;
            private bool ascending;
            public string Color;             // testing only, don't use in live system
            public IComparable<T> parentKey;     // testing only, don't use in live system
            public RedBlackTree<T> tree;
            ///<summary>
            ///Key
            ///</summary>
            public IComparable<T> Key { get; set; }

            ///<summary>
            ///Data
            ///</summary>
            public object Value { get; set; }

            public RedBlackEnumerator() {
            }

            ///<summary>
            /// Determine order, walk the tree and push the nodes onto the stack
            ///</summary>
            public RedBlackEnumerator(RedBlackTree<T> tree, bool keys, bool ascending) {
                this.tree = tree;
                var node = tree.root;

                stack = new Stack<RedBlackTree<T>.Node<T>>();
                this.ascending = ascending;

                // use depth-first traversal to push nodes into stack
                // the lowest node will be at the top of the stack
                if (ascending) {   // find the lowest node
                    while (node != tree.Nil) {
                        stack.Push(node);
                        node = node.Left;
                    }
                }
                else {
                    // the highest node will be at top of stack
                    while (node != tree.Nil) {
                        stack.Push(node);
                        node = node.Right;
                    }
                }

            }

            ///<summary>
            /// NextElement
            ///</summary>
            public T NextElement() {
                if (stack.Count == 0)
                    throw (new Exception("Element not found"));

                // the top of stack will always have the next item
                // get top of stack but don't remove it as the next nodes in sequence
                // may be pushed onto the top
                // the stack will be popped after all the nodes have been returned
                var node = stack.Peek(); //next node in sequence

                if (ascending) {
                    if (node.Right == tree.Nil) {
                        // yes, top node is lowest node in subtree - pop node off stack 
                        var tn = stack.Pop();
                        // peek at right node's parent 
                        // get rid of it if it has already been used
                        while (stack.Any() && (stack.Peek()).Right == tn)
                            tn = stack.Pop();
                    }
                    else {
                        // find the next items in the sequence
                        // traverse to left; find lowest and push onto stack
                        var tn = node.Right;
                        while (tn != tree.Nil) {
                            stack.Push(tn);
                            tn = tn.Left;
                        }
                    }
                }
                else            // descending, same comments as above apply
                {
                    if (node.Left == tree.Nil) {
                        // walk the tree
                        var tn = stack.Pop();
                        while (stack.Any() && (stack.Peek()).Left == tn)
                            tn = stack.Pop();
                    }
                    else {
                        // determine next node in sequence
                        // traverse to left subtree and find greatest node - push onto stack
                        var tn = node.Left;
                        while (tn != tree.Nil) {
                            stack.Push(tn);
                            tn = tn.Right;
                        }
                    }
                }

                Value = node.Value;

                // ******** testing only ********
                if (node.Color == false)                     // testing only
                    Color = "Red";
                else
                    Color = "Black";
                // ******** testing only ********

                return node.Value;
            }
            ///<summary>
            /// MoveNext
            /// For .NET compatibility
            ///</summary>
            public bool MoveNext() {
                if (stack.Any()) {
                    NextElement();
                    return true;
                }

                return false;
            }
        }
    }
}
