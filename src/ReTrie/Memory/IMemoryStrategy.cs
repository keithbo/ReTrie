namespace ReTrie.Memory
{
    using System.Collections.Generic;

    public interface IMemoryStrategy<TValue, TData>
    {
        ITrieNode<TData> Get(ITrieNode<TData> parent, TValue value);

        IEnumerable<KeyValuePair<TValue, ITrieNode<TData>>> Get(ITrieNode<TData> parent);

        ITrieNode<TData> Set(ITrieNode<TData> parent, TValue value, ITrieNode<TData> child);

        ITrieNode<TData> Remove(ITrieNode<TData> parent, TValue value);
    }
}
