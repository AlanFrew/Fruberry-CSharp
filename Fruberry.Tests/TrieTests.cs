using System.Linq;
using NUnit.Framework;

namespace Fruberry.Tests {
    public class TrieTests {
        [Test]
        public void Test() {
            var rootValue = default(char);

            var trie = new Trie<char>();

            Assert.True(trie.Count() == 0);
            Assert.True(trie.Count == 0);

            Assert.False(trie.IsReadOnly);

            Assert.False(trie.ContainsExact("c"));

            trie.Add("better");
            trie.Add("butter");
            trie.Add("bet");
            trie.Add("butt");

            Assert.True(trie.Count() == 4);
            Assert.True(trie.Count == 4);

            Assert.False(trie.ContainsExact("b"));

            Assert.True(trie.ContainsExact("better"));
            Assert.True(trie.ContainsExact("butter"));
            Assert.True(trie.ContainsExact("bet"));
            Assert.True(trie.ContainsExact("butt"));
            Assert.False(trie.ContainsExact("be"));

            Assert.True(trie.Prefix("better").Value != rootValue);
            Assert.True(trie.Prefix("butter").Value != rootValue);
            Assert.True(trie.Prefix("bet").Value != rootValue);
            Assert.True(trie.Prefix("butt").Value != rootValue);
            Assert.True(trie.Prefix("be").Value != rootValue);
            Assert.True(trie.Prefix("b").Value != rootValue);
            Assert.True(trie.Prefix("bu").Value != rootValue);
            Assert.True(trie.Prefix("c").Value == rootValue);

            trie.Remove("b");
            trie.Remove("b");
            trie.Remove("bitter");
            trie.Remove("bitter");

            Assert.True(trie.Count() == 4);
            Assert.True(trie.Count == 4);

            Assert.True(trie.Prefix("better").Value != '^');

            trie.Remove("better");

            Assert.True(trie.Count() == 3);
            Assert.True(trie.Count == 3);

            Assert.False(trie.ContainsExact("better"));
            Assert.True(trie.ContainsExact("butter"));
            Assert.True(trie.ContainsExact("bet"));
            Assert.True(trie.ContainsExact("butt"));

            var path = trie.PrefixPath("bet");
        }

        [Test]
        public void GetMatchesTest() {
            var trie = new Trie<char>();

            var matches = trie.GetMatches("Alan");

            Assert.False(matches.Any());

            matches = trie.GetMatches("");

            Assert.True(matches.Any());

            trie = new Trie<char> {
                "better",
                "butter",
                "bet",
                "butt"
            };

            matches = trie.GetMatches("");

            Assert.True(matches.Count() == 4);

            matches = trie.GetMatches("better");
            Assert.True(matches.Count() == 1);
            Assert.True(new string(matches.First().ToArray()) == "better");

            trie.Add("betted");

            matches = trie.GetMatches("bet");

            Assert.True(matches.Count() == 3);
            Assert.True(matches.Contains("bet"));
            Assert.True(matches.Contains("better"));
            Assert.True(matches.Contains("betted"));

            matches = trie.GetMatches("foo");

            Assert.True(matches.None());
        }
    }
}
