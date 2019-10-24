// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         AvlTree.cs
// 
// Created:          19.08.2019  12:34
// Last modified:    25.08.2019  15:58
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
using Unity_Tools.Collections.AvlTree;

namespace Unity_Tools.Collections
{
    /// <summary>
    /// The AVL tree with integer keys.
    /// </summary>
    public class AvlTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        #region Fields

        private AvlNode<TKey,TValue> root;

        #endregion

        #region Public Methods

        public bool Add(TKey key, TValue value)
        {
            if (this.root != null)
            {
                return this.root.Add(key, value, out this.root);
            }

            this.root = new AvlNode<TKey,TValue>(key, value);
            return true;
        }

        public bool ContainsKey(TKey key)
        {
            return (this.root != null) && this.root.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (this.root != null)
            {
                return this.root.Remove(key, out this.root);
            }

            return false;
        }

        public void SetValue(TKey key, TValue value)
        {
            if (this.root != null)
            {
                this.root = this.root.AddOrReplace(key, value);
            }
            else
            {
                this.root = new AvlNode<TKey,TValue>(key, value);
            }
        }

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
    }
}