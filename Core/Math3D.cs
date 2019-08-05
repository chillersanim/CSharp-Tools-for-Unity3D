// Copyright © 2019 Jasper Ermatinger

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Collections.Core
{
    public static class Math3D
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MinComponents(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 MaxComponent(Vector3 a, Vector3 b)
        {
            return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        public static Bounds GetBounds([NotNull]this IList<Vector3> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException();
            }

            if (points.Count == 0)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }

            var min = Vector3.positiveInfinity;
            var max = Vector3.negativeInfinity;

            foreach (var point in points)
            {
                min = MinComponents(min, point);
                max = MaxComponent(max, point);
            }

            var size = max - min;
            var center = min + size / 2f;
            return new Bounds(center, size);
        }
    }
}
