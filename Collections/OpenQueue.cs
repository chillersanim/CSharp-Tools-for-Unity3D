// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         OpenQueue.cs
// 
// Created:          19.03.2020  10:47
// Last modified:    21.03.2020  11:42
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

namespace UnityTools.Collections
{
    /// <summary>
    /// Implements a queue like data structure that also allows dequeue operations on subsets of the queued data.
    /// </summary>
    /// <typeparam name="T">The type of the queue items.</typeparam>
    public sealed class OpenQueue<T> : IReadOnlyCollection<T>, ICollection
    {
        /// <summary>
        /// The initial size of the items array, if no specific size was defined.
        /// </summary>
        private const int InitialSize = 32;

        /// <summary>
        /// The target compactness (count / items.Length) to aim for before extending the array (to prevent compaction on every enqueue operation)
        /// </summary>
        private const float TargetCompactness = 0.9f;

        /// <summary>
        /// Stores the amount of valid entries in the items array.
        /// </summary>
        private int count;

        /// <summary>
        /// Points to the first valid entry, or to 'items.Length' if there are no valid entries.
        /// </summary>
        private int first;

        /// <summary>
        /// Contains all queued items, there can be holes between items. Actual items are marked with 'valid' == true;
        /// </summary>
        [NotNull]
        private QueueEntry[] items;

        /// <summary>
        /// Points to the last valid entry, or to 'items.Length' if there are no valid entries.
        /// </summary>
        private int last;

        /// <summary>
        /// Gets the current state version, which increments on every change.
        /// </summary>
        private int version;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenQueue{T}"/> type.
        /// </summary>
        public OpenQueue()
        {
            items = new QueueEntry[InitialSize];
            count = 0;
            first = -1;
            last = -1;
            version = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenQueue{T}"/> type with the given initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the query queue, needs the be at least 1.</param>
        public OpenQueue(int initialCapacity)
        {
            if(initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The initial capacity needs to be positive and at least 1.");
            }

            items = new QueueEntry[initialCapacity];
            count = 0;
            first = -1;
            last = -1;
            version = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenQueue{T}"/> type with the given initial items.
        /// </summary>
        /// <param name="initialItems">The initial items of the query queue, cannot be null, can be empty.</param>
        public OpenQueue(IEnumerable<T> initialItems)
        {
            if(initialItems == null)
            {
                throw new ArgumentNullException(nameof(initialItems));
            }

            var itemsCache = new List<T>(initialItems);
            items = new QueueEntry[Mathf.Max(itemsCache.Count, 1)];

            count = itemsCache.Count;
            version = 0;

            for (var i = 0; i < count; i++)
            {
                items[i] = new QueueEntry(itemsCache[i], true);
            }

            if (count > 0)
            {
                first = 0;
                last = count - 1;
            }
            else
            {
                first = last = -1;
            }
        }

        /// <summary>
        /// Gets or sets the capacity of the query queue.
        /// </summary>
        public int Capacity
        {
            get => items.Length;
            set
            {
                if (value < count)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Can't set the capacity to a value lower than Count.");
                }

                if(value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The capacity needs to be positive and at least 1.");
                }

                if (value == items.Length)
                {
                    return;
                }

                version++;

                Compact();
                ChangeSize(value);
            }
        }

        /// <summary>
        /// <inheritdoc cref="ICollection.CopyTo(Array, int)"/>
        /// </summary>
        void ICollection.CopyTo(Array array, int index)
        {
            if (count == 0)
            {
                return;
            }

            Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                "When the queue contains items, the entry at the first index needs to be valid.");
            Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                "When the queue contains items, the entry at the last index needs to be valid.");

            var current = index;

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid)
                {
                    array.SetValue(items[i].Item, current);
                    current++;
                }
            }
        }

        /// <summary>
        /// <inheritdoc cref="ICollection.IsSynchronized"/>
        /// </summary>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// <inheritdoc cref="ICollection.SyncRoot"/>
        /// </summary>
        object ICollection.SyncRoot { get; } = new object();

        /// <summary>
        /// Gets the amount of queued items.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        /// </summary>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if(count == 0)
            {
                yield break;
            }

            var startVersion = version;

            for(var i = first; i <= last; i++)
            {
                if (items[i].Valid)
                {
                    yield return items[i].Item;

                    if (startVersion != version)
                    {
                        throw new InvalidOperationException(
                            "The collection has been modified since the last enumeration step.");
                    }
                }
            }
        }

        /// <summary>
        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        /// <summary>
        /// Evaluates whether all queued item match the predicate. (Count == 0 -> true)
        /// </summary>
        /// <param name="predicate">The predicate to test for.</param>
        /// <returns>Returns <c>true</c> if all items match the predicate or no items are queued, <c>false</c> otherwise.</returns>
        [Pure]
        public bool All(Predicate<T> predicate)
        {
            if(count == 0)
            {
                return true;
            }

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid && !predicate(items[i].Item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Evaluates whether any queued item matches the predicate. (Count == 0 -> false)
        /// </summary>
        /// <param name="predicate">The predicate to test for.</param>
        /// <returns>Returns <c>true</c> if any item matches the predicate, <c>false</c> otherwise.</returns>
        [Pure]
        public bool Any(Predicate<T> predicate)
        {
            if(count == 0)
            {
                return false;
            }

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid && predicate(items[i].Item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public void Clear()
        {
            if(count == 0)
            {
                return;
            }

            version++;

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid)
                {
                    items[i] = new QueueEntry(default, false);
                }
            }

            first = last = -1;
            count = 0;
        }

        /// <summary>
        /// Evaluates whether the specific item is queued.
        /// </summary>
        /// <param name="item">The item to test for.</param>
        /// <returns>Returns <c>true</c> if the item is queued, <c>false</c> otherwise.</returns>
        [Pure]
        public bool Contains(T item)
        {
            if(count == 0)
            {
                return false;
            }

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid && item.Equals(items[i].Item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Counts how many queued items match the predicate.
        /// </summary>
        /// <param name="predicate">The predicate to test for.</param>
        /// <returns>Returns the amount of queued items that match the predicate.</returns>
        [Pure]
        public int CountMatches(Predicate<T> predicate)
        {
            if(count == 0)
            {
                return 0;
            }

            var result = 0;

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid && predicate(items[i].Item))
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Counts how many times the given item is queued..
        /// </summary>
        /// <param name="item">The item to test for.</param>
        /// <returns>Returns the amount of times the given item is queued.</returns>
        [Pure]
        public int CountMatches(T item)
        {
            if(count == 0)
            {
                return 0;
            }

            var result = 0;

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid && item.Equals(items[i].Item))
                {
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// Dequeue the next item.
        /// </summary>
        /// <returns>Return the dequeued item.</returns>
        public T Dequeue()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("Can't dequeue items from an empty queue.");
            }

            Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                "When the queue contains items, the entry at the first index needs to be valid.");
            Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                "When the queue contains items, the entry at the last index needs to be valid.");

            version++;
            var item = items[first];
            items[first] = new QueueEntry(default, false);
            
            count--;
            AdaptIndices();

            return item.Item;
        }

        /// <summary>
        /// Dequeue all items that match the predicate, in queue order.
        /// </summary>
        /// <param name="predicate">The predicate to select the items to dequeue.</param>
        /// <returns>Returns an enumerable that enumerates over all matching items.</returns>
        public IEnumerable<T> DequeueAll(Predicate<T> predicate)
        {
            if(count == 0)
            {
                yield break;
            }

            Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                "When the queue contains items, the entry at the first index needs to be valid.");
            Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                "When the queue contains items, the entry at the last index needs to be valid.");

            var startVersion = version;

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid)
                {
                    var currentItem = items[i].Item;
                    if(predicate(currentItem))
                    {
                        version++;
                        startVersion++;

                        count--;
                        items[i] = new QueueEntry(default, false);

                        AdaptIndices();

                        yield return currentItem;

                        if (startVersion != version)
                        {
                            throw new InvalidOperationException(
                                "The collection has been modified since the last enumeration step.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Dequeue all items that match the predicate, in queue order.
        /// </summary>
        /// <param name="predicate">The predicate to select the items to dequeue.</param>
        /// <param name="output">The output in which to store the dequeued items.</param>
        public void DequeueAll(Predicate<T> predicate, IList<T> output)
        {
            if(output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if(count == 0)
            {
                return;
            }

            Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                "When the queue contains items, the entry at the first index needs to be valid.");
            Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                "When the queue contains items, the entry at the last index needs to be valid.");

            version++;

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid)
                {
                    var currentItem = items[i].Item;
                    if(predicate(currentItem))
                    {
                        count--;
                        items[i] = new QueueEntry(default, false);
                        output.Add(currentItem);
                    }
                }
            }

            // Adapt first and last index
            AdaptIndices();
        }

        /// <summary>
        /// Enqueue an item.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Enqueue(T item)
        {
            version++;

            if (count == 0)
            {
                Debug.Assert(items.Length >= 1);
                first = last = 0;
            }
            else
            {
                Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                    "When the queue contains items, the entry at the first index needs to be valid.");
                Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                    "When the queue contains items, the entry at the last index needs to be valid.");

                PrepareForNewItem();
                last++;
            }

            items[last] = new QueueEntry(item, true);
            count++;
        }

        /// <summary>
        /// Returns the next item in the queue without removing it.
        /// </summary>
        /// <returns>The next item.</returns>
        public T Peek()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("Can't peek from an empty queue.");
            }

            Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                "When the queue contains items, the entry at the first index needs to be valid.");
            Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                "When the queue contains items, the entry at the last index needs to be valid.");

            return items[first].Item;
        }

        /// <summary>
        /// Returns the last item in the queue without removing it.
        /// </summary>
        /// <returns>Returns the item.</returns>
        public T PeekLast()
        {
            if (count == 0)
            {
                throw new InvalidOperationException("Can't peek from an empty queue.");
            }

            Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                "When the queue contains items, the entry at the first index needs to be valid.");
            Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                "When the queue contains items, the entry at the last index needs to be valid.");

            return items[last].Item;
        }

        /// <summary>
        /// Tries to dequeue the next item.
        /// </summary>
        /// <param name="item">The item that was dequeued, undefined if nothing was dequeued.</param>
        /// <returns>Returns whether an item was successfully dequeued.</returns>
        public bool TryDequeue(out T item)
        {
            if (count == 0)
            {
                item = default;
                return false;
            }

            Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                "When the queue contains items, the entry at the first index needs to be valid.");
            Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                "When the queue contains items, the entry at the last index needs to be valid.");

            version++;
            item = items[first].Item;
            items[first] = new QueueEntry(default, false);
            
            count--;
            AdaptIndices();

            return true;
        }

        /// <summary>
        /// Tries to dequeue the first item that matches the predicate.
        /// </summary>
        /// <param name="predicate">The predicate to select the item to dequeue.</param>
        /// <param name="item">The item that was dequeued, undefined if nothing was dequeued.</param>
        /// <returns></returns>
        public bool TryDequeueFirst(Predicate<T> predicate, out T item)
        {
            if (count == 0)
            {
                item = default;
                return false;
            }

            Debug.Assert(first >= 0 && first < items.Length && items[first].Valid,
                "When the queue contains items, the entry at the first index needs to be valid.");
            Debug.Assert(last >= first && last < items.Length && items[last].Valid,
                "When the queue contains items, the entry at the last index needs to be valid.");

            for(var i = first; i <= last; i++)
            {
                if (items[i].Valid)
                {
                    var currentItem = items[i].Item;

                    if(predicate(currentItem))
                    {
                        version++;
                        count--;
                        items[i] = new QueueEntry(default, false);

                        AdaptIndices();

                        item = currentItem;
                        return true;
                    }
                }
            }

            item = default;
            return false;
        }

        /// <summary>
        /// Adapts the first and last index, assuming only removals.
        /// </summary>
        private void AdaptIndices()
        {
            if (count == 0)
            {
                first = last = -1;
                return;
            }

            Debug.Assert(first >= 0 && first < items.Length, "When the count is greater than 0, the first index cannot be out of bounds.");
            Debug.Assert(last >= first && last < items.Length, "When the count is greater than 0, the last index cannot be out of bounds.");

            var firstValid = items[first].Valid;
            var lastValid = items[last].Valid;

            if(firstValid && lastValid)
            {
                return;
            }

            if(!firstValid)
            {
                for (var i = first; i <= last; i++)
                {
                    if (items[i].Valid)
                    {
                        first = i;
                        break;
                    }
                }

                Debug.Assert(items[first].Valid,
                    "When the count is greater than 0, a valid first index must exist.");
            }

            if (!lastValid)
            {
                for (var i = last; i >= first; i--)
                {
                    if (items[i].Valid)
                    {
                        last = i;
                        break;
                    }
                }

                Debug.Assert(items[last].Valid,
                    "When the count is greater than 0, a valid last index must exist.");
            }
        }

        /// <summary>
        /// Changes the size of the items array to the new size.
        /// </summary>
        /// <param name="newSize">The new size of the items array.</param>
        private void ChangeSize(int newSize)
        {
            var newItems = new QueueEntry[newSize];

            for (var i = 0; i < items.Length; i++)
            {
                newItems[i] = items[i];
                items[i] = new QueueEntry(default, false);  // Break reference hierarchy to help GC
            }

            // In case we reduced the size of the array
            if (last >= items.Length)
            {
                last = items.Length - 1;

                if (first >= items.Length)
                {
                    first = last;
                }

                AdaptIndices();
            }

            items = newItems;
        }

        /// <summary>
        /// Compacts the items array so that all valid items are at the beginning of the array, in the same order as before.
        /// </summary>
        private void Compact()
        {
            if(count == 0)
            {
                return;
            }

            Debug.Assert(first >= 0);
            Debug.Assert(first <= last);
            Debug.Assert(last < items.Length);

            var current = 0;

            for(var i = first; i <= last; i++)
            {
                if(items[i].Valid)
                {
                    items[current] = items[i];
                    current++;
                }
            }

            for(var i = last + 1; i < items.Length; i++)
            {
                // Remove old references and invalidate items.
                if (items[i].Valid)
                {
                    items[i] = new QueueEntry(default, false);
                }
            }

            first = 0;
            last = current - 1;

            Debug.Assert(last - first == count - 1);
        }

        /// <summary>
        /// Ensures that the items array is ready for inserting one new item
        /// </summary>
        private void PrepareForNewItem()
        {
            // If the last index is not at the end of the array, no need to do anything.
            if (last < items.Length - 1)
            {
                return;
            }

            // If the last index is at the end of the array, but we have space for more, compact it.
            if (count < items.Length * TargetCompactness)
            {
                Compact();
                return;
            }

            // If we don't have enough space for more, expand the array
            Debug.Assert(first == 0);
            Debug.Assert(last == items.Length - 1);

            var newSize = items.Length * 2;

            // Can happen with overflow (when previous capacity was >= 2^30)
            if(newSize < items.Length)
            {
                newSize = int.MaxValue;

                if(newSize == items.Length)
                {
                    if (count < items.Length)
                    {
                        // We can fit more items by ignoring the compactness requirement
                        Compact();
                        return;
                    }

                    // We are already at max size, can't expand anymore.
                    throw new InvalidOperationException(
                        "The query queue is already at maximum capacity (2^31 items) and can't take anymore items.");
                }
            }

            ChangeSize(items.Length * 2);
        }

        /// <summary>
        /// Struct used to store the items of the query queue and mark valid/invalid items as such.
        /// </summary>
        private struct QueueEntry
        {
            /// <summary>
            /// The item.
            /// </summary>
            public readonly T Item;

            /// <summary>
            /// Gets a value indicating whether this item entry is valid (an actually queued item) or invalid (empty spot in the items array).
            /// </summary>
            public readonly bool Valid;

            public QueueEntry(T item, bool valid)
            {
                this.Item = item;
                this.Valid = valid;
            }
        }
    }
}
