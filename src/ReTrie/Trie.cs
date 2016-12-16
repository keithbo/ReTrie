namespace ReTrie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using ReTrie.Memory;

    public class Trie<TK, TV> : ITrie<TK, TV>
    {
        private long _currentNodeId;
        private readonly IMemoryStrategy<TK, TV> _memory;

        private readonly Func<TrieNode<TK, TV>> _nodeFactory;

        public Trie()
            : this(new DefaultMemoryStrategy<TK, TV>())
        {
        }

        public Trie(IMemoryStrategy<TK, TV> memory)
        {
            _memory = memory;
            _nodeFactory = () => new TrieNode<TK, TV>(Interlocked.Increment(ref _currentNodeId));
        }

        public void AddOrUpdate(IEnumerable<TK> sequence, TV add, Func<TV, TV> update)
        {
            AddOrUpdate(sequence, () => add, update);
        }

        public void AddOrUpdate(IEnumerable<TK> sequence, Func<TV> add, Func<TV, TV> update)
        {
            var target = GetOrAddNode(sequence);
            target.Value = target.HasValue ? update(target.Value) : add();
        }

        public bool TryAdd(IEnumerable<TK> sequence, TV add)
        {
            var target = GetOrAddNode(sequence);
            if (target.HasValue) return false;
            target.Value = add;
            return true;
        }

        public bool TryUpdate(IEnumerable<TK> sequence, Func<TV, TV> update)
        {
            var target = FindInternal(sequence);
            if (target == null || !target.HasValue) return false;
            target.Value = update(target.Value);
            return true;
        }

        public void Set(IEnumerable<TK> sequence, TV value)
        {
            var target = GetOrAddNode(sequence);
            target.Value = value;
        }

        public void Remove(IEnumerable<TK> sequence)
        {
            TryRemove(sequence, d => true);
        }

        public bool TryRemove(IEnumerable<TK> sequence)
        {
            return TryRemove(sequence, d => true);
        }

        public bool TryRemove(IEnumerable<TK> sequence, Func<TV, bool> predicate)
        {
            var stack = new Stack<Tuple<TrieNode<TK, TV>, TK, TrieNode<TK, TV>>>();
            TrieNode<TK, TV> previous = null;
            foreach (var d in sequence)
            {
                var current = _memory.Get(previous, d);
                if (current == null) return false;// full sequence does not exist
                stack.Push(Tuple.Create(previous, d, current));
                previous = current;
            }

            if (stack.Count == 0) return false;

            var success = false;
            var target = stack.Peek().Item3;
            if (target.HasValue && predicate(target.Value))
            {
                success = true;
                target.Value = default(TV);
            }

            do
            {
                var frame = stack.Pop();
                var node = frame.Item3;
                if (node.HasValue || node.ChildCount > 0) break;//cannot remove any nodes that are part of a larger chain or still a valid leaf
                _memory.Remove(frame.Item1, frame.Item2);

            } while (stack.Count > 0);

            return success;
        }

        public bool Contains(IEnumerable<TK> sequence)
        {
            return Contains(sequence, false);
        }

        public bool Contains(IEnumerable<TK> sequence, bool includeDescendants)
        {
            var target = FindInternal(sequence);
            return target != null && (target.HasValue || (includeDescendants && target.ChildCount > 0));
        }

        public TV Get(IEnumerable<TK> sequence)
        {
            var target = FindInternal(sequence);
            return target != null && target.HasValue ? target.Value : default(TV);
        }

        public IEnumerable<KeyValuePair<IEnumerable<TK>, TV>> GetEnumerator(IEnumerable<TK> sequence)
        {
            var sequenceArray = sequence.ToArray();
            var target = FindInternal(sequenceArray);
            if (target == null) yield break;

            var queue = new Queue<Tuple<TK[], TrieNode<TK, TV>>>();
            queue.Enqueue(Tuple.Create(sequenceArray, target));

            while (queue.Count > 0)
            {
                var t = queue.Dequeue();
                if (t.Item2.HasValue)
                {
                    yield return new KeyValuePair<IEnumerable<TK>, TV>(t.Item1, t.Item2.Value);
                }
                foreach (var c in t.Item2.GetChildren())
                {
                    var next = _memory.Get(t.Item2, c);
                    if (next == null) continue;

                    queue.Enqueue(Tuple.Create(Combine(t.Item1,c), next));
                }
            }

        }

        private TrieNode<TK, TV> GetOrAddNode(IEnumerable<TK> sequence)
        {
            return sequence.Aggregate((TrieNode<TK, TV>) null, (n, d) => _memory.Get(n, d) ?? _memory.Set(n, d, _nodeFactory()));
        }

        private TrieNode<TK, TV> FindInternal(IEnumerable<TK> sequence)
        {
            TrieNode<TK, TV> target = null;
            foreach (var d in sequence)
            {
                target = _memory.Get(target, d);
                if (target == null) break;// full sequence does not exist
            }

            return target;
        }

        private static TK[] Combine(IEnumerable<TK> array, TK single)
        {
            return array.Concat(new[] {single}).ToArray();
        }
    }
}
