// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         AvlTree.cs
// 
// Created:          19.08.2019  12:34
// Last modified:    05.02.2020  19:39
// 
// --------------------------------------------------------------------------------------
// 
// MIT License
// 
// Copyright (c) 2019 chillersanim
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityTools.Collections
{
    /// <summary>
    /// The AVL tree with integer keys.
    /// </summary>
    [Serializable]
    public class AvlTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        #region Fields

        [SerializeField]
        private AvlNode root;

        private int count;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the value that belongs to the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the value.</returns>
        public TValue this[TKey key]
        {
            get
            {
                if (this.root != null && this.root.TryGet(key, out var value))
                {
                    return value;
                }

                throw new ArgumentException("The key isn't part of this collection.", nameof(key));
            }
        }

        /// <summary>
        /// Gets the amount of key-values pairs stored in this tree.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Tries to add the key-value pair to the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns <c>true</c> when the key-value pair could be added, <c>false</c> otherwise.</returns>
        public bool Add(TKey key, TValue value)
        {
            if (this.root != null)
            {
                return this.root.Add(key, value, out this.root);
            }

            this.root = new AvlNode(key, value);
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the given key exists in the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns <c>true</c> if the key is part of the collection, <c>false</c> otherwise.</returns>
        public bool ContainsKey(TKey key)
        {
            return (this.root != null) && this.root.ContainsKey(key);
        }

        /// <summary>
        /// Tries to remove the key and its associated value from the collection.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns <c>true</c> when the key and value could be removed, <c>false</c> otherwise.</returns>
        public bool Remove(TKey key)
        {
            if (this.root != null)
            {
                return this.root.Remove(key, out this.root);
            }

            return false;
        }

        /// <summary>
        /// Adds the value or replaces an existing value with the new value for the corresponding key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetValue(TKey key, TValue value)
        {
            if (this.root != null)
            {
                this.root = this.root.AddOrReplace(key, value);
            }
            else
            {
                this.root = new AvlNode(key, value);
            }
        }

        /// <summary>
        /// Tries to get the value that belongs to the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">Outputs the value if found. The value is undefined if the key was not found.</param>
        /// <returns>Returns <c>true</c> if the key was found, <c>false</c> otherwise.</returns>
        public bool TryGet(TKey key, out TValue value)
        {
            if (this.root != null)
            {
                return this.root.TryGet(key, out value);
            }

            value = default;
            return false;
        }

        #endregion

        [Serializable]
        private sealed class AvlNode 
        { 
            #region Constructors

            public AvlNode()
            { }

            public AvlNode([NotNull]TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
                this.height = 1;
            }

            #endregion

            #region Fields

            [SerializeField]
            private TKey key;

            [SerializeField]
            private TValue value;

            [SerializeField]
            private int height;

            [SerializeField]
            private AvlNode left;

            [SerializeField]
            private AvlNode right;

            #endregion

            #region Public Methods

            public bool Add([NotNull]TKey key, TValue value, out AvlNode node)
            {
                var compare = key.CompareTo(this.key);

                if (compare == 0)
                {
                    node = this;
                    return false;
                }

                if (compare < 0)
                {
                    if (this.left != null)
                    {
                        var result = this.left.Add(key, value, out node);

                        // Check whether rebalance is necessary and update height
                        if (result)
                        {
                            this.left = node;
                            node = Rebalance(this);
                            node.height = CalculateHeight(node);
                        }

                        return result;
                    }

                    this.left = new AvlNode(key, value);
                }
                else
                {
                    if (this.right != null)
                    {
                        var result = this.right.Add(key, value, out node);

                        if (result)
                        {
                            this.right = node;
                            node = Rebalance(this);
                            node.height = CalculateHeight(node);
                        }

                        return result;
                    }

                    this.right = new AvlNode(key, value);
                }

                // Only reached when the new node was added to an empty place
                this.height = 2;
                node = this;
                return true;
            }

            public AvlNode AddOrReplace([NotNull]TKey key, TValue value)
            {
                var compare = key.CompareTo(this.key);

                if (compare == 0)
                {
                    this.value = value;
                    return this;
                }

                if (compare < 0)
                {
                    if (this.left != null)
                    {
                        this.left = this.left.AddOrReplace(key, value);

                        // Check whether rebalance is necessary and update height
                        var node = Rebalance(this);
                        node.height = CalculateHeight(node);
                        return node;
                    }
                    else
                    {
                        this.left = new AvlNode(key, value);
                        this.height = 2;
                        return this;
                    }
                }
                else
                {
                    if (this.right != null)
                    {
                        this.right = this.right.AddOrReplace(key, value);

                        var node = Rebalance(this);
                        node.height = CalculateHeight(node);
                        return node;
                    }
                    else
                    {
                        this.right = new AvlNode(key, value);
                        this.height = 2;
                        return this;
                    }
                }
            }

            public bool ContainsKey([NotNull]TKey key)
            {
                var current = this;

                while (true)
                {
                    var compare = key.CompareTo(current.key);

                    if (compare == 0)
                    {
                        return true;
                    }

                    if (compare < 0)
                    {
                        if (current.left != null)
                        {
                            current = current.left;
                            continue;
                        }
                    }
                    else
                    {
                        if (current.right != null)
                        {
                            current = current.right;
                            continue;
                        }
                    }

                    return false;
                }
            }

            public bool Remove([NotNull]TKey key, out AvlNode node)
            {
                var compare = key.CompareTo(this.key);

                if (compare == 0)
                {
                    if (this.left == null)
                    {
                        node = this.right;
                    }
                    else
                    {
                        if (this.right == null)
                        {
                            node = this.left;
                        }
                        else
                        {
                            var top = MinValueNode(this.right);
                            this.right.Remove(top.key, out top);
                            top.left = this.left;
                            top.right = this.right;
                            top = Rebalance(top);
                            top.height = CalculateHeight(top);
                            node = top;
                        }
                    }

                    return true;
                }

                if (compare < 0)
                {
                    if (this.left != null)
                    {
                        var result = this.left.Remove(key, out node);
                        if (result)
                        {
                            this.left = node;
                            node = Rebalance(this);
                            node.height = CalculateHeight(node);
                        }

                        return result;
                    }

                    node = this;
                    return false;
                }

                if (this.right != null)
                {
                    var result = this.right.Remove(key, out node);
                    if (result)
                    {
                        this.right = node;
                        node = Rebalance(this);
                        node.height = CalculateHeight(node);
                    }

                    return result;
                }

                node = this;
                return false;
            }

            public bool TryGet([NotNull]TKey key, out TValue value)
            {
                var current = this;

                while (true)
                {
                    var compare = key.CompareTo(current.key);

                    if (compare == 0)
                    {
                        value = current.value;
                        return true;
                    }

                    if (compare < 0)
                    {
                        if (current.left != null)
                        {
                            current = current.left;
                            continue;
                        }
                    }
                    else
                    {
                        if (current.right != null)
                        {
                            current = current.right;
                            continue;
                        }
                    }

                    value = default;
                    return false;
                }
            }

            #endregion

            #region Private Methods

            private static int BalanceFactor(AvlNode avlNode)
            {
                if (avlNode == null)
                {
                    return 0;
                }

                return Height(avlNode.left) - Height(avlNode.right);
            }

            private static int CalculateHeight(AvlNode avlNode)
            {
                return Math.Max(Height(avlNode.left), Height(avlNode.right)) + 1;
            }

            private static int Height(AvlNode avlNode)
            {
                if (avlNode == null)
                {
                    return 0;
                }

                return avlNode.height;
            }

            private static AvlNode LeftRightRotation(AvlNode avlNode)
            {
                avlNode.left = LeftRotation(avlNode.left);
                return RightRotation(avlNode);
            }

            private static AvlNode LeftRotation(AvlNode avlNode)
            {
                var top = avlNode.right;

                avlNode.right = top.left;
                top.left = avlNode;

                avlNode.height = CalculateHeight(avlNode);
                top.height = CalculateHeight(top);

                return top;
            }

            private static AvlNode MinValueNode(AvlNode avlNode)
            {
                var current = avlNode;

                /* loop down to find the leftmost leaf */
                while (current.left != null)
                {
                    current = current.left;
                }

                return current;
            }

            private static AvlNode Rebalance(AvlNode root)
            {
                if (root == null)
                {
                    return null;
                }

                var balance = BalanceFactor(root);

                if (balance < -1)
                {
                    // right heavy
                    if (BalanceFactor(root.right) > 0)
                    {
                        return RightLeftRotation(root);
                    }

                    return LeftRotation(root);
                }

                if (balance > 1)
                {
                    // left heavy
                    if (BalanceFactor(root.left) < 0)
                    {
                        return LeftRightRotation(root);
                    }

                    return RightRotation(root);
                }

                return root;
            }

            private static AvlNode RightLeftRotation(AvlNode avlNode)
            {
                avlNode.right = RightRotation(avlNode.right);
                return LeftRotation(avlNode);
            }

            private static AvlNode RightRotation(AvlNode avlNode)
            {
                var top = avlNode.left;

                avlNode.left = top.right;
                top.right = avlNode;

                avlNode.height = CalculateHeight(avlNode);
                top.height = CalculateHeight(top);

                return top;
            }

            #endregion
        }
    }
}