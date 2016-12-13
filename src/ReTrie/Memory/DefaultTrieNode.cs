namespace ReTrie.Memory
{
    internal class DefaultTrieNode<TData> : ITrieNode<TData>
    {
        private bool _hasData;
        private TData _data;

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

        public DefaultTrieNode(long id)
        {
            Id = id;
        }
    }
}
