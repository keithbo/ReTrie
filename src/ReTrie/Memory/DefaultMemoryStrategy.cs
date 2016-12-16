namespace ReTrie.Memory
{
    using System;
    using System.Collections.Generic;

    public class DefaultMemoryStrategy<TK, TV> : IMemoryStrategy<TK, TV>
    {
        private readonly Lazy<IDictionary<Tuple<TrieNode<TK, TV>, TK>, TrieNode<TK, TV>>> _lazyStore =
            new Lazy<IDictionary<Tuple<TrieNode<TK, TV>, TK>, TrieNode<TK, TV>>>(() =>
                new Dictionary<Tuple<TrieNode<TK, TV>, TK>, TrieNode<TK, TV>>());

        public IDictionary<Tuple<TrieNode<TK, TV>, TK>, TrieNode<TK, TV>> Store => _lazyStore.Value;

        public TrieNode<TK, TV> Get(TrieNode<TK, TV> parent, TK value)
        {
            var key = Key(parent, value);
            TrieNode<TK, TV> node;
            Store.TryGetValue(key, out node);
            return node;
        }

        public TrieNode<TK, TV> Set(TrieNode<TK, TV> parent, TK value, TrieNode<TK, TV> child)
        {
            var key = Key(parent, value);
            Store[key] = child;
            parent?.AddChild(value);
            return child;
        }

        public void Remove(TrieNode<TK, TV> parent, TK value)
        {
            var key = Key(parent, value);
            if (Store.Remove(key))
            {
                parent?.RemoveChild(value);
            }
        }

        private static Tuple<TrieNode<TK, TV>, TK> Key(TrieNode<TK, TV> node, TK value)
        {
            return Tuple.Create(node, value);
        }
    }
}
