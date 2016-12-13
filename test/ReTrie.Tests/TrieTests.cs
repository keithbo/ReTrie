namespace ReTrie.Tests
{
    using Xunit;

    public class TrieTests
    {
        [Fact]
        public void SimpleTest()
        {
            var t = new Trie<char, int>();

            t.AddOrUpdate("word", 1, n => n+1);
            t.AddOrUpdate("weird", 1, n => n + 1);

            Assert.NotNull(t);
        }
    }
}
