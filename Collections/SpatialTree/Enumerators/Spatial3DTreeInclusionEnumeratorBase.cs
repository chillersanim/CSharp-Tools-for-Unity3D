// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Spatial3DTreeInclusionEnumeratorBase.cs
// 
// Created:          24.10.2019  18:16
// Last modified:    25.10.2019  11:38
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

namespace Unity_Tools.Collections.SpatialTree.Enumerators
{
    /// <summary>
    ///     The spatial 3 d tree inclusion enumerator base.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public abstract class Spatial3DTreeInclusionEnumeratorBase<T> : IEnumerator<T>, IEnumerable<T>
    {
        /// <summary>
        ///     The path.
        /// </summary>
        [NotNull] private readonly PathEntry[] path;

        /// <summary>
        ///     The tree.
        /// </summary>
        [NotNull] private readonly Spatial3DTree<T> tree;

        private int pathDepth;

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
            this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
            path = new PathEntry[64];  // It's near impossible that a path depth of 64 will ever be reached, as the max depth in the tree is 16, and more depth can only be added by growing the tree.
            path[0] = new PathEntry(tree.Root, -1, IsBoundsFullyInside(tree.Root.Start, tree.Root.End));
            pathDepth = 1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (MoveNext())
            {
                yield return Current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            if (pathDepth == 0) return false;

            var c = path[pathDepth - 1];
            var ci = c.Index;
            var cc = c.Cell;
            var cf = c.FullyInside;

            while (true)
            {
                ci++;

                if (cc.Children == null)
                {
                    if (cc.Items.Count > ci)
                    {
                        if (cf || IsPointInside(cc.Items[ci].Position))
                        {
                            Current = cc.Items[ci].Item;
                            path[pathDepth - 1] = new PathEntry(cc, ci, cf);
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
                        if (child != null && (cf || IsBoundsIntersecting(child.Start, child.End)))
                        {
                            var childCf = cf;
                            if (!childCf)
                            {
                                childCf = IsBoundsFullyInside(child.Start, child.End);
                            }

                            path[pathDepth - 1] = new PathEntry(cc, ci, cf);
                            path[pathDepth] = new PathEntry(child, -1, childCf);
                            pathDepth++;
                            ci = -1; 
                            cc = child;
                            cf = childCf;
                        }

                        continue;
                    }
                }

                // Go one layer up
                pathDepth--;
                if (pathDepth == 0) return false;

                c = path[pathDepth - 1];
                ci = c.Index;
                cc = c.Cell;
                cf = c.FullyInside;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            path[0] = new PathEntry(tree.Root, -1, IsBoundsFullyInside(tree.Root.Start, tree.Root.End));
            pathDepth = 1;
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
        [Pure]
        protected abstract bool IsBoundsIntersecting(Vector3 start, Vector3 end);

        /// <summary>
        ///     Determines whether the point is inside the search area.
        /// </summary>
        /// <param name="point">
        ///     The point.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the point is inside; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        protected abstract bool IsPointInside(Vector3 point);

        [Pure]
        private bool IsBoundsFullyInside(Vector3 start, Vector3 end)
        {
            var size = end - start;

            return IsPointInside(start) &&
                   IsPointInside(end) &&
                   IsPointInside(new Vector3(start.x + size.x, start.y, start.z)) &&
                   IsPointInside(new Vector3(start.x, start.y + size.y, start.z)) &&
                   IsPointInside(new Vector3(start.x, start.y, start.z + size.z)) &&
                   IsPointInside(new Vector3(end.x - size.x, end.y, end.z)) &&
                   IsPointInside(new Vector3(end.x, end.y - size.y, end.z)) &&
                   IsPointInside(new Vector3(end.x, end.y, end.z - size.z));
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

            public readonly bool FullyInside;

            /// <summary>
            ///     Initializes a new instance of the <see cref="PathEntry" /> struct.
            /// </summary>
            /// <param name="cell">
            ///     The cell.
            /// </param>
            /// <param name="index">
            ///     The index.
            /// </param>
            public PathEntry(Spatial3DCell<T> cell, int index, bool fullyInside)
            {
                Cell = cell;
                Index = index;
                FullyInside = fullyInside;
            }
        }
    }
}