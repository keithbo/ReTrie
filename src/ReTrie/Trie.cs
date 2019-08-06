namespace ReTrie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ReTrie.Memory;

    public class Trie<TK, TV> : ITrie<TK, TV>
    {
        private readonly IMemoryStrategy<TK, TV> _memory;

        public Trie()
            : this(new DefaultMemoryStrategy<TK, TV>())
        {
        }

        public Trie(IMemoryStrategy<TK, TV> memory)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
        }

        public void AddOrUpdate(IEnumerable<TK> sequence, TV add, Func<TV, TV> update)
        {
            AddOrUpdate(sequence, () => add, update);
        }

        public void AddOrUpdate(IEnumerable<TK> sequence, Func<TV> add, Func<TV, TV> update)
        {
            AddOrUpdateInternal(sequence, target => target.SetValue(target.HasValue ? update(target.Value) : add()));
        }

        public void Set(IEnumerable<TK> sequence, TV value)
        {
            AddOrUpdateInternal(sequence, target => target.SetValue(value));
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
            Frame previousFrame = null;
            foreach (var d in sequence)
            {
                var currentId = _memory.GetRelation(previousFrame?.Node.Id, d);
                if (currentId == null) return false;// full sequence does not exist
                var currentNode = _memory.GetNode(currentId.Value);
                var currentFrame = new Frame
                {
                    Parent = previousFrame,
                    Key = d,
                    Node = currentNode
                };
                stack.Push(currentFrame);
                previousFrame = currentFrame;
            }

            if (stack.Count == 0) return false;

            var success = false;
            var frame = stack.Pop();
            if (frame.Node.HasValue && predicate(frame.Node.Value))
            {
                if (frame.Node.ChildCount == 0)
                {
                    if (frame.Parent != null)
                    {
                        frame.Parent.Node = frame.Parent.Node.RemoveChild(frame.Key);
                        _memory.SetNode(frame.Parent.Node);
                    }
                    _memory.RemoveRelation(frame.Parent?.Node.Id, frame.Key);
                    _memory.RemoveNode(frame.Node.Id);
                }
                else
                {
                    frame.Node = frame.Node.SetValue(default(TV));
                    _memory.SetNode(frame.Node);
                }
                success = true;
            }

            while (stack.Count > 0)
            {
                frame = stack.Pop();
                if (frame.Node.HasValue || frame.Node.ChildCount > 0) break;//cannot remove any nodes that are part of a larger chain or still a valid leaf

                if (frame.Parent != null)
                {
                    frame.Parent.Node = frame.Parent.Node.RemoveChild(frame.Key);
                    _memory.SetNode(frame.Parent.Node);
                }
                _memory.RemoveRelation(frame.Parent?.Node.Id, frame.Key);
                _memory.RemoveNode(frame.Node.Id);
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

            var queue = new Queue<Tuple<TK[], ITrieNode<TK, TV>>>();
            queue.Enqueue(Tuple.Create(sequenceArray, target));

            while (queue.Count > 0)
            {
                var t = queue.Dequeue();
                if (t.Item2.HasValue)
                {
                    yield return new KeyValuePair<IEnumerable<TK>, TV>(t.Item1, t.Item2.Value);
                }
                foreach (var c in t.Item2.Children)
                {
                    var nextId = _memory.GetRelation(t.Item2.Id, c);
                    if (!nextId.HasValue) continue;

                    queue.Enqueue(Tuple.Create(Combine(t.Item1,c), _memory.GetNode(nextId.Value)));
                }
            }

        }

        private void AddOrUpdateInternal(IEnumerable<TK> sequence, Func<ITrieNode<TK, TV>, ITrieNode<TK, TV>> modify)
        {
            modify = modify ?? (n => n);
            ITrieNode<TK, TV> node = null;
            long? nodeId = null;
            foreach (var v in sequence)
            {
                //clear any previously tracked node, its not a leaf
                if (node != null)
                {
                    _memory.SetNode(node);
                    node = null;
                }
                //get next level
                var childId = _memory.GetRelation(nodeId, v);
                //if level doesn't exist, add it
                if (childId == null)
                {
                    node = _memory.NewNode();
                    _memory.SetRelation(nodeId, v, node.Id);
                    //if new level had a parent try and alter parent with new key value
                    if (nodeId.HasValue)
                    {
                        var parent = _memory.GetNode(nodeId.Value);
                        var modifiedParent = parent.AddChild(v);
                        //only save node back if it actually added the child value
                        if (!ReferenceEquals(parent, modifiedParent))
                        {
                            _memory.SetNode(modifiedParent);
                        }
                    }

                    childId = node.Id;
                }

                nodeId = childId;
            }

            ITrieNode<TK, TV> modifiedNode = node;

            // if we are tracking a node it is an added leaf and must be saved
            if (node != null)
            {
                modifiedNode = modify(node);
            }
            // otherwise get the identified node by id if there is one
            else if (nodeId.HasValue)
            {
                node = _memory.GetNode(nodeId.Value);
                modifiedNode = modify(node);
            }

            if (!ReferenceEquals(node, modifiedNode))
            {
                _memory.SetNode(modifiedNode);
            }
        }

        private ITrieNode<TK, TV> FindInternal(IEnumerable<TK> sequence)
        {
            long? target = null;
            foreach (var d in sequence)
            {
                target = _memory.GetRelation(target, d);
                if (target == null) break;// full sequence does not exist
            }

            return target.HasValue ? _memory.GetNode(target.Value) : null;
        }

        private static TK[] Combine(IEnumerable<TK> array, TK single)
        {
            return array.Concat(new[] {single}).ToArray();
        }

        private class Frame
        {
            public Frame Parent { get; set; }
            public TK Key { get; set; }
            public ITrieNode<TK, TV> Node { get; set; }
        }
    }
}
