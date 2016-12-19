namespace ReTrie.Memory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class DefaultMemoryStrategy<TK, TV> : IMemoryStrategy<TK, TV>
    {
        private readonly ConcurrentDictionary<long, TrieNode<TK, TV>> _nodes = new ConcurrentDictionary<long, TrieNode<TK, TV>>();
        private readonly ConcurrentDictionary<Tuple<long?, TK>, long> _relations = new ConcurrentDictionary<Tuple<long?, TK>, long>();

        internal IDictionary<long, TrieNode<TK, TV>> Nodes => _nodes;
        internal IDictionary<Tuple<long?, TK>, long> Relations => _relations;

        public TrieNode<TK, TV> Get(long id)
        {
            TrieNode<TK, TV> node;
            _nodes.TryGetValue(id, out node);
            return node;
        }

        public long? Get(long? id, TK key)
        {
            long childId;
            return _relations.TryGetValue(Tuple.Create(id, key), out childId) ? (long?)childId : null;
        }

        public void Set(TrieNode<TK, TV> node)
        {
            if (node != null)
            {
                _nodes.AddOrUpdate(node.Id, node, (k, v) => node);
            }
        }

        public void Set(long? parentId, TK key, long childId)
        {
            _relations.AddOrUpdate(Tuple.Create(parentId, key), childId, (k, v) => childId);
        }

        public void Remove(long id)
        {
            TrieNode<TK, TV> node;
            _nodes.TryRemove(id, out node);
        }

        public void Remove(long? id, TK key)
        {
            long childId;
            _relations.TryRemove(Tuple.Create(id, key), out childId);
        }
    }
}
