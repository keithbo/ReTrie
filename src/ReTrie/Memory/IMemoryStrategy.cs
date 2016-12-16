namespace ReTrie.Memory
{
    public interface IMemoryStrategy<TK, TV>
    {
        TrieNode<TK, TV> Get(TrieNode<TK, TV> parent, TK value);

        TrieNode<TK, TV> Set(TrieNode<TK, TV> parent, TK value, TrieNode<TK, TV> child);

        void Remove(TrieNode<TK, TV> parent, TK value);
    }
}
