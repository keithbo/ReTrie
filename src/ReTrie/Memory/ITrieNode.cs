namespace ReTrie.Memory
{
    using System;
    using System.Collections.Generic;

    public interface ITrieNode<TK, TV> : IEquatable<TrieNode<TK, TV>>
    {
        /// <summary>
        /// Unique id of the node
        /// </summary>
        long Id { get; }

        /// <summary>
        /// True if this node has a <see cref="Value"/>
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Value of this node
        /// </summary>
        TV Value { get; }

        /// <summary>
        /// Number of mapped child nodes
        /// </summary>
        int ChildCount { get; }

        /// <summary>
        /// Child key branches from this node
        /// </summary>
        IEnumerable<TK> Children { get; }

        /// <summary>
        /// Apply a new value to this node
        /// </summary>
        /// <param name="value"><typeparamref name="TV"/> new value</param>
        /// <returns><see cref="ITrieNode{TK,TV}"/> immutable node with new value</returns>
        ITrieNode<TK, TV> SetValue(TV value);

        /// <summary>
        /// Apply a new child to this node
        /// </summary>
        /// <param name="child"><typeparamref name="TK"/> child branch key</param>
        /// <returns><see cref="ITrieNode{TK,TV}"/> immutable node with new child branch or the same node if no changes happened</returns>
        ITrieNode<TK, TV> AddChild(TK child);

        /// <summary>
        /// Remove an existing child from this node
        /// </summary>
        /// <param name="child"><typeparamref name="TK"/> child branch key</param>
        /// <returns><see cref="ITrieNode{TK,TV}"/> immutable node with child branch removed or the same node if no changes happened</returns>
        ITrieNode<TK, TV> RemoveChild(TK child);
    }
}