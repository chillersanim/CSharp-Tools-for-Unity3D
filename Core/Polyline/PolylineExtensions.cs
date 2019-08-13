using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity_Tools.Core.Polyline;
using UnityEngine;

namespace Assets.Unity_Tools.Core.Polyline
{
    public static class PolylineExtensions
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
