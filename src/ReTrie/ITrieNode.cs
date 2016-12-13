namespace ReTrie
{
    public interface ITrieNode<TData>
    {
        long Id { get; }
        bool HasData { get; }
        TData Data { get; set; }
    }
}
