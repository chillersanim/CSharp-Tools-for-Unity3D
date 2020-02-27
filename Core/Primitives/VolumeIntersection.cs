// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         VolumeIntersection.cs
// 
// Created:          27.01.2020  22:45
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

using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityTools.Core
{
    /// <summary>
    /// A ∩ B
    /// </summary>
    public struct VolumeIntersection<T1, T2> : IVolume where T1 : IVolume where T2 : IVolume
    {
        private readonly T1 shape1;

        private readonly T2 shape2;

        /// <summary>
        /// A ∩ B
        /// </summary>
        public VolumeIntersection(T1 shape1, T2 shape2)
        {
            this.shape1 = shape1;
            this.shape2 = shape2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector3 point)
        {
            return shape1.ContainsPoint(point) && shape2.ContainsPoint(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsAabb(Vector3 start, Vector3 end)
        {
            return shape1.IntersectsAabb(start, end) && shape2.IntersectsAabb(start, end);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAabb(Vector3 start, Vector3 end)
        {
            return shape1.ContainsAabb(start, end) && shape2.ContainsAabb(start, end);
        }
    }
}
