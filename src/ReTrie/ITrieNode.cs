namespace ReTrie
{
    public interface ITrieNode<TData, TValue>
    {
        bool HasChildren { get; }
        bool HasValue { get; }
        TValue Value { get; set; }

        ITrieNode<TData, TValue> AddOrGet(TData data);
    }
}
