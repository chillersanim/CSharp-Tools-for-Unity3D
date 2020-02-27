// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Interval.cs
// 
// Created:          29.01.2020  19:27
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

namespace UnityTools.Core
{
    public struct Interval<T> where T : IComparable<T>
    {
        public readonly T Min;

        public readonly T Max;

        public Interval(T min, T max)
        {
            if (min.CompareTo(max) > 0)
            {
                throw new ArgumentException("Max needs to be larger or equal than min.", nameof(max));
            }

            this.Min = min;
            this.Max = max;
        }

        public bool Contains(T element)
        {
            return Min.CompareTo(element) <= 0 && Max.CompareTo(element) >= 0;
        }

        public static bool operator <(Interval<T> interval, T value)
        {
            return interval.Max.CompareTo(value) < 0;
        }

        public static bool operator >(Interval<T> interval, T value)
        {
            return interval.Min.CompareTo(value) > 0;
        }

        public static bool operator <(T value, Interval<T> interval)
        {
            return value.CompareTo(interval.Min) < 0;
        }

        public static bool operator >(T value, Interval<T> interval)
        {
            return value.CompareTo(interval.Max) > 0;
        }
    }
}
