namespace ReTrie
{
    using System.IO;
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

        [Fact]
        public void WordsTextTest()
        {
            var t = new Trie<char, int>();

            using (var f = File.OpenText("words.txt"))
            {
                while (!f.EndOfStream)
                {
                    var text = f.ReadLine();
                    if (string.IsNullOrEmpty(text)) continue;
                    t.AddOrUpdate(text, 1, n => n+1);
                }
            }

            Assert.NotNull(t);
        }
    }
}
