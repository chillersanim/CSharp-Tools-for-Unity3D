// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         MeshBuilder.cs
// 
// Created:          29.01.2020  17:27
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
using UnityEngine;
using UnityTools.Collections;
using UnityTools.Core;
using UnityTools.Pooling;

namespace UnityTools.Experimental
{
    public class MeshBuilder
    {
        private static readonly ListPool<EdgeEntry> EdgeListPool = new ListPool<EdgeEntry>();

        private static readonly Pool<EdgeEntry> EdgePool = new Pool<EdgeEntry>();

        private static readonly ListPool<FaceEntry> FaceListPool = new ListPool<FaceEntry>();

        private static readonly Pool<FaceEntry> FacePool = new Pool<FaceEntry>();

        private static readonly ListPool<VertexEntry> VertexListPool = new ListPool<VertexEntry>();
        private static readonly Pool<VertexEntry> VertexPool = new Pool<VertexEntry>();

        private readonly List<EdgeEntry> edges;

        private readonly List<FaceEntry> faces;

        private readonly float maxDistForVertEq = 1e-6f;

        private readonly Spatial3DTree<VertexEntry> spatialTree;

        private readonly List<VertexEntry> vertices;

        public MeshBuilder()
        {
            this.vertices = new List<VertexEntry>();
            this.faces = new List<FaceEntry>();
            this.edges = new List<EdgeEntry>();

            this.spatialTree = new Spatial3DTree<VertexEntry>();
        }

        public MeshBuilder(float maxDistanceForVertexEquality) : this()
        {
            this.maxDistForVertEq = Mathf.Abs(maxDistanceForVertexEquality);
        }

        public int AddEdge(int v0, int v1)
        {
            CommonUtil.Sort(ref v0, ref v1);
            var ve0 = this.vertices[v0];
            var ve1 = this.vertices[v1];

            var newEdge = this.AddEdgeInternal(ve0, ve1);
            var lastEdgeIndex = this.edges.Count - 1;

            return this.edges[lastEdgeIndex] == newEdge ? lastEdgeIndex : this.edges.BinarySearch(newEdge);
        }

        public int AddFace(int v0, int v1, int v2)
        {
            CommonUtil.Sort(ref v0, ref v1, ref v2);

            if (v0 == v1 || v1 == v2)
            {
                throw new ArgumentException("A face cannot have duplicate vertices.");
            }

            var ve0 = this.vertices[v0];
            var ve1 = this.vertices[v1];
            var ve2 = this.vertices[v2];

            foreach(var face in ve0.Faces)
            {
                if (face.Vertices.Count == 3 && face.Vertices[1] == ve1 && face.Vertices[2] == ve2)
                {
                    return this.faces.BinarySearch(face);
                }
            }

            var newFace = FacePool.Get();

            newFace.Vertices.Add(ve0);
            newFace.Vertices.Add(ve1);
            newFace.Vertices.Add(ve2);
            ve0.Faces.Add(newFace);
            ve1.Faces.Add(newFace);
            ve2.Faces.Add(newFace);

            var ee0 = this.AddEdgeInternal(ve0, ve1);
            var ee1 = this.AddEdgeInternal(ve1, ve2);
            var ee2 = this.AddEdgeInternal(ve0, ve2);

            newFace.Edges.Add(ee0);
            newFace.Edges.Add(ee1);
            newFace.Edges.Add(ee2);
            ee0.Faces.Add(newFace);
            ee1.Faces.Add(newFace);
            ee2.Faces.Add(newFace);

            this.faces.Add(newFace);
            return this.faces.Count - 1;
        }

        public int AddFace(int v0, int v1, int v2, int v3)
        {
            CommonUtil.Sort(ref v0, ref v1, ref v2);

            if (v0 == v1 || v1 == v2 ||v2 == v3)
            {
                throw new ArgumentException("A face cannot have duplicate vertices.");
            }

            var ve0 = this.vertices[v0];
            var ve1 = this.vertices[v1];
            var ve2 = this.vertices[v2];
            var ve3 = this.vertices[v3];

            foreach (var face in ve0.Faces)
            {
                if (face.Vertices.Count == 4 && face.Vertices[1] == ve1 && face.Vertices[2] == ve2 && face.Vertices[3] == ve3)
                {
                    return this.faces.BinarySearch(face);
                }
            }

            var newFace = FacePool.Get();

            newFace.Vertices.Add(ve0);
            newFace.Vertices.Add(ve1);
            newFace.Vertices.Add(ve2);
            newFace.Vertices.Add(ve3);
            ve0.Faces.Add(newFace);
            ve1.Faces.Add(newFace);
            ve2.Faces.Add(newFace);
            ve3.Faces.Add(newFace);

            var ee0 = this.AddEdgeInternal(ve0, ve1);
            var ee1 = this.AddEdgeInternal(ve1, ve2);
            var ee2 = this.AddEdgeInternal(ve2, ve3);
            var ee3 = this.AddEdgeInternal(ve0, ve3);

            newFace.Edges.Add(ee0);
            newFace.Edges.Add(ee1);
            newFace.Edges.Add(ee2);
            newFace.Edges.Add(ee3);
            ee0.Faces.Add(newFace);
            ee1.Faces.Add(newFace);
            ee2.Faces.Add(newFace);
            ee3.Faces.Add(newFace);

            this.faces.Add(newFace);
            return this.faces.Count - 1;
        }

        public int AddFace(params int[] vertexIndices)
        {
            var existingIndex = this.GetFaceIndex(vertexIndices);
            if (existingIndex >= 0)
            {
                return existingIndex;
            }

            var vertexEntries = VertexListPool.Get(vertexIndices.Length);

            foreach (var index in vertexIndices)
            {
                vertexEntries.Add(this.vertices[index]);
            }

            var newFace = FacePool.Get();

            foreach (var vertexEntry in vertexEntries)
            {
                newFace.Vertices.Add(vertexEntry);
                vertexEntry.Faces.Add(newFace);
            }

            var ve0 = vertexEntries[0];
            var amount = vertexIndices.Length;

            for (var i = 1; i < amount; i++)
            {
                var ee = this.AddEdgeInternal(vertexEntries[i - 1], vertexEntries[i]);
                newFace.Edges.Add(ee);
                ee.Faces.Add(newFace);
            }

            var ee0 = this.AddEdgeInternal(ve0, vertexEntries[amount - 1]);
            newFace.Edges.Add(ee0);
            ee0.Faces.Add(newFace);

            this.faces.Add(newFace);
            var lastIndex = this.faces.Count - 1;

            VertexListPool.Put(vertexEntries);

            return lastIndex;
        }

        public int AddVertex(Vector3 vertex)
        {
            using (var enumerator = this.spatialTree.ShapeCast(new Sphere(vertex, this.maxDistForVertEq)).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return this.vertices.BinarySearch(enumerator.Current);
                }
            }

            var vertexEntry = VertexPool.Get();
            vertexEntry.Id = this.vertices.Count > 0 ? this.vertices[this.vertices.Count - 1].Id + 1u : 1u;
            vertexEntry.Vertex = vertex;

            this.vertices.Add(vertexEntry);
            return this.vertices.Count - 1;
        }

        public (int v0, int v1) GetEdge(int index)
        {
            var ee = this.edges[index];
            return (this.vertices.BinarySearch(ee.V0), this.vertices.BinarySearch(ee.V1));
        }

        public int GetEdgeIndex(int v0, int v1)
        {
            CommonUtil.Sort(ref v0, ref v1);
            var ve0 = this.vertices[v0];
            var ve1 = this.vertices[v1];

            var ee = this.FindEdgeEntry(ve0, ve1);
            return ee != null ? this.edges.BinarySearch(ee) : -1;
        }

        public int[] GetFace(int index)
        {
            var face = this.faces[index];
            var result = new int[face.Vertices.Count];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = this.vertices.BinarySearch(face.Vertices[i]);
            }

            return result;
        }

        public int GetFaceIndex(int v0, int v1, int v2)
        {
            CommonUtil.Sort(ref v0, ref v1, ref v2);

            if (v0 == v1 || v1 == v2)
            {
                throw new ArgumentException("A face cannot have duplicate vertices.");
            }

            var ve0 = this.vertices[v0];
            var ve1 = this.vertices[v1];
            var ve2 = this.vertices[v2];

            foreach (var face in ve0.Faces)
            {
                if (face.Vertices.Count == 3 && face.Vertices[1] == ve1 && face.Vertices[2] == ve2)
                {
                    return this.faces.BinarySearch(face);
                }
            }

            return -1;
        }

        public int GetFaceIndex(int v0, int v1, int v2, int v3)
        {
            CommonUtil.Sort(ref v0, ref v1, ref v2);

            if (v0 == v1 || v1 == v2 || v2 == v3)
            {
                throw new ArgumentException("A face cannot have duplicate vertices.");
            }

            var ve0 = this.vertices[v0];
            var ve1 = this.vertices[v1];
            var ve2 = this.vertices[v2];
            var ve3 = this.vertices[v3];

            foreach (var face in ve0.Faces)
            {
                if (face.Vertices.Count == 4 && face.Vertices[1] == ve1 && face.Vertices[2] == ve2 && face.Vertices[3] == ve3)
                {
                    return this.faces.BinarySearch(face);
                }
            }

            return -1;
        }

        public int GetFaceIndex(params int[] vertexIndices)
        {
            if (vertexIndices.Length < 3)
            {
                throw new ArgumentException("A face needs at least 3 vertices.");
            }

            Array.Sort(vertexIndices);

            if (this.HasDuplicates(vertexIndices))
            {
                throw new ArgumentException("A face cannot have duplicate vertices.");
            }

            var vertexEntries = VertexListPool.Get(vertexIndices.Length);

            foreach (var index in vertexIndices)
            {
                vertexEntries.Add(this.vertices[index]);
            }

            var ve0 = vertexEntries[0];
            var amount = vertexIndices.Length;

            foreach (var face in ve0.Faces)
            {
                if (face.Vertices.Count == amount)
                {
                    var match = true;
                    for (var i = 1; i < face.Vertices.Count; i++)
                    {
                        if (face.Vertices[i] != vertexEntries[i])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        VertexListPool.Put(vertexEntries);
                        return this.faces.BinarySearch(face);
                    }
                }
            }

            VertexListPool.Put(vertexEntries);

            return -1;
        }

        public Vector3 GetVertex(int index)
        {
            return this.vertices[index].Vertex;
        }

        public int GetVertexIndex(Vector3 vertex)
        {
            var ve = this.FindVertexEntry(vertex);
            return ve != null ? this.vertices.BinarySearch(ve) : -1;
        }

        public bool IsTwoManifold()
        {
            foreach (var edgeEntry in this.edges)
            {
                if (edgeEntry.Faces.Count != 2)
                {
                    return false;
                }
            }

            return true;
        }

        public void RemoveEdge(int index, bool cascade)
        {
            var edgeEntry = this.edges[index];
            this.RemoveEdgeInternal(edgeEntry, cascade);
            this.edges.RemoveAt(index);
            EdgePool.Put(edgeEntry);
        }

        public void RemoveEdge(int v0, int v1, bool cascade)
        {
            var index = this.GetEdgeIndex(v0, v1);
            if (index < 0)
            {
                throw new ArgumentException("No edge between vertices v0 and v1 exists.");
            }

            this.RemoveEdge(index, cascade);
        }

        public void RemoveFace(int index)
        {
            var faceEntry = this.faces[index];
            this.RemoveFaceInternal(faceEntry);
            this.faces.Remove(faceEntry);
            FacePool.Put(faceEntry);
        }

        public void RemoveFace(int v0, int v1, int v2)
        {
            var index = this.GetFaceIndex(v0, v1, v2);
            if (index < 0)
            {
                throw new ArgumentException("No faces between vertices v0, v1 and v2 exists.");
            }

            this.RemoveFace(index);
        }

        public void RemoveFace(int v0, int v1, int v2, int v3)
        {
            var index = this.GetFaceIndex(v0, v1, v2, v3);
            if (index < 0)
            {
                throw new ArgumentException("No faces between vertices v0, v1, v2 and v3 exists.");
            }

            this.RemoveFace(index);
        }

        public void RemoveFace(params int[] vertexIndices)
        {
            var index = this.GetFaceIndex(vertexIndices);
            if (index < 0)
            {
                throw new ArgumentException("No faces between the given vertices exists.");
            }

            this.RemoveFace(index);
        }

        public void RemoveVertex(int index, bool cascade = false)
        {
            var vertexEntry = this.vertices[index];
            this.RemoveVertexInternal(vertexEntry, cascade);
            this.vertices.RemoveAt(index);
            this.spatialTree.Remove(vertexEntry, vertexEntry.Vertex);
            VertexPool.Put(vertexEntry);
        }

        public void RemoveVertex(Vector3 vertex, bool cascade = false)
        {
            var vertexEntry = this.FindVertexEntry(vertex);

            if (vertexEntry == null)
            {
                throw new ArgumentException("The mesh doesn't have a vertex that is equal to the provided vertex.",
                    nameof(vertex));
            }

            this.RemoveVertexInternal(vertexEntry, cascade);
            this.vertices.Remove(vertexEntry);
            this.spatialTree.Remove(vertexEntry, vertexEntry.Vertex);
            VertexPool.Put(vertexEntry);
        }

        public Mesh ToMesh()
        {
            var vertexCache = GlobalListPool<Vector3>.Get(this.vertices.Count);
            var indexCache = GlobalListPool<int>.Get(this.vertices.Count);

            var meshVertices = GlobalListPool<Vector3>.Get(this.vertices.Count * 3);

            foreach (var face in this.faces)
            {
                vertexCache.Clear();
                indexCache.Clear();

                foreach (var vertex in face.Vertices)
                {
                    vertexCache.Add(vertex.Vertex);
                }

                vertexCache.TriangulateWithCuttingEars(indexCache);

                foreach(var index in indexCache)
                {
                    meshVertices.Add(vertexCache[index]);
                }
            }

            var mesh = new Mesh();
            mesh.vertices = meshVertices.ToArray();
            mesh.triangles = CollectionUtil.CreateArray(meshVertices.Count, 0, i => i + 1);

            GlobalListPool<Vector3>.Put(vertexCache);
            GlobalListPool<int>.Put(indexCache);
            GlobalListPool<Vector3>.Put(meshVertices);

            return mesh;
        }

        private EdgeEntry AddEdgeInternal(VertexEntry ve0, VertexEntry ve1)
        {
            var existing = this.FindEdgeEntry(ve0, ve1);
            if (existing != null)
            {
                return existing;
            }

            var newEdge = EdgePool.Get();
            newEdge.Id = this.edges.Count > 0 ? this.edges[this.edges.Count - 1].Id + 1u : 1u;
            newEdge.V0 = ve0;
            newEdge.V1 = ve1;

            this.edges.Add(newEdge);

            return newEdge;
        }

        private EdgeEntry FindEdgeEntry(VertexEntry ve0, VertexEntry ve1)
        {
            foreach (var existing in ve0.Edges)
            {
                if (existing.V1 == ve1)
                {
                    return existing;
                }
            }

            return null;
        }

        private VertexEntry FindVertexEntry(Vector3 vertex)
        {
            using (var enumerator = this.spatialTree.ShapeCast(new Sphere(vertex, this.maxDistForVertEq)).GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }

            return null;
        }

        private bool HasDuplicates<T>(IList<T> sortedItems) where T : IComparable<T>
        {
            for(var i = 1; i < sortedItems.Count; i++)
            {
                if (sortedItems[i].CompareTo(sortedItems[i - 1]) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void RemoveEdgeInternal(EdgeEntry edgeEntry, bool cascade)
        {
            if (!cascade)
            {
                if (edgeEntry.Faces.Count > 0)
                {
                    throw new InvalidOperationException("Some faces still depend on this edge.");
                }
            }
            else
            {
                foreach (var faceEntry in edgeEntry.Faces)
                {
                    this.faces.Remove(faceEntry);

                    foreach (var fve in faceEntry.Vertices)
                    {
                        fve.Faces.Remove(faceEntry);
                    }

                    foreach (var fee in faceEntry.Edges)
                    {
                        fee.Faces.Remove(faceEntry);
                    }

                    FacePool.Put(faceEntry);
                }
            }

            edgeEntry.V0.Edges.Remove(edgeEntry);
            edgeEntry.V1.Edges.Remove(edgeEntry);
        }

        private void RemoveFaceInternal(FaceEntry faceEntry)
        {
            foreach (var v in faceEntry.Vertices)
            {
                v.Faces.Remove(faceEntry);
            }

            foreach (var e in faceEntry.Edges)
            {
                e.Faces.Remove(faceEntry);
            }
        }

        private void RemoveVertexInternal(VertexEntry vertexEntry, bool cascade)
        {
            if (!cascade)
            {
                if (vertexEntry.Edges.Count > 0)
                {
                    throw new InvalidOperationException("The vertex is still part of some edges.");
                }

                if (vertexEntry.Faces.Count > 0)
                {
                    throw new InvalidOperationException("The vertex is still part of some faces.");
                }
            }
            else
            {
                foreach (var edgeEntry in vertexEntry.Edges)
                {
                    this.edges.Remove(edgeEntry);
                    if (edgeEntry.V0 == vertexEntry)
                    {
                        edgeEntry.V1.Edges.Remove(edgeEntry);
                    }
                    else
                    {
                        edgeEntry.V0.Edges.Remove(edgeEntry);
                    }

                    EdgePool.Put(edgeEntry);
                }

                foreach (var faceEntry in vertexEntry.Faces)
                {
                    this.faces.Remove(faceEntry);

                    foreach (var fve in faceEntry.Vertices)
                    {
                        fve.Faces.Remove(faceEntry);
                    }

                    foreach (var fee in faceEntry.Edges)
                    {
                        fee.Faces.Remove(faceEntry);
                    }

                    FacePool.Put(faceEntry);
                }
            }
        }

        private class EdgeEntry : IReusable, IComparable<EdgeEntry>
        {
            public readonly List<FaceEntry> Faces;
            public uint Id;

            public VertexEntry V0;

            public VertexEntry V1;

            public EdgeEntry()
            {
                this.Faces = FaceListPool.Get();
            }

            public EdgeEntry(uint id, VertexEntry v0, VertexEntry v1) : this()
            {
                this.Id = id;
                this.V0 = v0;
                this.V1 = v1;
            }

            public int CompareTo(EdgeEntry other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                return this.Id.CompareTo(other.Id);
            }

            public void Reuse()
            {
                this.Id = 0u;
                this.V0 = null;
                this.V1 = null;
                this.Faces.Clear();
            }
        }

        private class FaceEntry : IReusable, IComparable<FaceEntry>
        {
            public readonly List<EdgeEntry> Edges;

            public readonly List<VertexEntry> Vertices;
            public uint Id;

            public FaceEntry()
            {
                this.Vertices = VertexListPool.Get();
                this.Edges = EdgeListPool.Get();
            }

            public FaceEntry(uint id) : this()
            {
                this.Id = id;
            }

            public int CompareTo(FaceEntry other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                return this.Id.CompareTo(other.Id);
            }

            public void Reuse()
            {
                this.Id = 0u;
                this.Vertices.Clear();
                this.Edges.Clear();
            }
        }

        private class VertexEntry : IReusable, IComparable<VertexEntry>
        {
            public readonly List<EdgeEntry> Edges;

            public readonly List<FaceEntry> Faces;
            public uint Id;

            public Vector3 Vertex;

            public VertexEntry()
            {
                this.Edges = EdgeListPool.Get();
                this.Faces = FaceListPool.Get();
            }

            public VertexEntry(uint id, Vector3 vertex) : this()
            {
                this.Id = id;
                this.Vertex = vertex;
            }

            public int CompareTo(VertexEntry other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 1;
                return this.Id.CompareTo(other.Id);
            }

            public void Reuse()
            {
                this.Id = 0u;
                this.Vertex = Vector3.zero;
                this.Edges.Clear();
                this.Faces.Clear();
            }
        }
    }
}