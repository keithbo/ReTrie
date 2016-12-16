namespace ReTrie
{
    using System;
    using System.Collections.Generic;

    public interface ITrie<TK, TV>
    {
        void AddOrUpdate(IEnumerable<TK> sequence, TV add, Func<TV, TV> update);

        void AddOrUpdate(IEnumerable<TK> sequence, Func<TV> add, Func<TV, TV> update);

        bool TryAdd(IEnumerable<TK> sequence, TV add);

        bool TryUpdate(IEnumerable<TK> sequence, Func<TV, TV> update);

        void Set(IEnumerable<TK> sequence, TV value);

        void Remove(IEnumerable<TK> sequence);

        bool TryRemove(IEnumerable<TK> sequence);

        bool TryRemove(IEnumerable<TK> sequence, Func<TV, bool> predicate);

        bool Contains(IEnumerable<TK> sequence);

        bool Contains(IEnumerable<TK> sequence, bool includeDescendants);

        TV Get(IEnumerable<TK> sequence);

        IEnumerable<KeyValuePair<IEnumerable<TK>, TV>> GetEnumerator(IEnumerable<TK> sequence);
    }
}
