// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IPolyline.cs
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
using UnityEngine;

namespace Unity_Tools.Core
{
    public interface IPolyline
    {
        /// <summary>
        /// The length of the line in world units
        /// </summary>
        float Length { get; }

        /// <summary>
        /// Finds the point on the polyline that is nearest to the given point.
        /// </summary>
        /// <param name="point">The point relative to the polyline.</param>
        /// <returns>Returns the nearest point.</returns>
        Vector3 ClosestPoint(Vector3 point);

        /// <summary>
        /// Returns the position on the polyline that is nearest to the given point.
        /// </summary>
        /// <param name="point">The point relative to the polyline.</param>
        /// <returns>Returns the nearest position measured as distance from the start.</returns>
        float ClosestPosition(Vector3 point);

        /// <summary>
        /// Gets the point on the polyline at the given position.
        /// </summary>
        /// <param name="position">The position measured as distance from the start.<br/>Must be between zero (incl) and <see cref="Length"/> (incl).</param>
        /// <returns>Returns the point.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The given position must be between zero and <see cref="Length"/>.</exception>
        Vector3 GetPoint(float position);

        /// <summary>
        /// Gets the forward direction of the polyline at the given relative position.
        /// </summary>
        /// <param name="position">The position measured as distance from the start.<br/>Must be between zero (incl) and <see cref="Length"/> (incl).</param>
        /// <returns>Returns the forward position.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The given position must be between zero and <see cref="Length"/>.</exception>
        Vector3 GetTangent(float position);
    }
}
