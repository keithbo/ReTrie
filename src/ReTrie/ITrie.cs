namespace ReTrie
{
    using System;
    using System.Collections.Generic;

    public interface ITrie<TValue, TData>
    {
        void AddOrUpdate(IEnumerable<TValue> sequence, TData add, Func<TData, TData> update);

        void AddOrUpdate(IEnumerable<TValue> sequence, Func<TData> add, Func<TData, TData> update);

        void Remove(IEnumerable<TValue> sequence);

        bool Contains(IEnumerable<TValue> sequence);

        bool Contains(IEnumerable<TValue> sequence, bool includeDescendants);
    }
}
