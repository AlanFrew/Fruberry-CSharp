using System.Collections.Generic;
using NUnit.Framework;

namespace Fruberry.Tests {
    public class LinqTests {
        [Test]
        public void Contains() {
            IEnumerable<IEnumerable<char>> source = new List<IEnumerable<char>> {
                new List<char> {'A', 'l', 'a', 'n' },
                new List<char> {'F', 'r', 'e', 'w' }
            };

            Assert.True(source.Contains("Alan"));
            Assert.True(source.Contains("Frew"));

            //Assert.True(source.Contains(new[] { 'A', 'l', 'a', 'n'})));

            Assert.False(source.Contains("Al"));
            Assert.False(source.Contains(""));
            Assert.False(source.Contains("AlanFrew"));

            var source2 = new List<IEnumerable<char>> {
                new List<char> {'A', 'l', 'a', 'n' }
            };

            //Assert.True(source2.Contains("Alan"));
            Assert.True(Linq.Contains(source2, "Alan"));

            var source3 = new List<List<char>> {
                new List<char> {'A', 'l', 'a', 'n' }
            };

            Assert.True(source3.Contains("Alan"));
            Assert.True(Linq.Contains(source3, "Alan"));

            IEnumerable<List<char>> source4 = new List<List<char>> {
                new List<char> {'A', 'l', 'a', 'n' }
            };

            Assert.True(source4.Contains("Alan"));
            Assert.True(Linq.Contains(source4, "Alan"));
        }
    }
}
