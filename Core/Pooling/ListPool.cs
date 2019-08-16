using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Unity_Tools.Core.Pooling
{
    public class ListPool<T> : IPool<List<T>>
    {
        private int maxSize;
        private int maxListCapacity;

        private readonly List<List<T>> lists;

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

        public ListPool()
        {
            maxSize = 128;
            maxListCapacity = 4096;
            lists = new List<List<T>>();
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

        public List<T> Get(int minCapacity)
        {
            var smallest = ExtractSmallest(minCapacity);
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
