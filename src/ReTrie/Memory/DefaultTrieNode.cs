namespace ReTrie.Memory
{
    internal class DefaultTrieNode<TData, TValue> : ITrieNode<TData, TValue>
    {
        private readonly IMemoryStrategy<TData, TValue> _memory;

        private bool _hasValue;
        private TValue _value;

        public bool HasChildren => _memory.Count > 0;

        public bool HasValue => _hasValue;

        public TValue Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _hasValue = !Equals(value, default(TValue));
            }
        }

        public DefaultTrieNode()
        {
        }

        public DefaultTrieNode(IMemoryStrategy<TData, TValue> memory)
        {
            _memory = memory;
        }

        public ITrieNode<TData, TValue> AddOrGet(TData data)
        {
            return _memory.Get(data) ?? _memory.Allocate(data);
        }

    }
}
