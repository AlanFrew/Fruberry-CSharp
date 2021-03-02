using System.Collections.Generic;
using NUnit.Framework;

namespace Fruberry.Tests {
    [TestFixture]
    public class GraphTests {
        [Test]
        public void Example() {
            var graph = new Graph<string> {
                "home",
                "work",
                "Gregg's place"
            };

            graph.AddNeighbors("home", new List<string> { "work" });

            Assert.True(graph.GetNeighbors("home").Contains("work"));
            Assert.False(graph.GetNeighbors("home").Contains("Gregg's place"));

            graph.AddNeighbors("home", new List<string> { "Gregg's place" });

            Assert.True(graph.GetNeighbors("home").Contains("work"));
            Assert.True(graph.GetNeighbors("home").Contains("Gregg's place"));
        }
    }
}
