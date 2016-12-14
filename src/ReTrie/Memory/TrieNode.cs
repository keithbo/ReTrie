namespace ReTrie.Memory
{
    using System;
    using System.Collections.Generic;

    public class TrieNode<TValue, TData> : IEquatable<TrieNode<TValue, TData>>
    {
        private static readonly IEnumerable<TValue> NoChildren = new TValue[0];

        private bool _hasData;
        private TData _data;
        private List<TValue> _children;

        public long Id { get; }

        public bool HasData => _hasData;

        public TData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                _hasData = !Equals(value, default(TData));
            }
        }

        public int ChildCount => _children?.Count ?? 0;

        public TrieNode(long id)
        {
            Id = id;
        }

        public void AddChild(TValue value)
        {
            if (_children == null)
            {
                _children = new List<TValue> {value};
            }
            else if (!_children.Contains(value))
            {
                _children.Add(value);
            }
        }

        public void RemoveChild(TValue value)
        {
            if (_children != null && _children.Remove(value) && _children.Count == 0)
            {
                _children = null;
            }
        }

        public IEnumerable<TValue> GetChildren()
        {
            return _children ?? NoChildren;
        }

        public bool Equals(TrieNode<TValue, TData> other)
        {
            return other != null && (ReferenceEquals(this, other) || Id == other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TrieNode<TValue, TData>);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
