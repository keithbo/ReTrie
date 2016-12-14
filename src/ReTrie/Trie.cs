using System.Threading;

namespace ReTrie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ReTrie.Memory;

    public class Trie<TValue, TData> : ITrie<TValue, TData>
    {
        private long _currentNodeId;
        private readonly IMemoryStrategy<TValue, TData> _memory;

        private readonly Func<TrieNode<TValue, TData>> _nodeFactory;

        public Trie()
            : this(new DefaultMemoryStrategy<TValue, TData>())
        {
        }

        public Trie(IMemoryStrategy<TValue, TData> memory)
        {
            _memory = memory;
            _nodeFactory = () => new TrieNode<TValue, TData>(Interlocked.Increment(ref _currentNodeId));
        }

        public void AddOrUpdate(IEnumerable<TValue> sequence, TData add, Func<TData, TData> update)
        {
            AddOrUpdate(sequence, () => add, update);
        }

        public void AddOrUpdate(IEnumerable<TValue> sequence, Func<TData> add, Func<TData, TData> update)
        {
            var target = sequence.Aggregate((TrieNode<TValue, TData>)null, (n, d) => _memory.Get(n, d) ?? _memory.Set(n, d, _nodeFactory()));
            target.Data = target.HasData ? update(target.Data) : add();
        }

        public void Remove(IEnumerable<TValue> sequence)
        {
            var stack = new Stack<Tuple<TrieNode<TValue, TData>, TValue, TrieNode<TValue, TData>>>();
            TrieNode<TValue, TData> previous = null;
            foreach (var d in sequence)
            {
                var current = _memory.Get(previous, d);
                if (current == null) return;// full sequence does not exist
                stack.Push(Tuple.Create(previous, d, current));
                previous = current;
            }

            if (stack.Count == 0) return;

            stack.Peek().Item3.Data = default(TData);

            do
            {
                var frame = stack.Pop();
                var node = frame.Item3;
                if (node.HasData || node.ChildCount > 0) return;//cannot remove any nodes that are part of a larger chain or still a valid leaf
                _memory.Remove(frame.Item1, frame.Item2);

            } while (stack.Count > 0);
        }

        public bool Contains(IEnumerable<TValue> sequence)
        {
            return Contains(sequence, false);
        }

        public bool Contains(IEnumerable<TValue> sequence, bool includeDescendants)
        {
            var target = FindInternal(sequence);
            return target != null && (target.HasData || (includeDescendants && target.ChildCount > 0));
        }

        public TData Find(IEnumerable<TValue> sequence)
        {
            var target = FindInternal(sequence);
            return target != null && target.HasData ? target.Data : default(TData);
        }

        public IEnumerable<KeyValuePair<IEnumerable<TValue>, TData>> FindAll(IEnumerable<TValue> sequence)
        {
            var sequenceArray = sequence.ToArray();
            var target = FindInternal(sequenceArray);
            if (target == null) yield break;

            var queue = new Queue<Tuple<TValue[], TrieNode<TValue, TData>>>();
            queue.Enqueue(Tuple.Create(sequenceArray, target));

            while (queue.Count > 0)
            {
                var t = queue.Dequeue();
                if (t.Item2.HasData)
                {
                    yield return new KeyValuePair<IEnumerable<TValue>, TData>(t.Item1, t.Item2.Data);
                }
                foreach (var c in t.Item2.GetChildren())
                {
                    var next = _memory.Get(t.Item2, c);
                    if (next == null) continue;

                    queue.Enqueue(Tuple.Create(Combine(t.Item1,c), next));
                }
            }

        }

        private TrieNode<TValue, TData> FindInternal(IEnumerable<TValue> sequence)
        {
            TrieNode<TValue, TData> target = null;
            foreach (var d in sequence)
            {
                target = _memory.Get(target, d);
                if (target == null) break;// full sequence does not exist
            }

            return target;
        }

        private static TValue[] Combine(IEnumerable<TValue> array, TValue single)
        {
            return array.Concat(new[] {single}).ToArray();
        }
    }
}
