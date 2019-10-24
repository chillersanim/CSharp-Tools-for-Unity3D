// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CuttingEar.cs
// 
// Created:          20.08.2019  17:28
// Last modified:    25.08.2019  15:59
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
using System.Runtime.CompilerServices;
using Unity_Tools.Collections;
using Unity_Tools.Core.Pooling;
using UnityEngine;

namespace Unity_Tools.Core
{
    public static class CuttingEar
    {
        private static readonly List<int> cache = new List<int>();

        /// <summary>
        /// Triangulates the shape and outputs an array of triangle indices.
        /// </summary>
        /// <param name="shape">The shape to triangulate.</param>
        /// <returns>Returns an array of indices, where each index points to a point in the shape. The amount of indices is a multiple of 3.</returns>
        public static int[] Triangulate(this IList<Vector3> shape)
        {
            cache.Clear();
            Triangulate(shape, cache);
            return cache.ToArray();
        }

        /// <summary>
        /// Triangulates the shape and outputs an array of triangle indices.
        /// </summary>
        /// <param name="shape">The shape to triangulate.</param>
        /// <param name="normal">The normal of the shape.</param>
        /// <param name="isConvex">Set to true if the polygon is convex. If unknown, use <see cref="Math3D.IsPolygonConvex(IList{Vector3})"/> to determine convexity./></param>
        /// <returns>Returns an array of indices, where each index points to a point in the shape. The amount of indices is a multiple of 3.</returns>
        public static int[] Triangulate(this IList<Vector3> shape, Vector3 normal, bool isConvex)
        {
            cache.Clear();
            Triangulate(shape, normal, isConvex, cache);
            return cache.ToArray();
        }

        /// <summary>
        /// Triangulates the shape and outputs an array of triangle indices.
        /// </summary>
        /// <param name="vertices">The vertices of the shape.</param>
        /// <param name="indices">The indices reflecting the order of the shape vertices.</param>
        /// <returns>Returns an array of indices, where each index points to a point in the shape. The amount of indices is a multiple of 3.</returns>
        public static int[] Triangulate(this IList<Vector3> vertices, IList<int> indices)
        {
            cache.Clear();
            Triangulate(vertices, indices, cache);
            return cache.ToArray();
        }

        /// <summary>
        /// Triangulates the shape and outputs an array of triangle indices.
        /// </summary>
        /// <param name="vertices">The vertices of the shape.</param>
        /// <param name="indices">The indices reflecting the order of the shape vertices.</param>
        /// <param name="normal">The normal of the shape.</param>
        /// <param name="isConvex">Set to true if the polygon is convex. If unknown, use <see cref="Math3D.IsPolygonConvex(IList{Vector3})"/> to determine convexity./></param>
        /// <returns>Returns an array of indices, where each index points to a point in the shape. The amount of indices is a multiple of 3.</returns>
        public static int[] Triangulate(this IList<Vector3> vertices, IList<int> indices, Vector3 normal, bool isConvex)
        {
            cache.Clear();
            Triangulate(vertices, indices, normal, isConvex, cache);
            return cache.ToArray();
        }

        /// <summary>
        /// Triangulates the shape and outputs an array of triangle indices.
        /// </summary>
        /// <param name="shape">The shape to triangulate.</param>
        /// <param name="output">Output list where the resulting triangle indices will be inserted.</param>
        public static void Triangulate(this IList<Vector3> shape, List<int> output)
        {
            if (shape.Count < 3)
            {
                throw new ArgumentException("The polygon must have at least 3 points.");
            }

            switch (shape.Count)
            {
                case 3:
                    AddTriangleToOutput(output, new Vector3Int(0, 1, 2));
                    return;
                case 4:
                    TriangulateQuad(shape[0], shape[1], shape[2], shape[3], out var t0, out var t1);
                    AddTriangleToOutput(output, t0);
                    AddTriangleToOutput(output, t1);
                    break;
                default:
                    if (Math3D.IsPolygonConvex(shape))
                    {
                        TriangulateConvexPolygon(shape.Count, output);
                    }
                    else
                    {
                        var indices = CollectionUtil.CreateList(shape.Count, 0, i => i + 1);
                        var normal = Math3D.ExactAveragedPolygonNormal(shape);
                        TriangulateConcavePolygon(shape, indices, normal, output);
                        GlobalListPool<int>.Put(indices);
                    }

                    break;
            }
        }

        /// <summary>
        /// Triangulates the shape and outputs an array of triangle indices.
        /// </summary>
        /// <param name="shape">The shape to triangulate.</param>
        /// <param name="normal">The normal of the shape.</param>
        /// <param name="isConvex">Set to true if the polygon is convex. If unknown, use <see cref="Math3D.IsPolygonConvex(IList{Vector3})"/> to determine convexity./></param>
        /// <param name="output">Output list where the resulting triangle indices will be inserted.</param>
        public static void Triangulate(this IList<Vector3> shape, Vector3 normal, bool isConvex, List<int> output)
        {
            if (shape.Count < 3)
            {
                throw new ArgumentException("The polygon must have at least 3 points.");
            }

            switch (shape.Count)
            {
                case 3:
                    AddTriangleToOutput(output, new Vector3Int(0, 1, 2));
                    return;
                case 4:
                    if (isConvex)
                    {
                        AddTriangleToOutput(output, new Vector3Int(0, 1, 2));
                        AddTriangleToOutput(output, new Vector3Int(0, 2, 3));
                    }
                    else
                    {
                        TriangulateQuad(shape[0], shape[1], shape[2], shape[3], out var t0, out var t1);
                        AddTriangleToOutput(output, t0);
                        AddTriangleToOutput(output, t1);
                    }
                    break;
                default:
                    if (isConvex)
                    {
                        TriangulateConvexPolygon(shape.Count, output);
                    }
                    else
                    {
                        var indices = CollectionUtil.CreateList(shape.Count, 0, i => i + 1);
                        TriangulateConcavePolygon(shape, indices, normal, output);
                        GlobalListPool<int>.Put(indices);
                    }
                    break;
            }
        }

        /// <summary>
        /// Triangulates the shape and outputs an array of triangle indices.
        /// </summary>
        /// <param name="vertices">The vertices of the shape.</param>
        /// <param name="indices">The indices reflecting the order of the shape vertices.</param>
        /// <param name="output">Output list where the resulting triangle indices will be inserted.</param>
        public static void Triangulate(this IList<Vector3> vertices, IList<int> indices, List<int> output)
        {
            if (vertices.Count < 3)
            {
                throw new ArgumentException("The polygon must have at least 3 points.");
            }

            switch (vertices.Count)
            {
                case 3:
                    AddTriangleToOutput(output, new Vector3Int(indices[0], indices[1], indices[2]));
                    return;
                case 4:
                    TriangulateQuad(vertices, indices[0], indices[1], indices[2], indices[3], out var t0, out var t1);
                    AddTriangleToOutput(output, t0);
                    AddTriangleToOutput(output, t1);
                    break;
                default:
                    if (Math3D.IsPolygonConvex(vertices, indices))
                    {
                        TriangulateConvexPolygon(indices, output);
                    }
                    else
                    {
                        var indicesClone = GlobalListPool<int>.Get(indices.Count);
                        indicesClone.AddRange(indices);
                        var normal = Math3D.ExactAveragedPolygonNormal(vertices);
                        TriangulateConcavePolygon(vertices, indicesClone, normal, output);
                        GlobalListPool<int>.Put(indicesClone);
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// Triangulates the shape and outputs an array of triangle indices.
        /// </summary>
        /// <param name="vertices">The vertices of the shape.</param>
        /// <param name="indices">The indices reflecting the order of the shape vertices.</param>
        /// <param name="normal">The normal of the shape.</param>
        /// <param name="isConvex">Set to true if the polygon is convex. If unknown, use <see cref="Math3D.IsPolygonConvex(IList{Vector3})"/> to determine convexity./></param>
        /// <param name="output">Output list where the resulting triangle indices will be inserted.</param>
        public static void Triangulate(this IList<Vector3> vertices, IList<int> indices, Vector3 normal, bool isConvex, List<int> output)
        {
            if (vertices.Count < 3)
            {
                throw new ArgumentException("The polygon must have at least 3 points.");
            }

            switch (vertices.Count)
            {
                case 3:
                    AddTriangleToOutput(output, new Vector3Int(indices[0], indices[1], indices[2]));
                    return;
                case 4:
                    if (isConvex)
                    {
                        AddTriangleToOutput(output, new Vector3Int(indices[0], indices[1], indices[2]));
                        AddTriangleToOutput(output, new Vector3Int(indices[0], indices[2], indices[3]));
                    }
                    else
                    {
                        TriangulateQuad(vertices, indices[0], indices[1], indices[2], indices[3], out var t0,
                            out var t1);
                        AddTriangleToOutput(output, t0);
                        AddTriangleToOutput(output, t1);
                    }

                    break;
                default:
                    if (isConvex)
                    {
                        TriangulateConvexPolygon(indices, output);
                    }
                    else
                    {
                        var indicesClone = GlobalListPool<int>.Get(indices.Count);
                        indicesClone.AddRange(indices);
                        TriangulateConcavePolygon(vertices, indicesClone, normal, output);
                        GlobalListPool<int>.Put(indicesClone);
                    }

                    break;
            }
        }

        /// <summary>
        /// Triangulates a quad into two triangles spanning from p0 to p1 to p2 to p3 back to p0.
        /// </summary>
        /// <param name="p0">The first point of the quad.</param>
        /// <param name="p1">The second point of the quad.</param>
        /// <param name="p2">The third point of the quad.</param>
        /// <param name="p3">The fourth point of the quad.</param>
        /// <param name="t0">The indices of the first triangle.</param>
        /// <param name="t1">The indices of the second triangle.</param>
        public static void TriangulateQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, out Vector3Int t0,
            out Vector3Int t1)
        {
            var v0 = p1 - p0;
            var v1 = p2 - p1;
            var v2 = p3 - p2;
            var v3 = p0 - p3;

            if (Vector3.Dot(Vector3.Cross(v0, v1), Vector3.Cross(v2, v3)) > 0)
            {
                t0 = new Vector3Int(0, 1, 2);
                t1 = new Vector3Int(2, 3, 0);
            }
            else
            {
                // Else connect the not convex edges
                t0 = new Vector3Int(1, 2, 3);
                t1 = new Vector3Int(3, 0, 1);
            }
        }

        /// <summary>
        /// Triangulates a quad into two triangles spanning from p0 to p1 to p2 to p3 back to p0.
        /// </summary>
        /// <param name="vertices">The vertices of the quad.</param>
        /// <param name="i0">The first vertex index of the quad.</param>
        /// <param name="i1">The second vertex index of the quad.</param>
        /// <param name="i2">The third vertex index of the quad.</param>
        /// <param name="i3">The fourth vertex index of the quad.</param>
        /// <param name="t0">The indices of the first triangle.</param>
        /// <param name="t1">The indices of the second triangle.</param>
        public static void TriangulateQuad(this IList<Vector3> vertices, int i0, int i1, int i2, int i3, out Vector3Int t0,
            out Vector3Int t1)
        {
            var p0 = vertices[i0];
            var p1 = vertices[i1];
            var p2 = vertices[i2];
            var p3 = vertices[i3];

            var v0 = p1 - p0;
            var v1 = p2 - p1;
            var v2 = p3 - p2;
            var v3 = p0 - p3;

            if (Vector3.Dot(Vector3.Cross(v0, v1), Vector3.Cross(v2, v3)) > 0)
            {
                t0 = new Vector3Int(i0, i1, i2);
                t1 = new Vector3Int(i2, i3, i0);
            }
            else
            {
                // Else connect the not convex edges
                t0 = new Vector3Int(i1, i2, i3);
                t1 = new Vector3Int(i3, i0, i1);
            }
        }

        private static void TriangulateConcavePolygon(IList<Vector3> vertices, List<int> indexes, Vector3 normal, List<int> output)
        {
            var minCapacity = (indexes.Count - 2) * 3;
            if (output.Capacity < minCapacity)
            {
                output.Capacity = minCapacity;
            }

            while (indexes.Count > 3)
            {
                var earIndex = GetEarPoint(vertices, indexes, normal);

                if (earIndex == -1)
                {
                    throw new InvalidOperationException("The provided polygon data is corrupt.");
                }

                var i0 = indexes[earIndex > 0 ? earIndex - 1 : indexes.Count - 1];
                var i1 = indexes[earIndex];
                var i2 = indexes[(earIndex + 1) % indexes.Count];

                output.Add(i0);
                output.Add(i1);
                output.Add(i2);

                indexes.RemoveAt(earIndex);
            }

            if (indexes.Count == 3)
            {
                output.Add(indexes[0]);
                output.Add(indexes[1]);
                output.Add(indexes[2]);
            }
        }

        private static void TriangulateConvexPolygon(int vertexAmount, List<int> output)
        {
            var origin = 0;

            for (var i = 1; i < vertexAmount - 1; i++)
            {
                output.Add(origin);
                output.Add(i);
                output.Add(i + 1);
            }
        }

        private static void TriangulateConvexPolygon(IList<int> indexes, List<int> output)
        {
            var origin = indexes[0];

            for (var i = 1; i < indexes.Count - 1; i++)
            {
                output.Add(origin);
                output.Add(indexes[i]);
                output.Add(indexes[i + 1]);
            }
        }

        private static int GetEarPoint(IList<Vector3> vertices, List<int> indexes, Vector3 normale)
        {
            for (var i = 0; i < indexes.Count; i++)
            {
                var i0 = indexes[i > 0 ? i - 1 : indexes.Count - 1];
                var i1 = indexes[i];
                var i2 = indexes[(i + 1) % indexes.Count];

                var v1 = vertices[i1] - vertices[i0];
                var v2 = vertices[i2] - vertices[i1];

                var isInnerAngle = Vector3.Dot(normale, Vector3.Cross(v1, v2)) > 0;
                if (!isInnerAngle)
                {
                    continue;
                }

                var isValid = true;

                // Make sure no other point is in the triangle candidate (in case of con
                foreach (var ix in indexes)
                {
                    if (ix == i0 || ix == i1 || ix == i2)
                    {
                        continue;
                    }

                    if (Math3D.IsPointInTriangle(vertices[i0], vertices[i1], vertices[i2], vertices[ix]))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    return i;
                }
            }

            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AddTriangleToOutput(List<int> output, Vector3Int triangle)
        {
            output.Add(triangle.x);
            output.Add(triangle.y);
            output.Add(triangle.z);
        }
    }
}
