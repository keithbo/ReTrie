using System.Threading;

namespace ReTrie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ReTrie.Memory;

    public class Trie<TValue, TData> : ITrie<TValue>
    {
        private long _currentNodeId;
        private readonly IMemoryStrategy<TValue, TData> _memory;

        private readonly Func<ITrieNode<TData>> _nodeFactory;

        public Trie()
            : this(new DefaultMemoryStrategy<TValue, TData>())
        {
        }

        public Trie(IMemoryStrategy<TValue, TData> memory)
        {
            _memory = memory;
            _nodeFactory = () => new DefaultTrieNode<TData>(Interlocked.Increment(ref _currentNodeId));
        }

        public void AddOrUpdate(IEnumerable<TValue> sequence, TData add, Func<TData, TData> update)
        {
            AddOrUpdate(sequence, () => add, update);
        }

        public void AddOrUpdate(IEnumerable<TValue> sequence, Func<TData> add, Func<TData, TData> update)
        {
            var target = sequence.Aggregate((ITrieNode<TData>)null, (n, d) => _memory.Get(n, d) ?? _memory.Set(n, d, _nodeFactory()));
            target.Data = target.HasData ? update(target.Data) : add();
        }

        public void Remove(IEnumerable<TData> sequence)
        {
            
        }
    }
}
