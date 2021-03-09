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

            public Node(T value = default, bool color = true, Node<T> parent = null, Node<T> left = null, Node<T> right = null) {
                Value = value;
                Color = color;
                Parent = parent;
                Left = left;
                Right = right;
            }

            public override string ToString() {
                return $"{(Value == null ? "Nil" : Value.ToString())} ({(Color ? "Black" : "Red")})";
            }
        }

        protected static bool BLACK = true;
        protected static bool RED = false;
        protected Node<T> Nil = new Node<T>(default);
        protected Node<T> Root;

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes };

        public virtual int Length { get; protected set; }

        public virtual int Count => Length;

        int IStructure<T>.Count() { return Length; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        public virtual bool IsReadOnly => false;

        public virtual bool IsSynchronized => false;

        public virtual object SyncRoot => null;

        public RedBlackTree() {
            Root = Nil.Left = Nil.Right = Nil.Parent = Nil;
        }

        virtual protected int Compare(T left, T right) {
            if (IStructure<T>.Compare != null) return IStructure<T>.Compare(left, right);

            if (typeof(T).GetInterface(nameof(IComparable<T>)) != null) return ((IComparable<T>)left).CompareTo(right);

            return 0;
        }

        public virtual bool Contains(T key) {
            var result = Find(key) != null;

            return result;
        }

        protected virtual Node<T> Find(T key) {
            var isFound = false;
            var temp = Root;
            while (!isFound) {
                if (temp == Nil) break;

                if (Compare(key, temp.Value) == 0) return temp;

                if (Compare(key, temp.Value) < 0) temp = temp.Left;
                else temp = temp.Right;        
            }

            return isFound ? temp : null;
        }

        public virtual RedBlackTree<T> Add(T item) {
            var newItem = new Node<T>(item) { Left = Nil, Right = Nil, Parent = Nil };
            var newParent = Nil;        
            var temp = Root;

            while (temp != Nil) {
                newParent = temp;

                temp = Compare(newItem.Value, temp.Value) < 0 ? temp.Left : temp.Right;
            }

            newItem.Parent = newParent;

            if (newParent == Nil) {
                Root = newItem;
            }
            else if (Compare(newItem.Value, newParent.Value) < 0) {
                newParent.Left = newItem;
            }
            else {
                newParent.Right = newItem;
            }

            newItem.Color = RED;

            RebalanceAdd(newItem);

            Length++;

            return this;
        }

        public virtual bool Remove(T key) {
            var target = Find(key);

            if (target == null) {
                return false;
            }

            var temp1 = target.Left == Nil || target.Right == Nil ? target : Next(target);

            var tempChild = temp1.Left != Nil ? temp1.Left : temp1.Right;

            tempChild.Parent = temp1.Parent;

            if (temp1.Parent != Nil) {
                if (temp1 == temp1.Parent.Left) {
                    temp1.Parent.Left = tempChild;
                }
                else temp1.Parent.Right = tempChild;
            }
            else Root = tempChild;

            if (temp1 != target) {
                target.Value = temp1.Value;
            }

            if (temp1.Color == BLACK) {
                RebalanceRemove(tempChild);
            }

            Length--;

            return true;
        }

        public virtual T this[int i] { //TODO: expand
            get {
                if (i == 0) return Root.Value;

                return default;
            }
        }

        public virtual void TraverseInOrder(Node<T> current, Action<Node<T>> action) {
            if (current == null) return;

            TraverseInOrder(current.Left, action);

            action(current);

            TraverseInOrder(current.Right, action);
        }

        public virtual T Min() {
            var result = Root;
            while (result.Left != Nil) {
                result = result.Left;
            }

            return result.Value;
        }

        public virtual T Max() {
            var result = Root;
            while (result.Right != Nil) {
                result = result.Right;
            }

            return result.Value;
        }

        /// <summary>
        /// Display Tree
        /// </summary>
        public virtual void Print() {
            Console.WriteLine('{');

            TraverseInOrder(Root, (node) => Console.WriteLine(node));

            Console.WriteLine('}');
        }

        /// <summary>
        /// Makes the given node's subtree a child of its right child
        /// </summary>
        protected virtual void RotateLeft(Node<T> node) {
            var newParent = node.Right;

            node.Right = newParent.Left;

            if (newParent.Left != Nil) {
                newParent.Left.Parent = node;
            }

            newParent.Parent = node.Parent;

            if (node.Parent == Nil) {
                Root = newParent;
            }
            else if (node == node.Parent.Left) {
                node.Parent.Left = newParent;
            }
            else {
                node.Parent.Right = newParent;
            }

            newParent.Left = node;

            node.Parent = newParent;
        }

        /// <summary>
        /// Makes the given node's subtree a child of its right child
        /// </summary>
        protected virtual void RotateRight(Node<T> node) {
            var newParent = node.Left;

            node.Left = newParent.Right;

            if (newParent.Right != Nil) {
                newParent.Right.Parent = node;
            }

            newParent.Parent = node.Parent;

            if (node.Parent == Nil) {
                Root = newParent;
            }
            else if (node == node.Parent.Right) {
                node.Parent.Right = newParent;
            }
            else {
                node.Parent.Left = newParent;
            }

            newParent.Right = node;

            node.Parent = newParent;
        }

        protected virtual void RebalanceAdd(Node<T> redNode) {
            while (redNode.Parent.Color == RED) {
                if (redNode.Parent == redNode.Parent.Parent.Left) {
                    var uncle = redNode.Parent.Parent.Right;

                    if (uncle != null && uncle.Color == RED) {
                        redNode.Parent.Color = BLACK;

                        uncle.Color = BLACK;

                        redNode.Parent.Parent.Color = RED;

                        redNode = redNode.Parent.Parent;
                    }
                    else {
                        if (redNode == redNode.Parent.Right) {
                            redNode = redNode.Parent;

                            RotateLeft(redNode);
                        }

                        redNode.Parent.Color = BLACK;

                        redNode.Parent.Parent.Color = RED;

                        RotateRight(redNode.Parent.Parent);
                    }
                }
                else {
                    var aunt = redNode.Parent.Parent.Left;

                    if (aunt != null && aunt.Color == RED) {
                        redNode.Parent.Color = BLACK;

                        aunt.Color = BLACK;

                        redNode.Parent.Parent.Color = RED;

                        redNode = redNode.Parent.Parent;
                    }
                    else {
                        if (redNode == redNode.Parent.Left) {
                            redNode = redNode.Parent;

                            RotateRight(redNode);
                        }

                        redNode.Parent.Color = BLACK;

                        redNode.Parent.Parent.Color = RED;

                        RotateLeft(redNode.Parent.Parent);
                    }
                }
            }

            Root.Color = BLACK;
        }

        protected virtual void RebalanceRemove(Node<T> fixer) {
            while (fixer != Root && fixer.Color == BLACK) { //Red nodes never require further work
                if (fixer == fixer.Parent.Left) {
                    var brother = fixer.Parent.Right;

                    if (brother.Color == RED) {
                        brother.Color = BLACK;

                        fixer.Parent.Color = RED;

                        RotateLeft(fixer.Parent);

                        brother = fixer.Parent.Right;
                    }
                    if (brother.Left.Color == BLACK && brother.Right.Color == BLACK) {
                        brother.Color = RED;

                        fixer = fixer.Parent;
                    }
                    else {
                        if (brother.Right.Color == BLACK) {
                            brother.Left.Color = BLACK;

                            brother.Color = RED;

                            RotateRight(brother);

                            brother = fixer.Parent.Right;
                        }

                        brother.Color = fixer.Parent.Color;

                        fixer.Parent.Color = BLACK;

                        brother.Right.Color = BLACK;

                        RotateLeft(fixer.Parent);

                        fixer = Root;
                    }
                }
                else // mirror code from above with "right" and "left" exchanged
                {
                    var sister = fixer.Parent.Left;
                    if (sister?.Color == RED) {
                        sister.Color = BLACK;

                        fixer.Parent.Color = RED;

                        RotateRight(fixer.Parent);

                        sister = fixer.Parent.Left;
                    }
                    if (sister.Right.Color == BLACK && sister.Left.Color == BLACK) {
                        sister.Color = RED;

                        fixer = fixer.Parent;
                    }
                    else {
                        if (sister.Left.Color == BLACK) {
                            sister.Right.Color = BLACK;

                            sister.Color = RED;

                            RotateLeft(sister);

                            sister = fixer.Parent.Left;
                        }

                        sister.Color = fixer.Parent.Color;

                        fixer.Parent.Color = BLACK;

                        sister.Left.Color = BLACK;

                        RotateRight(fixer.Parent);

                        fixer = Root;
                    }
                }
            }

            fixer.Color = BLACK;
        }

        protected virtual Node<T> Next(Node<T> current) {
            if (current.Right != Nil) {
                return Min(current.Right);
            }

            var result = current.Parent;
            while (result != Nil && current == result.Right) {
                current = result;

                result = result.Parent;
            }

            return result;
        }

        protected virtual Node<T> Min(Node<T> node) {
            while (node.Left != Nil) {
                node = node.Left;
            }

            return node;
        }

        protected virtual bool Validate() {
            if (Root == Nil) return Length == 0;

            if (Root.Parent != Nil) return false;

            if (Root.Color == RED) return false;

            return Validate(Root).isValid;
        }

        protected virtual (bool isValid, int blackCount) Validate(Node<T> current) {
            if (current == Nil) return (true, 1);

            int blackCount;
            if (current.Color == RED) {
                blackCount = 0;

                if (current.Left != null && current.Left.Color == RED
                || current.Right != null && current.Right.Color == RED) {
                    return (false, -1);
                }

                if (current.Left.Color != BLACK || current.Right.Color != BLACK) {
                    return (false, -1);
                }
            }
            else {
                blackCount = 1;
            }

            (var leftValid, var leftCount) = Validate(current.Left);
            (var rightValid, var rightCount) = Validate(current.Right);

            if (!leftValid || !rightValid || leftCount != rightCount) Debugger.Break();

            return (leftValid && rightValid && leftCount == rightCount, leftCount + blackCount);
        }

        IStructure<T> IStructure<T>.Add(T item) {
            Add(item);

            return this;
        }

        public virtual RedBlackTree<T> Clear() {
            Root = Nil;

            return this;
        }

        public virtual T Peek() {
            return Root.Value; //Same element as the one returned by Pop()
        }

        public virtual T Pop() {
            var result = Root.Value;

            Remove(Root.Value); //TODO: test to find out what node is fastest to remove

            return result;
        }        

        public virtual RedBlackTree<T> Enqueue(T item) {
            return Add(item);
        }

        public virtual T Dequeue() {
            return Pop();
        }

        void ICollection<T>.Clear() {
            Clear();
        }

        public virtual void CopyTo(T[] array, int arrayIndex) {
            var increment = 0;
            foreach (var item in this) {
                array[arrayIndex + increment] = item;
            }
        }

        public virtual IEnumerator<T> GetEnumerator() {
            return new RedBlackEnumerator<T>(this);
        }

        public virtual void CopyTo(Array array, int index) {
            var indexable = (IList)array;
            var increment = 0;
            foreach (var item in this) {
                indexable[index + increment] = item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        void ICollection<T>.Add(T item) {
            Add(item);
        }

        IStructure<T> IStructure<T>.Clear() {
            return Clear();
        }

        IStructure<T> IStructure<T>.Enqueue(T item) {
            return Add(item);
        }

        public class RedBlackEnumerator<T> : IEnumerator<T> {
            protected Stack<RedBlackTree<T>.Node<T>> Stack;
            protected bool IsAscending;
            protected string Color;             // testing only, don't use in live system
            protected IComparable<T> ParentKey;     // testing only, don't use in live system
            protected RedBlackTree<T> Structure;

            public virtual T Current { get; set; }

            object IEnumerator.Current => Current;

            ///<summary>
            /// Create a new enumerator
            ///</summary>
            public RedBlackEnumerator(RedBlackTree<T> structure, bool isAscending = true) {
                Structure = structure;
                IsAscending = isAscending;

                Reset();
            }

            ///<summary>
            /// Advances the enumerator to the next element of the collection.
            ///</summary>
            ///<returns>true if the enumerator was successfully advanced to the next element, or false if the enumerator has passed the end of the collection</returns>
            public virtual bool MoveNext() {
                if (Stack.Count == 0) return false;

                var node = Stack.Peek();

                if (IsAscending) {
                    if (node.Right == Structure.Nil) {

                        var tn = Stack.Pop();

                        while (Stack.Any() && (Stack.Peek()).Right == tn)
                            tn = Stack.Pop();
                    }
                    else {
                        var tn = node.Right;
                        while (tn != Structure.Nil) {
                            Stack.Push(tn);
                            tn = tn.Left;
                        }
                    }
                }
                else {
                    if (node.Left == Structure.Nil) {
                        var tn = Stack.Pop();
                        while (Stack.Any() && (Stack.Peek()).Left == tn)
                            tn = Stack.Pop();
                    }
                    else {
                        var tn = node.Left;
                        while (tn != Structure.Nil) {
                            Stack.Push(tn);
                            tn = tn.Right;
                        }
                    }
                }

                // testing only
                if (node.Color == false) 
                    Color = "Red";
                else
                    Color = "Black";

                Current = node.Value;

                return true;
            }

            public virtual void Reset() {
                var node = Structure.Root;
                Stack = new Stack<RedBlackTree<T>.Node<T>>();

                if (IsAscending) {
                    while (node != Structure.Nil) {
                        Stack.Push(node);
                        node = node.Left;
                    }
                }
                else {
                    while (node != Structure.Nil) {
                        Stack.Push(node);
                        node = node.Right;
                    }
                }
            }

            public virtual void Dispose() {
                //should be a no-op; stack and tree reference will be garbage collected
            }
        }
    }
}
