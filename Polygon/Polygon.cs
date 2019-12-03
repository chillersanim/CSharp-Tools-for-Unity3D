// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Polygon.cs
// 
// Created:          24.10.2019  13:15
// Last modified:    03.12.2019  08:37
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
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Unity_Tools.Core;
using Unity_Tools.Polygon;

namespace Common
{
    [Serializable]
    public class Polygon : IPolygon<Vector3>, IEnumerable<Vector3>
    {
        private readonly List<Vector3> vertices;
        private float? area;
        private Vector3? center;
        private Vector3? normal;

        public Polygon(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            vertices = new List<Vector3>();

            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
        }

        public Polygon(params Vector3[] vertices)
        {
            this.vertices = new List<Vector3>(vertices);
        }

        public Polygon([NotNull]Polygon other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            this.vertices = new List<Vector3>(other.vertices);
        }

        public float Area
        {
            get
            {
                if (!area.HasValue)
                {
                    area = CalculateArea();
                }

                return area.Value;
            }
        }

        public Vector3 Center
        {
            get
            {
                if (!center.HasValue)
                {
                    center = CalculateCenter();
                }

                return center.Value;
            }
        }

        public Vector3 Normal
        {
            get
            {
                if (!normal.HasValue)
                {
                    normal = CalculateNormal();
                }

                return normal.Value;
            }
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return this.vertices.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int VertexCount => vertices.Count;

        public Vector3 this[int index]
        {
            get => vertices[index];
            set => vertices[index] = value;
        }

        public void Triangulate(List<int> triangles)
        {
            this.vertices.Triangulate(this.Normal, triangles);
        }

        public void Flip()
        {
            for (var i = 0; i < vertices.Count / 2; i++)
            {
                var endIndex = vertices.Count - i - 1;
                var tmp = vertices[i];
                vertices[i] = vertices[endIndex];
                vertices[endIndex] = tmp;
            }

            if (normal.HasValue)
            {
                normal = -normal.Value;
            }
        }

        public void InsertVertex(int index, Vector3 vertex)
        {
            vertices.Insert(index, vertex);

            area = null;
            center = null;
            normal = null;
        }

        public static Polygon[] PolygonizeTriangles([NotNull] IList<Vector3> vertices, IList<int> triangles)
        {
            var polygons = new List<Polygon>();

            for (var i = 0; i < triangles.Count; i += 3)
            {
                var i0 = triangles[i];
                var i1 = triangles[i + 1];
                var i2 = triangles[i + 2];

                var polygon = new Polygon(vertices[i0], vertices[i1], vertices[i2]);

                for(var j = polygons.Count - 1; j >= 0; j--)
                {
                    var existingPolygon = polygons[j];

                    // Test implementation, remove after fixing
                    var copy = new Polygon(existingPolygon);
                    // End test implementation

                    if (polygon.TryMerge(existingPolygon))
                    {
                        polygons.RemoveAt(j);
                    }
                    // Test implementation, remove after fixing
                    else if (existingPolygon.TryMerge(polygon))
                    {
                        polygon.TryMerge(copy);
                        throw new Exception("This shouldn't happen... :(");
                    }
                    // End test implementation
                }

                polygons.Add(polygon);
            }

            return polygons.ToArray();
        }

        public void RemoveVertexAt(int index)
        {
            if (vertices.Count <= 3)
            {
                throw new InvalidOperationException(
                    "Can't remove vertex from polygon. A polygon needs at least 3 vertices.");
            }

            vertices.RemoveAt(index);

            area = null;
            center = null;
            normal = null;
        }

        public void SimplifyShape()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices.Count <= 3)
                {
                    return;
                }

                var prevIndex = Nums.Mod(i - 1, vertices.Count);
                var nextIndex = Nums.Mod(i + 1, vertices.Count);

                var prev = vertices[prevIndex];
                var current = vertices[i];
                var next = vertices[nextIndex];

                if (Math3D.IsLinearExtension(prev, current, next))
                {
                    vertices.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Translate(Vector3 offset)
        {
            this.vertices.ForAll(v => v + offset);

            this.center = null;
        }

        public bool TryMerge([NotNull]Polygon other, float maxError = 1e-4f)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (other == this)
            {
                return false;
            }

            var sqError = maxError * maxError;
            var (firstMatchThis, firstMatchOther) = FindFirstMatch(other, sqError);
            if (firstMatchThis < 0)
            {
                return false;
            }

            var otherDirection = FindOtherConnectionDirection(other, firstMatchThis, firstMatchOther, sqError);
            if (otherDirection == 0)
            {
                return false;
            }

            var end = FindLastMatch(other, firstMatchThis, firstMatchOther, 1, otherDirection, sqError);

            if (end == this.vertices.Count - 1 || end == other.vertices.Count - 1)
            {
                // Both polygons seem to fully match, don't change anything.
                return true;
            }

            var begin = FindLastMatch(other, firstMatchThis, firstMatchOther, -1, -otherDirection, sqError);

            var thisFirst = Nums.Mod((firstMatchThis + begin), this.vertices.Count);
            var thisLast = Nums.Mod((firstMatchThis + end), this.vertices.Count);
            var otherFirst = Nums.Mod((firstMatchOther + (begin * otherDirection)), other.vertices.Count);
            var otherLast = Nums.Mod((firstMatchOther + (end * otherDirection)), other.vertices.Count);

            var toRemove = Nums.Mod((thisLast - thisFirst), this.vertices.Count) - 1;
            for (var i = 0; i < toRemove; i++)
            {
                this.vertices.RemoveAt(Nums.Mod((thisFirst + 1), this.vertices.Count));
            }

            var insertIndex = thisFirst + 1 - toRemove;
            for (var i = Nums.Mod((otherFirst - otherDirection), other.vertices.Count);
                i != otherLast;
                i = Nums.Mod((i - otherDirection), other.vertices.Count))
            {
                this.vertices.Insert(insertIndex, other.vertices[i]);
                insertIndex++;
            }

            area = null;
            center = null;
            normal = null;

            return true;
        }

        private float CalculateArea()
        {
            return Math3D.PlanarPolygonArea(this.vertices);
        }

        private Vector3 CalculateCenter()
        {
            var c = Vector3.zero;

            foreach (var vertex in vertices)
            {
                c += vertex;
            }

            return c / vertices.Count;
        }

        private Vector3 CalculateNormal()
        {
            return Math3D.PlanarPolygonNormal(this.vertices);
        }

        private (int thisMatch, int otherMatch) FindFirstMatch([NotNull]Polygon other, float maxSqError)
        {
            for (var i = 0; i < this.vertices.Count; i++)
            {
                var vertex = this.vertices[i];
                for (var j = 0; j < other.vertices.Count; j++)
                {
                    var otherVertex = other.vertices[j];

                    if ((vertex - otherVertex).sqrMagnitude <= maxSqError)
                    {
                        return (i, j);
                    }
                }
            }

            return (-1, -1);
        }

        private int FindLastMatch([NotNull] Polygon other, int thisIndex, int otherIndex, int thisDirection, int otherDirection, float maxSqError)
        {
            if (thisDirection != -1 && thisDirection != 1)
            {
                throw new ArgumentException("The direction for this polygon needs to be 1 or -1.");
            }

            if (otherDirection != -1 && otherDirection != 1)
            {
                throw new ArgumentException("The direction for the other polygon needs to be 1 or -1.");
            }

            var lastIndex = -1;

            for (var i = 0; i < this.vertices.Count && i < other.vertices.Count; i++)
            {
                var thisVertex = this.vertices[Nums.Mod((i * thisDirection + thisIndex), this.vertices.Count)];
                var otherVertex = other.vertices[Nums.Mod((i * otherDirection + otherIndex), other.vertices.Count)];

                if ((thisVertex - otherVertex).sqrMagnitude <= maxSqError)
                {
                    lastIndex = i;
                }
                else
                {
                    return lastIndex * thisDirection;
                }
            }

            return lastIndex * thisDirection;
        }

        private int FindOtherConnectionDirection([NotNull] Polygon other, int thisIndex, int otherIndex,
            float maxSqError)
        {
            var nextThisVertex = this.vertices[Nums.Mod((thisIndex + 1), this.vertices.Count)];
            var prevThisVertex = this.vertices[Nums.Mod((thisIndex - 1), this.vertices.Count)];
            var nextOtherVertex = other.vertices[Nums.Mod((otherIndex + 1), other.vertices.Count)];
            var prevOtherVertex = other.vertices[Nums.Mod((otherIndex - 1), other.vertices.Count)];

            if ((nextThisVertex - nextOtherVertex).sqrMagnitude <= maxSqError)
            {
                return 1;
            }

            if ((nextThisVertex - prevOtherVertex).sqrMagnitude <= maxSqError)
            {
                return -1;
            }

            if ((prevThisVertex - nextOtherVertex).sqrMagnitude <= maxSqError)
            {
                return -1;
            }

            if ((prevThisVertex - prevOtherVertex).sqrMagnitude <= maxSqError)
            {
                return 1;
            }

            return 0;
        }
    }
}
