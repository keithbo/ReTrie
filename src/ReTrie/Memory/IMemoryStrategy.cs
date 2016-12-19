namespace ReTrie.Memory
{
    public interface IMemoryStrategy<TK, TV>
    {
        TrieNode<TK, TV> Get(long id);
        long? Get(long? id, TK key);
        void Set(TrieNode<TK, TV> node);
        void Set(long? parentId, TK key, long childId);
        void Remove(long id);
        void Remove(long? id, TK key);
    }
}
