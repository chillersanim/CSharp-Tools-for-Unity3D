// Copyright © 2019 Jasper Ermatinger

#region usings

using UnityEngine;

#endregion

namespace Unity_Collections.SpatialTree
{
    #region Usings

    #endregion

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