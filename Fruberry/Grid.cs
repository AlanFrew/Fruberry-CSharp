using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fruberry {
    public class Grid<T> : IStructure<T> {
        protected static int _directionCount = Enum.GetValues(typeof(Direction)).Length;
        protected static int _orthogonalDirectionCount = Enum.GetValues(typeof(OrthogonalDirection)).Length;
        protected T[,] Cells;
        public static ExceptionBehavior ExceptionBehavior = ExceptionBehavior.BestEffort;

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

        public virtual (int Rows, int Columns) Dimensions => (Cells.GetLength(0), Cells.GetLength(1));

        public Grid(int rows, int columns) {
            Cells = new T[rows, columns];
        }

        public virtual T this[int row, int column] {
            get {
                if (row < 0) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(row), row, "Row must be nonnegative");
                        case ExceptionBehavior.Abort: return default;
                        default: row = 0; break;
                    }
                }
                if (row >= Dimensions.Rows) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(row), row, "Row must be less than Dimenions.Rows");
                        case ExceptionBehavior.Abort: return default;
                        default:
                            if (row == 0) return default;
                            row = Dimensions.Rows - 1;
                            break;
                    }
                }
                if (column < 0) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(column), column, "Column must be nonnegative");
                        case ExceptionBehavior.Abort: return default;
                        default: column = 0; break;
                    }
                }
                if (column >= Dimensions.Columns) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(column), column, "Column must be less than Dimenions.Columns");
                        case ExceptionBehavior.Abort: return default;
                        default:
                            if (column == 0) return default;
                            column = Dimensions.Columns - 1;
                            break;
                    }
                }

                return Cells[row, column];
            }
            set {
                if (row < 0) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(row), row, "Row must be nonnegative");
                        case ExceptionBehavior.Abort: return;
                        default: row = 0; break;
                    }
                }
                if (row >= Dimensions.Rows) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(row), row, "Row must be less than Dimensions.Rows");
                        case ExceptionBehavior.Abort: return;
                        default: row = Dimensions.Rows - 1; break;
                    }
                }
                if (column < 0) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(column), column, "Column must be nonnegative");
                        case ExceptionBehavior.Abort: return;
                        default: column = 0; break;
                    }
                }
                if (column >= Length) {
                    switch (ExceptionBehavior) {
                        case ExceptionBehavior.Throw: throw new ArgumentOutOfRangeException(nameof(column), column, "Column must be less than Dimensions.Columns");
                        case ExceptionBehavior.Abort: return;
                        default:
                            if (column == 0) return;
                            column = Length - 1;
                            break;
                    }
                }

                var oldValue = Cells[row, column];
                if (oldValue != null) Map.Remove(oldValue);

                Map[value] = (row, column);

                //if (row < 0 || column < 0 || row >= Dimensions.Rows || column >= Dimensions.Columns) {
                //    var rowSpan = Math.Max(Dimensions.Rows - row, Dimensions.Rows);
                //    var columnSpan = Math.Max(Dimensions.Columns - row, Dimensions.Columns);

                //    var rowDrift = 0;
                //    if (row < 0) rowDrift = row;
                //    //if (rowSpan > Dimensions.Rows) rowDrift = rowSpan - Dimensions.Rows;

                //    var columnDrift = 0;
                //    if (column < 0) columnDrift = column;
                //    //if (columnSpan > Dimensions.Columns) columnDrift = columnSpan - Dimensions.Columns;

                //    Resize(rowSpan, columnSpan, rowDrift, columnDrift);
                //}

                Cells[row, column] = value;
            }
        }

        public Grid<T> Resize(int rows, int columns, int rowDrift = 0, int columnDrift = 0) {
            var newCells = new T[rows, columns];

            for (var i = 0; i < Dimensions.Rows; i++) {
                for (var j = 0; j < Dimensions.Columns; j++) {
                    newCells[i + rowDrift, j + columnDrift] = Cells[i, j];
                }
            }

            Cells = newCells;

            return this;
        }

        public (int Row, int Column) SplitIndex(int index) {
            return (index / Dimensions.Rows, index % Dimensions.Columns);
        }

        public bool IsEdge(T item) {
            (var row, var column) = Map[item];

            return (row == 0 || column == 0 || row == Dimensions.Rows || column == Dimensions.Columns);
        }

        public IList<(int, int)> CellsWhere(Func<T, bool> selector) {
            if (selector == null) {
                switch (ExceptionBehavior) {
                    case ExceptionBehavior.Throw: throw new ArgumentNullException(nameof(selector));
                    default: return new List<(int, int)>();
                }
            }

            var result = new List<(int, int)>(Length);

            if (selector == null) return result;

            for(var i = 0; i < Dimensions.Rows; i++) {
                for (var j = 0; j < Dimensions.Columns; j++) {
                    if (selector(this[i, j])) result.Add((i, j));
                }
            }

            return result;
        }

        //public IList<(int, int)> CellsWhere(Func<int, int, bool> selector) {
        //    var result = new List<(int, int)>(Length);

        //    if (selector == null) return result;

        //    for (var i = 0; i < Dimensions.Rows; i++) {
        //        for (var j = 0; j < Dimensions.Columns; j++) {
        //            if (selector(this[i, j])) result.Add((i, j));
        //        }
        //    }

        //    return result;
        //}

        public List<Direction> GetEdges(T item) {
            if (Map.ContainsKey(item) == false) return new List<Direction>();

            var coords = Map[item];

            return (GetEdges(coords.Item1, coords.Item2));
        }

        public List<Direction> GetEdges(int row, int column) {
            var result = new List<Direction>();

            if (row == Dimensions.Columns - 1) result.Add(Direction.East);

            if (row == Dimensions.Columns - 1 || column == 0) result.Add(Direction.Northeast);

            if (column == 0) result.Add(Direction.North);

            if (row == 0 || column == 0) result.Add(Direction.Northwest);

            if (row == 0) result.Add(Direction.West);

            if (row == 0 || column == Dimensions.Rows - 1) result.Add(Direction.Southwest);

            if (column == Dimensions.Rows - 1) result.Add(Direction.South);

            if (row == Dimensions.Columns - 1 || column == Dimensions.Rows - 1) result.Add(Direction.Southeast);

            return result;
        }

        public List<Direction> GetOpenNeighbors(T item) {
            var result = new List<Direction>();

            foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
                if (GetEdges(item).Contains(direction)) continue;

                var (Row, Column) = Move(item, direction);

                if (Row < 0 || Column < 0) continue;

                if (EqualityComparer<T>.Default.Equals(Cells[Row, Column], default)) {
                    result.Add(direction);
                }
            }

            return result;
        }

        public virtual (int Row, int Column) Move(T item, Direction direction, int distance = 1) {
            if (Map.ContainsKey(item) == false) return (-1, -1);

            (var startRow, var startColumn) = Map[item];

            return Move(startRow, startColumn, direction, distance);
        }

        public virtual (int Row, int Column) Move(int startRow, int startColumn, Direction direction, int distance = 1) {
            return direction switch {
                Direction.North => (startRow, startColumn - distance),
                Direction.Northeast => (startRow + distance, startColumn - distance),
                Direction.East => (startRow + distance, startColumn),
                Direction.Southeast => (startRow + distance, startColumn + distance),
                Direction.South => (startRow, startColumn + distance),
                Direction.Southwest => (startRow - distance, startColumn + distance),
                Direction.West => (startRow - distance, startColumn),
                Direction.Northwest => (startRow - distance, startColumn - distance),
                _ => (startRow, startColumn),
            };
        }

        public virtual T GetNeighbor(T item, Direction direction) {
            if (Map.ContainsKey(item) == false) return default;

            var coords = Map[item];

            return GetNeighbor(coords.Item1, coords.Item2, direction);
        }

        public virtual T GetNeighbor(int row, int column, Direction direction) {
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

        public virtual IList<T> GetNeighbors(int row, int column, bool orthogonalOnly = true, bool excludeDefaults = true, Func<T, bool> selector = null) {
            if (excludeDefaults == false) {
                var index = 0;

                if (orthogonalOnly) {
                    var result = new T[_orthogonalDirectionCount];
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        if ((int)direction <= 4) {
                            var neighbor = GetNeighbor(row, column, direction);

                            if (selector == null || selector(neighbor)) {
                                result[index] = neighbor;
                            }

                            index++;
                        }
                    }

                    return result;
                }
                else {
                    var result = new T[_directionCount];
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        var neighbor = GetNeighbor(row, column, direction);

                        if (selector == null || selector(neighbor)) {
                            result[index] = neighbor;
                        }

                        index++;
                    }

                    return result;
                }

            }
            else {
                var result = new List<T>();

                if (orthogonalOnly) {
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        var neighbor = GetNeighbor(row, column, direction);

                        if ((selector == null || selector(neighbor))
                        && (int)direction <= 4
                        && neighbor != null
                        && !neighbor.Equals(default(T))) {
                            result.Add(neighbor);
                        }
                    }
                }
                else {
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        var neighbor = GetNeighbor(row, column, direction);

                        if ((selector == null || selector(neighbor))
                        && neighbor != null
                        && !neighbor.Equals(default(T))) {
                            result.Add(neighbor);
                        }
                    }
                }

                return result;
            }
        }

        public virtual IList<T> GetNeighbors(T item, bool orthogonalOnly = true, bool excludeDefaults = true, Func<T, bool> selector = null) {
            if (excludeDefaults == false) {
                var index = 0;

                if (orthogonalOnly) {
                    var result = new T[_orthogonalDirectionCount];
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        if ((int)direction <= 4) {
                            var neighbor = GetNeighbor(item, direction);

                            if (selector == null || selector(neighbor)) {
                                result[index] = neighbor;
                            }

                            index++;
                        }
                    }

                    return result;
                }
                else {
                    var result = new T[_directionCount];
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        var neighbor = GetNeighbor(item, direction);

                        if (selector == null || selector(neighbor)) {
                            result[index] = neighbor;
                        }

                        index++;
                    }

                    return result;
                }

            }
            else {
                var result = new List<T>();

                if (orthogonalOnly) {
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        var neighbor = GetNeighbor(item, direction);

                        if ((selector == null || selector(neighbor))
                        && (int)direction <= 4
                        && neighbor != null
                        && !neighbor.Equals(default(T))) {
                            result.Add(neighbor);
                        }
                    }
                }
                else {
                    foreach (var direction in Enum.GetValues(typeof(Direction)).Cast<Direction>()) {
                        var neighbor = GetNeighbor(item, direction);

                        if ((selector == null || selector(neighbor))
                        && neighbor != null
                        && !neighbor.Equals(default(T))) {
                            result.Add(neighbor);
                        }
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
            for (var i = 0; i < Dimensions.Rows; i++) {
                for (var j = 0; j < Dimensions.Columns; j++) {
                    array[arrayIndex] = this[i, j];
                    arrayIndex++;
                }
            }
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
            public int index;

            T IEnumerator<T>.Current => Current; //implements IEnumerable<T>
            object IEnumerator.Current => Current; //implements IEnumerable and used in foreach loop when cast to IEnumerable
            public T Current => (T)Subenumerator.Current; //used in foreach loop when typed as Chain<T>

            public Enumerator(Grid<T> collection) {
                Subenumerator = collection.Cells.GetEnumerator();
                //Current = isValid ? (T)Subenumerator.Current : default;

                Collection = collection;
                index = -1;

                //var isValid = MoveNext();
            }

            public bool MoveNext() {
                var result = Subenumerator.MoveNext();

                if (result) index++;

                return result;
            }

            public void Reset() {
                Subenumerator.Reset();
            }

            public void Dispose() { }
        }

        public override string ToString() {
            var result = new StringBuilder();

            for (var i = 0; i < Dimensions.Rows; i++) {
                for (var j = 0; j < Dimensions.Columns; j++) {
                    result.Append($"{this[j, i]} ");
                }

                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }
    }
}