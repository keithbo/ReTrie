namespace ReTrie.Memory
{
    using System.Collections.Generic;

    public interface IMemoryStrategy<TData, TValue> : IEnumerable<KeyValuePair<TData, ITrieNode<TData, TValue>>>
    {
        int Count { get; }

        ITrieNode<TData, TValue> Allocate(TData data);

        ITrieNode<TData, TValue> Get(TData data);

        bool Remove(TData data);
    }
}
