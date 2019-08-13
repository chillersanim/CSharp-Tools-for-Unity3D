// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         VectorMath.cs
// 
// Created:          09.08.2019  15:13
// Last modified:    09.08.2019  15:54
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

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Tools.Core
{
    /// <summary>
    /// Class VectorMath.
    /// </summary>
    public static class VectorMath
    {
        public static readonly Vector2 NaN2 = new Vector2(float.NaN, float.NaN);

        public static readonly Vector3 NaN3 = new Vector3(float.NaN, float.NaN, float.NaN);

        public static Vector2 AbsComponents(this Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static Vector2Int AbsComponents(this Vector2Int vector)
        {
            return new Vector2Int(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static Vector3 AbsComponents(this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static Vector3Int AbsComponents(this Vector3Int vector)
        {
            return new Vector3Int(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static Vector4 AbsComponents(this Vector4 vector)
        {
            return new Vector4(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z), Mathf.Abs(vector.w));
        }

        public static (Vector2 min, Vector2 max) Bounds(this IList<Vector2> vectors)
        {
            if (vectors.Count == 0)
            {
                return (Vector2.zero, Vector2.zero);
            }

            var min = vectors[0];
            var max = min;

            foreach (var v in vectors)
            {
                min = Min(min, v);
                max = Max(max, v);
            }

            return (min, max);
        }

        public static (Vector2Int min, Vector2Int max) Bounds(this IList<Vector2Int> vectors)
        {
            if (vectors.Count == 0)
            {
                return (Vector2Int.zero, Vector2Int.zero);
            }

            var min = vectors[0];
            var max = min;

            foreach (var v in vectors)
            {
                min = Min(min, v);
                max = Max(max, v);
            }

            return (min, max);
        }

        public static Bounds Bounds(this IList<Vector3> vectors)
        {
            if (vectors.Count == 0)
            {
                return new Bounds();
            }

            var bounds = new Bounds(vectors[0], Vector3.zero);

            foreach (var v in vectors)
            {
                bounds.Encapsulate(v);
            }

            return bounds;
        }

        public static BoundsInt Bounds(this IList<Vector3Int> vectors)
        {
            if (vectors.Count == 0)
            {
                return new BoundsInt();
            }

            var min = vectors[0];
            var max = min;

            foreach (var v in vectors)
            {
                min = Min(min, v);
                max = Max(max, v);
            }

            var size = max - min;
            return new BoundsInt(min.x, min.y, min.z, size.x, size.y, size.z);
        }

        /// <summary>
        /// Clamps the components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2 ClampComponents(this Vector2 vector, Vector2 min, Vector2 max)
        {
            return new Vector2(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y)
            );
        }

        /// <summary>
        /// Clamps the components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>Vector2Int.</returns>
        [PublicAPI]
        public static Vector2Int ClampComponents(this Vector2Int vector, Vector2Int min, Vector2Int max)
        {
            return new Vector2Int(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y)
            );
        }

        /// <summary>
        /// Clamps the components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>Vector3.</returns>
        [PublicAPI]
        public static Vector3 ClampComponents(this Vector3 vector, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y),
                Mathf.Clamp(vector.z, min.z, max.z)
            );
        }

        /// <summary>
        /// Clamps the components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>Vector3Int.</returns>
        [PublicAPI]
        public static Vector3Int ClampComponents(this Vector3Int vector, Vector3Int min, Vector3Int max)
        {
            return new Vector3Int(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y),
                Mathf.Clamp(vector.z, min.z, max.z)
            );
        }

        /// <summary>
        /// Clamps the components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>Vector4.</returns>
        [PublicAPI]
        public static Vector4 ClampComponents(this Vector4 vector, Vector4 min, Vector4 max)
        {
            return new Vector4(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y),
                Mathf.Clamp(vector.z, min.z, max.z),
                Mathf.Clamp(vector.w, min.w, max.w)
            );
        }

        /// <summary>
        /// Determines whether a vector is inside or on a sphere.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns><c>true</c> if the vector is inside or on the sphere; otherwise, <c>false</c>.</returns>
        [PublicAPI]
        public static bool IsInSphere(this Vector3 vector, Vector3 center, float radius)
        {
            var x = vector.x - center.x;
            var y = vector.y - center.y;
            var z = vector.z - center.z;

            return x * x + y * y + z * z <= radius * radius;
        }

        /// <summary>
        /// Determines whether a vector is inside or on the boundary of an axis aligned bounding box (aabb).
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="center">The center of the aabb.</param>
        /// <param name="size">The size of the aabb.</param>
        /// <returns><c>true</c> if the vector is inside or on the aabb; otherwise, <c>false</c>.</returns>
        [PublicAPI]
        public static bool IsInAabb(this Vector3 vector, Vector3 center, Vector3 size)
        {
            var halfSize = size / 2f;
            var min = center - halfSize;
            var max = center + halfSize;

            return vector.x >= min.x && vector.y >= min.y && vector.z >= min.z &&
                   vector.x <= max.x && vector.y <= max.y && vector.z <= max.z;
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(
                Mathf.Max(a.x, b.x),
                Mathf.Max(a.y, b.y)
            );
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2Int Max(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(
                Mathf.Max(a.x, b.x),
                Mathf.Max(a.y, b.y)
            );
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(
                Mathf.Max(a.x, b.x),
                Mathf.Max(a.y, b.y),
                Mathf.Max(a.z, b.z)
            );
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector3Int Max(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(
                Mathf.Max(a.x, b.x),
                Mathf.Max(a.y, b.y),
                Mathf.Max(a.z, b.z)
            );
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector4 Max(Vector4 a, Vector4 b)
        {
            return new Vector4(
                Mathf.Max(a.x, b.x),
                Mathf.Max(a.y, b.y),
                Mathf.Max(a.z, b.z),
                Mathf.Max(a.w, b.w)
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(
                Mathf.Min(a.x, b.x),
                Mathf.Min(a.y, b.y)
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2Int Min(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(
                Mathf.Min(a.x, b.x),
                Mathf.Min(a.y, b.y)
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(
                Mathf.Min(a.x, b.x),
                Mathf.Min(a.y, b.y),
                Mathf.Min(a.z, b.z)
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector3Int Min(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(
                Mathf.Min(a.x, b.x),
                Mathf.Min(a.y, b.y),
                Mathf.Min(a.z, b.z)
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector4 Min(Vector4 a, Vector4 b)
        {
            return new Vector4(
                Mathf.Min(a.x, b.x),
                Mathf.Min(a.y, b.y),
                Mathf.Min(a.z, b.z),
                Mathf.Min(a.w, b.w)
            );
        }

        /// <summary>
        /// Scales the vector component wise by another vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">The scale.</param>
        public static Vector2 ScaleComponents(this Vector2 v, Vector2 scale)
        {
            return new Vector2(v.x * scale.x, v.y * scale.y);
        }

        /// <summary>
        /// Scales the vector component wise by another vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">The scale.</param>
        public static Vector2Int ScaleComponents(this Vector2Int v, Vector2Int scale)
        {
            return new Vector2Int(v.x * scale.x, v.y * scale.y);
        }

        /// <summary>
        /// Scales the vector component wise by another vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">The scale.</param>
        public static Vector3 ScaleComponents(this Vector3 v, Vector3 scale)
        {
            return new Vector3(v.x * scale.x, v.y * scale.y, v.z * scale.z);
        }

        /// <summary>
        /// Scales the vector component wise by another vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">The scale.</param>
        public static Vector3Int ScaleComponents(this Vector3Int v, Vector3Int scale)
        {
            return new Vector3Int(v.x * scale.x, v.y * scale.y, v.z * scale.z);
        }

        /// <summary>
        /// Calculates the square distance between a and b.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Single.</returns>
        [PublicAPI]
        public static float SqrDistance(Vector2 a, Vector2 b)
        {
            var x = a.x - b.x;
            var y = a.y - b.y;
            return x * x + y * y;
        }

        /// <summary>
        /// Calculates the square distance between a and b.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Single.</returns>
        [PublicAPI]
        public static float SqrDistance(Vector2Int a, Vector2Int b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            return x * x + y * y;
        }

        /// <summary>
        /// Calculates the square distance between a and b.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Single.</returns>
        [PublicAPI]
        public static float SqrDistance(Vector3 a, Vector3 b)
        {
            var x = a.x - b.x;
            var y = a.y - b.y;
            var z = a.z - b.z;
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// Calculates the square distance between a and b.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Single.</returns>
        [PublicAPI]
        public static float SqrDistance(Vector3Int a, Vector3Int b)
        {
            float x = a.x - b.x;
            float y = a.y - b.y;
            float z = a.z - b.z;
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// Calculates the square distance between a and b.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Single.</returns>
        [PublicAPI]
        public static float SqrDistance(Vector4 a, Vector4 b)
        {
            var x = a.x - b.x;
            var y = a.y - b.y;
            var z = a.z - b.z;
            var w = a.w - b.w;
            return x * x + y * y + z * z + w * w;
        }
    }
}