namespace ReTrie.Memory
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract(Name = "TrieNode_{0}_{1}")]
    public sealed class TrieNode<TK, TV> : ITrieNode<TK, TV>
    {
        private static readonly IEnumerable<TK> NoChildren = new TK[0];

        [DataMember(Name = "Children", IsRequired = false, EmitDefaultValue = false, Order = 3)]
        private List<TK> _children;

        [DataMember(Name = "Id", IsRequired = true, Order = 1)]
        public long Id { get; private set; }

        [DataMember(Name = "Value", IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public TV Value { get; private set; }

        [IgnoreDataMember]
        public bool HasValue => !Equals(Value, default(TV));

        [IgnoreDataMember]
        public IEnumerable<TK> Children => _children ?? NoChildren;

        [IgnoreDataMember]
        public int ChildCount => _children?.Count ?? 0;

        public TrieNode(long id)
        {
            Id = id;
        }

        internal TrieNode(long id, TV value, List<TK> children)
        {
            Id = id;
            Value = value;
            _children = children;
        }

        public ITrieNode<TK, TV> SetValue(TV value)
        {
            return new TrieNode<TK, TV>(Id, value, _children?.ToList());
        }

        public ITrieNode<TK, TV> AddChild(TK child)
        {
            if (_children == null)
            {
                return new TrieNode<TK, TV>(Id, Value, new List<TK> { child });
            }

            if (!_children.Contains(child))
            {
                return new TrieNode<TK, TV>(Id, Value, _children.Concat(new [] { child }).ToList());
            }

            return this;
        }

        public ITrieNode<TK, TV> RemoveChild(TK child)
        {
            if (_children == null)
            {
                return this;
            }

            var children = _children.ToList();
            if (!children.Remove(child))
            {
                return this;
            }

            return new TrieNode<TK, TV>(Id, Value, children.Count > 0 ? children : null);
        }

        public bool Equals(TrieNode<TK, TV> other)
        {
            return other != null && (ReferenceEquals(this, other) || Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TrieNode<TK, TV>);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
