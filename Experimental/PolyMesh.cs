using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityTools.Experimental
{
    public class PolyMesh
    {
        private const int InitialVertexCapacity = 64;
        private const int InitialEdgeCapacity = 64;
        private const int InitialPolyCapacity = 64;

        private readonly HashSet<int> intHashSet = new HashSet<int>();

        private Vector3[] vertices;
        private VertexEdgeRef[] vertexEdgeRefs;
        private int vertexCount;

        private Edge[] edges;
        private EdgePolyRef[] edgePolyRefs;
        private int edgeCount;

        private Polygon[] polygons;
        private int polyCount;

        public PolyMesh()
        {
            this.vertices = new Vector3[InitialVertexCapacity];
            this.vertexEdgeRefs = new VertexEdgeRef[InitialVertexCapacity];
            this.vertexCount = 0;

            this.edges = new Edge[InitialEdgeCapacity];
            this.edgePolyRefs = new EdgePolyRef[InitialEdgeCapacity];
            this.edgeCount = 0;

            this.polygons = new Polygon[InitialPolyCapacity];
            this.polyCount = 0;
        }

        public int AddVertex(Vector3 vertex)
        {
            return this.AddVertexInternal(vertex);
        }

        public bool RemoveVertex(Vector3 vertex, bool destructive = false)
        {
            for (var i = 0; i < vertexCount; i++)
            {
                if (vertices[i] == vertex)
                {
                    RemoveVertexAt(i, destructive);
                    return true;
                }
            }

            return false;
        }

        public void RemoveVertexAt(int index, bool destructive = false)
        {
            if (index < 0 || index >= vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (vertexEdgeRefs[index].Count > 0)
            {
                if (!destructive)
                {
                    // Removing the vertex would be destructive
                    throw new InvalidOperationException(
                        "Removing the vertex would be destructive, some edges are still using it.");
                }

                for (var i = 0; i < vertexEdgeRefs[index].Count; i++)
                {
                    RemoveEdgeAt(vertexEdgeRefs[index].Edges[i], true);
                }
            }

            RemoveVertexInternal(index);
        }

        public int MakeEdge(int v0, int v1)
        {
            if (v0 == v1)
            {
                throw new ArgumentException("The v0 and v1 must point to two different vertices.");
            }

            if (v0 < 0 || v0 >= vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(v0));
            }

            if (v1 < 0 || v1 >= vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(v1));
            }

            // Ensure v0 < v1
            if (v1 < v0)
            {
                var tmp = v0;
                v0 = v1;
                v1 = tmp;
            }

            // Test whether the edge already exists
            for (var i = 0; i < vertexEdgeRefs[v0].Count; i++)
            {
                var edgeIndex = vertexEdgeRefs[v0].Edges[i];
                if (edges[edgeIndex].V1 == v1)
                {
                    // Edge already exists, don't add another instance
                    return edgeIndex;
                }
            }

            return AddEdgeInternal(v0, v1);
        }

        public bool RemoveEdge(int v0, int v1, bool destructive = false)
        {
            if (v0 == v1)
            {
                throw new ArgumentException("The v0 and v1 must point to two different vertices.");
            }

            if (v0 < 0 || v0 >= vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(v0));
            }

            if (v1 < 0 || v1 >= vertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(v1));
            }

            // Ensure v0 < v1
            if (v1 < v0)
            {
                var tmp = v0;
                v0 = v1;
                v1 = tmp;
            }

            // Find matching edge
            for (var i = 0; i < vertexEdgeRefs[v0].Count; i++)
            {
                var edgeIndex = vertexEdgeRefs[v0].Edges[i];
                if (edges[edgeIndex].V1 == v1)
                {
                    RemoveEdgeAt(edgeIndex, destructive);
                    return true;
                }
            }

            return false;
        }

        public void RemoveEdgeAt(int index, bool destructive = false)
        {
            if (index < 0 || index >= edgeCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (edgePolyRefs[index].Count > 0)
            {
                if (!destructive)
                {
                    // Removing the edge would be destructive
                    throw new InvalidOperationException(
                        "Removing the edge would be destructive, some surfaces are still using it.");
                }

                for (var i = 0; i < edgePolyRefs[index].Count; i++)
                {
                    RemoveSurfaceAt(edgePolyRefs[index].Polygons[i]);
                }
            }

            RemoveEdgeInternal(index);
        }

        public void MakeSurface(IList<int> edgeIndices)
        {
            if(edgeIndices == null)
            {
                throw new ArgumentNullException();
            }

            if (edgeIndices.Count < 3)
            {
                throw new ArgumentException("At least 3 edges are required to form a surface.");
            }

            // Ensure that the edges exist

            // Verify that every vertex in the edges is used by exactly two edges

            // Ensure that the edges are unique (Remove duplicates)

            // Ensure that the edges form a loop (Reorder)

            // Ensure the surface doesn't yet exist

            // Add surface

            throw new NotImplementedException();
        }

        public void RemoveSurface(IList<int> vertexIndices)
        {
            throw new NotImplementedException();
        }

        public void RemoveSurfaceAt(int index)
        {
            throw new NotImplementedException();
        }

        private int AddVertexInternal(Vector3 vertex)
        {
            if (vertexCount >= vertices.Length)
            {
                var newLength = vertexCount * 2;
                var oldVertices = vertices;
                var oldVertexEdgeRefs = vertexEdgeRefs;
                vertices = new Vector3[newLength];
                vertexEdgeRefs = new VertexEdgeRef[newLength];

                for (var i = 0; i < vertexCount; i++)
                {
                    vertices[i] = oldVertices[i];
                    vertexEdgeRefs[i] = oldVertexEdgeRefs[i];
                }
            }

            vertices[vertexCount] = vertex;
            vertexEdgeRefs[vertexCount] = new VertexEdgeRef();
            return vertexCount++;
        }

        private void RemoveVertexInternal(int index)
        {
            Debug.Assert(index > 0);
            Debug.Assert(index < vertexCount);
            Debug.Assert(vertexEdgeRefs[index].Count == 0);
            Debug.Assert(vertexEdgeRefs[index].Edges == null);

            vertexCount--;

            if (index < vertexCount)
            {
                // Swap last vertex with removed vertex to prevent holes
                vertices[index] = vertices[vertexCount];
                vertexEdgeRefs[index] = vertexEdgeRefs[vertexCount];

                // Update edges pointing to swapped vertex
                foreach (var edgeRef in vertexEdgeRefs[vertexCount].Edges)
                {
                    var edge = edges[edgeRef];

                    if (edge.V0 == vertexCount)
                    {
                        edges[edgeRef] = new Edge(index, edge.V1);
                    }
                    else
                    {
                        Debug.Assert(edge.V1 == index);
                        edges[edgeRef] = new Edge(edge.V0, index);
                    }
                }
            }

            vertexEdgeRefs[vertexCount] = default;
        }

        private void RemoveVertexEdgeRef(int vertex, int edge)
        {
            Debug.Assert(vertex >= 0);
            Debug.Assert(vertex < vertexCount);
            Debug.Assert(edge >= 0);
            Debug.Assert(edge <= edgeCount);    // Allows equals as this method can be called inside removeEdgeInternal where the edge count is already updated.
            Debug.Assert(vertexEdgeRefs[vertex].Count > 0);
            Debug.Assert(vertexEdgeRefs[vertex].Edges != null);
            Debug.Assert(vertexEdgeRefs[vertex].Contains(edge));

            var edgeRef = vertexEdgeRefs[vertex];
            if (edgeRef.Count == 1)
            {
                vertexEdgeRefs[vertex] = new VertexEdgeRef(null, 0);
            }
            else
            {
                vertexEdgeRefs[vertex] = edgeRef.Remove(edge);
            }
        }

        private int AddEdgeInternal(int v0, int v1)
        {
            Debug.Assert(v0 >= 0);
            Debug.Assert(v1 >= 0);
            Debug.Assert(v0 < vertexCount);
            Debug.Assert(v1 < vertexCount);

            if (edgeCount >= edges.Length)
            {
                var newLength = edgeCount * 2;
                var oldEdges = edges;
                var oldEdgePolyRefs = edgePolyRefs;
                edges = new Edge[newLength];
                edgePolyRefs = new EdgePolyRef[newLength];

                for (var i = 0; i < edgeCount; i++)
                {
                    edges[i] = oldEdges[i];
                    edgePolyRefs[i] = oldEdgePolyRefs[i];
                }
            }

            edges[edgeCount] = new Edge(v0, v1);
            edgePolyRefs[edgeCount] = new EdgePolyRef();
            return edgeCount++;
        }

        private void RemoveEdgeInternal(int index)
        {
            Debug.Assert(index > 0);
            Debug.Assert(index < edgeCount);
            Debug.Assert(edgePolyRefs[index].Polygons == null);

            edgeCount--;

            // Remove vertexEdgeRef entries for this edge
            var edge = edges[index];
            RemoveVertexEdgeRef(edge.V0, index);
            RemoveVertexEdgeRef(edge.V1, index);

            if (index < edgeCount)
            {
                // Swap last edge with removed edge to prevent holes
                edges[index] = edges[edgeCount];
                edgePolyRefs[index] = edgePolyRefs[edgeCount];

                // Update vertices pointing to swapped edge
                var v0 = vertexEdgeRefs[edges[edgeCount].V0];

                for (var i = 0; i < 2; i++)
                {
                    for (var j = 0; j < v0.Count; j++)
                    {
                        if (v0.Edges[j] == edgeCount)
                        {
                            v0.Edges[j] = index;
                            break;
                        }
                    }

                    // Do the same for the other vertex.
                    v0 = vertexEdgeRefs[edges[edgeCount].V1];
                }

                // Update polygons pointing to swapped edge
                foreach (var polyRef in edgePolyRefs[edgeCount].Polygons)
                {
                    var poly = polygons[polyRef];
                    for (var i = 0; i < poly.Count; i++)
                    {
                        if (poly.Edges[i] == edgeCount)
                        {
                            poly.Edges[i] = index;
                        }
                    }
                }
            }

            edgePolyRefs[edgeCount] = default;
        }

        private void RemoveEdgePolyRef(int edge, int polygon)
        {
            
            Debug.Assert(edge >= 0);
            Debug.Assert(edge < edgeCount);
            Debug.Assert(polygon >= 0);
            Debug.Assert(polygon <= polyCount);         // Allows equals as this method can be called inside removeEdgeInternal where the edge count is already updated.
            Debug.Assert(edgePolyRefs[edge].Count > 0);
            Debug.Assert(edgePolyRefs[edge].Polygons != null);
            Debug.Assert(edgePolyRefs[edge].Contains(polygon));

            var polyRef = edgePolyRefs[edge];
            if (polyRef.Count == 1)
            {
                edgePolyRefs[edge] = new EdgePolyRef(null, 0);
            }
            else
            {
                edgePolyRefs[edge] = polyRef.Remove(polygon);
            }
        }

        private int AddPolygonInternal(int[] polyEdges, int count)
        {
            Debug.Assert(count >= 3);
            Debug.Assert(polyEdges != null);
            Debug.Assert(polyEdges.Length >= count);

            for (var i = 0; i < count; i++)
            {
                Debug.Assert(polyEdges[i] >= 0);
                Debug.Assert(polyEdges[i] < edgeCount);
            }

            if (polyCount >= polygons.Length)
            {
                var newLength = polyCount * 2;
                var oldPolygons = polygons;
                polygons = new Polygon[newLength];

                for (var i = 0; i < polyCount; i++)
                {
                    polygons[i] = oldPolygons[i];
                }
            }

            polygons[polyCount] = new Polygon(polyEdges, count);
            return polyCount++;
        }

        private void RemovePolygonInternal(int index)
        {
            Debug.Assert(index > 0);
            Debug.Assert(index < polyCount);

            polyCount--;

            // Remove edgePolyRef entries for this polygon
            var polygon = polygons[index];
            for (var i = 0; i < polygon.Count; i++)
            {
                RemoveEdgePolyRef(polygon.Edges[i], index);
            }

            if (index < polyCount)
            {
                // Swap last polygon with removed polygon to prevent holes
                polygons[index] = polygons[polyCount];

                // Update edges pointing to swapped polygon
                polygon = polygons[polyCount];
                for (var i = 0; i < polygon.Count; i++)
                {
                    var polyRef = edgePolyRefs[polygon.Edges[i]];
                    for (var j = 0; j < polyRef.Count; j++)
                    {
                        if (polyRef.Polygons[j] == polyCount)
                        {
                            polyRef.Polygons[j] = index;
                            break;
                        }
                    }
                }
            }

            edgePolyRefs[polyCount] = default;
        }

        private struct VertexEdgeRef
        {
            public readonly int[] Edges;

            public readonly int Count;

            public VertexEdgeRef(int[] edges, int count)
            {
                Debug.Assert((count == 0 && edges == null) || (count > 0 && edges != null));

                this.Edges = edges;
                this.Count = count;
            }

            public VertexEdgeRef Add(int edgeIndex)
            {
                Debug.Assert(!Contains(edgeIndex));

                var newEdges = Edges;

                if (newEdges == null)
                {
                    newEdges = new int[4];
                }
                else if (newEdges.Length <= Count)
                {
                    newEdges = new int[newEdges.Length * 2];
                    for (var i = 0; i < Count; i++)
                    {
                        newEdges[i] = Edges[i];
                    }
                }

                newEdges[Count] = edgeIndex;

                return new VertexEdgeRef(newEdges, Count + 1);
            }

            public VertexEdgeRef Remove(int edgeIndex)
            {
                Debug.Assert(Contains(edgeIndex));

                if (Count <= 1)
                {
                    return new VertexEdgeRef(null, 0);
                }

                for (var i = 0; i < Count; i++)
                {
                    if (Edges[i] == edgeIndex)
                    {
                        if (i < Count - 1)
                        {
                            Edges[i] = Edges[Count - 1];
                        }

                        break;
                    }
                }

                return new VertexEdgeRef(Edges, Count - 1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Contains(int edgeIndex)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (Edges[i] == edgeIndex)
                    {
                        return true;
                    }
                }

                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int IndexOf(int edgeIndex)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (Edges[i] == edgeIndex)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        private struct Edge
        {
            public readonly int V0;

            public readonly int V1;

            public Edge(int v0, int v1)
            {
                this.V0 = v0;
                this.V1 = v1;
            }
        }

        private struct EdgePolyRef
        {
            public readonly int[] Polygons;

            public readonly int Count;

            public EdgePolyRef(int[] polygons, int count)
            {
                Debug.Assert((count == 0 && polygons == null) || (count > 0 && polygons != null));

                this.Polygons = polygons;
                this.Count = count;
            }

            public EdgePolyRef Add(int polyIndex)
            {
                Debug.Assert(!Contains(polyIndex));

                var newPolygons = Polygons;

                if (newPolygons == null)
                {
                    newPolygons = new int[4];
                }
                else if (newPolygons.Length <= Count)
                {
                    newPolygons = new int[newPolygons.Length * 2];
                    for (var i = 0; i < Count; i++)
                    {
                        newPolygons[i] = Polygons[i];
                    }
                }

                newPolygons[Count] = polyIndex;

                return new EdgePolyRef(newPolygons, Count + 1);
            }

            public EdgePolyRef Remove(int polyIndex)
            {
                Debug.Assert(Contains(polyIndex));

                if (Count <= 1)
                {
                    return new EdgePolyRef(null, 0);
                }

                for (var i = 0; i < Count; i++)
                {
                    if (Polygons[i] == polyIndex)
                    {
                        if (i < Count - 1)
                        {
                            Polygons[i] = Polygons[Count - 1];
                        }

                        break;
                    }
                }

                return new EdgePolyRef(Polygons, Count - 1);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Contains(int polyIndex)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (Polygons[i] == polyIndex)
                    {
                        return true;
                    }
                }

                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int IndexOf(int polyIndex)
            {
                for (var i = 0; i < Count; i++)
                {
                    if (Polygons[i] == polyIndex)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        private struct Polygon
        {
            public readonly int[] Edges;

            public readonly int Count;

            public Polygon(int[] edges, int count)
            {
                Debug.Assert(count >= 3);
                Debug.Assert(edges != null);
                Debug.Assert(edges.Length >= 3);

                this.Edges = edges;
                this.Count = count;
            }
        }
    }
}
