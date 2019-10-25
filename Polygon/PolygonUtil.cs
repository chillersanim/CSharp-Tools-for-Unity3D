// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PolygonUtil.cs
// 
// Created:          25.10.2019  10:31
// Last modified:    25.10.2019  11:38
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

namespace Unity_Tools.Polygon
{
    public static class PolygonUtil
    {
        /// <summary>
        /// Generates a mesh out of multiple polygons.
        /// </summary>
        /// <param name="polygons">The polygons.</param>
        /// <returns>Returns a mesh built from the polygons, only vertices and triangles are generated.</returns>
        public static Mesh ToMesh([NotNull]this ICollection<IPolygon<Vector3>> polygons)
        {
            if (polygons.Count == 0)
            {
                throw new ArgumentException("At least one polygon is required to build a mesh.", nameof(polygons));
            }

            var triangleCache = new List<int>();

            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            foreach (var polygon in polygons)
            {
                triangleCache.Clear();
                polygon.Triangulate(triangleCache);

                foreach (var index in triangleCache)
                {
                    vertices.Add(polygon[index]);
                    triangles.Add(vertices.Count - 1);
                }
            }

            var mesh = new Mesh();
            mesh.name = "Generated polygon mesh";
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);

            return mesh;
        }
    }
}
