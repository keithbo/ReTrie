namespace ReTrie
{
    using System.Linq;
    using ReTrie.Memory;
    using Xunit;

    public class TrieTests
    {
        [Fact]
        public void AddOneAndRemoveOneTest()
        {
            var memory = new DefaultMemoryStrategy<char, int>();

            var t = new Trie<char, int>(memory);
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.Equal(4, memory.Nodes.Count);

            t.Remove("word");

            Assert.Equal(0, memory.Nodes.Count);
        }

        [Fact]
        public void AddTwoAndRemoveOneTest()
        {
            var memory = new DefaultMemoryStrategy<char, int>();

            var t = new Trie<char, int>(memory);
            t.AddOrUpdate("words", 1, n => n + 1);
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.Equal(5, memory.Nodes.Count);

            t.Remove("words");

            Assert.Equal(4, memory.Nodes.Count);

            memory = new DefaultMemoryStrategy<char, int>();

            t = new Trie<char, int>(memory);
            t.AddOrUpdate("word", 1, n => n + 1);
            t.AddOrUpdate("weird", 1, n => n + 1);

            Assert.Equal(8, memory.Nodes.Count);

            t.Remove("word");

            Assert.Equal(5, memory.Nodes.Count);
        }

        [Fact]
        public void AddOneAndRemoveDifferentTest()
        {
            var memory = new DefaultMemoryStrategy<char, int>();

            var t = new Trie<char, int>(memory);
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.Equal(4, memory.Nodes.Count);

            t.Remove("words");

            Assert.Equal(4, memory.Nodes.Count);
        }

        [Fact]
        public void ContainsTest()
        {
            var t = new Trie<char, int>();
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.True(t.Contains("word"));
            Assert.False(t.Contains("wor"));
            Assert.False(t.Contains("words"));
            Assert.True(t.Contains("wor", true));
            Assert.True(t.Contains("word", true));
            Assert.True(t.Contains("word", false));
            Assert.False(t.Contains("words", false));
            Assert.False(t.Contains("words", false));
        }

        [Fact]
        public void GetTest()
        {
            var t = new Trie<char, int>();
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.Equal(1, t.Get("word"));
            Assert.Equal(0, t.Get("wor"));
            Assert.Equal(0, t.Get("words"));
        }

        [Fact]
        public void GetEnumeratorTest()
        {
            var t = new Trie<char, int>();
            t.AddOrUpdate("word", 1, n => n + 1);
            t.AddOrUpdate("words", 1, n => n + 1);
            t.AddOrUpdate("wording", 1, n => n + 1);

            var result = t.GetEnumerator("word").ToArray();

            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
        }

        [Fact]
        public void GetEnumeratorPartialTest()
        {
            var t = new Trie<char, int>();
            t.AddOrUpdate("word", 1, n => n + 1);
            t.AddOrUpdate("words", 1, n => n + 1);
            t.AddOrUpdate("wording", 1, n => n + 1);

            var result = t.GetEnumerator("wo").ToArray();

            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
        }
    }
}
