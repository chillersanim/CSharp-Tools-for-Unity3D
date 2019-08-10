// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         CollectionUtil.cs
// 
// Created:          05.08.2019  13:51
// Last modified:    09.08.2019  15:54
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
using UnityEditor.WindowsStandalone;
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
        public static void RandomizeOrder<T>(IList<T> items)
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

        public static TOut[] Map<TIn, TOut>(IList<TIn> items, Func<TIn, TOut> mapper)
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
