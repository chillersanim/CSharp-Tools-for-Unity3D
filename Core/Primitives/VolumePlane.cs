// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         VolumePlane.cs
// 
// Created:          28.01.2020  17:10
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

namespace Unity_Tools.Core
{
    /// <summary>
    /// Data structure that represents the volume on the backside (side pointing away from the normal) of a plane.
    /// </summary>
    public struct VolumePlane : IVolume
    {
        private readonly Vector3 negNormal;

        private readonly float distance;

        public VolumePlane(Vector3 normal, float distance)
        {
            this.negNormal = -normal.normalized;
            this.distance = distance;
        }

        public VolumePlane(Vector3 normal, Vector3 point)
        {
            this.negNormal = -normal.normalized;
            this.distance = Vector3.Dot(this.negNormal, point);
        }

        public VolumePlane(Vector3 a, Vector3 b, Vector3 c)
        {
            this.negNormal = -Vector3.Cross(b - a, c - a).normalized;
            this.distance = Vector3.Dot(this.negNormal, a);
        }

        private VolumePlane(Plane plane)
        {
            this.negNormal = -plane.normal;
            this.distance = plane.distance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAabb(Vector3 start, Vector3 end)
        {
            var c = (start + end) / 2f;
            var e = end - c;

            var r = e.x * Mathf.Abs(negNormal.x) + e.y * Mathf.Abs(negNormal.y) + e.z * Mathf.Abs(negNormal.z);
            var s = Vector3.Dot(negNormal, c) - distance;

            return s >= r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector3 point)
        {
            return point.x * negNormal.x + point.y * negNormal.y + point.z * negNormal.z >= distance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsAabb(Vector3 start, Vector3 end)
        {
            if (ContainsPoint(start) || ContainsPoint(end))
            {
                return true;
            }

            var c = (start + end) / 2f;
            var e = end - c;

            var r = e.x * Mathf.Abs(negNormal.x) + e.y * Mathf.Abs(negNormal.y) + e.z * Mathf.Abs(negNormal.z);
            var s = Vector3.Dot(negNormal, c) - distance;

            return s >= -r;
        }

        public static implicit operator VolumePlane (Plane plane)
        {
            return new VolumePlane(plane);
        }

        public static implicit operator Plane(VolumePlane volumePlane)
        {
            return new Plane(volumePlane.negNormal, volumePlane.distance);
        }
    }
}
