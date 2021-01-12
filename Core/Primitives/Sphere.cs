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

using UnityEngine;

namespace Unity_Tools.Core
{
    public struct Sphere : IVolume
    {
        private readonly Vector3 center;

        private readonly float squareRadius;

        public Sphere(Vector3 center, float radius)
        {
            this.center = center;
            this.squareRadius = radius * radius;
        }

        public bool ContainsPoint(Vector3 point)
        {
            var x = point.x - center.x;
            var y = point.y - center.y;
            var z = point.z - center.z;
            var sqrDist = x * x + y * y + z * z;
            return sqrDist <= squareRadius;
        }

        public bool IntersectsAabb(Vector3 start, Vector3 end)
        {
            var x = Mathf.Clamp(center.x, start.x, end.x) - center.x;
            var y = Mathf.Clamp(center.y, start.y, end.y) - center.y;
            var z = Mathf.Clamp(center.z, start.z, end.z) - center.z;
            var sqrDist = x * x + y * y + z * z;
            return sqrDist <= squareRadius;
        }

        public bool ContainsAabb(Vector3 start, Vector3 end)
        {
            var x = Mathf.Max(Mathf.Abs(start.x - center.x), Mathf.Abs(end.x - center.x));
            var y = Mathf.Max(Mathf.Abs(start.y - center.y), Mathf.Abs(end.y - center.y));
            var z = Mathf.Max(Mathf.Abs(start.z - center.z), Mathf.Abs(end.z - center.z));

            var sqrDist = x * x + y * y + z * z;
            return sqrDist <= squareRadius;
        }
    }
}
