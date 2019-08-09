// Copyright © 2019 Jasper Ermatinger

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Unity_Collections.Core
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

        public static void ToArray<T>(this IEnumerator<T> enumerator, IList<T> output)
        {
            while (enumerator.MoveNext())
            {
                output.Add(enumerator.Current);
            }
        }
    }
}
