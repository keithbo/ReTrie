namespace ReTrie.Memory
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Name = "TrieNode_{0}_{1}")]
    public sealed class TrieNode<TK, TV> : IEquatable<TrieNode<TK, TV>>
    {
        private static readonly IEnumerable<TK> NoChildren = new TK[0];

        [DataMember(Name = "Children", IsRequired = false, EmitDefaultValue = false, Order = 3)]
        private List<TK> _children;

        [DataMember(Name = "Id", IsRequired = true, Order = 1)]
        public long Id { get; private set; }

        [IgnoreDataMember]
        public bool HasValue => !Equals(Value, default(TV));

        [DataMember(Name = "Value", IsRequired = false, EmitDefaultValue = false, Order = 2)]
        public TV Value { get; set; }

        [IgnoreDataMember]
        public int ChildCount => _children?.Count ?? 0;

        public TrieNode(long id)
        {
            Id = id;
        }

        public void AddChild(TK value)
        {
            if (_children == null)
            {
                _children = new List<TK> {value};
            }
            else if (!_children.Contains(value))
            {
                _children.Add(value);
            }
        }

        public void RemoveChild(TK value)
        {
            if (_children != null && _children.Remove(value) && _children.Count == 0)
            {
                _children = null;
            }
        }

        public IEnumerable<TK> GetChildren()
        {
            return _children ?? NoChildren;
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
