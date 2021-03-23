using NUnit.Framework;
using System.Diagnostics;

namespace Fruberry.Tests {
    public class GridGraph<T> : Grid<T> {
        public GridGraph(int rows, int columns) : base(rows, columns) { }

        public Graph<T> Portals = new Graph<T>();
    }

    public class GridTests {
        [Test]
        public void Create() {
            var rect = new Grid<int>(1, 2) {
                [0, 0] = 1,
                [0, 1] = 2
            };

            Assert.AreEqual(1, rect[0, 0]);
            Assert.AreEqual(2, rect[0, 1]);

            Assert.AreEqual(rect.GetNeighbor(rect[0, 0], Direction.South), rect[0, 1]);
            Assert.AreEqual(rect.GetNeighbor(rect[0, 1], Direction.North), rect[0, 0]);

            var grid = new Grid<int>(3, 3);

            for (var i = 0; i < grid.Dimensions.Rows; i++) {
                for (var j = 0; j < grid.Dimensions.Columns; j++) {
                    grid[i, j] = i * grid.Dimensions.Rows + j + 1;
                }
            }

            Assert.True(grid.Length == 9);

            var topleft = grid[0, 0];
            var topmid = grid[1, 0];
            var topright = grid[2, 0];
            var midleft = grid[0, 1];
            var middle = grid[1, 1];
            var botright = grid[2, 2];
            var midright = grid[2, 1];
            var botmid = grid[1, 2];
            var botleft = grid[0, 2];

            Assert.True(topleft == 1);
            Assert.AreEqual(grid.GetNeighbor(topleft, Direction.North), default(int));
            Assert.AreEqual(grid.GetNeighbor(topleft, Direction.South), midleft);
            Assert.AreEqual(grid.GetNeighbor(topleft, Direction.East), topmid);
            Assert.AreEqual(grid.GetNeighbor(topleft, Direction.West), default(int));
            Assert.AreEqual(grid.GetNeighbor(topleft, Direction.Northeast), default(int));
            Assert.AreEqual(grid.GetNeighbor(topleft, Direction.Northwest), default(int));
            Assert.AreEqual(grid.GetNeighbor(topleft, Direction.Southwest), default(int));
            Assert.AreEqual(grid.GetNeighbor(topleft, Direction.Southeast), middle);

            Assert.True(botright == 9);
            Assert.AreEqual(grid.GetNeighbor(botright, Direction.North), midright);
            Assert.AreEqual(grid.GetNeighbor(botright, Direction.South), default(int));
            Assert.AreEqual(grid.GetNeighbor(botright, Direction.East), default(int));
            Assert.AreEqual(grid.GetNeighbor(botright, Direction.West), botmid);
            Assert.AreEqual(grid.GetNeighbor(botright, Direction.Northeast), default(int));
            Assert.AreEqual(grid.GetNeighbor(botright, Direction.Northwest), middle);
            Assert.AreEqual(grid.GetNeighbor(botright, Direction.Southwest), default(int));
            Assert.AreEqual(grid.GetNeighbor(botright, Direction.Southeast), default(int));

            Assert.True(middle == 5);
            Assert.AreEqual(grid.GetNeighbor(middle, Direction.North), topmid);
            Assert.AreEqual(grid.GetNeighbor(middle, Direction.South), botmid);
            Assert.AreEqual(grid.GetNeighbor(middle, Direction.East), midright);
            Assert.AreEqual(grid.GetNeighbor(middle, Direction.West), midleft);
            Assert.AreEqual(grid.GetNeighbor(middle, Direction.Northeast), topright);
            Assert.AreEqual(grid.GetNeighbor(middle, Direction.Northwest), topleft);
            Assert.AreEqual(grid.GetNeighbor(middle, Direction.Southwest), botleft);
            Assert.AreEqual(grid.GetNeighbor(middle, Direction.Southeast), botright);

            var middleNeighbors = grid.GetNeighbors(middle, orthogonalOnly: false, excludeDefaults: false);

            Assert.True(middleNeighbors.Count == 8);

            Check.Contains(middleNeighbors, topleft);
            Check.Contains(middleNeighbors, topmid);
            Check.Contains(middleNeighbors, topright);
            Check.Contains(middleNeighbors, midleft);
            Check.Contains(middleNeighbors, botright);
            Check.Contains(middleNeighbors, midright);
            Check.Contains(middleNeighbors, botleft);
            Check.Contains(middleNeighbors, botmid);

            middleNeighbors = grid.GetNeighbors(middle, orthogonalOnly: true, excludeDefaults: false);

            Assert.True(middleNeighbors.Count == 4);

            Check.Contains(middleNeighbors, topmid);
            Check.Contains(middleNeighbors, midleft);
            Check.Contains(middleNeighbors, midright);
            Check.Contains(middleNeighbors, botmid);

            var topleftNeighbors = grid.GetNeighbors(topleft, false, false);

            Assert.True(topleftNeighbors.Count == 8);

            Check.Contains(topleftNeighbors, topmid);
            Check.Contains(topleftNeighbors, midleft);
            Check.Contains(topleftNeighbors, middle);
            Check.Contains(topleftNeighbors, 0);

            topleftNeighbors = grid.GetNeighbors(topleft, false, true);

            Assert.True(topleftNeighbors.Count == 3);

            Check.Contains(topleftNeighbors, topmid);
            Check.Contains(topleftNeighbors, midleft);
            Check.Contains(topleftNeighbors, middle);

            topleftNeighbors = grid.GetNeighbors(topleft, true, true);

            Assert.True(topleftNeighbors.Count == 2);

            Check.Contains(topleftNeighbors, topmid);
            Check.Contains(topleftNeighbors, midleft);

            Assert.True(grid.GetNeighbor(middle, Direction.North) == topmid);
            Assert.True(grid.GetNeighbor(middle, Direction.Southeast) == botright);
        }

        [Test]
        public void CellsWhere() {
            var grid = new Grid<string>(3, 3) {
                [0, 0] = ".",
                [0, 1] = "Armory",
                [0, 2] = "Kitchen",
                [1, 0] = ".",
                [1, 1] = "Well",
                [1, 2] = "Treasure Room",
                [2, 0] = ".",
                [2, 1] = ".",
                [2, 2] = "."
            };

            var emptyRooms = grid.CellsWhere(_ => _ == ".");

            Assert.AreEqual(5, emptyRooms.Count);
            Check.Contains(emptyRooms, (0, 0));
            Check.Contains(emptyRooms, (1, 0));
            Check.Contains(emptyRooms, (2, 0));
            Check.Contains(emptyRooms, (2, 1));
            Check.Contains(emptyRooms, (2, 2));
        }

        [Test]
        public void Subclass() {
            var dungeon = new GridGraph<string>(3, 3) {
                [0, 0] = "Entryway",
                [0, 1] = "Armory",
                [0, 2] = "Kitchen",
                [1, 0] = "Storeroom",
                [1, 1] = "Well",
                [1, 2] = "Treasure Room",
                [2, 0] = "Throne Room",
                [2, 1] = "Great Hall",
                [2, 2] = "Billiards"
            };

            dungeon.Portals.Add("Throne Room", new[] { "Billiards" });
            dungeon.Portals.Add("Billiards", new[] { "Throne Room" });

            foreach (var room in dungeon) {
                if (room == "Throne Room") {
                    Assert.True(dungeon.Portals.GetNeighbors(room).Contains("Billiards"));
                }
                else if (room == "Billiards") {
                    Assert.True(dungeon.Portals.GetNeighbors(room).Contains("Throne Room"));
                }
                else {
                    Assert.False(dungeon.Portals.GetNeighbors(room).Contains("Billiards"));
                    Assert.False(dungeon.Portals.GetNeighbors(room).Contains("Throne Room"));
                }
            }
        }

        [Test]
        public void GetEdges() {
            var grid = new Grid<int>(3, 3);

            for (var i = 0; i < grid.Dimensions.Rows; i++) {
                for (var j = 0; j < grid.Dimensions.Columns; j++) {
                    grid[i, j] = i * grid.Dimensions.Rows + j + 1;
                }
            }

            var edges = grid.GetEdges(0, 0);
            Assert.AreEqual(5, edges.Count);
            Check.Contains(edges, Direction.North);
            Check.Contains(edges, Direction.Northeast);
            Check.Contains(edges, Direction.Northwest);
            Check.Contains(edges, Direction.West);
            Check.Contains(edges, Direction.Southwest);

            edges = grid.GetEdges(0, 1);
            Assert.AreEqual(3, edges.Count);
            Check.Contains(edges, Direction.Northwest);
            Check.Contains(edges, Direction.West);
            Check.Contains(edges, Direction.Southwest);

            edges = grid.GetEdges(0, 2);
            Assert.AreEqual(5, edges.Count);
            Check.Contains(edges, Direction.Northwest);
            Check.Contains(edges, Direction.West);
            Check.Contains(edges, Direction.Southwest);
            Check.Contains(edges, Direction.South);
            Check.Contains(edges, Direction.Southeast);

            edges = grid.GetEdges(1, 0);
            Assert.AreEqual(3, edges.Count);
            Check.Contains(edges, Direction.Northwest);
            Check.Contains(edges, Direction.North);
            Check.Contains(edges, Direction.Northeast);

            edges = grid.GetEdges(1, 1);
            Assert.AreEqual(0, edges.Count);

            edges = grid.GetEdges(1, 2);
            Check.Contains(edges, Direction.East);
            Check.Contains(edges, Direction.Northeast);
            Check.Contains(edges, Direction.Southeast);

            edges = grid.GetEdges(2, 0);
            Assert.AreEqual(5, edges.Count);
            Check.Contains(edges, Direction.Northwest);
            Check.Contains(edges, Direction.North);
            Check.Contains(edges, Direction.Northeast);
            Check.Contains(edges, Direction.East);
            Check.Contains(edges, Direction.Southeast);

            edges = grid.GetEdges(2, 1);
            Assert.AreEqual(3, edges.Count);
            Check.Contains(edges, Direction.East);
            Check.Contains(edges, Direction.Northeast);
            Check.Contains(edges, Direction.Southeast);

            edges = grid.GetEdges(2, 2);
            Assert.AreEqual(5, edges.Count);
            Check.Contains(edges, Direction.South);
            Check.Contains(edges, Direction.East);
            Check.Contains(edges, Direction.Southwest);
            Check.Contains(edges, Direction.Northeast);
            Check.Contains(edges, Direction.Southeast);
        }

        [Test]
        public void Move() {
            var grid = new Grid<int>(3, 3);

            for (var i = 0; i < grid.Dimensions.Rows; i++) {
                for (var j = 0; j < grid.Dimensions.Columns; j++) {
                    grid[i, j] = i * grid.Dimensions.Rows + j + 1;
                }
            }

            Assert.AreEqual((1, 0), grid.Move(0, 0, Direction.East));
            Assert.AreEqual((0, 1), grid.Move(0, 0, Direction.South));
            Assert.AreEqual((1, 1), grid.Move(0, 0, Direction.Southeast));

            Assert.AreEqual((1, 1), grid.Move(0, 1, Direction.East));
            Assert.AreEqual((0, 2), grid.Move(0, 1, Direction.South));
            Assert.AreEqual((1, 2), grid.Move(0, 1, Direction.Southeast));
            Assert.AreEqual((0, 0), grid.Move(0, 1, Direction.North));
            Assert.AreEqual((1, 0), grid.Move(0, 1, Direction.Northeast));

            Assert.AreEqual((1, 2), grid.Move(0, 2, Direction.East));
            Assert.AreEqual((0, 1), grid.Move(0, 2, Direction.North));
            Assert.AreEqual((1, 1), grid.Move(0, 2, Direction.Northeast));
        }
    }  
}
