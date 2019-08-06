namespace ReTrie.Memory
{
    /// <summary>
    /// Abstraction interface for providing exchangeable data storage for the Trie data type.
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    /// <see cref="DefaultMemoryStrategy{TK,TV}"/>
    public interface IMemoryStrategy<TK, TV>
    {
        /// <summary>
        /// Generate a new empty node
        /// </summary>
        /// <returns><see cref="ITrieNode{TK,TV}"/></returns>
        ITrieNode<TK, TV> NewNode();
        /// <summary>
        /// Attempt to retrieve a node by its <paramref name="id"/>
        /// </summary>
        /// <param name="id">unique id of the node</param>
        /// <returns><see cref="ITrieNode{TK,TV}"/> the node that has <paramref name="id"/> or null if none found</returns>
        ITrieNode<TK, TV> GetNode(long id);
        /// <summary>
        /// Get the child node id of a parent-key branch relationship
        /// </summary>
        /// <param name="parentId">The id of the parent node (if one exists). Null if this is a root key</param>
        /// <param name="key">The <typeparamref name="TK"/> value of the child node to look for</param>
        /// <returns>Id of the child node, or null if no relationship exists</returns>
        long? GetRelation(long? parentId, TK key);
        /// <summary>
        /// Apply a node to memory, either adding a new node or replacing the existing.
        /// </summary>
        /// <param name="node"><see cref="ITrieNode{TK,TV}"/> to set</param>
        void SetNode(ITrieNode<TK, TV> node);
        /// <summary>
        /// Apply a relationship to memory, either adding the parent/key or updating the existing.
        /// </summary>
        /// <param name="parentId">Id of the parent node, if one exists</param>
        /// <param name="key">The relation branch key</param>
        /// <param name="childId">Id of the child node being related</param>
        void SetRelation(long? parentId, TK key, long childId);
        /// <summary>
        /// Remove a node with the specified id
        /// </summary>
        /// <param name="id">Id of the node to remove</param>
        void RemoveNode(long id);
        /// <summary>
        /// Remove a relationship from memory
        /// </summary>
        /// <param name="parentId">Id of the parent node, if one exists</param>
        /// <param name="key">The relation branch key</param>
        void RemoveRelation(long? parentId, TK key);
    }
}
