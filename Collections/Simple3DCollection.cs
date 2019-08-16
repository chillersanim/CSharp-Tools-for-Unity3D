// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Simple3DCollection.cs
// 
// Created:          09.08.2019  14:51
<<<<<<< HEAD
// Last modified:    16.08.2019  16:31
=======
// Last modified:    15.08.2019  17:56
>>>>>>> refs/remotes/origin/master
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

using System.Collections;
using System.Collections.Generic;
using Unity_Tools.Core;

namespace Unity_Tools.Collections
{
    public class Simple3DCollection<T> : I3DCollection<T>
    {
        [NotNull]
        private readonly IList<ItemEntry> items;

        private readonly List<T> searchCache;

        public Simple3DCollection()
        {
            this.items = new List<ItemEntry>();
            searchCache = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (ItemEntry item in items)
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
                    if (i == items.Count - 1)
                    {
                        items.RemoveAt(i);
                    }
                    else
                    {
                        items[i] = items[items.Count - 1];
                        items.RemoveAt(items.Count - 1);
                    }

                    return true;
                }
            }

            return false;
        }

        public T[] FindInRadius(Vector3 center, float radius)
        {
            var sqRad = radius * radius;
            searchCache.Clear();

            foreach (var entry in items)
            {
                var sqrDist = (entry.Position - center).sqrMagnitude;
                if (sqrDist <= sqRad)
                {
                    searchCache.Add(entry.Item);
                }
            }

            return searchCache.ToArray();
        }

        public T[] FindInAabb(Vector3 center, Vector3 size)
        {
            searchCache.Clear();

            foreach (var entry in items)
            {
                if (entry.Position.IsInAabb(center, size))
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
