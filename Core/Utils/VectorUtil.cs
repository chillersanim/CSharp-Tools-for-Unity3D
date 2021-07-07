// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         VectorUtil.cs
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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityTools.Core
{
    /// <summary>
    /// Class VectorUtil.
    /// </summary>
    public static class VectorUtil
    {
        public static readonly Vector2 NaN2 = new Vector2(float.NaN, float.NaN);

        public static readonly Vector3 NaN3 = new Vector3(float.NaN, float.NaN, float.NaN);

        public static readonly Vector2 NegInf2 = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        public static readonly Vector3 NegInf3 = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public static readonly Vector2 PosInf2 = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Vector3 PosInf3 = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Vector2Int MaxValue2 = new Vector2Int(int.MaxValue, int.MaxValue);

        public static readonly Vector2Int MinValue2 = new Vector2Int(int.MinValue, int.MinValue);

        public static readonly Vector3Int MaxValue3 = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

        public static readonly Vector3Int MinValue3 = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

        public static Vector2 AbsComponents(in this Vector2 vector)
        {
            return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static Vector2Int AbsComponents(in this Vector2Int vector)
        {
            return new Vector2Int(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        }

        public static Vector3 AbsComponents(in this Vector3 vector)
        {
            return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static Vector3Int AbsComponents(in this Vector3Int vector)
        {
            return new Vector3Int(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        }

        public static Vector4 AbsComponents(in this Vector4 vector)
        {
            return new Vector4(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z), Mathf.Abs(vector.w));
        }

        public static (Vector2 min, Vector2 max) Bounds(this IList<Vector2> vectors)
        {
            if (vectors.Count == 0)
            {
                return (Vector2.zero, Vector2.zero);
            }

            var min = Vector2.positiveInfinity;
            var max = Vector2.negativeInfinity;

            foreach (var v in vectors)
            {
                min = Min(in min, in v);
                max = Max(in max, in v);
            }

            return (min, max);
        }

        public static (Vector2Int min, Vector2Int max) Bounds(this IList<Vector2Int> vectors)
        {
            if (vectors.Count == 0)
            {
                return (Vector2Int.zero, Vector2Int.zero);
            }

            var min = MaxValue2;
            var max = MinValue2;

            foreach (var v in vectors)
            {
                min = Min(in min, in v);
                max = Max(in max, in v);
            }

            return (min, max);
        }

        public static (Vector3 min, Vector3 max) Bounds(this IList<Vector3> vectors)
        {
            if (vectors.Count == 0)
            {
                return (Vector3.zero, Vector3.zero);
            }

            var min = Vector3.positiveInfinity;
            var max = Vector3.negativeInfinity;

            foreach (var v in vectors)
            {
                min = Min(in min, in v);
                max = Max(in max, in v);
            }

            return (min, max);
        }

        public static (Vector3Int min, Vector3Int max) Bounds(this IList<Vector3Int> vectors)
        {
            if (vectors.Count == 0)
            {
                return (Vector3Int.zero, Vector3Int.zero);
            }

            var min = MaxValue3;
            var max = MinValue3;

            foreach (var v in vectors)
            {
                min = Min(in min, in v);
                max = Max(in max, in v);
            }

            return (min, max);
        }

        /// <summary>
        /// Clamps the components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2 ClampComponents(in this Vector2 vector, in Vector2 min, in Vector2 max)
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
        public static Vector2Int ClampComponents(in this Vector2Int vector, in Vector2Int min, in Vector2Int max)
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
        public static Vector3 ClampComponents(in this Vector3 vector, in Vector3 min, in Vector3 max)
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
        public static Vector3Int ClampComponents(in this Vector3Int vector, in Vector3Int min, in Vector3Int max)
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
        public static Vector4 ClampComponents(in this Vector4 vector, in Vector4 min, in Vector4 max)
        {
            return new Vector4(
                Mathf.Clamp(vector.x, min.x, max.x),
                Mathf.Clamp(vector.y, min.y, max.y),
                Mathf.Clamp(vector.z, min.z, max.z),
                Mathf.Clamp(vector.w, min.w, max.w)
            );
        }

        /// <summary>
        /// Determines whether a vector is inside or on the boundary of an axis aligned bounding box (aabb).
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="center">The center of the aabb.</param>
        /// <param name="size">The size of the aabb.</param>
        /// <returns><c>true</c> if the vector is inside or on the aabb; otherwise, <c>false</c>.</returns>
        [PublicAPI]
        public static bool IsInAabb(in this Vector3 vector, in Vector3 center, in Vector3 size)
        {
            var halfSize = size / 2f;
            var min = center - halfSize;
            var max = center + halfSize;

            return vector.x >= min.x && vector.y >= min.y && vector.z >= min.z &&
                   vector.x <= max.x && vector.y <= max.y && vector.z <= max.z;
        }

        /// <summary>
        /// Determines whether a vector is inside or on a sphere.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns><c>true</c> if the vector is inside or on the sphere; otherwise, <c>false</c>.</returns>
        [PublicAPI]
        public static bool IsInSphere(in this Vector3 vector, in Vector3 center, in float radius)
        {
            var x = vector.x - center.x;
            var y = vector.y - center.y;
            var z = vector.z - center.z;

            return x * x + y * y + z * z <= radius * radius;
        }

        /// <summary>
        /// Determines whether the vector has any infinity components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Returns <c>true</c> if for at least one component <see cref="float.IsInfinity"/> returns true, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool IsInfinite(in this Vector2 vector)
        {
            return float.IsInfinity(vector.x) || float.IsInfinity(vector.y);
        }

        /// <summary>
        /// Determines whether the vector has any infinity components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Returns <c>true</c> if for at least one component <see cref="float.IsInfinity"/> returns true, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool IsInfinite(in this Vector3 vector)
        {
            return float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z);
        }

        /// <summary>
        /// Determines whether the vector has any infinity components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Returns <c>true</c> if for at least one component <see cref="float.IsInfinity"/> returns true, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool IsInfinite(in this Vector4 vector)
        {
            return float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z) || float.IsInfinity(vector.w);
        }

        /// <summary>
        /// Determines whether the vector has any nan components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Returns <c>true</c> if at least one component is <see cref="float.NaN"/>, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool IsNaN(in this Vector2 vector)
        {
            return float.IsNaN(vector.x) || float.IsNaN(vector.y);
        }

        /// <summary>
        /// Determines whether the vector has any nan components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Returns <c>true</c> if at least one component is <see cref="float.NaN"/>, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool IsNaN(in this Vector3 vector)
        {
            return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
        }

        /// <summary>
        /// Determines whether the vector has any nan components.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Returns <c>true</c> if at least one component is <see cref="float.NaN"/>, <c>false</c> otherwise.</returns>
        [PublicAPI]
        public static bool IsNaN(in this Vector4 vector)
        {
            return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z) || float.IsNaN(vector.w);
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2 Max(in Vector2 a, in Vector2 b)
        {
            return new Vector2(
                a.x > b.x ? a.x : b.x,
                a.y > b.y ? a.y : b.y
            );
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2Int Max(in Vector2Int a, in Vector2Int b)
        {
            return new Vector2Int(
                a.x > b.x ? a.x : b.x,
                a.y > b.y ? a.y : b.y
            );
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector3 Max(in Vector3 a, in Vector3 b)
        {
            return new Vector3(
                a.x > b.x ? a.x : b.x,
                a.y > b.y ? a.y : b.y,
                a.z > b.z ? a.z : b.z
            );
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector3Int Max(in Vector3Int a, in Vector3Int b)
        {
            return new Vector3Int(
                a.x > b.x ? a.x : b.x,
                a.y > b.y ? a.y : b.y,
                a.z > b.z ? a.z : b.z
            );
        }

        /// <summary>
        /// Gets the maximum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector4 Max(in Vector4 a, in Vector4 b)
        {
            return new Vector4(
                a.x > b.x ? a.x : b.x,
                a.y > b.y ? a.y : b.y,
                a.z > b.z ? a.z : b.z,
                a.w > b.w ? a.w : b.w
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2 Min(in Vector2 a, in Vector2 b)
        {
            return new Vector2(
                a.x < b.x ? a.x : b.x,
                a.y < b.y ? a.y : b.y
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector2Int Min(in Vector2Int a, in Vector2Int b)
        {
            return new Vector2Int(
                a.x < b.x ? a.x : b.x,
                a.y < b.y ? a.y : b.y
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector3 Min(in Vector3 a, in Vector3 b)
        {
            return new Vector3(
                a.x < b.x ? a.x : b.x,
                a.y < b.y ? a.y : b.y,
                a.z < b.z ? a.z : b.z
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector3Int Min(in Vector3Int a, in Vector3Int b)
        {
            return new Vector3Int(
                a.x < b.x ? a.x : b.x,
                a.y < b.y ? a.y : b.y,
                a.z < b.z ? a.z : b.z
            );
        }

        /// <summary>
        /// Gets the minimum from a and b for all components.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2.</returns>
        [PublicAPI]
        public static Vector4 Min(in Vector4 a, in Vector4 b)
        {
            return new Vector4(
                a.x < b.x ? a.x : b.x,
                a.y < b.y ? a.y : b.y,
                a.z < b.z ? a.z : b.z,
                a.w < b.w ? a.w : b.w
            );
        }

        public static double PreciseDistance(in Vector3 a, in Vector3 b)
        {
            var x = (double) (a.x) - b.x;
            var y = (double) (a.y) - b.y;
            var z = (double) (a.z) - b.z;

            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Scales the vector component wise by another vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">The scale.</param>
        public static Vector2 ScaleComponents(in this Vector2 v, in Vector2 scale)
        {
            return new Vector2(v.x * scale.x, v.y * scale.y);
        }

        /// <summary>
        /// Scales the vector component wise by another vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">The scale.</param>
        public static Vector2Int ScaleComponents(in this Vector2Int v, in Vector2Int scale)
        {
            return new Vector2Int(v.x * scale.x, v.y * scale.y);
        }

        /// <summary>
        /// Scales the vector component wise by another vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">The scale.</param>
        public static Vector3 ScaleComponents(in this Vector3 v, in Vector3 scale)
        {
            return new Vector3(v.x * scale.x, v.y * scale.y, v.z * scale.z);
        }

        /// <summary>
        /// Scales the vector component wise by another vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <param name="scale">The scale.</param>
        public static Vector3Int ScaleComponents(in this Vector3Int v, in Vector3Int scale)
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
        public static float SqrDistance(in Vector2 a, in Vector2 b)
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
        public static float SqrDistance(in Vector2Int a, in Vector2Int b)
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
        public static float SqrDistance(in Vector3 a, in Vector3 b)
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
        public static float SqrDistance(in Vector3Int a, in Vector3Int b)
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
        public static float SqrDistance(in Vector4 a, in Vector4 b)
        {
            var x = a.x - b.x;
            var y = a.y - b.y;
            var z = a.z - b.z;
            var w = a.w - b.w;
            return x * x + y * y + z * z + w * w;
        }
        
        /// <summary>
        /// Shortcut method for: <b>new Vector2(v.x, v.z)</b>
        /// </summary>
        public static Vector2 AsXZ(in this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        /// <summary>
        /// Shortcut method for: <b>new Vector3(v.x, 0f, v.y)</b>
        /// </summary>
        public static Vector3 AsX0Y(in this Vector2 v)
        {
            return new Vector3(v.x, 0f, v.y);
        }
    }
}