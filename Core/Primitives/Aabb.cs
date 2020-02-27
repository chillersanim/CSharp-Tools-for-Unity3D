// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Aabb.cs
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
    public struct Aabb : IVolume
    {
        private readonly Vector3 min, max;

        public Aabb(Vector3 center, Vector3 size)
        {
            var halfSize = size.AbsComponents() / 2f;
            min = center - halfSize;
            max = center + halfSize;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector3 point)
        {
            return point.x >= min.x && point.y >= min.y && point.z >= min.z &&
                   point.x <= max.x && point.y <= max.y && point.z <= max.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsAabb(Vector3 start, Vector3 end)
        {
            return min.x <= end.x && min.y <= end.y && min.z <= end.z &&
                   max.x >= start.x && max.y >= start.y && max.z >= start.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAabb(Vector3 start, Vector3 end)
        {
            return min.x <= start.x && min.y <= start.y && min.z <= start.z &&
                   max.x >= end.x && max.y >= end.y && max.z >= end.z;
        }
    }
}
