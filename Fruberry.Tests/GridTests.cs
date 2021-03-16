using NUnit.Framework;

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

            Assert.AreEqual(rect.GetNeigbor(rect[0, 0], Grid<int>.Direction.South), rect[0,1]);
            Assert.AreEqual(rect.GetNeigbor(rect[0, 1], Grid<int>.Direction.North), rect[0, 0]);

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
            Assert.AreEqual(grid.GetNeigbor(topleft, Grid<int>.Direction.North), default(int));
            Assert.AreEqual(grid.GetNeigbor(topleft, Grid<int>.Direction.South), midleft);
            Assert.AreEqual(grid.GetNeigbor(topleft, Grid<int>.Direction.East), topmid);
            Assert.AreEqual(grid.GetNeigbor(topleft, Grid<int>.Direction.West), default(int));
            Assert.AreEqual(grid.GetNeigbor(topleft, Grid<int>.Direction.Northeast), default(int));
            Assert.AreEqual(grid.GetNeigbor(topleft, Grid<int>.Direction.Northwest), default(int));
            Assert.AreEqual(grid.GetNeigbor(topleft, Grid<int>.Direction.Southwest), default(int));
            Assert.AreEqual(grid.GetNeigbor(topleft, Grid<int>.Direction.Southeast), middle);

            Assert.True(botright == 9);
            Assert.AreEqual(grid.GetNeigbor(botright, Grid<int>.Direction.North), midright);
            Assert.AreEqual(grid.GetNeigbor(botright, Grid<int>.Direction.South), default(int));
            Assert.AreEqual(grid.GetNeigbor(botright, Grid<int>.Direction.East), default(int));
            Assert.AreEqual(grid.GetNeigbor(botright, Grid<int>.Direction.West), botmid);
            Assert.AreEqual(grid.GetNeigbor(botright, Grid<int>.Direction.Northeast), default(int));
            Assert.AreEqual(grid.GetNeigbor(botright, Grid<int>.Direction.Northwest), middle);
            Assert.AreEqual(grid.GetNeigbor(botright, Grid<int>.Direction.Southwest), default(int));
            Assert.AreEqual(grid.GetNeigbor(botright, Grid<int>.Direction.Southeast), default(int));

            Assert.True(middle == 5);
            Assert.AreEqual(grid.GetNeigbor(middle, Grid<int>.Direction.North), topmid);
            Assert.AreEqual(grid.GetNeigbor(middle, Grid<int>.Direction.South), botmid);
            Assert.AreEqual(grid.GetNeigbor(middle, Grid<int>.Direction.East), midright);
            Assert.AreEqual(grid.GetNeigbor(middle, Grid<int>.Direction.West), midleft);
            Assert.AreEqual(grid.GetNeigbor(middle, Grid<int>.Direction.Northeast), topright);
            Assert.AreEqual(grid.GetNeigbor(middle, Grid<int>.Direction.Northwest), topleft);
            Assert.AreEqual(grid.GetNeigbor(middle, Grid<int>.Direction.Southwest), botleft);
            Assert.AreEqual(grid.GetNeigbor(middle, Grid<int>.Direction.Southeast), botright);

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
                [0, 0] = "Throne Room",
                [0, 0] = "Great Hall",
                [0, 0] = "Billiards"
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
    }  
}
