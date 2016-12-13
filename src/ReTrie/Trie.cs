namespace ReTrie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ReTrie.Memory;

    public class Trie<TData, TValue> : ITrie<TData>
    {
        private readonly IMemoryStrategy<TData, TValue> _memory;

        public Trie()
            : this(new DefaultMemoryStrategy<TData, TValue>())
        {
        }

        public Trie(IMemoryStrategy<TData, TValue> memory)
        {
            _memory = memory;
        }

        public void AddOrUpdate(IEnumerable<TData> sequence, TValue add, Func<TValue, TValue> update)
        {
            AddOrUpdate(sequence, () => add, update);
        }

        public void AddOrUpdate(IEnumerable<TData> sequence, Func<TValue> add, Func<TValue, TValue> update)
        {
            var target = sequence.Aggregate((ITrieNode<TData, TValue>)null, (n, d) => n != null ? n.AddOrGet(d) : _memory.Get(d) ?? _memory.Allocate(d));
            target.Value = target.HasValue ? update(target.Value) : add();
        }

        public void Remove(IEnumerable<TData> sequence)
        {
            
        }
    }
}
