using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Unity_Collections.Core;
using Unity_Tools.Core;

namespace Assets.Unity_Tools.Collections
{
    public class Simple3DCollection<T> : I3DCollection<T> where T : class
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
            var halfSize = size / 2f;
            var min = center - halfSize;
            var max = center + halfSize;
            searchCache.Clear();

            foreach (var entry in items)
            {
                if (entry.Position.IsInAabb(min, max))
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
                return this.Position == otherPosition && this.Item == other;
            }

            public ItemEntry(T item, Vector3 position)
            {
                this.Item = item;
                this.Position = position;
            }
        }
    }
}
