using System;
using UnityEngine;

namespace Unity_Tools.Core.Polyline
{
    public interface IPolyline
    {
        /// <summary>
        /// The length of the line in world units
        /// </summary>
        float Length { get; }

        /// <summary>
        /// Gets the point on the polyline at the given position.
        /// </summary>
        /// <param name="position">The position measured as distance from the start.<br/>Must not be less than zero or more than <see cref="Length"/>.</param>
        /// <returns>Returns the point.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The given position must be between zero and <see cref="Length"/>.</exception>
        Vector3 GetPointAtPosition(float position);

        /// <summary>
        /// Gets the forward direction of the polyline at the given relative position.
        /// </summary>
        /// <param name="position">The position measured as distance from the start.<br/>Must not be less than zero or more than <see cref="Length"/>.</param>
        /// <returns>Returns the forward position.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The given position must be between zero and <see cref="Length"/>.</exception>
        Vector3 GetDirectionAtPosition(float position);

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
    }
}
