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

namespace Unity_Tools.Core
{
    /// <summary>
    ///     Interface IVolume
    /// </summary>
    public interface IVolume
    {
        /// <summary>
        ///     Determines whether the <see cref="IVolume" /> fully intersects with the aabb.
        /// </summary>
        /// <param name="start">The start of the aabb.</param>
        /// <param name="end">The end of the aabb.</param>
        /// <returns>Returns <c>true</c> if the aabb is fully inside the shape, <c>false</c> otherwise.</returns>
        bool ContainsAabb(Vector3 start, Vector3 end);

        /// <summary>
        ///     Determines whether the <see cref="IVolume" /> contains the point.
        /// </summary>
        /// <param name="point">
        ///     The point.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the point is contained; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsPoint(Vector3 point);

        /// <summary>
        ///     Determines whether the <see cref="IVolume" /> contains or touches the aabb.
        /// </summary>
        /// <param name="start">
        ///     The start of the aabb.
        /// </param>
        /// <param name="end">
        ///     The end of the aabb.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified start contains or touches the aabb; otherwise, <c>false</c>.
        /// </returns>
        bool IntersectsAabb(Vector3 start, Vector3 end);
    }
}