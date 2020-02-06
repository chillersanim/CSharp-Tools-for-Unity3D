// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         AvlNode.cs
// 
// Created:          27.01.2020  20:36
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
using JetBrains.Annotations;

namespace Unity_Tools.Collections.Internals
{
    internal sealed class AvlNode<TKey, TValue> where TKey : IComparable<TKey>
    {
        #region Constructors

        public AvlNode([NotNull]TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
            this.height = 1;
        }

        #endregion

        #region Fields

        private readonly TKey key;

        private TValue value;

        private int height;

        private AvlNode<TKey, TValue> left;

        private AvlNode<TKey, TValue> right;

        #endregion

        #region Public Methods

        public bool Add([NotNull]TKey key, TValue value, out AvlNode<TKey, TValue> node)
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

                this.left = new AvlNode<TKey, TValue>(key, value);
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

                this.right = new AvlNode<TKey, TValue>(key, value);
            }

            // Only reached when the new node was added to an empty place
            this.height = 2;
            node = this;
            return true;
        }

        public AvlNode<TKey, TValue> AddOrReplace([NotNull]TKey key, TValue value)
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
                    this.left = new AvlNode<TKey, TValue>(key, value);
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
                    this.right = new AvlNode<TKey, TValue>(key, value);
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

        public bool Remove([NotNull]TKey key, out AvlNode<TKey, TValue> node)
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

        private static int BalanceFactor(AvlNode<TKey, TValue> avlNode)
        {
            if (avlNode == null)
            {
                return 0;
            }

            return Height(avlNode.left) - Height(avlNode.right);
        }

        private static int CalculateHeight(AvlNode<TKey,TValue> avlNode)
        {
            return Math.Max(Height(avlNode.left), Height(avlNode.right)) + 1;
        }

        private static int Height(AvlNode<TKey,TValue> avlNode)
        {
            if (avlNode == null)
            {
                return 0;
            }

            return avlNode.height;
        }

        private static AvlNode<TKey,TValue> LeftRightRotation(AvlNode<TKey,TValue> avlNode)
        {
            avlNode.left = LeftRotation(avlNode.left);
            return RightRotation(avlNode);
        }

        private static AvlNode<TKey,TValue> LeftRotation(AvlNode<TKey,TValue> avlNode)
        {
            var top = avlNode.right;

            avlNode.right = top.left;
            top.left = avlNode;

            avlNode.height = CalculateHeight(avlNode);
            top.height = CalculateHeight(top);

            return top;
        }

        private static AvlNode<TKey,TValue> MinValueNode(AvlNode<TKey,TValue> avlNode)
        {
            var current = avlNode;

            /* loop down to find the leftmost leaf */
            while (current.left != null)
            {
                current = current.left;
            }

            return current;
        }

        private static AvlNode<TKey,TValue> Rebalance(AvlNode<TKey,TValue> root)
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

        private static AvlNode<TKey,TValue> RightLeftRotation(AvlNode<TKey,TValue> avlNode)
        {
            avlNode.right = RightRotation(avlNode.right);
            return LeftRotation(avlNode);
        }

        private static AvlNode<TKey,TValue> RightRotation(AvlNode<TKey,TValue> avlNode)
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