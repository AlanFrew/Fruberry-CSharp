using System;

namespace Fruberry {
    public class BinomialHeap {
        public class Node {
            public int Data;
            public int Degree;
            public Node Child;
            public Node Sibling;
            public Node Parent;

            public Node(int key) {
                Data = key;
                Degree = 0;
            }
        };

        public BinomialHeap Add(Chain<Node> _head, int key) {
            Node temp = new Node(key);
            InsertTreeIntoHeap(_head, temp);

            return this;
        }

        public Node Peek(Chain<Node> _heap) {
            var it = _heap.Head;
            var temp = it.Value;
            while (it.Next != null) {
                if (it.Value.Data < temp.Data)
                    temp = it.Value;
                it = it.Next;
            }

            return temp;
        }

        public Node Pop(Chain<Node> _heap) {
            var new_heap = new Chain<Node>();
            var temp = Peek(_heap);
            var it = _heap.Head;

            while (it.Value != null) {
                if (it.Value != temp) {
                    new_heap.Add(it.Value);
                }
                it = it.Next;
            }
            var lo = PopFromTree(temp);

            new_heap = UnionBionomialHeaps(new_heap, lo);

            new_heap = Heapify(new_heap);

            return new_heap.First;
        }

        public void PrintTree(Node h) {
            while (h != null) {
                Console.WriteLine(h.Data + " ");
                PrintTree(h.Child);
                h = h.Sibling;
            }
        }

        // print function for binomial heap 
        public void PrintHeap(Chain<Node> _heap) {
            foreach (var item in _heap) {
                PrintTree(item.Value);
            }
        }

        public Chain<Node> UnionBionomialHeaps(Chain<Node> left, Chain<Node> right) {
            // _new to another binomial heap which contain 
            // new heap after merging l1 & l2 
            var _new = new Chain<Node>();
            //List<Node>::iterator it = l1.begin();
            //List<Node>::iterator ot = l2.begin();
            var it = left.Head;
            var ot = right.Head;
            while (it.Next != null && ot.Next != null) {
                // if D(l1) <= D(l2) 
                if (it.Value.Degree <= ot.Value.Degree) {
                    _new.Add(it.Value);
                    it = it.Next;
                }
                // if D(l1) > D(l2) 
                else {
                    _new.Add(ot.Value);
                    ot = ot.Next;
                }
            }

            while (it.Next != null) {
                _new.Add(it.Value);
                it = it.Next;
            }

            while (ot.Next != null) {
                _new.Add(ot.Value);
                ot = ot.Next;
            }

            return _new;
        }

        #region Private
        private Node MergeBinomialTrees(Node left, Node right) {
            if (left.Data > right.Data) {
                var temp = left;
                left = right;
                right = temp;
            }

            right.Parent = left;

            right.Sibling = left.Child;

            left.Child = right;

            left.Degree++;

            return left;
        }

        // adjust function rearranges the heap so that 
        // heap is in increasing order of degree and 
        // no two binomial trees have same degree in this heap 
        private Chain<Node> Heapify(Chain<Node> _heap) {
            if (_heap.Count == 0) return _heap;

            var it1 = _heap.Head;
            var it2 = _heap.Head;
            Chainlink<Node> it3;

            if (_heap.Count == 2) {
                it2 = it1;
                it2 = it2.Next;
                it3 = null;
            }
            else {
                it2 = it2.Next;
                it3 = it2;
                it3 = it3.Next;
            }
            while (it1.Next != null) {
                // if only one element remains to be processed 
                if (it2.Next == null) it1 = it1.Next;

                // If D(it1) < D(it2) i.e. merging of Binomial 
                // Tree pointed by it1 & it2 is not possible 
                // then move next in heap 
                else if (it1.Value.Degree < it2.Value.Degree) {
                    it1 = it1.Next;
                    it2 = it2.Next;
                    it3 = it3.Next;
                }

                // if D(it1),D(it2) & D(it3) are same i.e. 
                // degree of three consecutive Binomial Tree are same 
                // in heap 
                else if (it3.Next != null
                    && it1.Value.Degree == it2.Value.Degree
                    && it1.Value.Degree == it3.Value.Degree) {
                    it1 = it1.Next;
                    it2 = it2.Next;
                    it3 = it3.Next;
                }

                // if degree of two Binomial Tree are same in heap 
                else if (it1.Value.Degree == it2.Value.Degree) {
                    it1.Value = MergeBinomialTrees(it1.Value, it2.Value);
                    _heap.Remove(it2);
                    it2 = it2.Next;
                    it3 = it3.Next;
                }
            }
            return _heap;
        }

        private Chain<Node> InsertTreeIntoHeap(Chain<Node> _heap, Node tree) {
            var temp = new Chain<Node> { tree };

            temp = UnionBionomialHeaps(_heap, temp);

            return Heapify(temp);
        }

        /// <summary>
        /// Remove the minimum element from the heap
        /// </summary>
        /// <returns>New minimum element</returns>
        private Chain<Node> PopFromTree(Node binomialTree) {
            var heap = new Chain<Node>();
            var temp = binomialTree.Child;
            Node minElement;

            // making a binomial heap from Binomial Tree 
            while (temp != null) {
                minElement = temp;
                temp = temp.Sibling;
                minElement.Sibling = null;
                heap.AddFirst(minElement);
            }
            return heap;
        }
        #endregion
    }
}
