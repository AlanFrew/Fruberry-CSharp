using System;
using System.Collections.Generic;
using System.Text;

namespace Fruberry {
    public class Tree<T> where T : IComparable<T> {
        public enum Color {
            Red = 0, Black = 1
        }

        public enum Direction {
            Left,
            Right
        }

        public class Node<T> where T : IComparable<T> {
            public T data;
            public Node<T> left;
            public Node<T> right;
            public Color color = Color.Black;
            public Node(T data) : this(data, null, null) { }
            public Node(T data, Node<T> left, Node<T> right) {
                this.data = data;
                this.left = left;
                this.right = right;
            }
        }

        protected Node<T> root;
        protected Node<T> freshNode;
        protected Node<T> currentNode;
        public int Count;

        protected Tree() {
            freshNode = new Node<T>(default);
            freshNode.left = freshNode.right = freshNode;
            root = new Node<T>(default);
        }
        protected int Compare(T item, Node<T> node) {
            if (node != root) return item.CompareTo(node.data);
            else return 1;
        }

        public bool Contains(T data) {
            return Search(data) != null;
        }
        public T Search(T data) {
            freshNode.data = data;
            currentNode = root.right;
            while (true) {
                if (Compare(data, currentNode) < 0) currentNode = currentNode.left;
                else if (Compare(data, currentNode) > 0) currentNode = currentNode.right;
                else if (currentNode != freshNode) return currentNode.data;
                else return default;
            }
        }
        protected void Display() {
            Display(root.right);
        }
        protected void Display(Node<T> temp) {
            if (temp != freshNode) {
                Display(temp.left);
                Console.WriteLine(temp.data);
                Display(temp.right);
            }
        }
    }

    public sealed class rbt<T> : Tree<T> where T : IComparable<T>{
        private Color Black = Color.Black;
        private Color Red = Color.Red;
        private Node<T> parentNode;
        private Node<T> grandParentNode;
        private Node<T> tempNode;

        public void Add(T item) {
            currentNode = parentNode = grandParentNode = root;
            freshNode.data = item;
            int returnedValue;
            while (Compare(item, currentNode) != 0) {
                tempNode = grandParentNode;
                grandParentNode = parentNode;
                parentNode = currentNode;
                returnedValue = Compare(item, currentNode);
                if (returnedValue < 0) currentNode = currentNode.left;
                else currentNode = currentNode.right;

                if (currentNode.left.color == Color.Red && currentNode.right.color == Color.Red) ReArrange(item);
            }
            if (currentNode == freshNode) {
                currentNode = new Node<T>(item, freshNode, freshNode);
                if (Compare(item, parentNode) < 0) parentNode.left = currentNode;
                else parentNode.right = currentNode;
                ReArrange(item);
            }

            Count++;
        }

        private void ReArrange(T item) {
            currentNode.color = Red;
            currentNode.left.color = Color.Black;
            currentNode.right.color = Color.Black;
            if (parentNode.color == Color.Red) {
                grandParentNode.color = Color.Red;
                bool compareWithGrandParentNode = (Compare(item, grandParentNode) < 0);
                bool compareWithParentNode = (Compare(item, parentNode) < 0);
                if (compareWithGrandParentNode != compareWithParentNode) parentNode = Rotate(item, grandParentNode);
                currentNode = Rotate(item, tempNode);
                currentNode.color = Black;
            }
            root.right.color = Color.Black;
        }

        private Node<T> Rotate(T item, Node<T> parentNode) {
            int value;
            if (Compare(item, parentNode) < 0) {
                value = Compare(item, parentNode.left);
                if (value < 0) parentNode.left = Rotate(parentNode.left, Direction.Left);
                else parentNode.left = Rotate(parentNode.left, Direction.Right);
                return parentNode.left;
            }
            else {
                value = Compare(item, parentNode.right);
                if (value < 0) parentNode.right = Rotate(parentNode.right, Direction.Left);
                else parentNode.right = Rotate(parentNode.right, Direction.Right);
                return parentNode.right;
            }
        }

        private Node<T> Rotate(Node<T> node, Direction direction) {
            Node<T> tempNode;
            if (direction == Direction.Left) {
                tempNode = node.left;
                node.left = tempNode.right;
                tempNode.right = node;
                return tempNode;
            }
            else {
                tempNode = node.right;
                node.right = tempNode.left;
                tempNode.left = node;
                return tempNode;
            }
        }
    }
}
