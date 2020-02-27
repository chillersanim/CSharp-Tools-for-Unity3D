// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PolylineUtil.cs
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

using UnityEngine;

namespace UnityTools.Core
{
    public static class PolylineUtil
    {
        /// <summary>
        /// Calculates the distance of the point to the polyline.
        /// </summary>
        /// <param name="polyline">The polyline.</param>
        /// <param name="point">The point.</param>
        /// <returns>Returns the distance.</returns>
        public static float Distance(this IPolyline polyline, Vector3 point)
        {
            return (point - polyline.ClosestPoint(point)).magnitude;
        }

        /// <summary>
        /// Calculates the square distance of the point to the polyline.
        /// </summary>
        /// <param name="polyline">The polyline.</param>
        /// <param name="point">The point.</param>
        /// <returns>Returns the squared distance.</returns>
        public static float SqrDistance(this IPolyline polyline, Vector3 point)
        {
            return (point - polyline.ClosestPoint(point)).sqrMagnitude;
        }
    }
}
