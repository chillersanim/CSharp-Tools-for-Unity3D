// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Simple3DCollection.cs
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
using UnityTools.Core;

namespace UnityTools.Collections
{
    public class Simple3DCollection<T> : IPoint3DCollection<T>
    {
        [NotNull]
        private readonly List<(T item, Vector3 position)> items;

        private readonly HashSet<(T, Vector3)> lookup;

        public Simple3DCollection(bool allowDuplicates = false)
        {
            this.items = new List<(T, Vector3)>();
            this.AllowsDuplicates = allowDuplicates;

            if (!allowDuplicates)
            {
                this.lookup = new HashSet<(T, Vector3)>();
            }
        }

        public Simple3DCollection(int capacity, bool allowDuplicates = false)
        {
            this.items = new List<(T, Vector3)>(capacity);
            this.AllowsDuplicates = allowDuplicates;

            if (!allowDuplicates)
            {
                this.lookup = new HashSet<(T, Vector3)>();
            }
        }

        public bool AllowsDuplicates { get; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in items)
            {
                yield return item.item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => items.Count;

        public void Add(T item, Vector3 position)
        {
            if (!AllowsDuplicates && this.Contains(item, position))
            {
                return;
            }

            items.Add((item, position));

            if (!AllowsDuplicates)
            {
                lookup.Add((item, position));
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(T item, Vector3 position)
        {
            if (!AllowsDuplicates)
            {
                return lookup.Contains((item, position));
            }
            else
            {
                // Can't use the lookup, as HashSet doesn't support duplicate entries
                return items.Contains((item, position));
            }
        }

        public IEnumerable<T> ShapeCast<TShape>(TShape shape) where TShape : IVolume
        {
            foreach (var item in items)
            {
                if (shape.ContainsPoint(item.position))
                {
                    yield return item.item;
                }
            }
        }

        public void ShapeCast<TShape>(TShape shape, IList<T> output) where TShape : IVolume
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            foreach (var item in items)
            {
                if (shape.ContainsPoint(item.position))
                {
                    output.Add(item.item);
                }
            }
        }

        public bool MoveItem(T item, Vector3 @from, Vector3 to)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var entry = items[i];
                if (entry.Equals((item, from)))
                {
                    if (!AllowsDuplicates)
                    {
                        lookup.Remove((item, from));

                        if (Contains(item, to))
                        {
                            items.RemoveAt(i);
                            return true;
                        }

                        items[i] = (item, to);
                        lookup.Add((item, to));
                    }
                    else
                    {
                        items[i] = (item, to);
                    }
                    
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
                if (entry.Equals((item, position)))
                {
                    items[i] = items[items.Count - 1];
                    items.RemoveAt(items.Count - 1);

                    if (!AllowsDuplicates)
                    {
                        lookup.Remove((item, position));
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
