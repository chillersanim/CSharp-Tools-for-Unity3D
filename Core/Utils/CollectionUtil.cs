// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CollectionUtil.cs
// 
// Created:          29.01.2020  19:31
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
using JetBrains.Annotations;
using Unity_Tools.Pooling;
using Random = UnityEngine.Random;

namespace Unity_Tools.Core
{
    public static class CollectionUtil
    {
        /// <summary>
        /// Searches for the index of the last element that is still smaller than the value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="items">The list to search. Must be ordered in ascending order.</param>
        /// <param name="value">The value to look for.</param>
        /// <returns>Returns the zero based index.</returns>
        public static int BinarySearchLocation<T>(this IReadOnlyList<T> items, T value)
        {
            var comparer = Comparer<T>.Default;
            return BinarySearchLocation(items, value, comparer);
        }

        /// <summary>
        /// Searches for the index of the last element that is still smaller than the value.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="items">The list to search. Must be ordered in ascending order.</param>
        /// <param name="value">The value to look for.</param>
        /// <param name="comparer">The comparer to use for item comparison.</param>
        /// <returns>Returns the zero based index.</returns>
        public static int BinarySearchLocation<T>(this IReadOnlyList<T> items, T value, IComparer<T> comparer)
        {
            return BinarySearchLocation(items, value, comparer.Compare);
        }

        /// <summary>
        /// Searches for the index of the last element that is still smaller than the value.
        /// </summary>
        /// <typeparam name="TCol">The type of the collection items.</typeparam>
        /// <typeparam name="TVal">The type of the item to compare with.</typeparam>
        /// <param name="items">The list to search. Must be ordered in ascending order.</param>
        /// <param name="value">The value to look for.</param>
        /// <param name="comparer">The comparer to use for item comparison.</param>
        /// <returns>Returns the zero based index.</returns>
        public static int BinarySearchLocation<TCol, TVal>(this IReadOnlyList<TCol> items, TVal value,
            Func<TCol, TVal, int> comparer)
        {
            if (items.Count == 0)
            {
                return -1;
            }

            var start = 0;
            var end = items.Count;

            while (end > start)
            {
                var current = start + (end - start) / 2;
                var item = items[current];
                var compared = comparer(item, value);

                if (compared <= 0)
                {
                    start = current + 1;
                }
                else
                {
                    end = current;
                }
            }

            return start - 1;
        }

        /// <summary>
        /// Copies any data set to the output.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="enumerator">The data source.</param>
        /// <param name="output">The data target.</param>
        public static void CopyTo<T>(this IEnumerator<T> enumerator, IList<T> output)
        {
            while (enumerator.MoveNext())
            {
                output.Add(enumerator.Current);
            }
        }

        /// <summary>
        /// Builds an array using the initial element and a builder function.
        /// </summary>
        /// <typeparam name="T">The type of the array items.</typeparam>
        /// <param name="length">The target length of the array.</param>
        /// <param name="first">The first element.</param>
        /// <param name="next">A function generating a followup element.</param>
        /// <returns>Returns the generated array.</returns>
        public static T[] CreateArray<T>(int length, T first, Func<T, T> next)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
            }

            if (length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new T[length];
            result[0] = first;

            for (var i = 1; i < length; i++)
            {
                result[i] = next(result[i - 1]);
            }

            return result;
        }

        /// <summary>
        /// Builds a list using the initial element and a builder function.
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="length">The target length of the list.</param>
        /// <param name="first">The first element.</param>
        /// <param name="next">A function generating a followup element.</param>
        /// <returns>Returns the generated list.</returns>
        public static List<T> CreateList<T>(int length, T first, Func<T, T> next)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
            }

            if (length == 0)
            {
                return new List<T>();
            }

            var result = GlobalListPool<T>.Get(length);
            result.Add(first);
            var prev = first;

            for (var i = 1; i < length; i++)
            {
                var item = next(prev);
                prev = item;
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Removes the first element from the list and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="items">The list.</param>
        /// <returns>Returns the removed item.</returns>
        public static T ExtractFirst<T>(this IList<T> items)
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("Can't extract the last element of an empty list.");
            }

            var result = items[0];
            items.RemoveAt(0);
            return result;
        }

        /// <summary>
        /// Removes the last element from the list and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="items">The list.</param>
        /// <returns>Returns the removed item.</returns>
        public static T ExtractLast<T>(this IList<T> items)
        {
            var cnt = items.Count;
            if (cnt == 0)
            {
                throw new InvalidOperationException("Can't extract the last element of an empty list.");
            }

            var result = items[cnt - 1];
            items.RemoveAt(cnt - 1);
            return result;
        }

        /// <summary>
        /// Applies a filter on a list and returns the matches.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The list to search.</param>
        /// <param name="filter">The filter function.</param>
        /// <returns></returns>
        public static T[] Filter<T>(this IReadOnlyList<T> items, Func<T, bool> filter)
        {
            var filtered = GlobalListPool<T>.Get(items.Count);

            foreach (var item in items)
            {
                if (filter(item))
                {
                    filtered.Add(item);
                }
            }

            var result = filtered.ToArray();
            GlobalListPool<T>.Put(filtered);
            return result;
        }

        /// <summary>
        /// Executes an action for all items in an enumerable.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The enumerable.</param>
        /// <param name="action">The action to perform for every item.</param>
        public static void ForAll<T>([NotNull] this IEnumerable<T> items, [NotNull]Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Modifies a collection in place using a modifier function.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items list.</param>
        /// <param name="modifier">The modifier to apply to every item, getting the item and returning the new item.</param>
        public static void ForAll<T>([NotNull] this IList<T> items, Func<T, T> modifier)
        {
            for (var i = 0; i < items.Count; i++)
            {
                items[i] = modifier(items[i]);
            }
        }

        /// <summary>
        /// Modifies a collection in place using a modifier function.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items list.</param>
        /// <param name="modifier">The modifier to apply to every item, getting the item and it's index and returning the new item.</param>
        public static void ForAll<T>([NotNull] this IList<T> items, Func<T, int, T> modifier)
        {
            for (var i = 0; i < items.Count; i++)
            {
                items[i] = modifier(items[i], i);
            }
        }

        public static TOut[] Map<TIn, TOut>(this IReadOnlyList<TIn> items, Func<TIn, TOut> mapper)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            var cnt = items.Count;
            var result = new TOut[cnt];

            for (var i = 0; i < cnt; i++)
            {
                result[i] = mapper(items[i]);
            }

            return result;
        }

        /// <summary>
        /// Randomizes the order of the items.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="items">The items to be randomized</param>
        public static void Shuffle<T>(this IList<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var cnt = items.Count;
            for (var i = 0; i < cnt; i++)
            {
                var newIndex = Random.Range(0, cnt);
                var tmp = items[newIndex];
                items[newIndex] = items[i];
                items[i] = tmp;
            }
        }

        /// <summary>
        /// Converts any enumerator to an array.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="enumerator">The enumerator to convert.</param>
        /// <returns>Returns the resulting array.</returns>
        public static T[] ToArray<T>(this IEnumerator<T> enumerator)
        {
            var result = GlobalListPool<T>.Get();
            enumerator.CopyTo(result);
            var output = result.ToArray();
            GlobalListPool<T>.Put(result);
            return output;
        }

        /// <summary>
        /// Tries to remove the first element from the list and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="items">The list.</param>
        /// <param name="item">The extracted item.</param>
        /// <returns>Returns a value indicating whether an item could be extracted.</returns>
        public static bool TryExtractFirst<T>(this IList<T> items, out T item)
        {
            if (items.Count == 0)
            {
                item = default;
                return false;
            }

            item = items[0];
            items.RemoveAt(0);
            return true;
        }

        /// <summary>
        /// Tries to remove the last element from the list and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the list items.</typeparam>
        /// <param name="items">The list.</param>
        /// <param name="item">The extracted item.</param>
        /// <returns>Returns a value indicating whether an item could be extracted.</returns>
        public static bool TryExtractLast<T>(this IList<T> items, out T item)
        {
            var cnt = items.Count;
            if (cnt == 0)
            {
                item = default;
                return false;
            }

            item = items[cnt - 1];
            items.RemoveAt(cnt - 1);
            return true;
        }
    }
}
