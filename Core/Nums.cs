// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Nums.cs
// 
// Created:          19.08.2019  18:57
// Last modified:    25.08.2019  15:59
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

namespace Unity_Tools.Core
{
    /// <summary>
    /// Contains tools for basic operations and numeric operations
    /// </summary>
    public static class Nums
    {
        /// <summary>
        /// The hash code used for null types (is prime)
        /// </summary>
        [PublicAPI]
        public const int NullHashCode = 999983;

        /// <summary>
        ///     Generates a hash code for the combination of two <see cref="T" />.
        /// </summary>
        /// <param name="v0">The first <see cref="T" />.</param>
        /// <param name="v1">The second <see cref="T" />.</param>
        /// <returns>Returns the hash code.</returns>
        [PublicAPI]
        public static int HashCode<T>([NotNull] T v0, [NotNull] T v1)
        {
            // Overflow is fine, just wrap
            unchecked
            {
                var hash = (int)2166136261;
                hash = (hash * 16777619) ^ v0.GetHashCode();
                hash = (hash * 16777619) ^ v1.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///     Generates a hash code for the combination of three <see cref="T" />.
        /// </summary>
        /// <param name="v0">The first <see cref="T" />.</param>
        /// <param name="v1">The second <see cref="T" />.</param>
        /// <param name="v2">The thirth <see cref="T" />.</param>
        /// <returns>Returns the hash code.</returns>
        [PublicAPI]
        public static int HashCode<T>([NotNull] T v0, [NotNull] T v1, [NotNull] T v2)
        {
            // Overflow is fine, just wrap
            unchecked
            {
                var hash = (int)2166136261;
                hash = (hash * 16777619) ^ v0.GetHashCode();
                hash = (hash * 16777619) ^ v1.GetHashCode();
                hash = (hash * 16777619) ^ v2.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///     Generates a hash code for the combination of four <see cref="T" />.
        /// </summary>
        /// <param name="v0">The first <see cref="T" />.</param>
        /// <param name="v1">The second <see cref="T" />.</param>
        /// <param name="v2">The thirth <see cref="T" />.</param>
        /// <param name="v3">The fourth <see cref="T" />.</param>
        /// <returns>Returns the hash code.</returns>
        [PublicAPI]
        public static int HashCode<T>([NotNull] T v0, [NotNull] T v1, [NotNull] T v2, [NotNull] T v3)
        {
            // Overflow is fine, just wrap
            unchecked
            {
                var hash = (int)2166136261;
                hash = (hash * 16777619) ^ v0.GetHashCode();
                hash = (hash * 16777619) ^ v1.GetHashCode();
                hash = (hash * 16777619) ^ v2.GetHashCode();
                hash = (hash * 16777619) ^ v3.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///     Generates a hash code for the combination of multiple <see cref="T" />.
        /// </summary>
        /// <param name="values">The <see cref="T" />'s.</param>
        /// <returns>Returns the hash code.</returns>
        [PublicAPI]
        public static int HashCode<T>([NotNull] params T[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            // Overflow is fine, just wrap
            unchecked
            {
                var hash = (int)2166136261;
                foreach (var t in values)
                {
                    var currentHash = Equals(t, null) ? NullHashCode : t.GetHashCode();
                    hash = (hash * 16777619) ^ currentHash;
                }

                return hash;
            }
        }

        /// <summary>
        ///     Generates a hash code for the combination of multiple <see cref="T" />.
        /// </summary>
        /// <param name="values">The <see cref="T" />'s.</param>
        /// <returns>Returns the hash code.</returns>
        [PublicAPI]
        public static int HashCode<T>([NotNull] IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            // Overflow is fine, just wrap
            unchecked
            {
                var hash = (int)2166136261;
                foreach (var t in values)
                {
                    var currentHash = Equals(t, null) ? NullHashCode : t.GetHashCode();
                    hash = (hash * 16777619) ^ currentHash;
                }

                return hash;
            }
        }

        public static void Sort<T>(ref T t0, ref T t1) where T : IComparable<T>
        {
            if (t0.CompareTo(t1) > 0)
            {
                var tmp = t0;
                t0 = t1;
                t1 = tmp;
            }
        }

        public static void Sort<T>(ref T t0, ref T t1, ref T t2) where T : IComparable<T>
        {
            Sort(ref t0, ref t1);
            Sort(ref t0, ref t2);
            Sort(ref t1, ref t2);
        }

        public static void Sort<T>(ref T t0, ref T t1, ref T t2, ref T t3) where T : IComparable<T>
        {
            Sort(ref t0, ref t1);
            Sort(ref t0, ref t2);
            Sort(ref t0, ref t3);
            Sort(ref t1, ref t2);
            Sort(ref t1, ref t3);
            Sort(ref t2, ref t3);
        }

        public static void Sort<T>(ref T t0, ref T t1, ref T t2, ref T t3, ref T t4) where T : IComparable<T>
        {
            Sort(ref t0, ref t1);
            Sort(ref t0, ref t2);
            Sort(ref t0, ref t3);
            Sort(ref t0, ref t4);
            Sort(ref t1, ref t2);
            Sort(ref t1, ref t3);
            Sort(ref t1, ref t4);
            Sort(ref t2, ref t3);
            Sort(ref t2, ref t4);
            Sort(ref t3, ref t4);
        }

        public static void SortDescending<T>(ref T t0, ref T t1) where T : IComparable<T>
        {
            Sort(ref t1, ref t0);
        }

        public static void SortDescending<T>(ref T t0, ref T t1, ref T t2) where T : IComparable<T>
        {
            Sort(ref t2, ref t1, ref t0);
        }

        public static void SortDescending<T>(ref T t0, ref T t1, ref T t2, ref T t3) where T : IComparable<T>
        {
            Sort(ref t3, ref t2, ref t1, ref t0);
        }

        public static void SortDescending<T>(ref T t0, ref T t1, ref T t2, ref T t3, ref T t4) where T : IComparable<T>
        {
            Sort(ref t4, ref t3, ref t2, ref t1, ref t0);
        }

        public static void Revert<T>(ref T t0, ref T t1)
        {
            var tmp = t0;
            t0 = t1;
            t1 = tmp;
        }

        public static void Revert<T>(ref T t0, ref T t1, ref T t2, ref T t3)
        {
            Revert(ref t0, ref t3);
            Revert(ref t1, ref t2);
        }

        /// <summary>
        /// A modulo function that properly does modulo for negative numbers as well.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <param name="m">The modulo.</param>
        /// <returns>The positive definite modulo result.</returns>
        public static int Mod(int x, int m)
        {
            var r = x % m;
            return r < 0 ? r + m : r;
        }
    }
}
