using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fruberry {
    public class Grid<T> : IStructure<T> {
        public enum Direction { North = 1, South, East, West, Northeast, Southeast, Southwest, Northwest }
        public enum OrthogonalDirection { North, South, East, West };

        static private int _directionCount = Enum.GetValues(typeof(Direction)).Length;
        static private int _orthogonalDirectionCount = Enum.GetValues(typeof(OrthogonalDirection)).Length;

        public T[,] Cells;

        public Dictionary<T, (int, int)> Map = new Dictionary<T, (int, int)>();

        public IList<Prefer> Constraints => new[] { Prefer.AllowDupes };

        public bool IsReadOnly => false;

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public int Length { get => Cells?.Length ?? 0; protected set { } } //TODO: Fix when dictionary is replaced

        public int Count => Length;

        int IStructure<T>.Count() { return Length; }

        int IStructure<T>.Length {
            get => Length;
            set => Length = value;
        }

        public Grid(int rows, int columns) {
            Cells = new T[rows, columns];
        }

        public T this[int row, int column] {
            get {
                return Cells[row, column];
            }
            set {
                Map.Remove(Cells[row, column]);

                Map[value] = (row, column);
                Cells[row, column] = value;
            }
        }

        public T GetNeigbor(T item, Direction direction) {
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

        public IList<T> GetNeighbors(T item, bool orthogonalOnly = true, bool excludeDefaults = true) {
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

        public IStructure<T> Add(T item) {
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

        public IStructure<T> Clear() {
            throw new NotImplementedException();
        }

        public bool Contains(T item) {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        public T Dequeue() {
            throw new NotImplementedException();
        }

        public IStructure<T> Enqueue(T item) {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() {
            throw new NotImplementedException();
        }

        public T Peek() {
            throw new NotImplementedException();
        }

        public T Pop() {
            throw new NotImplementedException();
        }

        public bool Remove(T item) {
            throw new NotImplementedException();
        }

        void ICollection<T>.Add(T item) {
            throw new NotImplementedException();
        }

        void ICollection<T>.Clear() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}