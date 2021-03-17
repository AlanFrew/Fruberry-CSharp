using System;
using System.Linq;
using System.Diagnostics;

namespace Fruberry.GridTester {
    //West = -column
    //East = +column
    //North = -row
    //South = +row
    class Program {
        static void Main(string[] args) {
            var (rows, columns) = (29, 29);
           
            var random = new Random();

            while (true) {
                var roomCount = 30;

                var dungeon = new Grid<char>(rows, columns);

                for (var i = 0; i < dungeon.Dimensions.Rows; i++) {
                    for (var j = 0; j < dungeon.Dimensions.Columns; j++) {
                        dungeon[i, j] = '.';
                    }
                }

                dungeon[rows / 2, columns / 2] = 'S';

                var directions = new List<Grid<char>.Direction>((Grid<char>.Direction[])Enum.GetValues(typeof(Grid<char>.OrthogonalDirection)));

                while (roomCount > 0) {
                    var perimeter = dungeon.CellsWhere(room => room != '.')
                        .Where((coords) => dungeon.GetNeighbors(coords.Item1, coords.Item2, selector: space => space == '.')
                        .Count() >= 3);

                    if (perimeter.None()) {
                        //Console.Write(dungeon);
                        //Debugger.Break();

                        perimeter = dungeon.CellsWhere(room => room != '.')
                         .Where((coords) => dungeon.GetNeighbors(coords.Item1, coords.Item2, selector: space => space == '.')
                         .Count() >= 2);
                    }

                    var (startRow, startColumn) = perimeter.ElementAt(random.Next(perimeter.Count()));

                    var direction = directions
                        .Where(dir => dungeon.GetEdges(startRow, startColumn).Contains(dir) == false)
                        .Where(dir => dungeon.GetNeighbor(startRow, startColumn, dir) == '.')
                        .Random();

                    var (Row, Column) = dungeon.Move(startRow, startColumn, direction);

                    if (dungeon[Row, Column] != '.') Debugger.Break();

                    if (Math.Abs(Row - startRow) + Math.Abs(Column - startColumn) > 1) Debugger.Break();

                    if ((Row, Column) == (1, 0)) Debugger.Break();

                    dungeon[Row, Column] = 'R';

                    roomCount--;
                }

                Console.Write(dungeon);

                Console.ReadLine();
            }
        }
    }
}
