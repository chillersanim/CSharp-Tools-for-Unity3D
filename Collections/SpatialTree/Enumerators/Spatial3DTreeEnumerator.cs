// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Spatial3DTreeEnumerator.cs
// 
// Created:          16.08.2019  16:33
// Last modified:    16.08.2019  16:56
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
// 

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Unity_Tools.Collections.SpatialTree.Enumerators
{
    /// <summary>
    ///     The spatial 3 d tree enumerator.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public sealed class Spatial3DTreeEnumerator<T> : IEnumerator<T>
    {
        /// <summary>
        ///     The path.
        /// </summary>
        [NotNull] private readonly List<PathEntry> path;

        /// <summary>
        ///     The tree.
        /// </summary>
        [NotNull] private readonly Spatial3DTree<T> tree;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spatial3DTreeEnumerator{T}" /> class.
        /// </summary>
        /// <param name="tree">
        ///     The tree.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public Spatial3DTreeEnumerator([NotNull] Spatial3DTree<T> tree)
        {
            if (tree == null) throw new ArgumentNullException("tree");

            this.tree = tree;
            path = new List<PathEntry>(4) {new PathEntry(tree.Root, -1)};
        }

        /// <inheritdoc />
        public T Current { get; private set; }

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (path.Count == 0) return false;

            var ci = path[path.Count - 1].Index;
            var cc = path[path.Count - 1].Cell;

            while (true)
            {
                ci++;

                if (cc.Children == null)
                {
                    if (cc.Items.Count > ci)
                    {
                        Current = cc.Items[ci].Item;
                        path[path.Count - 1] = new PathEntry(cc, ci);
                        return true;
                    }
                }
                else
                {
                    if (cc.Children.Length > ci)
                    {
                        var child = cc.Children[ci];
                        if (child != null)
                        {
                            path[path.Count - 1] = new PathEntry(cc, ci);
                            path.Add(new PathEntry(child, -1));
                            ci = -1;
                            cc = child;
                        }

                        continue;
                    }
                }

                // Go one layer up
                path.RemoveAt(path.Count - 1);
                if (path.Count == 0) return false;

                ci = path[path.Count - 1].Index;
                cc = path[path.Count - 1].Cell;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            path.Clear();
            path.Add(new PathEntry(tree.Root, -1));
        }

        /// <summary>
        ///     The path entry.
        /// </summary>
        private struct PathEntry
        {
            /// <summary>
            ///     The cell.
            /// </summary>
            public readonly Spatial3DCell<T> Cell;

            /// <summary>
            ///     The index.
            /// </summary>
            public readonly int Index;

            /// <summary>
            ///     Initializes a new instance of the <see cref="PathEntry" /> struct.
            /// </summary>
            /// <param name="cell">
            ///     The cell.
            /// </param>
            /// <param name="index">
            ///     The index.
            /// </param>
            public PathEntry(Spatial3DCell<T> cell, int index)
            {
                Cell = cell;
                Index = index;
            }
        }
    }
}