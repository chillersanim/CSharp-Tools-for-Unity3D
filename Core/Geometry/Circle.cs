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
using UnityEngine.XR;

namespace UnityTools.Core
{
    public struct Circle : IArea
    {
        public readonly Vector2 Center;

        public readonly float Radius;

        public float Diameter => this.Radius + this.Radius;

        public Circle(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector2 point)
        {
            var x = point.x - this.Center.x;
            var y = point.y - this.Center.y;

            var sqrDist = x * x + y * y;
            return sqrDist <= this.Radius * this.Radius;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsRect(Vector2 start, Vector2 end)
        {
            var x = Mathf.Clamp(this.Center.x, start.x, end.x) - this.Center.x;
            var y = Mathf.Clamp(this.Center.y, start.y, end.y) - this.Center.y;

            var sqrDist = x * x + y * y;
            return sqrDist <= this.Radius * this.Radius;
        }

        /// <inheritdoc />
        public bool Raycast(Vector2 orig, Vector2 dir, out float t, out Vector2 normal)
        {
            t = float.PositiveInfinity;

            var delta = orig - this.Center;
            var deltaDir = dir.x * delta.x + dir.y * delta.y;
            var sqDir = dir.x * dir.x + dir.y * dir.y;
            var sqDelta = delta.x * delta.x + delta.y * delta.y;
            var gamma = deltaDir * deltaDir - sqDir * (sqDelta - this.Radius * this.Radius);

            if (gamma < 0)
            {
                normal = Vector2.zero;
                return false;
            }

            var sqrGamma = Mathf.Sqrt(gamma); 
            var t0 = (-deltaDir + sqrGamma) / sqDir;
            var t1 = (-deltaDir - sqrGamma) / sqDir;

            if (0f <= t0)
            {
                t = t0;
            }

            if (0f <= t1 && t1 < t)
            {
                t = t1;
            }

            if (float.IsPositiveInfinity(t))
            {
                normal = Vector2.zero;
                return false;
            }

            normal = (orig + t * dir - this.Center) / this.Radius;
            return true;
        }

        /// <inheritdoc />
        public Bounds2 Bounds => new Bounds2(this.Center, Vector2.one * this.Radius * 2f);

        /// <inheritdoc />
        public bool Inverted => false;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsRect(Vector2 start, Vector2 end)
        {
            var x = Mathf.Max(Mathf.Abs(start.x - this.Center.x), Mathf.Abs(end.x - this.Center.x));
            var y = Mathf.Max(Mathf.Abs(start.y - this.Center.y), Mathf.Abs(end.y - this.Center.y));

            var sqrDist = x * x + y * y;
            return sqrDist <= this.Radius * this.Radius;
        }
    }
}
