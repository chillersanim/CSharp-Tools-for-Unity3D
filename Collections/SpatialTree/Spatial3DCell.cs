// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Spatial3DCell.cs
// 
// Created:          05.08.2019  11:26
// Last modified:    16.08.2019  16:31
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

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Tools.Collections.SpatialTree
{
    /// <summary>
    ///     The spatial 3 d cell.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class Spatial3DCell<T>
    {
        /// <summary>
        ///     The max cell pool items.
        /// </summary>
        private const int MaxCellPoolItems = 1024;

        /// <summary>
        ///     The max child array pool items.
        /// </summary>
        private const int MaxChildArrayPoolItems = 128;

        /// <summary>
        ///     Gets the amount of columns to create per axis per subdivision.
        /// </summary>
        public const int SubdivisionAmount = 3;

        /// <summary>
        ///     The maximum amount of children a <see cref="Spatial3DCell{T}" /> can have before it is being subdivided.
        /// </summary>
        private const int MaximumChildren = 128;

        /// <summary>
        ///     Gets the maximum depth a <see cref="Spatial3DTree{T}" /> can reach, this property is stronger than the
        ///     <see cref="MaximumChildren" /> amount.
        /// </summary>
        private const byte MaximumDepth = 16;

        /// <summary>
        ///     The minimum amount of children a <see cref="Spatial3DCell{T}" /> can have before it is being merged.
        /// </summary>
        private const int MinimumChildren = 52;

        /// <summary>
        ///     The cell pool.
        /// </summary>
        private static readonly Stack<Spatial3DCell<T>> CellPool;

        /// <summary>
        ///     The child array pool.
        /// </summary>
        private static readonly Stack<IList<Spatial3DCell<T>>> ChildArrayPool;

        /// <summary>
        ///     The items.
        /// </summary>
        [NotNull] public readonly List<ItemEntry> Items;

        /// <summary>
        ///     The children.
        /// </summary>
        [CanBeNull] public Spatial3DCell<T>[] Children;

        /// <summary>
        ///     Gets or sets the size of the bounding box of this cell.
        /// </summary>
        public Vector3 Size;

        /// <summary>
        ///     Gets or sets the start vector of the bounding box of this cell.
        /// </summary>
        public Vector3 Start;

        /// <summary>
        ///     The total amount of items in this cell and its child cells.
        /// </summary>
        public int TotalItemAmount;

        /// <summary>
        ///     Initializes static members of the <see cref="Spatial3DCell" /> class.
        /// </summary>
        static Spatial3DCell()
        {
            CellPool = new Stack<Spatial3DCell<T>>();
            ChildArrayPool = new Stack<IList<Spatial3DCell<T>>>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spatial3DCell{T}" /> class.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <param name="size">
        ///     The size.
        /// </param>
        public Spatial3DCell(Vector3 start, Vector3 size)
        {
            Start = start;
            Size = size;
            Children = null;
            Items = new List<ItemEntry>();
            TotalItemAmount = 0;
        }

        /// <summary>
        ///     The center.
        /// </summary>
        public Vector3 Center => Start + Size / 2f;

        /// <summary>
        ///     The end.
        /// </summary>
        public Vector3 End => Start + Size;

        /// <summary>
        ///     The get cell.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <param name="size">
        ///     The size.
        /// </param>
        /// <param name="addChildren">Should the child array be attached.</param>
        /// <returns>
        ///     The <see cref="Spatial3DCell" />.
        /// </returns>
        [NotNull]
        public static Spatial3DCell<T> GetCell(Vector3 start, Vector3 size, bool addChildren = false)
        {
            Spatial3DCell<T> cell;

            if (CellPool.Count > 0)
            {
                cell = CellPool.Pop();
                cell.Start = start;
                cell.Size = size;
            }
            else
            {
                cell = new Spatial3DCell<T>(start, size);
            }

            if (addChildren) cell.Children = GetChildArray();

            return cell;
        }

        /// <summary>
        ///     The pool.
        /// </summary>
        /// <param name="cell">
        ///     The cell.
        /// </param>
        public static void Pool([NotNull] Spatial3DCell<T> cell)
        {
            if (CellPool.Count >= MaxCellPoolItems) return;

            if (cell.Children != null)
            {
                Pool(cell.Children);
                cell.Children = null;
            }

            cell.Items.Clear();
            cell.TotalItemAmount = 0;
            CellPool.Push(cell);
        }

        /// <summary>
        ///     The get child array.
        /// </summary>
        /// <returns>
        ///     The <see cref="Spatial3DCell" />.
        /// </returns>
        [NotNull]
        private static Spatial3DCell<T>[] GetChildArray()
        {
            if (ChildArrayPool.Count > 0) return (Spatial3DCell<T>[]) ChildArrayPool.Pop();

            return new Spatial3DCell<T>[SubdivisionAmount * SubdivisionAmount * SubdivisionAmount];
        }

        /// <summary>
        ///     The pool.
        /// </summary>
        /// <param name="childArray">
        ///     The child array.
        /// </param>
        private static void Pool([NotNull] Spatial3DCell<T>[] childArray)
        {
            if (ChildArrayPool.Count >= MaxChildArrayPoolItems) return;

            for (var i = 0; i < childArray.Length; i++)
                if (childArray[i] != null)
                {
                    Pool(childArray[i]);
                    childArray[i] = null;
                }

            ChildArrayPool.Push(childArray);
        }

        /// <summary>
        ///     Adds the item at the given position.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="depth">
        ///     The depth.
        /// </param>
        public void AddItem(T item, Vector3 position, byte depth)
        {
            TotalItemAmount++;

            if (Children == null)
            {
                Items.Add(new ItemEntry(item, position));

                if (Items.Count >= MaximumChildren && depth < MaximumDepth) SubdivideSelf(depth);
            }
            else
            {
                var containerIndex = GetChildCellIndex(position);
                var container = Children[containerIndex];
                if (container == null)
                {
                    container = CreateContainer(containerIndex);
                    Children[containerIndex] = container;
                }

                container.AddItem(item, position, ++depth);
            }
        }

        /// <summary>
        ///     Determines whether this cell contains the item at the specified position.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the item is contained; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item, Vector3 position)
        {
            if (Children == null)
            {
                for (var i = 0; i < Items.Count; i++)
                    if (ReferenceEquals(Items[i].Item, item) && Items[i].Position == position)
                        return true;
            }
            else
            {
                var containerIndex = GetChildCellIndex(position);
                var container = Children[containerIndex];
                if (container != null && container.Contains(item, position)) return true;
            }

            return false;
        }

        /// <summary>
        ///     The get cell count.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int GetCellCount()
        {
            if (Children == null) return 1;

            var total = 1;

            foreach (var child in Children)
                if (child != null)
                    total += child.GetCellCount();

            return total;
        }

        /// <summary>
        ///     The get depth.
        /// </summary>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        public int GetDepth()
        {
            if (Children == null) return 1;

            var max = 0;

            foreach (var child in Children)
                if (child != null)
                    max = Mathf.Max(max, child.GetDepth());

            return 1 + max;
        }

        /// <summary>
        ///     Moves the item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="from">
        ///     The original position.
        /// </param>
        /// <param name="to">
        ///     The new position.
        /// </param>
        /// <param name="depth">
        ///     The current depth of this cell in the tree.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool MoveItem(T item, Vector3 from, Vector3 to, byte depth)
        {
            if (Children == null)
            {
                for (var i = 0; i < Items.Count; i++)
                    if (ReferenceEquals(Items[i].Item, item) && Items[i].Position == from)
                    {
                        Items[i] = new ItemEntry(item, to);
                        return true;
                    }

                return false;
            }

            var oldContainerIndex = GetChildCellIndex(from);
            var newContainerIndex = GetChildCellIndex(to);

            if (oldContainerIndex == newContainerIndex)
            {
                var container = Children[oldContainerIndex];
                if (container != null) return Children[oldContainerIndex].MoveItem(item, from, to, ++depth);

                return false;
            }

            if (RemoveItem(item, from))
            {
                AddItem(item, to, depth);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes the item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the item was successfully removed, <c>false</c> otherwise.
        /// </returns>
        public bool RemoveItem(T item, Vector3 position)
        {
            if (Children == null)
            {
                for (var i = 0; i < Items.Count; i++)
                    if (ReferenceEquals(Items[i].Item, item) && Items[i].Position == position)
                    {
                        Items.RemoveAt(i);
                        TotalItemAmount--;
                        return true;
                    }
            }
            else
            {
                var containerIndex = GetChildCellIndex(position);
                var container = Children[containerIndex];
                if (container != null && container.RemoveItem(item, position))
                {
                    TotalItemAmount--;
                    if (TotalItemAmount < MinimumChildren)
                    {
                        MergeChildren();
                    }
                    else if (container.TotalItemAmount <= 0)
                    {
                        Pool(container);
                        Children[containerIndex] = null;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     The create container.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The <see cref="Spatial3DCell" />.
        /// </returns>
        [Pure]
        private Spatial3DCell<T> CreateContainer(int index)
        {
            var x = index % SubdivisionAmount;
            var y = index / SubdivisionAmount % SubdivisionAmount;
            var z = index / (SubdivisionAmount * SubdivisionAmount);
            var size = Size / SubdivisionAmount;

            return GetCell(Start + new Vector3(x * size.x, y * size.y, z * size.z), size);
        }

        /// <summary>
        ///     Gets the index of the child cell that contains the given position.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     The index in the <see cref="Children" /> collection of the cell.
        /// </returns>
        [Pure]
        private int GetChildCellIndex(Vector3 position)
        {
            var size = Size / SubdivisionAmount;
            var x = (int) ((position.x - Start.x) / size.x);
            var y = (int) ((position.y - Start.y) / size.y);
            var z = (int) ((position.z - Start.z) / size.z);

            return x + SubdivisionAmount * y + SubdivisionAmount * SubdivisionAmount * z;
        }

        /// <summary>
        ///     The merge children.
        /// </summary>
        private void MergeChildren()
        {
            Debug.Assert(Children != null, "this.Children != null");

            // ReSharper disable once PossibleNullReferenceException
            foreach (var child in Children)
                if (child != null)
                {
                    foreach (var item in child.Items) Items.Add(item);

                    child.Items.Clear();
                }

            Pool(Children);
            Children = null;
        }

        /// <summary>
        ///     The subdivide self.
        /// </summary>
        /// <param name="depth">
        ///     The depth.
        /// </param>
        private void SubdivideSelf(byte depth)
        {
            Children = GetChildArray();

            foreach (var item in Items)
            {
                var containerIndex = GetChildCellIndex(item.Position);
                var container = Children[containerIndex];
                if (container == null)
                {
                    container = CreateContainer(containerIndex);
                    Children[containerIndex] = container;
                }

                container.AddItem(item.Item, item.Position, ++depth);
            }

            Items.Clear();
        }

        /// <summary>
        ///     The item entry.
        /// </summary>
        public struct ItemEntry
        {
            /// <summary>
            ///     The item.
            /// </summary>
            public readonly T Item;

            /// <summary>
            ///     The position.
            /// </summary>
            public readonly Vector3 Position;

            /// <summary>
            ///     Initializes a new instance of the <see cref="ItemEntry" /> struct.
            /// </summary>
            /// <param name="item">
            ///     The item.
            /// </param>
            /// <param name="position">
            ///     The position.
            /// </param>
            public ItemEntry(T item, Vector3 position)
            {
                Item = item;
                Position = position;
            }
        }
    }
}