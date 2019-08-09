// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         MeshUtil.cs
// 
// Created:          09.08.2019  15:50
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
#region usings

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Unity_Tools.Core
{
    #region Usings

    #endregion

    /// <summary>
    ///     The mesh util.
    /// </summary>
    public static class MeshUtil
    {
        /// <summary>
        ///     Determines whether the mesh consisting of the vertices and triangles is convex.
        /// </summary>
        /// <param name="vertices">
        ///     The vertices.
        /// </param>
        /// <param name="triangles">
        ///     The triangles.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsConvex(IList<Vector3> vertices, IList<int> triangles)
        {
            // For every triangle, generate a plane
            for (var i = 0; i < triangles.Count; i += 3)
            {
                var p0 = vertices[triangles[i]];
                var p1 = vertices[triangles[i + 1]];
                var p2 = vertices[triangles[i + 2]];

                var plane = new Plane(p0, p1, p2);

                // Test if all vertices are behind the plane
                foreach (var v in vertices)
                {
                    if (plane.GetSide(v))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Determines whether the mesh consisting of the vertices and triangles is approximately a cuboid.
        /// </summary>
        /// <param name="vertices">
        ///     The vertices.
        /// </param>
        /// <param name="triangles">
        ///     The triangles.
        /// </param>
        /// <param name="precision">
        ///     The precision.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsCuboid(IList<Vector3> vertices, IList<int> triangles, float precision = 0.01f)
        {
            if (precision < 0)
            {
                precision = -precision;
            }

            var min = Vector3.positiveInfinity;
            var max = Vector3.negativeInfinity;

            // Calculate bounding box
            foreach (var v in vertices)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }

            // Calculate volume of mesh
            var volume = Math3D.VolumeOfMesh(vertices, triangles);

            // If volume of the mesh is similar enough to the volume of the bounding box, the mesh is considered a cuboid (99% is good enough)
            var boundingVolume = (max.x - min.x) * (max.y - min.y) * (max.z - min.z);
            return Math.Abs(1 - volume / boundingVolume) < precision;
        }

        /// <summary>
        ///     Calculates the intersection between a mesh and a plane and returns the outline of the intersection.
        /// </summary>
        /// <param name="vertices">
        ///     The vertices of the mesh.
        /// </param>
        /// <param name="triangles">
        ///     The triangles of the mesh.
        /// </param>
        /// <param name="localPlane">
        ///     The plane.
        /// </param>
        /// <returns>
        ///     The <see cref="(Vector3 v0, Vector3 v1, Vector3 normal)[]" />.
        /// </returns>
        public static (Vector3 v0, Vector3 v1, Vector3 normal)[] MeshPlaneIntersection(
            Vector3[] vertices,
            int[] triangles,
            Plane localPlane)
        {
            var edges = new List<(Vector3 v0, Vector3 v1, Vector3 normal)>();

            for (var i = 0; i < triangles.Length; i += 3)
            {
                var v0 = vertices[triangles[i]];
                var v1 = vertices[triangles[i + 1]];
                var v2 = vertices[triangles[i + 2]];

                if (TrianglePlaneIntersection(v0, v1, v2, localPlane, out var l0, out var l1))
                {
                    var normal = Math3D.TriangleNormal(v0, v1, v2);
                    Debug.Assert(Mathf.Approximately(normal.magnitude, 1f));
                    edges.Add((l0, l1, normal));
                }
            }

            return ConnectIntersectionEdges(edges);
        }

        /// <summary>
        ///     Generates a connected and ordered outline from the edges by connecting neighboring edges.
        /// </summary>
        /// <param name="edges">
        ///     The edges.
        /// </param>
        /// <returns>
        ///     The <see cref="(Vector3 v0, Vector3 v1, Vector3 normal)[]" />.
        /// </returns>
        private static (Vector3 v0, Vector3 v1, Vector3 normal)[] ConnectIntersectionEdges(
            List<(Vector3 v0, Vector3 v1, Vector3 normal)> edges)
        {
            if (edges.Count < 3)
            {
                return Array.Empty<(Vector3 v0, Vector3 v1, Vector3 normal)>();
            }

            var result = new List<(Vector3 v0, Vector3 v1, Vector3 normal)>();

            var (v0, v1, n) = edges[edges.Count - 1];
            edges.RemoveAt(edges.Count - 1);

            var current = v1;
            result.Add((v0, v1, n));

            while (edges.Count > 0)
            {
                var found = false;
                foreach (var edge in edges)
                {
                    var side = -1;

                    if (edge.v0 == current)
                    {
                        side = 0;
                    }
                    else if (edge.v1 == current)
                    {
                        side = 1;
                    }

                    if (side != -1)
                    {
                        edges.Remove(edge);
                        current = side == 0 ? edge.v1 : edge.v0;
                        var prev = side == 0 ? edge.v0 : edge.v1;
                        result.Add((prev, current, edge.normal));
                        found = true;

                        break;
                    }
                }

                if (!found)
                {
                    // No connected edge was found, use the nearest edge instead.
                    (v0, v1, n) = edges[edges.Count - 1];
                    edges.RemoveAt(edges.Count - 1);

                    current = v1;
                    result.Add((v0, v1, n));
                }
            }

            return result.ToArray();
        }

        /// <summary>
        ///     Calculates the intersection between a plane and a triangle and if it results in a line segment returns true.
        /// </summary>
        /// <param name="v0">
        ///     The first point of the triangle.
        /// </param>
        /// <param name="v1">
        ///     The second point of the triangle.
        /// </param>
        /// <param name="v2">
        ///     The third point of the triangle.
        /// </param>
        /// <param name="localPlane">
        ///     The plane.
        /// </param>
        /// <param name="l0">
        ///     The first point of the line segment.
        /// </param>
        /// <param name="l1">
        ///     The second point of the line segment.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private static bool TrianglePlaneIntersection(
            Vector3 v0,
            Vector3 v1,
            Vector3 v2,
            Plane localPlane,
            out Vector3 l0,
            out Vector3 l1)
        {
            l0 = Vector3.zero;
            l1 = Vector3.zero;

            var dir01 = v1 - v0;
            var dir02 = v2 - v0;
            var dir12 = v2 - v1;

            var dist01 = Math3D.LinePlaneIntersection(v0, dir01, localPlane.normal, localPlane.distance);
            var dist02 = Math3D.LinePlaneIntersection(v0, dir02, localPlane.normal, localPlane.distance);
            var dist12 = Math3D.LinePlaneIntersection(v1, dir12, localPlane.normal, localPlane.distance);

            var found = 0;

            if (dist01.HasValue && dist01.Value >= 0 && dist01.Value <= 1)
            {
                l0 = v0 + dir01 * dist01.Value;
                found++;
            }

            if (dist02.HasValue && dist02.Value >= 0 && dist02.Value <= 1)
            {
                var pos = v0 + dir02 * dist02.Value;

                if (found == 0)
                {
                    l0 = pos;
                }
                else
                {
                    l1 = pos;
                }

                found++;
            }

            if (dist12.HasValue && dist12.Value >= 0 && dist12.Value <= 1)
            {
                if (found == 1)
                {
                    l1 = v1 + dir12 * dist12.Value;
                }

                found++;
            }

            if (found == 2)
            {
                return true;
            }

            l0 = Vector3.zero;
            l1 = Vector3.zero;
            return false;
        }
    }
}