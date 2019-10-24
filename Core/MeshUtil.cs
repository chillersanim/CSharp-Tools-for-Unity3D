// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         MeshUtil.cs
// 
// Created:          12.08.2019  19:04
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
        ///     The error percentage that is allowed for a mesh to be considered cuboid.
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
    }
}