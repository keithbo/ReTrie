namespace ReTrie.Memory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class DefaultMemoryStrategy<TK, TV> : IMemoryStrategy<TK, TV>
    {
        private readonly ConcurrentDictionary<long, TrieNode<TK, TV>> _nodes = new ConcurrentDictionary<long, TrieNode<TK, TV>>();
        private readonly ConcurrentDictionary<Tuple<long?, TK>, long> _relations = new ConcurrentDictionary<Tuple<long?, TK>, long>();

        private long _currentNodeId;

        internal IDictionary<long, TrieNode<TK, TV>> Nodes => _nodes;
        internal IDictionary<Tuple<long?, TK>, long> Relations => _relations;

        public DefaultMemoryStrategy()
        {
        }

        public ITrieNode<TK, TV> NewNode()
        {
            return new TrieNode<TK, TV>(Interlocked.Increment(ref _currentNodeId));
        }

        public ITrieNode<TK, TV> GetNode(long id)
        {
            _nodes.TryGetValue(id, out var node);
            return node;
        }

        public long? GetRelation(long? parentId, TK key)
        {
            return _relations.TryGetValue(Tuple.Create(parentId, key), out var childId) ? (long?)childId : null;
        }

        public void SetNode(ITrieNode<TK, TV> node)
        {
            if (node != null)
            {
                var realized = Cast(node);
                _nodes.AddOrUpdate(node.Id, realized, (k, v) => realized);
            }
        }

        public void SetRelation(long? parentId, TK key, long childId)
        {
            _relations.AddOrUpdate(Tuple.Create(parentId, key), childId, (k, v) => childId);
        }

        public void RemoveNode(long id)
        {
            _nodes.TryRemove(id, out var node);
        }

        public void RemoveRelation(long? parentId, TK key)
        {
            _relations.TryRemove(Tuple.Create(parentId, key), out var childId);
        }

        internal TrieNode<TK, TV> Cast(ITrieNode<TK, TV> node)
        {
            if (node is TrieNode<TK, TV> realized)
            {
                return realized;
            }

            return new TrieNode<TK, TV>(node.Id, node.Value, node.Children.ToList());
        }
    }
}
