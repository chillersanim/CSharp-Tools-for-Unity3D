﻿// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Spatial3DTreeInclusionEnumeratorBase.cs
// 
// Created:          12.08.2019  19:08
// Last modified:    20.08.2019  21:49
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
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Tools.Collections.SpatialTree
{
    /// <summary>
    ///     The spatial 3 d tree inclusion enumerator base.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public abstract class Spatial3DTreeInclusionEnumeratorBase<T> : IEnumerator<T>
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
        ///     Initializes a new instance of the <see cref="Spatial3DTreeInclusionEnumeratorBase{T}" /> class.
        /// </summary>
        /// <param name="tree">
        ///     The tree.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        protected Spatial3DTreeInclusionEnumeratorBase([NotNull] Spatial3DTree<T> tree)
        {
            if (tree == null) throw new ArgumentNullException("tree");

            this.tree = tree;
            path = new List<PathEntry>(4);
            path.Add(new PathEntry(tree.Root, -1));
        }

        /// <inheritdoc />
        public T Current { get; private set; }

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public virtual void Dispose()
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
                        if (IsPointInside(cc.Items[ci].Position))
                        {
                            Current = cc.Items[ci].Item;
                            path[path.Count - 1] = new PathEntry(cc, ci);
                            return true;
                        }

                        continue;
                    }
                }
                else
                {
                    if (cc.Children.Length > ci)
                    {
                        var child = cc.Children[ci];
                        if (child != null && IsAabbIntersecting(child.Start, child.End))
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
        ///     Determines whether the axis aligned bounding box (aabb) is inside the search area.
        /// </summary>
        /// <param name="start">
        ///     The start of the aabb.
        /// </param>
        /// <param name="end">
        ///     The end of the aabb.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the aabb is inside; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsAabbIntersecting(Vector3 start, Vector3 end);

        /// <summary>
        ///     Determines whether the point is inside the search area.
        /// </summary>
        /// <param name="point">
        ///     The point.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the point is inside; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsPointInside(Vector3 point);

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