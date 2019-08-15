// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CollectionUtil.cs
// 
// Created:          05.08.2019  13:51
// Last modified:    15.08.2019  17:56
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

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Unity_Tools.Collections
{
    public static class CollectionUtil
    {
        /// <summary>
        /// Randomizes the order of the items.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="items">The items to be randomized</param>
        public static void RandomizeOrder<T>(this IList<T> items)
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

        public static TOut[] Map<TIn, TOut>(this IList<TIn> items, Func<TIn, TOut> mapper)
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

        public static T[] Create<T>(int length, T first, Func<T, T> next)
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

        public static void CopyTo<T>(this IEnumerator<T> enumerator, IList<T> output)
        {
            while (enumerator.MoveNext())
            {
                output.Add(enumerator.Current);
            }
        }

        public static T[] ToArray<T>(this IEnumerator<T> enumerator)
        {
            var result = new List<T>();
            enumerator.CopyTo(result);
            return result.ToArray();
        }
    }
}
