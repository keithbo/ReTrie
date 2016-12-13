using System.Linq;

namespace ReTrie.Memory
{
    using System;
    using System.Collections.Generic;

    public class DefaultMemoryStrategy<TValue, TData> : IMemoryStrategy<TValue, TData>
    {
        private readonly Lazy<IDictionary<Tuple<long, TValue>, ITrieNode<TData>>> _lazyStore =
            new Lazy<IDictionary<Tuple<long, TValue>, ITrieNode<TData>>>(() =>
                new Dictionary<Tuple<long, TValue>, ITrieNode<TData>>());

        public IDictionary<Tuple<long, TValue>, ITrieNode<TData>> Store => _lazyStore.Value;

        public DefaultMemoryStrategy()
        {
        }

        public ITrieNode<TData> Get(ITrieNode<TData> parent, TValue value)
        {
            var key = Key(parent, value);
            ITrieNode<TData> node;
            Store.TryGetValue(key, out node);
            return node;
        }

        public IEnumerable<KeyValuePair<TValue, ITrieNode<TData>>> Get(ITrieNode<TData> parent)
        {
            return Store
                .Where(p => p.Key.Item1 == parent.Id)
                .Select(p => new KeyValuePair<TValue, ITrieNode<TData>>(p.Key.Item2, p.Value));
        }

        public ITrieNode<TData> Set(ITrieNode<TData> parent, TValue value, ITrieNode<TData> child)
        {
            var key = Key(parent, value);
            Store[key] = child;
            return child;
        }

        public ITrieNode<TData> Remove(ITrieNode<TData> parent, TValue value)
        {
            var key = Key(parent, value);
            ITrieNode<TData> node;
            if (Store.TryGetValue(key, out node))
            {
                Store.Remove(key);
            }
            return node;
        }

        private static Tuple<long, TValue> Key(ITrieNode<TData> node, TValue value)
        {
            return Tuple.Create(node?.Id ?? 0, value);
        }
    }
}
