// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Sphere.cs
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
    public struct Sphere : IVolume
    {
        public readonly Vector3 Center;

        public readonly float Radius;

        public float Diameter => this.Radius + this.Radius;

        public Sphere(Vector3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector3 point)
        {
            var x = point.x - this.Center.x;
            var y = point.y - this.Center.y;
            var z = point.z - this.Center.z;

            var sqrDist = x * x + y * y + z * z;
            return sqrDist <= this.Radius * this.Radius;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsAabb(Vector3 start, Vector3 end)
        {
            var x = Mathf.Clamp(this.Center.x, start.x, end.x) - this.Center.x;
            var y = Mathf.Clamp(this.Center.y, start.y, end.y) - this.Center.y;
            var z = Mathf.Clamp(this.Center.z, start.z, end.z) - this.Center.z;

            var sqrDist = x * x + y * y + z * z;
            return sqrDist <= this.Radius * this.Radius;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAabb(Vector3 start, Vector3 end)
        {
            var x = Mathf.Max(Mathf.Abs(start.x - this.Center.x), Mathf.Abs(end.x - this.Center.x));
            var y = Mathf.Max(Mathf.Abs(start.y - this.Center.y), Mathf.Abs(end.y - this.Center.y));
            var z = Mathf.Max(Mathf.Abs(start.z - this.Center.z), Mathf.Abs(end.z - this.Center.z));

            var sqrDist = x * x + y * y + z * z;
            return sqrDist <= this.Radius * this.Radius;
        }
    }
}
