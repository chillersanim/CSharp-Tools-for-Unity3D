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
        public readonly Vector3 Min;

        public readonly Vector3 Max;

        public Vector3 Center => (this.Min + this.Max) / 2f;

        public Vector3 Extend => (this.Max - this.Min) / 2f;

        public Vector3 Size => (this.Max - this.Min);

        public Aabb(Vector3 center, Vector3 size)
        {
            var extend = size.AbsComponents() / 2f;
            this.Min = center - extend;
            this.Max = center + extend;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector3 point)
        {
            return point.x >= this.Min.x && point.y >= this.Min.y && point.z >= this.Min.z &&
                   point.x <= this.Max.x && point.y <= this.Max.y && point.z <= this.Max.z;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsAabb(Vector3 start, Vector3 end)
        {
            return this.Min.x <= end.x && this.Min.y <= end.y && this.Min.z <= end.z &&
                   this.Max.x >= start.x && this.Max.y >= start.y && this.Max.z >= start.z;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAabb(Vector3 start, Vector3 end)
        {
            return this.Min.x <= start.x && this.Min.y <= start.y && this.Min.z <= start.z &&
                   this.Max.x >= end.x && this.Max.y >= end.y && this.Max.z >= end.z;
        }
    }
}
