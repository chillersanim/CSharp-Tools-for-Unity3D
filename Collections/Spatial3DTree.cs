// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Spatial3DTree.cs
// 
// Created:          12.08.2019  19:04
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
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityTools.Collections.Internals;
using UnityTools.Core;

namespace UnityTools.Collections
{
    /// <summary>
    ///     The spatial 3 d tree.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class Spatial3DTree<T> : IPoint3DCollection<T>
    {
        /// <summary>
        /// Contains cached cast path arrays for inclusion and exclusion casts
        /// </summary>
        private static readonly List<PathEntry[]> CastPathCache;

        /// <summary>
        /// Field indicating whether duplicate elements are detected and duplicate entries are prevented when adding or moving items (Two item entries are duplicates if item and position are equal)
        /// </summary>
        private readonly bool allowDuplicates;

        /// <summary>
        ///     The initial offset.
        /// </summary>
        private readonly Vector3 center;

        /// <summary>
        ///     The initial size.
        /// </summary>
        private readonly Vector3 initialSize;

        /// <summary>
        ///     The root.
        /// </summary>
        [NotNull] private Spatial3DCell<T> root;

        private int version;

        static Spatial3DTree()
        {
            CastPathCache = new List<PathEntry[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spatial3DTree{T}" /> class.
        /// <param name="allowDuplicates">A value indicating whether duplicate elements are detected and duplicate entries are prevented when adding or moving items (Two item entries are duplicates if item and position are equal)</param>
        /// </summary>
        public Spatial3DTree(bool allowDuplicates = false) : this(new Vector3(3, 3, 3), Vector3.zero, allowDuplicates)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spatial3DTree{T}" /> class.
        /// </summary>
        /// <param name="size">The initial size of the root.</param>
        /// <param name="center">The center of the root.</param>
        /// <param name="allowDuplicates">A value indicating whether duplicate elements are detected and duplicate entries are prevented when adding or moving items (Two items are duplicates if value and position are equal)</param>
        public Spatial3DTree(Vector3 size, Vector3 center, bool allowDuplicates = true)
        {
            this.allowDuplicates = allowDuplicates;
            this.initialSize = size;
            this.center = center;
            root = Spatial3DCell<T>.GetCell(this.center - initialSize / 2f, initialSize, false);
        }

        public Vector3 Center{
            get { return center; }
        }

        /// <summary>
        ///     The depth.
        /// </summary>
        public int Depth => root.GetDepth();

        public Vector3 InitialSize
        {
            get { return initialSize; }
        }

        [NotNull] public Spatial3DCell<T> Root => root;

        /// <summary>
        ///     The total cell count.
        /// </summary>
        public int TotalCellCount => root.GetCellCount();

        /// <summary>
        /// Gets a value indicating whether duplicate elements are detected and duplicate entries are prevented when adding or moving items (Two item entries are duplicates if item and position are equal)
        /// </summary>
        public bool AllowsDuplicates => allowDuplicates;

        /// <summary>
        ///     The count.
        /// </summary>
        public int Count => root.TotalItemAmount;

        /// <summary>
        ///     The get enumerator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in ShapeCast(new VolumeAll()))
            {
                yield return item;
            }
        }

        /// <summary>
        ///     The get enumerator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        public void Add(T item, Vector3 position)
        {
            version++;

            while (!FitsInRoot(position)) Grow();

            root.AddItem(item, position, 1, !allowDuplicates);
        }

        /// <summary>
        ///     The clear.
        /// </summary>
        public void Clear()
        {
            version++;
            Spatial3DCell<T>.Pool(root);
            root = Spatial3DCell<T>.GetCell(center - initialSize / 2, initialSize);
        }

        /// <summary>
        ///     The contains.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Contains(T item, Vector3 position)
        {
            return root.Contains(item, position);
        }

        public IEnumerable<T> ShapeCast<TShape>(TShape shape) where TShape : IVolume
        {
            if (!CastPathCache.TryExtractLast(out var path))
            {
                // A search depth of 64 will probably never be reached, so this is on the safe side.
                path = new PathEntry[64];
            }

            var startVersion = version;
            var pathDepth = 0;
            var cell = root;
            var childIndex = -1;
            var fullyInside = shape.ContainsAabb(root.Start, root.End);

            while (pathDepth >= 0)
            {
                childIndex++;

                if (cell.Children == null)
                {
                    foreach (var item in cell.Items)
                    {
                        if (fullyInside || shape.ContainsPoint(item.Position))
                        {
                            if (startVersion != version)
                            {
                                CastPathCache.Add(path);
                                throw new InvalidOperationException("The enumerator has been modified since the last step and cannot be used anymore.");
                            }

                            yield return item.Item;
                        }
                    }
                }
                else
                {
                    if (childIndex < cell.Children.Length)
                    {
                        var child = cell.Children[childIndex];
                        if (child != null && (fullyInside || shape.IntersectsAabb(child.Start, child.End)))
                        {
                            var childFullyInside = fullyInside;
                            if (!childFullyInside)
                            {
                                childFullyInside = shape.ContainsAabb(child.Start, child.End);
                            }

                            path[pathDepth] = new PathEntry(cell, childIndex, fullyInside);
                            pathDepth++;
                            cell = child;
                            childIndex = -1;
                            fullyInside = childFullyInside;
                        }

                        continue;
                    }
                }

                // Go one layer up
                pathDepth--;
                if (pathDepth < 0) break;

                var c = path[pathDepth];
                cell = c.Cell;
                childIndex = c.ChildIndex;
                fullyInside = c.Flag;
            }

            
            CastPathCache.Add(path);
        }

        public void ShapeCast<TShape>(TShape shape, IList<T> output) where TShape : IVolume
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!CastPathCache.TryExtractLast(out var path))
            {
                // A search depth of 64 will probably never be reached, so this is on the safe side.
                path = new PathEntry[64];
            }

            var pathDepth = 0;
            var cell = root;
            var childIndex = -1;
            var fullyInside = shape.ContainsAabb(root.Start, root.End);

            while (pathDepth >= 0)
            {
                childIndex++;

                if (cell.Children == null)
                {
                    foreach (var item in cell.Items)
                    {
                        if (fullyInside || shape.ContainsPoint(item.Position))
                        {
                            output.Add(item.Item);
                        }
                    }
                }
                else
                {
                    if (childIndex < cell.Children.Length)
                    {
                        var child = cell.Children[childIndex];
                        if (child != null && (fullyInside || shape.IntersectsAabb(child.Start, child.End)))
                        {
                            var childFullyInside = fullyInside;
                            if (!childFullyInside)
                            {
                                childFullyInside = shape.ContainsAabb(child.Start, child.End);
                            }

                            path[pathDepth] = new PathEntry(cell, childIndex, fullyInside);
                            pathDepth++;
                            cell = child;
                            childIndex = -1;
                            fullyInside = childFullyInside;
                        }

                        continue;
                    }
                }

                // Go one layer up
                pathDepth--;
                if (pathDepth < 0) break;

                var c = path[pathDepth];
                cell = c.Cell;
                childIndex = c.ChildIndex;
                fullyInside = c.Flag;
            }


            CastPathCache.Add(path);
        }

        /// <summary>
        ///     The move item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="from">
        ///     The from.
        /// </param>
        /// <param name="to">
        ///     The to.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool MoveItem(T item, Vector3 from, Vector3 to)
        {
            version++;

            while (!FitsInRoot(to)) Grow();

            var result = root.MoveItem(item, from, to, 1, !allowDuplicates);

            while (CanShrink()) Shrink();

            return result;
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Remove(T item, Vector3 position)
        {
            version++;
            var result = root.RemoveItem(item, position);

            while (CanShrink()) Shrink();

            return result;
        }

        public void AddRange([NotNull] IList<T> items, [NotNull]IList<Vector3> positions)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (positions == null)
            {
                throw new ArgumentNullException(nameof(positions));
            }

            if (items.Count != positions.Count)
            {
                throw new ArgumentException("The amount of items doesn't match the amount of positions.");
            }

            if (items.Count == 0)
            {
                return;
            }

            version++;

            var bounds = positions.Bounds();

            while (!FitsInRoot(bounds.min) || !FitsInRoot(bounds.max))
            {
                Grow();
            }

            var cnt = items.Count;
            for (var i = 0; i < cnt; i++)
            {
                root.AddItem(items[i], positions[i], 1, !allowDuplicates);
            }
        }

        /// <summary>
        ///     The can shrink.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool CanShrink()
        {
            if (root.Size.x <= initialSize.x) return false;

            if (root.Children == null)
            {
                var sc = Spatial3DCell<T>.SubdivisionAmount;
                var centerSize = root.Size / sc;
                var centerStart = root.Start + centerSize * (sc / 2);

                for (var i = 0; i < root.Items.Count; i++)
                    if (!FitsInAabb(root.Items[i].Position, centerStart, centerSize))
                        return false;

                return true;
            }

            var middle = root.Children.Length / 2;

            if (root.Children[middle] == null) return false;

            for (var i = 0; i < root.Children.Length; i++)
                if (i != middle && root.Children[i] != null && root.Children[i].TotalItemAmount != 0)
                    return false;

            return true;
        }

        /// <summary>
        ///     The fits in aabb.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <param name="end">
        ///     The end.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool FitsInAabb(Vector3 position, Vector3 start, Vector3 end)
        {
            return start.x <= position.x && start.y <= position.y && start.z <= position.z && end.x > position.x
                   && end.y > position.y && end.z > position.z;
        }

        /// <summary>
        ///     The fits in root.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool FitsInRoot(Vector3 position)
        {
            return FitsInAabb(position, root.Start, root.End);
        }

        /// <summary>
        ///     The grow.
        /// </summary>
        private void Grow()
        {
            const int sc = Spatial3DCell<T>.SubdivisionAmount;
            const int scCenter = sc / 2;

            var start = root.Start - scCenter * root.Size;
            var size = root.Size * sc;

            if (root.Children == null)
            {
                root.Start = start;
                root.Size = size;
            }
            else
            {
                var newRoot = Spatial3DCell<T>.GetCell(start, size, true);
                newRoot.TotalItemAmount = root.TotalItemAmount;
                newRoot.Children[newRoot.Children.Length / 2] = root;
                root = newRoot;
            }
        }

        /// <summary>
        ///     The shrink.
        /// </summary>
        private void Shrink()
        {
            Debug.Assert(CanShrink(), "The tree cannot be shrunk.");

            if (root.Children == null)
            {
                var sc = Spatial3DCell<T>.SubdivisionAmount;
                root.Size = root.Size / sc;
                root.Start = root.Start + root.Size * (sc / 2);
            }
            else
            {
                var middle = root.Children.Length / 2;
                var center = root.Children[middle];
                root.Children[middle] = null;
                Spatial3DCell<T>.Pool(root);
                root = center;

                Debug.Assert(center != null);
            }
        }

        private struct PathEntry
        {
            /// <summary>
            ///     The cell.
            /// </summary>
            public readonly Spatial3DCell<T> Cell;

            /// <summary>
            ///     The childIndex.
            /// </summary>
            public readonly int ChildIndex;

            public readonly bool Flag;

            /// <summary>
            ///     Initializes a new instance of the <see cref="PathEntry" /> struct.
            /// </summary>
            /// <param name="cell">
            ///     The cell.
            /// </param>
            /// <param name="childIndex">
            ///     The childIndex.
            /// </param>
            public PathEntry(Spatial3DCell<T> cell, int childIndex, bool flag)
            {
                Cell = cell;
                ChildIndex = childIndex;
                Flag = flag;
            }
        }
    }
}