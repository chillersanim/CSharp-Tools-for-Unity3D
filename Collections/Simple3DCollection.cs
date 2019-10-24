// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Simple3DCollection.cs
// 
// Created:          12.08.2019  19:04
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

using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity_Tools.Core;
using UnityEngine;
using Unity_Tools.Collections.SpatialTree;

namespace Unity_Tools.Collections
{
    public class Simple3DCollection<T> : IPoint3DCollection<T>
    {
        [NotNull]
        private readonly List<ItemEntry> items;

        private readonly List<T> searchCache;

        public Simple3DCollection()
        {
            this.items = new List<ItemEntry>();
            searchCache = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item.Item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => items.Count;

        public void Add(T item, Vector3 position)
        {
            if (this.Contains(item, position))
            {
                return;
            }

            items.Add(new ItemEntry(item, position));
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(T item, Vector3 position)
        {
            foreach (var entry in items)
            {
                if (entry.Equals(item, position))
                {
                    return true;
                }
            }

            return false;
        }

        public bool MoveItem(T item, Vector3 @from, Vector3 to)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var entry = items[i];
                if (entry.Equals(item, from))
                {
                    items[i] = new ItemEntry(item, to);
                    return true;
                }
            }

            return false;
        }

        public bool Remove(T item, Vector3 position)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var entry = items[i];
                if (entry.Equals(item, position))
                {
                    items[i] = items[items.Count - 1];
                    items.RemoveAt(items.Count - 1);

                    return true;
                }
            }

            return false;
        }

        public IEnumerable<T> SphereCast(Vector3 center, float radius)
        {
            foreach (var item in items)
            {
                if (Math3D.IsPointInSphere(item.Position, center, radius))
                {
                    yield return item.Item;
                }
            }
        }

        public IEnumerable<T> BoundsCast(Bounds bounds)
        {
            foreach (var item in items)
            {
                if (bounds.Contains(item.Position))
                {
                    yield return item.Item;
                }
            }
        }

        public IEnumerable<T> ShapeCast(IShape shape)
        {
            foreach (var item in items)
            {
                if (shape.ContainsPoint(item.Position))
                {
                    yield return item.Item;
                }
            }
        }

        public IEnumerable<T> InverseSphereCast(Vector3 center, float radius)
        {
            foreach (var item in items)
            {
                if (!Math3D.IsPointInSphere(item.Position, center, radius))
                {
                    yield return item.Item;
                }
            }
        }

        public IEnumerable<T> InverseBoundsCast(Bounds bounds)
        {
            foreach (var item in items)
            {
                if (!bounds.Contains(item.Position))
                {
                    yield return item.Item;
                }
            }
        }

        public IEnumerable<T> InverseShapeCast(IShape shape)
        {
            foreach (var item in items)
            {
                if (!shape.ContainsPoint(item.Position))
                {
                    yield return item.Item;
                }
            }
        }

        private struct ItemEntry
        {
            public readonly T Item;

            public readonly Vector3 Position;

            public bool Equals(T other, Vector3 otherPosition)
            {
                return this.Position == otherPosition && Equals(this.Item, other);
            }

            public ItemEntry(T item, Vector3 position)
            {
                this.Item = item;
                this.Position = position;
            }
        }
    }
}
