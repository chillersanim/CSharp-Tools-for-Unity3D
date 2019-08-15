// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IShape.cs
// 
// Created:          05.08.2019  11:26
// Last modified:    15.08.2019  17:56
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
// 

namespace Unity_Tools.Collections.SpatialTree
{
    /// <summary>
    ///     Interface IShape
    /// </summary>
    public interface IShape
    {
        /// <summary>
        ///     Determines whether the <see cref="IShape" /> contains the point.
        /// </summary>
        /// <param name="point">
        ///     The point.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the point is contained; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsPoint(Vector3 point);

        /// <summary>
        ///     Determines whether the <see cref="IShape" /> contains or touches the aabb.
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

        /// <summary>
        ///     Determines whether the <see cref="IShape" /> touches the aabb but doesn't fully cover the aabb.
        /// </summary>
        /// <param name="start">The start of the aabb.</param>
        /// <param name="end">The end of the aabb.</param>
        /// <returns>Returns <c>true</c> if the shape touches but not fully covers the aabb, <c>false</c> otherwise.</returns>
        /// <remarks>
        ///     Touches but not fully covers means:<br />
        ///     - There is at least one point in the aabb space that is inside the shape and<br />
        ///     - There is at least one point in the aabb space that is outside the shape.
        /// </remarks>
        bool IsAabbNotFullyInside(Vector3 start, Vector3 end);
    }
}