// Copyright © 2019 Jasper Ermatinger

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Collections.SpatialTree
{
    public abstract class Spatial3DTreeExclusionEnumeratorBase<T> : IEnumerator<T> where T : class
    {
        [NotNull] private readonly List<PathEntry> path;

        [NotNull] private readonly Spatial3DTree<T> tree;

        protected Spatial3DTreeExclusionEnumeratorBase([NotNull] Spatial3DTree<T> tree)
        {
            if (tree == null) throw new ArgumentNullException("tree");

            this.tree = tree;
            path = new List<PathEntry>(4);
            path.Add(new PathEntry(tree.Root, -1));
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
                        if (IsPointOutside(cc.Items[ci].Position))
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
                        if (child != null && IsAabbOutside(child.Start, child.End))
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

        /// <inheritdoc />
        public T Current { get; private set; }

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }

        /// <summary>
        ///     Determines whether the axis aligned bounding box (aabb) is inside the search area.
        /// </summary>
        /// <param name="start">The start of the aabb.</param>
        /// <param name="end">The end of the aabb.</param>
        /// <returns><c>true</c> if the aabb is inside; otherwise, <c>false</c>.</returns>
        protected abstract bool IsAabbOutside(Vector3 start, Vector3 end);

        /// <summary>
        ///     Determines whether the point is inside the search area.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns><c>true</c> if the point is inside; otherwise, <c>false</c>.</returns>
        protected abstract bool IsPointOutside(Vector3 point);

        private struct PathEntry
        {
            public readonly Spatial3DCell<T> Cell;

            public readonly int Index;

            public PathEntry(Spatial3DCell<T> cell, int index)
            {
                Cell = cell;
                Index = index;
            }
        }
    }
}