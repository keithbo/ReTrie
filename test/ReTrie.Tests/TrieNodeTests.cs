namespace ReTrie
{
    using System.IO;
    using System.Runtime.Serialization;
    using ReTrie.Memory;
    using Xunit;

    public class TrieNodeTests
    {
        [Fact]
        public void SerializationTest()
        {
            var testObject = new TrieNode<char, int>(1)
            {
                Value = 101
            };

            var serializer = new DataContractSerializer(testObject.GetType());
            using (var memory = new MemoryStream())
            {
                serializer.WriteObject(memory, testObject);

                memory.Seek(0, SeekOrigin.Begin);
                var output = serializer.ReadObject(memory);

                Assert.NotSame(testObject, output);

                var test = output as TrieNode<char, int>;
                Assert.NotNull(test);
                Assert.Equal(1, test.Id);
                Assert.Equal(101, test.Value);
            }

        }
    }
}