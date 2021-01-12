using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace UnityTools.Dots.Containers.Internal
{
    [BurstCompile]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct NativePolyMeshData : IDisposable
    {
        #region Constants

        private const int InitVertexCapacity = 32;
        private const int InitEdgeCapacity = 32;
        private const int InitFaceCapacity = 16;

        #endregion

        #region Private Fields

        public Allocator allocator;
        
        private Vertex* vertices;
        private int vertexCapacity;
        private int vertexCount;
        private int firstFreeVertex;

        private Edge* edges;
        private int edgeCapacity;
        private int edgeCount;
        private int firstFreeEdge;

        private Face* faces;
        private int faceCapacity;
        private int faceCount;
        private int firstFreeFace;

        private VertexToEdge* v2e;
        private int v2eCapacity;
        private int v2eCount;

        private EdgeToFace* e2f;
        private int e2fCapacity;
        private int e2fCount;

        #endregion

        #region Public Properties

        public bool IsValid => this.vertices != null;

        public int VertexCount => this.vertexCount;

        public int EdgeCount => this.edgeCount;

        public int FaceCount => this.faceCount;

        #endregion

        #region Public Utility Methods

        public void Initialize(Allocator allocator)
        {
            this.allocator = allocator;

            this.vertices = (Vertex*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Vertex>() * InitVertexCapacity, UnsafeUtility.AlignOf<Vertex>(), allocator);
            this.edges = (Edge*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Edge>() * InitEdgeCapacity, UnsafeUtility.AlignOf<Edge>(), allocator);
            this.faces = (Face*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Face>() * InitFaceCapacity, UnsafeUtility.AlignOf<Face>(), allocator);
            this.v2e = (VertexToEdge*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<VertexToEdge>() * InitVertexCapacity, UnsafeUtility.AlignOf<VertexToEdge>(), allocator);
            this.e2f = (EdgeToFace*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<EdgeToFace>() * InitEdgeCapacity, UnsafeUtility.AlignOf<EdgeToFace>(), allocator);

            this.vertexCapacity = InitVertexCapacity;
            this.edgeCapacity = InitEdgeCapacity;
            this.faceCapacity = InitFaceCapacity;
            this.v2eCapacity = InitVertexCapacity;
            this.e2fCapacity = InitEdgeCapacity;

            this.vertexCount = 0;
            this.edgeCount = 0;
            this.faceCount = 0;
            this.v2eCount = 0;
            this.e2fCount = 0;

            this.firstFreeVertex = -1;
            this.firstFreeEdge = -1;
            this.firstFreeEdge = -1;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            for (var i = 0; i < this.faceCount; i++)
            {
                UnsafeUtility.Free((this.faces + i)->Edges, this.allocator);
            }

            UnsafeUtility.Free(this.vertices, this.allocator);
            UnsafeUtility.Free(this.edges, this.allocator);
            UnsafeUtility.Free(this.faces, this.allocator);
            UnsafeUtility.Free(this.v2e, this.allocator);
            UnsafeUtility.Free(this.e2f, this.allocator);

            this.vertices = null;
            this.edges = null;
            this.faces = null;
            this.v2e = null;
            this.e2f = null;
        }

        #endregion

        #region Public Mesh Modification Methods

        /// <summary>
        /// Adds a vertex to the mesh, multiple vertices can be at the same position.
        /// </summary>
        /// <param name="position">The position of the new vertex.</param>
        /// <returns>Returns the index of the new vertex.</returns>
        public int AddVertex(float3 position)
        {
            var newIndex = this.AllocVertex();

            var newVertex = this.vertices + newIndex;
            newVertex->Position = position;
            newVertex->VertexToEdge = -1;

            return newIndex;
        }

        /// <summary>
        /// Connects two vertices with an edge or reuses an existing connection.
        /// </summary>
        /// <param name="v0">The one vertex.</param>
        /// <param name="v1">The other vertex.</param>
        /// <returns>Returns the index of the edge connecting the vertices.</returns>
        public int MakeEdge([AssumeRange(0, int.MaxValue)]int v0, [AssumeRange(0, int.MaxValue)]int v1)
        {
            // Enforce ordering
            if (v1 < v0)
            {
                var tmp = v0;
                v0 = v1;
                v1 = tmp;
            }

            // Test whether an edge already exists
            var edgeIndex = this.GetEdgeIndex(v0, v1);
            if (edgeIndex >= 0)
            {
                return edgeIndex;
            }

            if (this.edgeCount >= this.edgeCapacity)
            {
                this.GrowEdgeCapacity();
            }

            edgeIndex = this.edgeCount;
            this.edgeCount++;

            // Create a new edge
            var newEdge = this.edges + edgeIndex;
            newEdge->Vertex0 = v0;
            newEdge->Vertex1 = v1;
            newEdge->EdgeToFace = -1;

            // Add the new edge to the vertices
            this.AddVertexEdgeRef(v0, edgeIndex);
            this.AddVertexEdgeRef(v1, edgeIndex);
            
            return edgeIndex;
        }
        
        /// <summary>
        /// Removes the vertex at the given index and all edges and faces using that vertex.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveVertex(int index)
        {
            // TODO
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Removes the edge at the given index and all faces using that edge, but not the vertices.
        /// </summary>
        /// <param name="index">The index of the edge.</param>
        public void RemoveEdge(int index)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the face at the given index, doesn't remove edges and vertices belonging to that face.
        /// </summary>
        /// <param name="index">The index of the face.</param>
        public void RemoveFace(int index)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion

        #region Public Mesh Query Methods

        public int GetEdgeIndex([AssumeRange(0, int.MaxValue)] int v0, [AssumeRange(0, int.MaxValue)] int v1)
        {
            if (v0 == v1)
            {
                return -1;
            }

            if (v1 < v0)
            {
                var tmp = v0;
                v0 = v1;
                v1 = tmp;
            }

            // Use vertex 0 to search for the edge
            var vepIndex = (this.vertices + v0)->VertexToEdge;

            while (vepIndex >= 0)
            {
                var vep = this.v2e + vepIndex;
                var e0p = &(vep->Edge0);    // Pointer to the first edge index of the v2e

                // Test whether any stored edge connects v0 with v1
                for (var i = 0; i < 6; i++)
                {
                    var edgeIndex = *(e0p + i);
                    if (edgeIndex < 0)
                    {
                        // Tested all edges, found no match
                        return -1;
                    }

                    var eip = this.edges + edgeIndex;
                    if (eip->Vertex1 == v1)
                    {
                        // The edge connects v0 with v1
                        return edgeIndex;
                    }
                }

                vepIndex = vep->NextV2E;
            }

            return -1;
        }

        public bool HasEdge([AssumeRange(0, int.MaxValue)] int v0, [AssumeRange(0, int.MaxValue)] int v1)
        {
            return this.GetEdgeIndex(v0, v1) >= 0;
        }

        #endregion

        #region Private Mesh Modification Methods
        
        private void AddVertexEdgeRef([AssumeRange(0, int.MaxValue)] int v, [AssumeRange(0, int.MaxValue)] int e)
        {
            var vp = this.vertices + v;
            if (vp->VertexToEdge < 0)
            {
                // No edges yet, add a new v2e
                var newIndex = this.AddV2E(v, e);
                vp->VertexToEdge = newIndex;
                return;
            }

            // Find the last v2e that belongs to this vertex
            var veIndex = vp->VertexToEdge;
            var vep = this.v2e + veIndex;
            while (vep->NextV2E >= 0)
            {
                veIndex = vep->NextV2E;
                vep = this.v2e + veIndex;
            }

            var edgeRefCount = vep->EdgeCount;
            if (edgeRefCount < 6)
            {
                // The v2e still has space for another entry
                var eip = &(vep->Edge0) + edgeRefCount;
                *(eip) = e;
            }
            else
            {
                // The v2e is full, make a new one
                var newIndex = this.AddV2E(-(veIndex + 1), e);
                vep->NextV2E = newIndex;
            }
        }

        private int AddV2E(int parentIndex, int e)
        {
            if (this.v2eCount >= this.v2eCapacity)
            {
                this.GrowV2ECapacity();
            }

            var v2eIndex = this.v2eCount;
            this.v2eCount++;

            var newV2E = this.v2e + v2eIndex;
            newV2E->Initialize();
            newV2E->Parent = parentIndex;
            newV2E->Edge0 = e;

            return v2eIndex;
        }

        #endregion

        #region Private Utility Methods

        /// <summary>
        /// Allocates a fixed space for a new vertex.
        /// </summary>
        /// <returns>Returns the index of the allocated space.</returns>
        private int AllocVertex()
        {
            int index;

            if (this.firstFreeVertex < 0)
            {
                // The array has no holes, allocate space at the end of the array instead.
                if (this.vertexCount >= this.vertexCapacity)
                {
                    this.GrowVertexCapacity();
                }

                index = this.vertexCount;
            }
            else
            {
                // The array has holes, use the first hole
                index = this.firstFreeVertex;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if((this.vertices + index)->VertexToEdge > -2)
                    throw new Exception("The first free vertex pointer pointed to a vertex that wasn't marked as free.");
#endif

                // The offset to the next hole is stored in the VertexToEdge as a negative number [min, -2]
                var nextOffset = (this.vertices + this.firstFreeVertex)->VertexToEdge;

                if (nextOffset == -2)
                {
                    // This was the last hole, reset the hole index
                    this.firstFreeVertex = -1;
                }
                else
                {
                    // A next free index exists, shift the hole index
                    nextOffset = -(nextOffset + 2);
                    this.firstFreeVertex += nextOffset;
                }
            }

            this.vertexCount++;
            return index;
        }

        /// <summary>
        /// Frees the vertex memory.
        /// </summary>
        /// <param name="index">The index of the vertex.</param>
        private void FreeVertex([AssumeRange(0, int.MaxValue)]int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (index < 0 || index >= this.vertexCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            if((this.vertices + index)->VertexToEdge <= -2)
                throw new InvalidOperationException("The vertex is already freed.");
#endif

            var vp = this.vertices + index;

            if (this.firstFreeVertex < 0)
            {
                // No hole yet, mark the vertex as last hole and set the hole index to the vertex index.
                vp->VertexToEdge = -2;
                this.firstFreeVertex = index;
            }
            else if (index < this.firstFreeVertex)
            {
                // All holes are later in the array, calculate the hole offset and set the hole index
                vp->VertexToEdge = -(this.firstFreeVertex - index + 2);
                this.firstFreeVertex = index;
            }
            else
            {
                // Find the nearest previous invalid vertex in order to insert this vertex into the linked list of holes
                var prevFreeIndex = this.firstFreeVertex;

                for (var i = index - 1; i > this.firstFreeVertex; i--)
                {
                    if ((this.vertices + i)->VertexToEdge <= -2)
                    {
                        prevFreeIndex = i;
                        break;
                    }
                }

                var prevFree = this.vertices + prevFreeIndex;

                // Update the hole offset of the previous and current vertex
                if (prevFree->VertexToEdge == -2)
                {
                    // This vertex is the last hole
                    vp->VertexToEdge = -2;
                    prevFree->VertexToEdge = -(index - prevFreeIndex + 2);
                }
                else
                {
                    // There is a hole after this vertex
                    var nextFreeIndex = -(prevFree->VertexToEdge + 2) + prevFreeIndex;
                    vp->VertexToEdge = -(nextFreeIndex - index + 2);
                    prevFree->VertexToEdge = -(index - prevFreeIndex + 2);
                }
            }

            this.vertexCount--;
        }

        private void GrowVertexCapacity()
        {
            this.vertexCapacity *= 2;
            if (this.vertexCapacity < 0)
            {
                throw new InsufficientMemoryException("NativePolyMesh reached max capacity for internal buffers.");
            }

            var newVertices = (Vertex*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Vertex>() * this.vertexCapacity, UnsafeUtility.AlignOf<Vertex>(), this.allocator);

            for (var i = 0; i < this.vertexCount; i++)
            {
                newVertices[i] = this.vertices[i];
            }

            UnsafeUtility.Free(this.vertices, this.allocator);
            this.vertices = newVertices;
        }

        private void GrowEdgeCapacity()
        {
            this.edgeCapacity *= 2;
            if (this.edgeCapacity < 0)
            {
                throw new InsufficientMemoryException("NativePolyMesh reached max capacity for internal buffers.");
            }

            var newEdges = (Edge*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Edge>() * this.edgeCapacity, UnsafeUtility.AlignOf<Edge>(), this.allocator);

            for (var i = 0; i < this.edgeCount; i++)
            {
                newEdges[i] = this.edges[i];
            }

            UnsafeUtility.Free(this.edges, this.allocator);
            this.edges = newEdges;
        }

        private void GrowFaceCapacity()
        {
            this.faceCapacity *= 2;
            if (this.faceCapacity < 0)
            {
                throw new InsufficientMemoryException("NativePolyMesh reached max capacity for internal buffers.");
            }

            var newFaces = (Face*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Face>() * this.faceCapacity, UnsafeUtility.AlignOf<Face>(), this.allocator);

            for (var i = 0; i < this.faceCount; i++)
            {
                newFaces[i] = this.faces[i];
            }

            UnsafeUtility.Free(this.faces, this.allocator);
            this.faces = newFaces;
        }

        private void GrowV2ECapacity()
        {
            this.v2eCapacity *= 2;
            if (this.v2eCapacity < 0)
            {
                throw new InsufficientMemoryException("NativePolyMesh reached max capacity for internal buffers.");
            }

            var newV2E = (VertexToEdge*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<VertexToEdge>() * this.v2eCapacity, UnsafeUtility.AlignOf<VertexToEdge>(), this.allocator);

            for (var i = 0; i < this.v2eCount; i++)
            {
                newV2E[i] = this.v2e[i];
            }

            UnsafeUtility.Free(this.v2e, this.allocator);
            this.v2e = newV2E;
        }

        #endregion

        #region Data Structs

        [BurstCompile]
        [StructLayout(LayoutKind.Sequential)]
        private struct Vertex
        {
            public float3 Position;

            // V2E >= 0: Index of v2e; V2E == -1: No v2e; V2E <= -2: Invalid vertex, -(offset + 2) to next free space in array
            public int VertexToEdge;
        }

        [BurstCompile]
        [StructLayout(LayoutKind.Sequential)]
        private struct Edge
        {
            public int Vertex0;

            public int Vertex1;

            public int EdgeToFace;
        }

        [BurstCompile]
        [StructLayout(LayoutKind.Sequential)]
        private struct Face
        {
            public int* Edges;

            public int Capacity;

            public int Count;

            public bool IsValid => this.Edges != null;

            public void Initialize(int capacity, Allocator allocator)
            {
                this.Edges = (int*) UnsafeUtility.Malloc(sizeof(int) * capacity, 4, allocator);
                this.Capacity = capacity;
                this.Count = 0;
            }
        }

        [BurstCompile]
        [StructLayout(LayoutKind.Sequential)]
        private struct VertexToEdge
        {
            // Parent >= 0: Index of vertex; Parent < 0: -(Index + 1) of previous V2E (linked list style)
            public int Parent;
            
            // In case the V2E is full, this points to the next one with data (linked list style)
            public int NextV2E;

            public int Edge0;

            public int Edge1;

            public int Edge2;

            public int Edge3;

            public int Edge4;

            public int Edge5;

            public int EdgeCount
            {
                get
                {
                    // Divide & Conquer style counting...
                    if (this.Edge2 < 0)
                    {
                        if (this.Edge0 < 0)
                        {
                            return 0;
                        }

                        return this.Edge1 < 0 ? 1 : 2;
                    }

                    if (this.Edge4 < 0)
                    {
                        return this.Edge3 < 0 ? 3 : 4;
                    }

                    return this.Edge5 < 0 ? 5 : 6;
                }
            }

            public void Initialize()
            {
                this.NextV2E = -1;
                this.Edge0 = -1;
                this.Edge1 = -1;
                this.Edge2 = -1;
                this.Edge3 = -1;
                this.Edge4 = -1;
                this.Edge5 = -1;
            }
        }

        [BurstCompile]
        [StructLayout(LayoutKind.Sequential)]
        private struct EdgeToFace
        {
            // Parent >= 0: Index of edge; Parent < 0: -(Index + 1) of previous E2F (linked list style)
            public int Parent;

            // In case the E2F is full, this points to the next one with data (linked list style)
            public int NextE2F;

            public int Face0;

            public int Face1;

            public int FaceCount
            {
                get
                {
                    if (this.Face0 < 0)
                    {
                        return 0;
                    }

                    return this.Face1 < 0 ? 1 : 2;
                }
            }

            public void Initialize()
            {
                this.NextE2F = -1;
                this.Face0 = -1;
                this.Face1 = -1;
            }
        }

        #endregion
    }
}
