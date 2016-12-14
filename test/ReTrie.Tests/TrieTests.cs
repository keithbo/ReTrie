namespace ReTrie
{
    using System;
    using System.IO;
    using System.Linq;
    using ReTrie.Memory;
    using Xunit;

    public class TrieTests
    {
        [Fact]
        public void LargeWordsMemoryTest()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var before = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;

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

            var after = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            var diff = after - before;

            Assert.NotEqual(0, diff);
        }

        [Fact]
        public void AddOneAndRemoveOneTest()
        {
            var memory = new DefaultMemoryStrategy<char, int>();

            var t = new Trie<char, int>(memory);
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.Equal(4, memory.Store.Count);

            t.Remove("word");

            Assert.Equal(0, memory.Store.Count);
        }

        [Fact]
        public void AddTwoAndRemoveOneTest()
        {
            var memory = new DefaultMemoryStrategy<char, int>();

            var t = new Trie<char, int>(memory);
            t.AddOrUpdate("words", 1, n => n + 1);
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.Equal(5, memory.Store.Count);

            t.Remove("words");

            Assert.Equal(4, memory.Store.Count);

            memory = new DefaultMemoryStrategy<char, int>();

            t = new Trie<char, int>(memory);
            t.AddOrUpdate("word", 1, n => n + 1);
            t.AddOrUpdate("weird", 1, n => n + 1);

            Assert.Equal(8, memory.Store.Count);

            t.Remove("word");

            Assert.Equal(5, memory.Store.Count);
        }

        [Fact]
        public void AddOneAndRemoveDifferentTest()
        {
            var memory = new DefaultMemoryStrategy<char, int>();

            var t = new Trie<char, int>(memory);
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.Equal(4, memory.Store.Count);

            t.Remove("words");

            Assert.Equal(4, memory.Store.Count);
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
        public void FindTest()
        {
            var t = new Trie<char, int>();
            t.AddOrUpdate("word", 1, n => n + 1);

            Assert.Equal(1, t.Find("word"));
            Assert.Equal(0, t.Find("wor"));
            Assert.Equal(0, t.Find("words"));
        }

        [Fact]
        public void FindAllTest()
        {
            var t = new Trie<char, int>();
            t.AddOrUpdate("word", 1, n => n + 1);
            t.AddOrUpdate("words", 1, n => n + 1);
            t.AddOrUpdate("wording", 1, n => n + 1);

            var result = t.FindAll("word").ToArray();

            Assert.NotNull(result);
            Assert.Equal(3, result.Length);
        }
    }
}
