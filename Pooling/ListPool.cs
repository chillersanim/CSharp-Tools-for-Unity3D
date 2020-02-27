// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         ListPool.cs
// 
// Created:          29.01.2020  19:32
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
using System.Collections.Generic;

namespace UnityTools.Pooling
{
    public class ListPool<T> : IPool<List<T>>
    {
        private readonly List<List<T>> lists;
        private int maxListCapacity;
        private int maxSize;

        public ListPool()
        {
            maxSize = 128;
            maxListCapacity = 4096;
            lists = new List<List<T>>();
        }

        public int MaxListCapacity
        {
            get => maxListCapacity;
            set
            {
                if (maxListCapacity == value)
                {
                    return;
                }

                maxListCapacity = value;

                if (maxListCapacity < 1)
                {
                    maxListCapacity = 1;
                }

                foreach (var list in lists)
                {
                    if (list.Capacity > maxListCapacity)
                    {
                        list.Capacity = maxListCapacity;
                    }
                }
            }
        }

        public int MaxSize
        {
            get => maxSize;
            set
            {
                if (maxSize == value)
                {
                    return;
                }

                maxSize = value;

                if (maxSize < 1)
                {
                    maxSize = 1;
                }

                while (lists.Count > maxSize)
                {
                    ExtractSmallest(0);
                }
            }
        }

        public List<T> Get()
        {
            var smallest = ExtractSmallest(0);
            if (smallest == null)
            {
                return new List<T>();
            }

            return smallest;
        }

        public void Put(List<T> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            item.Clear();

            if (lists.Capacity >= maxSize)
            {
                return;
            }

            if (item.Capacity > maxListCapacity)
            {
                item.Capacity = maxListCapacity;
            }

            lists.Add(item);
        }

        public List<T> Get(int minCapacity)
        {
            var smallest = ExtractSmallest(minCapacity);
            if (smallest == null)
            {
                return new List<T>();
            }

            return smallest;
        }

        private List<T> ExtractSmallest(int requiredCapacity)
        {
            var minIndex = -1;
            var minCapacity = int.MaxValue;

            for (var i = 0; i < lists.Count; i++)
            {
                var list = lists[i];
                var capacity = list.Capacity;
                if (capacity >= requiredCapacity && capacity < minCapacity)
                {
                    minCapacity = capacity;
                    minIndex = i;
                }
            }

            if (minIndex == -1)
            {
                return null;
            }

            var result = lists[minIndex];
            lists[minIndex] = lists[lists.Count - 1];
            lists.RemoveAt(lists.Count - 1);

            return result;
        }
    }
}
