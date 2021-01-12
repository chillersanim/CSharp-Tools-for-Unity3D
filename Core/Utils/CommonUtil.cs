// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CommonUtil.cs
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
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Unity_Tools.Core
{
    /// <summary>
    /// Contains tools for basic operations and numeric operations
    /// </summary>
    public static class CommonUtil
    {
        /// <summary>
        ///     Evaulates whether <see cref="a" /> and <see cref="b" /> are numerically equal, using <see cref="FloatEpsilon" /> as
        ///     maximum difference.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>Returns <c>true</c> if the values are equals, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool Equals(float a, float b)
        {
            if (float.IsPositiveInfinity(a) && float.IsPositiveInfinity(b))
            {
                return true;
            }

            if (float.IsNegativeInfinity(a) && float.IsNegativeInfinity(b))
            {
                return true;
            }

            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return Math.Abs(a - b) <= FloatEpsilon;
        }

        /// <summary>
        ///     Evaulates whether <see cref="a" /> and <see cref="b" /> are numerically equal, using <see cref="epsilon" /> as
        ///     maximum difference.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="epsilon">The maximum difference.</param>
        /// <returns>Returns <c>true</c> if the values are equals, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool Equals(float a, float b, float epsilon)
        {
            if (float.IsPositiveInfinity(a) && float.IsPositiveInfinity(b))
            {
                return true;
            }

            if (float.IsNegativeInfinity(a) && float.IsNegativeInfinity(b))
            {
                return true;
            }

            if (float.IsNaN(a) || float.IsNaN(b))
            {
                return false;
            }

            return Math.Abs(a - b) <= epsilon;
        }

        /// <summary>
        ///     Evaulates whether <see cref="a" /> and <see cref="b" /> are numerically equal, using <see cref="DoubleEpsilon" /> as
        ///     maximum difference.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>Returns <c>true</c> if the values are equals, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool Equals(double a, double b)
        {
            if (double.IsPositiveInfinity(a) && double.IsPositiveInfinity(b))
            {
                return true;
            }

            if (double.IsNegativeInfinity(a) && double.IsNegativeInfinity(b))
            {
                return true;
            }

            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return Math.Abs(a - b) <= DoubleEpsilon;
        }

        /// <summary>
        ///     Evaulates whether <see cref="a" /> and <see cref="b" /> are numerically equal, using <see cref="epsilon" /> as
        ///     maximum difference.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="epsilon">The maximum difference.</param>
        /// <returns>Returns <c>true</c> if the values are equals, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool Equals(double a, double b, double epsilon)
        {
            if (double.IsPositiveInfinity(a) && double.IsPositiveInfinity(b))
            {
                return true;
            }

            if (double.IsNegativeInfinity(a) && double.IsNegativeInfinity(b))
            {
                return true;
            }

            if (double.IsNaN(a) || double.IsNaN(b))
            {
                return false;
            }

            return Math.Abs(a - b) <= epsilon;
        }

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

        /// <summary>
        ///     Finds the maximum from two elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IComparable{T}" />.</typeparam>
        /// <param name="a">The first element.</param>
        /// <param name="b">The second element.</param>
        /// <returns>Returns the maximum element.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>([NotNull] T a, [NotNull] T b)
            where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        /// <summary>
        ///     Finds the maximum from 3 elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IComparable{T}" />.</typeparam>
        /// <param name="a">The first element.</param>
        /// <param name="b">The second element.</param>
        /// <param name="c">The third element.</param>
        /// <returns>Returns the maximum element.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Max<T>([NotNull] T a, [NotNull] T b, [NotNull] T c)
            where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? (a.CompareTo(c) > 0 ? a : c) : (b.CompareTo(c) > 0 ? b : c);
        }

        /// <summary>
        ///     Finds the maximum from a set of elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IComparable{T}" />.</typeparam>
        /// <param name="values">The value elements.</param>
        /// <returns>Returns the maximum element.</returns>
        [NotNull]
        [PublicAPI]
        public static T Max<T>([NotNull] params T[] values)
            where T : IComparable<T>
        {
            if ((values == null) || (values.Length == 0))
            {
                throw new ArgumentException("Values cannot be null or empty.", nameof(values));
            }

            // Preliminary null check for all items.
            if (!typeof(T).IsValueType)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    if (Equals(values[i], null))
                    {
                        throw new ArgumentNullException(nameof(values), "All items must not be null.");
                    }
                }
            }

            // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException

            if (values.Length == 1)
            {
                return values[0];
            }

            var max = values[0];

            for (var i = 1; i < values.Length; i++)
            {
                if (values[i].CompareTo(max) > 0)
                {
                    max = values[i];
                }
            }

            return max;

            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
        }

        /// <summary>
        ///     Finds the minimum from two elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IComparable{T}" />.</typeparam>
        /// <param name="a">The first element.</param>
        /// <param name="b">The second element.</param>
        /// <returns>Returns the minimum element.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>([NotNull] T a, [NotNull] T b)
            where T : IComparable<T>
        {
            return a.CompareTo(b) < 0 ? a : b;
        }

        /// <summary>
        ///     Finds the minimum from 3 elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IComparable{T}" />.</typeparam>
        /// <param name="a">The first element.</param>
        /// <param name="b">The second element.</param>
        /// <param name="c">The third element.</param>
        /// <returns>Returns the minimum element.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Min<T>([NotNull] T a, [NotNull] T b, [NotNull] T c)
            where T : IComparable<T>
        {
            return a.CompareTo(b) < 0 ? (a.CompareTo(c) < 0 ? a : c) : (b.CompareTo(c) < 0 ? b : c);
        }

        /// <summary>
        ///     Finds the minimum from a set of elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements, must implement <see cref="IComparable{T}" />.</typeparam>
        /// <param name="values">The value elements.</param>
        /// <returns>Returns the minimum element.</returns>
        [NotNull]
        [PublicAPI]
        public static T Min<T>([NotNull] params T[] values)
            where T : IComparable<T>
        {
            if ((values == null) || (values.Length == 0))
            {
                throw new ArgumentException("Values cannot be null or empty.", nameof(values));
            }

            // Preliminary null check for all items.
            if (!typeof(T).IsValueType)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    if (Equals(values[i], null))
                    {
                        throw new ArgumentNullException(nameof(values), "All items must not be null.");
                    }
                }
            }

            // ReSharper disable AssignNullToNotNullAttribute, PossibleNullReferenceException

            if (values.Length == 1)
            {
                return values[0];
            }

            var min = values[0].CompareTo(values[1]) < 0 ? values[0] : values[1];

            for (var i = 2; i < values.Length; i++)
            {
                if (values[i].CompareTo(min) < 0)
                {
                    min = values[i];
                }
            }

            return min;

            // ReSharper restore AssignNullToNotNullAttribute, PossibleNullReferenceException
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

        /// <summary>
        ///     Formats the number to a string using scientific notation if the absolute value of the number is larger than
        ///     <see cref="SmallestScientificNumber" />.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Returns the string representation of the number.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NumberString(int number)
        {
            return number.ToString(Math.Abs(number) < SmallestScientificNumber ? "0" : "0e0", CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Formats the number to a string using scientific notation if the absolute value of the number is larger than
        ///     <see cref="SmallestScientificNumber" />.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Returns the string representation of the number.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NumberString(long number)
        {
            return number.ToString(Math.Abs(number) < SmallestScientificNumber ? "0" : "0e0", CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Formats the number to a string with up to 4 decimals using scientific notation if the absolute value of the number
        ///     is larger than <see cref="SmallestScientificNumber" />.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Returns the string representation of the number.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NumberString(float number)
        {
            return number.ToString(Math.Abs(number) < SmallestScientificNumber ? "0.####" : "0.####e0", CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Formats the number to a string with up to 4 decimals using scientific notation if the absolute value of the number
        ///     is larger than <see cref="SmallestScientificNumber" />.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>Returns the string representation of the number.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NumberString(double number)
        {
            return number.ToString(Math.Abs(number) < SmallestScientificNumber ? "0.####" : "0.####e0", CultureInfo.CurrentCulture);
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

        /// <summary>
        ///     Swaps two elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements to swap.</typeparam>
        /// <param name="a">The first element.</param>
        /// <param name="b">The second element.</param>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        #region  Static Fields and Constants

        /// <summary>
        ///     The epsilon used to compare double precision numbers.
        /// </summary>
        [PublicAPI]
        public const double DoubleEpsilon = 1e-9;

        /// <summary>
        ///     Thee epsilon used to compare single precision numbers.
        /// </summary>
        [PublicAPI]
        public const float FloatEpsilon = 1e-6f;

        /// <summary>
        ///     The smallest number to be written in scientific notation. Smaller numbers get written as plain number.
        /// </summary>
        [PublicAPI]
        public const int SmallestScientificNumber = 1000;

        /// <summary>
        /// The hash code used for null types (is prime)
        /// </summary>
        [PublicAPI]
        public const int NullHashCode = 999983;

        #endregion
    }
}
