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
            AddOrUpdateInternal(sequence, target =>
            {
                var old = target.Value;
                target.Value = target.HasValue ? update(target.Value) : add();
                return !Equals(old, target.Value);
            });
        }

        public void Set(IEnumerable<TK> sequence, TV value)
        {
            AddOrUpdateInternal(sequence, target =>
            {
                var old = target.Value;
                target.Value = value;
                return !Equals(old, target.Value);
            });
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
            var stack = new Stack<Frame>();
            TrieNode<TK, TV> previous = null;
            foreach (var d in sequence)
            {
                var currentId = _memory.Get(previous?.Id, d);
                if (currentId == null) return false;// full sequence does not exist
                var current = _memory.Get(currentId.Value);
                stack.Push(new Frame { Parent = previous, Key = d, Child = current });
                previous = current;
            }

            if (stack.Count == 0) return false;

            var success = false;
            var frame = stack.Pop();
            if (frame.Child.HasValue && predicate(frame.Child.Value))
            {
                if (frame.Child.ChildCount == 0)
                {
                    if (frame.Parent != null && frame.Parent.RemoveChild(frame.Key))
                    {
                        _memory.Set(frame.Parent);
                    }
                    _memory.Remove(frame.Parent?.Id, frame.Key);
                    _memory.Remove(frame.Child.Id);
                }
                else
                {
                    frame.Child.Value = default(TV);
                    _memory.Set(frame.Child);
                }
                success = true;
            }

            while (stack.Count > 0)
            {
                frame = stack.Pop();
                if (frame.Child.HasValue || frame.Child.ChildCount > 0) break;//cannot remove any nodes that are part of a larger chain or still a valid leaf

                if (frame.Parent != null && frame.Parent.RemoveChild(frame.Key))
                {
                    _memory.Set(frame.Parent);
                }
                _memory.Remove(frame.Parent?.Id, frame.Key);
                _memory.Remove(frame.Child.Id);
            }

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
                    var nextId = _memory.Get(t.Item2.Id, c);
                    if (!nextId.HasValue) continue;

                    queue.Enqueue(Tuple.Create(Combine(t.Item1,c), _memory.Get(nextId.Value)));
                }
            }

        }

        private void AddOrUpdateInternal(IEnumerable<TK> sequence, Func<TrieNode<TK, TV>, bool> modify)
        {
            TrieNode<TK, TV> node = null;
            long? nodeId = null;
            foreach (var v in sequence)
            {
                //clear any previously tracked node, its not a leaf
                if (node != null)
                {
                    _memory.Set(node);
                    node = null;
                }
                //get next level
                var childId = _memory.Get(nodeId, v);
                //if level doesn't exist, add it
                if (childId == null)
                {
                    node = _nodeFactory();
                    _memory.Set(nodeId, v, node.Id);
                    //if new level had a parent try and alter parent with new key value
                    if (nodeId.HasValue)
                    {
                        var parent = _memory.Get(nodeId.Value);
                        //only save node back if it actually added the child value
                        if (parent.AddChild(v))
                        {
                            _memory.Set(parent);
                        }
                    }

                    childId = node.Id;
                }

                nodeId = childId;
            }

            // if we are tracking a node it is an added leaf and must be saved
            if (node != null)
            {
                modify?.Invoke(node);
                _memory.Set(node);
            }
            // otherwise get the identified node by id if there is one
            else if (nodeId.HasValue)
            {
                node = _memory.Get(nodeId.Value);
                if (modify != null && modify(node))
                {
                    _memory.Set(node);
                }
            }
        }

        private TrieNode<TK, TV> FindInternal(IEnumerable<TK> sequence)
        {
            long? target = null;
            foreach (var d in sequence)
            {
                target = _memory.Get(target, d);
                if (target == null) break;// full sequence does not exist
            }

            return target.HasValue ? _memory.Get(target.Value) : null;
        }

        private static TK[] Combine(IEnumerable<TK> array, TK single)
        {
            return array.Concat(new[] {single}).ToArray();
        }

        private class Frame
        {
            public TrieNode<TK, TV> Parent { get; set; }
            public TK Key { get; set; }
            public TrieNode<TK, TV> Child { get; set; }
        }
    }
}
