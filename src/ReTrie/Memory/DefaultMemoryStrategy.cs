namespace ReTrie.Memory
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class DefaultMemoryStrategy<TData, TValue> : IMemoryStrategy<TData, TValue>
    {
        private readonly long _id;
        private readonly MemoryContext _context;
        private int _count;

        public int Count => _count;

        public DefaultMemoryStrategy()
            : this(new MemoryContext())
        {
            
        }

        protected DefaultMemoryStrategy(MemoryContext context)
        {
            _context = context;
            _id = context.NextId();
        }

        public ITrieNode<TData, TValue> Allocate(TData data)
        {
            var node = new DefaultTrieNode<TData, TValue>(new DefaultMemoryStrategy<TData, TValue>(_context));
            _context.Store.Add(new Tuple<long, TData>(_id, data), node);
            _count++;
            return node;
        }

        public ITrieNode<TData, TValue> Get(TData data)
        {
            var key = Tuple.Create(_id, data);
            ITrieNode<TData, TValue> node;
            _context.Store.TryGetValue(key, out node);
            return node;
        }

        public bool Remove(TData data)
        {
            var key = Tuple.Create(_id, data);
            var success = _context.Store.Remove(key);
            if (success)
            {
                _count--;
            }
            return success;
        }

        public IEnumerator<KeyValuePair<TData, ITrieNode<TData, TValue>>> GetEnumerator()
        {
            return _context.Store
                .Where(p => p.Key.Item1 == _id)
                .Select(p => new KeyValuePair<TData, ITrieNode<TData, TValue>>(p.Key.Item2, p.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected class MemoryContext
        {
            private long _currentId;

            private readonly Lazy<IDictionary<Tuple<long, TData>, ITrieNode<TData, TValue>>> _lazyStore =
                new Lazy<IDictionary<Tuple<long, TData>, ITrieNode<TData, TValue>>>(() =>
                    new Dictionary<Tuple<long, TData>, ITrieNode<TData, TValue>>());

            public IDictionary<Tuple<long, TData>, ITrieNode<TData, TValue>> Store => _lazyStore.Value;

            public long NextId()
            {
                return Interlocked.Increment(ref _currentId);
            }
        }
    }
}
