// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IVolume.cs
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

using UnityEngine;

namespace UnityTools.Core
{
    /// <summary>
    ///     Interface IArea
    /// </summary>
    public interface IArea
    {
        Bounds2 Bounds { get; }

        bool Inverted { get; }

        /// <summary>
        ///     Determines whether the <see cref="IArea" /> fully intersects with the rect.
        /// </summary>
        /// <param name="start">The start of the rect.</param>
        /// <param name="end">The end of the rect.</param>
        /// <returns>Returns <c>true</c> if the rect is fully inside the shape, <c>false</c> otherwise.</returns>
        bool ContainsRect(Vector2 start, Vector2 end);

        /// <summary>
        ///     Determines whether the <see cref="IArea" /> contains the point.
        /// </summary>
        /// <param name="point">
        ///     The point.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the point is contained; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsPoint(Vector2 point);

        /// <summary>
        ///     Determines whether the <see cref="IArea" /> contains or touches the rect.
        /// </summary>
        /// <param name="start">
        ///     The start of the rect.
        /// </param>
        /// <param name="end">
        ///     The end of the rect.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified start contains or touches the rect; otherwise, <c>false</c>.
        /// </returns>
        bool IntersectsRect(Vector2 start, Vector2 end);

        bool Raycast(Vector2 orig, Vector2 dir, out float t, out Vector2 normal);
    }
}