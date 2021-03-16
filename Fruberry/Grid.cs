using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fruberry {
    public class Grid<T> : IStructure<T> {
        public enum Direction { North = 1, South, East, West, Northeast, Southeast, Southwest, Northwest }
        public enum OrthogonalDirection { North, South, East, West };

        static protected int _directionCount = Enum.GetValues(typeof(Direction)).Length;
        static protected int _orthogonalDirectionCount = Enum.GetValues(typeof(OrthogonalDirection)).Length;

        protected T[,] Cells;

        public Dictionary<T, (int, int)> Map = new Dictionary<T, (int, int)>();

        public virtual IList<Prefer> Constraints => new[] { Prefer.AllowDupes };

        public virtual bool IsReadOnly => false;

        public virtual bool IsSynchronized => false;

        public virtual object SyncRoot => null;

        public virtual int Length { get => Cells?.Length ?? 0; protected set { } } //TODO: Fix when dictionary is replaced

        public virtual int Count => Length;

        int IStructure<T>.Count() { return Length; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        public virtual (int Rows, int Columns) Dimensions => (Cells.GetLength(1), Cells.GetLength(0));

        public Grid(int rows, int columns) {
            Cells = new T[rows, columns];
        }

        public virtual T this[int row, int column] {
            get {
                return Cells[row, column];
            }
            set {
                var oldValue = Cells[row, column];
                if (oldValue != null) Map.Remove(oldValue);

                Map[value] = (row, column);

                Cells[row, column] = value;
            }
        }

        public virtual T GetNeigbor(T item, Direction direction) {
            (var row, var column) = Map[item];

            switch (direction) {
                case Direction.North:
                    if (column > 0) return Cells[row, column - 1];

                    return default;
                case Direction.South:
                    if (Cells.GetLength(1) > column + 1) return Cells[row, column + 1];

                    return default;
                case Direction.East:
                    if (Cells.GetLength(0) > row + 1) return Cells[row + 1, column];

                    return default;
                case Direction.West:
                    if (row > 0) return Cells[row - 1, column];

                    return default;
                case Direction.Northeast:
                    if (column > 0 && Cells.GetLength(1) > row + 1) return Cells[row + 1, column - 1];

                    return default;
                case Direction.Southeast:
                    if (Cells.GetLength(0) > row + 1 && Cells.GetLength(1) > column + 1) return Cells[row + 1, column + 1];

                    return default;
                case Direction.Southwest:
                    if (Cells.GetLength(1) > column + 1 && row > 0) return Cells[row - 1, column + 1];

                    return default;
                case Direction.Northwest:
                    if (row > 0 && column > 0) return Cells[row - 1, column - 1];

                    return default;
                default:
                    return default;
            }
        }

        public virtual IList<T> GetNeighbors(T item, bool orthogonalOnly = true, bool excludeDefaults = true) {
            if (excludeDefaults == false) {
                var index = 0;

                if (orthogonalOnly) {
                    var result = new T[_orthogonalDirectionCount];
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        if ((int)direction <= 4) {
                            result[index] = GetNeigbor(item, direction);

                            index++;
                        }
                    }

                    return result;
                }
                else {
                    var result = new T[_directionCount];
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        result[index] = GetNeigbor(item, direction);

                        index++;
                    }

                    return result;
                }

            }
            else {
                var result = new List<T>();

                if (orthogonalOnly) {
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        var neighbor = GetNeigbor(item, direction);

                        if ((int)direction <= 4 && neighbor != null && !neighbor.Equals(default(T))) result.Add(neighbor);
                    }
                }
                else {
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        var neighbor = GetNeigbor(item, direction);

                        if (neighbor != null && !neighbor.Equals(default(T))) result.Add(neighbor);
                    }
                }

                return result;
            }

        }

        public virtual (int row, int col) ByValue(T item) {
            return Map[item];
        }

        public virtual IStructure<T> Add(T item) {
            for (var i = 0; i < Cells.GetLength(0); i++) {
                for (var j = 0; j < Cells.GetLength(1); j++) {
                    if (Cells[i, j] == null || Cells[i, j].Equals(default(T))) {
                        Cells[i, j] = item;
                        return this;
                    }
                }
            }

            return this;
        }

        public virtual IStructure<T> Clear() {
            throw new NotImplementedException();
        }

        public virtual bool Contains(T item) {
            throw new NotImplementedException();
        }

        public virtual void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public virtual void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        public virtual T Dequeue() {
            throw new NotImplementedException();
        }

        public virtual IStructure<T> Enqueue(T item) {
            throw new NotImplementedException();
        }

        public virtual IEnumerator<T> GetEnumerator() {
            return new Enumerator<T>(this);
        }

        public virtual T Peek() {
            throw new NotImplementedException();
        }

        public virtual T Pop() {
            throw new NotImplementedException();
        }

        public virtual bool Remove(T item) {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item) {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public struct Enumerator<T> : IEnumerator<T> {
            private IEnumerator Subenumerator;
            private Grid<T> Collection;

            T IEnumerator<T>.Current => Current; //implements IEnumerable<T>
            object IEnumerator.Current => Current; //implements IEnumerable and used in foreach loop when cast to IEnumerable
            public T Current { get; private set; } //used in foreach loop when typed as Chain<T>

            public Enumerator(Grid<T> collection) {
                Subenumerator = collection.Cells.GetEnumerator();
                var isValid = Subenumerator.MoveNext();

                Current = isValid ? (T)Subenumerator.Current : default;

                Collection = collection;
            }

            public bool MoveNext() {
                return Subenumerator.MoveNext();
            }

            public void Reset() {
                Subenumerator.Reset();
            }

            public void Dispose() { }
        }
    }
}