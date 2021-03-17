using NUnit.Framework;

namespace Fruberry.Tests {
    public class CharTrieTests {
        [Test]
        public void Test() {
            var rootValue = default(char);

            var trie = new CharTrie();

            Assert.False(trie.ContainsExact("c"));

            trie.Add("better");
            trie.Add("butter");
            trie.Add("bet");
            trie.Add("butt");

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

            trie.Delete("b");
            trie.Delete("b");
            trie.Delete("bitter");
            trie.Delete("bitter");

            Assert.True(trie.Prefix("better").Value != '^');

            trie.Delete("better");

            Assert.False(trie.ContainsExact("better"));
            Assert.True(trie.ContainsExact("butter"));
            Assert.True(trie.ContainsExact("bet"));
            Assert.True(trie.ContainsExact("butt"));

            var path = trie.PrefixPath("bet");
        }

        [Test]
        public void GetMatchesTest() {
            var trie = new CharTrie {
                "better",
                "butter",
                "bet",
                "butt"
            };

            var foo = trie.GetMatches("better");
            Assert.True(foo.Count == 1);
            Assert.True(foo[0] == "better");

            trie.Add("betted");

            var matches2 = trie.GetMatches("bet");

            Assert.AreEqual(3, matches2.Count);
            Assert.True(matches2.Contains("bet"));
            Assert.True(matches2.Contains("better"));
            Assert.True(matches2.Contains("betted"));
        }
    }
}
