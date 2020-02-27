// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Simple3DSphereCollection.cs
// 
// Created:          24.08.2019  23:15
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

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityTools.Core;
using UnityEngine;

namespace UnityTools.Collections
{
    /// <summary>
    /// Reference implementation for <see cref="ISphere3DCollection{T}"/>.<br/>
    /// For small amount of items, this collection may very well be the fastest solution.<br/>
    /// This implementation can be used to test other implementations, it is very simple, thus good as a reference.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    public class Simple3DSphereCollection<T> : ISphere3DCollection<T>
    {
        [NotNull]
        private readonly List<ItemEntry> items;

        private readonly List<T> searchCache;

        public Simple3DSphereCollection()
        {
            this.items = new List<ItemEntry>();
            this.searchCache = new List<T>();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item.Item;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public int Count => items.Count;

        /// <inheritdoc/>
        public void Add(T item, Vector3 position, float radius)
        {
            if (this.Contains(item, position, radius))
            {
                return;
            }

            items.Add(new ItemEntry(item, position, radius));
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.items.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item, Vector3 position, float radius)
        {
            foreach (var entry in items)
            {
                if (entry.Equals(item, position, radius))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool MoveAndResizeItem(T item, Vector3 oldPosition, float oldRadius,Vector3 newPosition, float newRadius)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var entry = items[i];
                if (entry.Equals(item, oldPosition, oldRadius))
                {
                    items[i] = new ItemEntry(item, newPosition, newRadius);
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Remove(T item, Vector3 position, float radius)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var entry = items[i];
                if (entry.Equals(item, position, radius))
                {
                    items[i] = items[items.Count - 1];
                    items.RemoveAt(items.Count - 1);

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public T[] FindInRadius(Vector3 center, float radius)
        {
            searchCache.Clear();

            foreach (var entry in items)
            {
                var sqrDist = (center - entry.Position).sqrMagnitude;
                var combinedRadius = radius + entry.Radius;
                if (sqrDist <= combinedRadius * combinedRadius)
                {
                    searchCache.Add(entry.Item);
                }
            }

            return searchCache.ToArray();
        }

        /// <inheritdoc/>
        public T[] FindInBounds(Bounds bounds)
        {
            searchCache.Clear();

            foreach (var entry in items)
            {
                var sqrDist = bounds.SqrDistance(entry.Position);
                if (sqrDist <= entry.Radius * entry.Radius)
                {
                    searchCache.Add(entry.Item);
                }
            }

            return searchCache.ToArray();
        }

        private struct ItemEntry
        {
            public readonly T Item;

            public readonly Vector3 Position;

            public readonly float Radius;

            public bool Equals(T other, Vector3 position, float radius)
            {
                return this.Position == position && Mathf.Approximately(this.Radius, radius) && Equals(this.Item, other);
            }

            public ItemEntry(T item, Vector3 position, float radius)
            {
                this.Item = item;
                this.Position = position;
                this.Radius = radius;
            }
        }
    }
}
