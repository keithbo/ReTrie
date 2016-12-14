using System.Linq;

namespace ReTrie.Memory
{
    using System;
    using System.Collections.Generic;

    public class DefaultMemoryStrategy<TValue, TData> : IMemoryStrategy<TValue, TData>
    {
        private readonly Lazy<IDictionary<Tuple<TrieNode<TValue, TData>, TValue>, TrieNode<TValue, TData>>> _lazyStore =
            new Lazy<IDictionary<Tuple<TrieNode<TValue, TData>, TValue>, TrieNode<TValue, TData>>>(() =>
                new Dictionary<Tuple<TrieNode<TValue, TData>, TValue>, TrieNode<TValue, TData>>());

        public IDictionary<Tuple<TrieNode<TValue, TData>, TValue>, TrieNode<TValue, TData>> Store => _lazyStore.Value;

        public DefaultMemoryStrategy()
        {
        }

        public TrieNode<TValue, TData> Get(TrieNode<TValue, TData> parent, TValue value)
        {
            var key = Key(parent, value);
            TrieNode<TValue, TData> node;
            Store.TryGetValue(key, out node);
            return node;
        }

        public IEnumerable<KeyValuePair<TValue, TrieNode<TValue, TData>>> Get(TrieNode<TValue, TData> parent)
        {
            var children = parent.GetChildren().ToArray();
            foreach (var c in children)
            {
                var node = Store[Tuple.Create(parent, c)];

                yield return new KeyValuePair<TValue, TrieNode<TValue, TData>>(c, node);
            }
        }

        public TrieNode<TValue, TData> Set(TrieNode<TValue, TData> parent, TValue value, TrieNode<TValue, TData> child)
        {
            var key = Key(parent, value);
            Store[key] = child;
            parent?.AddChild(value);
            return child;
        }

        public void Remove(TrieNode<TValue, TData> parent, TValue value)
        {
            var key = Key(parent, value);
            if (Store.Remove(key))
            {
                parent?.RemoveChild(value);
            }
        }

        private static Tuple<TrieNode<TValue, TData>, TValue> Key(TrieNode<TValue, TData> node, TValue value)
        {
            return Tuple.Create(node, value);
        }
    }
}
