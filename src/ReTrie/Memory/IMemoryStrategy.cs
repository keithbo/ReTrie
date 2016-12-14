namespace ReTrie.Memory
{
    using System.Collections.Generic;

    public interface IMemoryStrategy<TValue, TData>
    {
        TrieNode<TValue, TData> Get(TrieNode<TValue, TData> parent, TValue value);

        IEnumerable<KeyValuePair<TValue, TrieNode<TValue, TData>>> Get(TrieNode<TValue, TData> parent);

        TrieNode<TValue, TData> Set(TrieNode<TValue, TData> parent, TValue value, TrieNode<TValue, TData> child);

        void Remove(TrieNode<TValue, TData> parent, TValue value);
    }
}
