// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Spatial3DTree.cs
// 
// Created:          12.08.2019  19:04
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
using Unity_Tools.Collections.SpatialTree;
using Unity_Tools.Collections.SpatialTree.Enumerators;
using Unity_Tools.Core;
using UnityEngine;

namespace Unity_Tools.Collections
{
    /// <summary>
    ///     The spatial 3 d tree.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class Spatial3DTree<T> : I3DCollection<T>
    {
        private readonly AabbCastEnumerator<T> aabbCaster;

        private readonly List<T> castCache;

        /// <summary>
        ///     The initial offset.
        /// </summary>
        private readonly Vector3 center;

        /// <summary>
        ///     The initial size.
        /// </summary>
        private readonly Vector3 initialSize;

        private readonly SphereCastEnumerator<T> sphereCaster;

        /// <summary>
        ///     The root.
        /// </summary>
        [NotNull] private Spatial3DCell<T> root;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spatial3DTree{T}" /> class.
        /// </summary>
        public Spatial3DTree() : this(new Vector3(3, 3, 3), Vector3.zero)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Spatial3DTree{T}" /> class.
        /// </summary>
        /// <param name="size">The initial size of the root.</param>
        /// <param name="center">The center of the root.</param>
        public Spatial3DTree(Vector3 size, Vector3 center)
        {
            initialSize = size;
            this.center = center;
            root = Spatial3DCell<T>.GetCell(this.center - initialSize / 2f, initialSize, false);
            castCache = new List<T>();

            this.sphereCaster = new SphereCastEnumerator<T>(this, this.center, 0f);
            this.aabbCaster = new AabbCastEnumerator<T>(this, this.center, this.center);
        }

        public Vector3 Center{
            get { return center; }
        }

        public Vector3 InitialSize
        {
            get { return initialSize; }
        }

        [NotNull] public Spatial3DCell<T> Root => root;

        /// <summary>
        ///     The depth.
        /// </summary>
        public int Depth => root.GetDepth();

        /// <summary>
        ///     The total cell count.
        /// </summary>
        public int TotalCellCount => root.GetCellCount();

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
            return new Spatial3DTreeEnumerator<T>(this);
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
        public void Add([NotNull]T item, Vector3 position)
        {
            if (Equals(item, null))
            {
                throw new ArgumentNullException(nameof(item));
            }

            while (!FitsInRoot(position)) Grow();

            root.AddItem(item, position, 1);
        }

        /// <summary>
        ///     The clear.
        /// </summary>
        public void Clear()
        {
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
        public bool Contains([CanBeNull]T item, Vector3 position)
        {
            if (Equals(item, null))
            {
                return false;
            }

            return root.Contains(item, position);
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
        public bool MoveItem([NotNull]T item, Vector3 from, Vector3 to)
        {
            if (Equals(item, null))
            {
                throw new ArgumentNullException(nameof(item));
            }

            while (!FitsInRoot(to)) Grow();

            var result = root.MoveItem(item, from, to, 1);

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
        public bool Remove([CanBeNull]T item, Vector3 position)
        {
            if (Equals(item, null))
            {
                return false;
            }

            var result = root.RemoveItem(item, position);

            while (CanShrink()) Shrink();

            return result;
        }

        public T[] FindInRadius(Vector3 center, float radius)
        {
            sphereCaster.Restart(center, radius);
            castCache.Clear();
            sphereCaster.CopyTo(castCache);
            return castCache.ToArray();
        }

        public T[] FindInAabb(Vector3 center, Vector3 size)
        {
            aabbCaster.Restart(center, size);
            castCache.Clear();
            aabbCaster.CopyTo(castCache);
            return castCache.ToArray();
        }

        public void AddRange([NotNull] IList<T> items, IList<Vector3> positions)
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

            var bounds = positions.Bounds();

            while (!FitsInRoot(bounds.min) || !FitsInRoot(bounds.max))
            {
                Grow();
            }

            var cnt = items.Count;
            for (var i = 0; i < cnt; i++)
            {
                root.AddItem(items[i], positions[i], 1);
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
    }
}