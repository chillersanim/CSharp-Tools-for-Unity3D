// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CuttingEars.cs
// 
// Created:          25.10.2019  11:29
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
using UnityEngine;
using Unity_Tools.Core.Pooling;

namespace Unity_Tools.Core
{
    public static class CuttingEars
    {
        /// <summary>
        /// Triangulates a list of vertices, interpreted as the vertices of a planar polygon in clockwise order.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        public static int[] Triangulate(this IList<Vector3> vertices)
        {
            var output = GlobalListPool<int>.Get(vertices.Count * 3 - 2);
            vertices.Triangulate(output);
            var result = output.ToArray();
            GlobalListPool<int>.Put(output);
            return result;
        }

        /// <summary>
        /// Triangulates a list of vertices, interpreted as the vertices of a planar polygon in clockwise order.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="normal">The normal of the polygon, if not know, use the overload without normal.</param>
        public static int[] Triangulate(this IList<Vector3> vertices, Vector3 normal)
        {
            var output = GlobalListPool<int>.Get(vertices.Count * 3 - 2);
            vertices.Triangulate(normal, output);
            var result = output.ToArray();
            GlobalListPool<int>.Put(output);
            return result;
        }

        /// <summary>
        /// Triangulates a list of vertices, interpreted as the vertices of a planar polygon in clockwise order.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="triangles">The output in which the resulting triangles will be written.</param>
        public static void Triangulate(this IList<Vector3> vertices, List<int> triangles)
        {
            Triangulate(vertices, Math3D.PlanarPolygonNormal(vertices), triangles);
        }

        /// <summary>
        /// Triangulates a list of vertices, interpreted as the vertices of a planar polygon in clockwise order.
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        /// <param name="normal">The normal of the polygon, if not know, use the overload without normal.</param>
        /// <param name="triangles">The output in which the resulting triangles will be written.</param>
        public static void Triangulate(this IList<Vector3> vertices, Vector3 normal, List<int> triangles)
        {
            if (vertices.Count == 3)
            {
                triangles.AddRange(new[] {0, 1, 2});
                return;
            }

            if (vertices.Count == 4)
            {
                var d1 = Vector3.SqrMagnitude(vertices[0] - vertices[2]);
                var d2 = Vector3.SqrMagnitude(vertices[1] - vertices[3]);
                
                if (d1 > d2)
                {
                    triangles.AddRange(new[] {0, 1, 3, 1, 2, 3});
                    return;
                }

                triangles.AddRange(new[] {0, 1, 2, 0, 2, 3});
                return;
            }
            
            var vertexReferences = new LinkedList<(Vector3 v, float a, int i)>();
            for (var i = 0; i < vertices.Count; i++)
            {
                var prev = vertices[Nums.Mod(i - 1, vertices.Count)];
                var current = vertices[i];
                var next = vertices[(i + 1) % vertices.Count];

                vertexReferences.AddLast((current, Math3D.InnerAngle(prev, current, next, normal), i));
            }
            
            while (vertexReferences.Count > 3)
            {
                var prevCount = vertexReferences.Count;
                var currentNode = vertexReferences.First;

                // The inner angle when all inner angles are equal, used to exclude inner angles larger than this, as a smaller angle must exist then. (incl. epsilon)
                var minAngle = 180f - (360f / vertexReferences.Count) + 1e-5f;

                while (currentNode != null)
                {
                    var prevNode = currentNode.Previous ?? vertexReferences.Last;
                    var nextNode = currentNode.Next ?? vertexReferences.First;
                    var current = currentNode.Value;

                    var angle = current.a;

                    // Test whether the three points are nearly collinear.
                    if (Mathf.Abs((angle - 0.001f) % 180) <= 0.001f)
                    {
                        // Don't create invalid triangles.
                        continue;
                    }

                    // If the inner angle of this vertex is pointy enough
                    if (angle <= minAngle)
                    {
                        var v0 = prevNode.Value.v;
                        var v1 = current.v;
                        var v2 = nextNode.Value.v;
                        var collision = false;

                        // Test whether any other vertex intersects with the triangle that we want as ear
                        foreach (var vr in vertexReferences)
                        {
                            if (vr.i == prevNode.Value.i || vr.i == current.i || vr.i == nextNode.Value.i)
                            {
                                continue;
                            }

                            if (Math3D.IsPointInTriangle(v0, v1, v2, vr.v))
                            {
                                collision = true;
                                break;
                            }
                        }

                        // Extract the triangle, update the data structure.
                        if (!collision)
                        {
                            vertexReferences.Remove(currentNode);
                            triangles.Add(prevNode.Value.i);
                            triangles.Add(current.i);
                            triangles.Add(nextNode.Value.i);

                            // Update angles for prev and next
                            var pPrevVertex = (prevNode.Previous ?? vertexReferences.Last).Value.v;
                            var nNextVertex = (nextNode.Next ?? vertexReferences.First).Value.v;
                            
                            var prevValue = prevNode.Value;
                            var nextValue = nextNode.Value;

                            prevNode.Value = (prevValue.v, Math3D.InnerAngle(pPrevVertex, prevValue.v, nextValue.v, normal), prevValue.i);
                            nextNode.Value = (nextValue.v, Math3D.InnerAngle(prevValue.v, nextValue.v, nNextVertex, normal), nextValue.i);

                            break;
                        }
                    }

                    currentNode = currentNode.Next;
                }

                // Prevents endless loops in case the provided polygon somehow breaks the algorithm.
                if (vertexReferences.Count >= prevCount)
                {
                    throw new ArgumentException("This polygon cannot be triangulated.");
                }
            }

            if (vertexReferences.Count == 3)
            {
                foreach (var value in vertexReferences)
                {
                    triangles.Add(value.i);
                }
            }
        }
    }
}
