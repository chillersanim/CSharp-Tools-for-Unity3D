using System;
using UnityEngine;
using UnityTools.Core;

namespace UnityTools.Experimental
{
    public sealed class VoxelArea2 : IArea
    {
        #region Constants

        private const int InitialNodeCount = 4;

        private const int InitialLeafCount = 16;

        private const int EmptyLeaf = 0;

        private const int FullLeaf = -1;

        #endregion

        #region Private Fields

        // TODO: Change node deletion strategy from swap-back to deletion tracking as outlined in the summary
        /// <summary>
        /// Contains all the nodes, amount is tracked by nodeCount field.
        /// First item is root node
        /// When capacity is reached, array gets replaced with new array of double size.
        /// Can contain gaps from deleted nodes where deleted nodes:
        /// -> Have a negative parent index
        /// -> Form a linked list using the ChildBL field to point to the previously deleted node (tail has negative field value)
        /// Head of deleted nodes is tracked by firstDeletedNode field (negative value = no deleted nodes)
        /// </summary>
        private Node[] nodes;

        private int nodeCount;

        private int firstDeletedNode;

        // TODO: Implement leaf deletion strategy by tracking deleted nodes as outlined in the summary
        /// <summary>
        /// Contains all the leafs, amount is tracked by leafCount field.
        /// When capacity is reached, array gets replaced with new array of double size.
        /// Can contain gaps from deleted leafs where deleted leafs:
        /// -> Have a negative parent index
        /// -> Form a linked list using the Flags field to point to the previously deleted leaf (tail has negative field value)
        /// Head of deleted leafs is tracked by firstDeletedLeaf field (negative value = no deleted leafs)
        /// </summary>
        private Leaf[] leafs;

        private int leafCount;

        private int firstDeletedLeaf;

        private Vector2 min;

        private Vector2 size;

        private int depth;

        private bool inverted;

        #endregion

        #region Public Properties

        /// <inheritdoc />
        public Bounds2 Bounds => new Bounds2(this.min + this.size / 2f, this.size);

        /// <inheritdoc />
        public bool Inverted => this.inverted;

        #endregion

        #region Constructors

        public VoxelArea2()
        {
            this.nodes = new Node[InitialNodeCount];
            this.nodeCount = 1;
            this.firstDeletedNode = -1;
            this.leafs = new Leaf[InitialLeafCount];
            this.leafCount = 0;
            this.firstDeletedLeaf = -1;
            this.min = Vector2.zero;
            this.size = Vector2.one;
            this.inverted = false;
        }
        
        #endregion

        #region Public Methods

        /// <inheritdoc />
        public bool ContainsRect(in Vector2 start, in Vector2 end)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool ContainsPoint(in Vector2 point)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool IntersectsRect(in Vector2 start, in Vector2 end)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool Raycast(in Vector2 orig, in Vector2 dir, out float t, out Vector2 normal)
        {
            throw new System.NotImplementedException();
        }

        public void Add<T>(in T area) where T : IArea
        {
            if (!this.Inverted)
            {
                var bounds = area.Bounds;
                this.GrowToInclude(bounds.Min);
                this.GrowToInclude(bounds.Max);
            }

            this.AddToNode(0, area, this.depth, this.min, this.size / 2f);

            this.ShrinkToFit();
        }

        #endregion

        #region Private Methods

        private bool AddToNode<T>(int nodeIndex, T area, int localDepth, Vector2 localMin, Vector2 halfSize) where T : IArea
        {
            // TODO: Finish implementing AddToNode
            ref var node = ref this.nodes[nodeIndex];
            var notFullFlag = 0;

            for (var i = 0; i < 4; i++)
            {
                var notFull = false;
                var nodeMin = localMin;

                if (i > 2)
                {
                    nodeMin.x += halfSize.x;
                }

                if (i == 1 || i == 2)
                {
                    nodeMin.y += halfSize.y;
                }

                // If the child doesn't intersect with the given area, the child will not be affected.
                if (!area.IntersectsRect(nodeMin, nodeMin + halfSize))
                {
                    continue;
                }

                if (node[i] > 0)
                {
                    // Child is node
                    break;
                }
            }

            
            /*
            for (var i = 0; i < 4; i++)
            {
                var hasEmpty = false;

                if (node.Children[i] is Node childNode)
                {
                    hasEmpty = AddToNode(childNode, area, localDepth - 1);
                }
                else if (node.Children[i] is Leaf leaf)
                {
                    if (leaf.InsideFlag == 0x0F)
                    {
                        hasEmpty = false;
                    }
                    else if (localDepth > 0)
                    {
                        if (area.ContainsRect(leaf.Min, leaf.Max))
                        {
                            leaf.InsideFlag = 0x0F;
                            leaf.UpdateVertex();
                            hasEmpty = false;
                        }
                        else if (area.IntersectsRect(leaf.Min, leaf.Max))
                        {
                            childNode = new Node(leaf.Min, leaf.Size, false);
                            node.Children[i] = childNode;
                            childNode.Parent = node;
                            hasEmpty = AddToNode(childNode, area, localDepth - 1);
                        }
                        else
                        {
                            hasEmpty = true;
                        }
                    }
                    else
                    {
                        hasEmpty = AddToLeaf(leaf, area);
                    }
                }

                emptyFlag |= hasEmpty ? 1 << i : 0;
            }

            if (emptyFlag == 0)
            {
                return false;
            }

            for (var i = 0; i < 4; i++)
            {
                var hasEmpty = ((emptyFlag >> i) & 0x01) == 0x01;

                if (!hasEmpty && node.Children[i] is Node childNode)
                {
                    node.Children[i] = new Leaf(childNode.Min, childNode.Size, true);
                    node.Children[i].Parent = node;
                }
            }

            return true;
            */

            throw new NotImplementedException();
        }

        private void GrowToInclude(Vector2 point)
        {
            Debug.Assert(!point.IsNaN());
            Debug.Assert(!point.IsInfinite());

            ref var root = ref this.nodes[0];
            var max = this.min + this.size;

            if (point.InRectMm(this.min, max))
            {
                return;
            }
            
            var hasContent = root.HasContent;

            do
            {
                this.depth++;
                var hDir = this.min.x < point.x;
                var vDir = this.min.y < point.y;

                if (hasContent)
                {
                    var newNodeIndex = this.AddNodeUninitialized();
                    this.MoveNode(0, newNodeIndex);

                    root.ChildBL = root.ChildTL = root.ChildTR = root.ChildBR = 0;
                }

                if (vDir)
                {
                    if (hDir)
                    {
                        // Up right
                        root.ChildBL = this.nodeCount - 1;
                    }
                    else
                    {
                        // Up left
                        this.min.x -= this.size.x;
                        root.ChildBR = this.nodeCount - 1;
                    }
                }
                else
                {
                    if (hDir)
                    {
                        // Down right
                        this.min.y -= this.size.y;
                        root.ChildTL = this.nodeCount - 1;
                    }
                    else
                    {
                        // Down left
                        this.min -= this.size;
                        root.ChildTR = this.nodeCount - 1;
                    }
                }

                this.size *= 2f;

            } while (!point.InRectMm(min, min + size));
        }

        private void ShrinkToFit()
        {
            ref var root = ref this.nodes[0];

            while (this.depth > 1)
            {
                var emptyLeafFlags = 0;

                for (var i = 0; i < 4; i++)
                {
                    if (root[i] == 0)
                    {
                        emptyLeafFlags |= 1 << i;
                    }
                }

                int nodeIndex;

                if (emptyLeafFlags == 0x0E)
                {
                    // BL is node, TL, BR and TR are leafs
                    nodeIndex = root[0];
                }
                else if (emptyLeafFlags == 0x0D)
                {
                    // TL is node, BL, BR and TR are leafs
                    nodeIndex = root[1];
                }
                else if (emptyLeafFlags == 0x0B)
                {
                    // BR is node, BL, TL and TR are leafs
                    nodeIndex = root[2];
                }
                else if (emptyLeafFlags == 0x07)
                {
                    // TR is node, BL, TL and BR are leafs
                    nodeIndex = root[3];
                }
                else if (emptyLeafFlags == 0x0F)
                {
                    // All children are empty leafs, revert to default
                    root.ChildBL = 0;
                    root.ChildTL = 0;
                    root.ChildTL = 0;
                    root.ChildBR = 0;
                    this.depth = 1;
                    this.min = Vector2.zero;
                    this.size = Vector2.one;
                    return;
                }
                else
                {
                    return;
                }

                // Move new root to first array index
                this.MoveNode(nodeIndex, 0);

                if (nodeIndex < this.nodeCount - 1)
                {
                    // Move last node in array to hole and remove last element
                    this.MoveNode(this.nodeCount - 1, nodeIndex);
                    this.nodeCount--;
                }

                this.depth--;
            }
        }

        private int AddNodeUninitialized()
        {
            if (this.firstDeletedNode > 0)
            {
                ref var node = ref this.nodes[this.firstDeletedNode];
                var newIndex = this.firstDeletedNode;
                Debug.Assert(node.ChildBL < 0);

                if (node.ChildBL > 0)
                {
                    this.firstDeletedNode = node.ChildBL;
                }
                else
                {
                    this.firstDeletedNode = -1;
                }

                this.nodeCount++;
                return newIndex;
            }

            if (this.nodeCount == this.nodes.Length)
            {
                var newNodes = new Node[this.nodes.Length * 2];
                this.nodes.CopyTo(newNodes, 0);
                this.nodes = newNodes;
            }

            return this.nodeCount++;
        }

        private void RemoveNodeRecursive(int nodeIndex)
        {
            Debug.Assert(nodeIndex > 0);
            Debug.Assert(nodeIndex < this.nodeCount);

            ref var node = ref this.nodes[nodeIndex];

            for (var i = 0; i < 4; i++)
            {
                var childIndex = node[i];

                if (childIndex > 0)
                {
                    RemoveNodeRecursive(childIndex);
                }
                else if (childIndex < -1)
                {
                    RemoveLeaf(-(childIndex + 2));
                }
            }

            if (nodeIndex < this.nodeCount - 1)
            {
                // Node is not at the end of the array, move last node to fill the gap
                this.MoveNode(this.nodeCount - 1, nodeIndex);
            }

            this.nodeCount--;
        }

        private void RemoveLeaf(int leafIndex)
        {
            Debug.Assert(leafIndex >= 0);
            Debug.Assert(leafIndex < this.leafCount);

            
        }

        private void Invert()
        {
            // Invert nodes by swapping empty and full leaf indices
            for (var i = 0; i < this.nodeCount; i++)
            {
                ref var node = ref this.nodes[i];

                for (var j = 0; j < 4; j++)
                {
                    var index = node[j];
                    if (index == 0)
                    {
                        node[j] = -1;
                    }
                    else if(index == -1)
                    {
                        node[j] = 0;
                    }
                }
            }

            // Invert leafs by flipping inside flags and flipping intersection normals
            for (var i = 0; i < this.leafCount; i++)
            {
                ref var leaf = ref this.leafs[i];
                leaf.Flags ^= 0x0F; // Flip inside flags
                leaf.Left.x = -leaf.Left.x;
                leaf.Left.y = -leaf.Left.y;
                leaf.Top.x = -leaf.Top.x;
                leaf.Top.y = -leaf.Top.y;
                leaf.Right.x = -leaf.Right.x;
                leaf.Right.y = -leaf.Right.y;
                leaf.Bottom.x = -leaf.Bottom.x;
                leaf.Bottom.y = -leaf.Bottom.y;
            }
        }

        /// <summary>
        /// Moves the node at the given index to the new index and adapts all references to the moved node, overwriting any node at the new index and leaving the original node index unchanged.
        /// </summary>
        /// <param name="index">The index of the node to move.</param>
        /// <param name="newIndex">The target index to which the node should be moved.</param>
        private void MoveNode(int index, int newIndex)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < this.nodeCount);
            Debug.Assert(newIndex >= 0);
            Debug.Assert(newIndex < this.nodeCount);

            ref var node = ref this.nodes[index];

            // Only adapt parent reference if node is not root.
            if (index != 0)
            {
                ref var parent = ref this.nodes[node.Parent];
                for (var i = 0; i < 4; i++)
                {
                    if (parent[i] == index)
                    {
                        parent[i] = newIndex;
                    }
                }
            }

            // Adapt child references
            for (var i = 0; i < 4; i++)
            {
                var childIndex = node[i];
                if (childIndex > 0)        // Child is a node
                {
                    ref var childNode = ref this.nodes[childIndex];
                    childNode.Parent = newIndex;
                }
                else if (childIndex < -1)
                {
                    ref var childLeaf = ref this.leafs[-(childIndex + 2)];
                    childLeaf.Parent = newIndex;
                }
            }

            // Move node
            this.nodes[newIndex] = node;
        }

        #endregion

        #region Structs

        private struct Node
        {
            public int Parent;

            /// <summary>
            /// Child indices.
            /// positive:       value = node index
            /// 0:              empty leaf
            /// -1:             full leaf
            /// less than -1:   -(value + 2) = leaf index
            /// </summary>
            public int ChildBL, ChildTL, ChildTR, ChildBR;

            public int this[int childIndex]
            {
                readonly get
                {
                    switch (childIndex)
                    {
                        case 0: return this.ChildBL;
                        case 1: return this.ChildTL;
                        case 2: return this.ChildTR;
                        case 3: return this.ChildBR;
                        default: throw new ArgumentOutOfRangeException(nameof(childIndex));
                    }
                }
                set
                {
                    switch (childIndex)
                    {
                        case 0: this.ChildBL = value; break;
                        case 1: this.ChildTL = value; break;
                        case 2: this.ChildTR = value; break;
                        case 3: this.ChildBR = value; break;
                        default: throw new ArgumentOutOfRangeException(nameof(childIndex));
                    }
                }
            }

            public readonly bool HasContent => ChildBL != 0 && this.ChildTL != 0 &&
                                                this.ChildTR != 0 && this.ChildBR != 0;
        }

        private struct Leaf
        {
            /// <summary>
            /// The index of the parent node, negative values means no parent.
            /// </summary>
            public int Parent;

            /// <summary>
            /// Intersection information, x and y are normal, z is position along the edge (clockwise)
            /// </summary>
            public Vector3 Left, Top, Right, Bottom;

            public Vector2 Vertex1;

            public Vector2 Vertex2;

            /// <summary>
            /// Flag bits:
            /// 0x01: Bottom left inside
            /// 0x02: Top left inside
            /// 0x04: Top right inside
            /// 0x08: Bottom right inside
            /// 0x10: Has vertex 2
            /// 0x20: Vertex connectivity type
            /// 0x40+: Unused
            /// </summary>
            public int Flags;

            public bool BottomLeftInside
            {
                readonly get => (this.Flags & 0x01) == 0x01;
                set
                {
                    if (value)
                    {
                        this.Flags |= 0x01;
                    }
                    else
                    {
                        this.Flags &= ~0x01;
                    }
                }
            }

            public bool TopLeftInside
            {
                readonly get => (this.Flags & 0x02) == 0x02;
                set
                {
                    if (value)
                    {
                        this.Flags |= 0x02;
                    }
                    else
                    {
                        this.Flags &= ~0x02;
                    }
                }
            }

            public bool TopRightInside
            {
                readonly get => (this.Flags & 0x04) == 0x04;
                set
                {
                    if (value)
                    {
                        this.Flags |= 0x04;
                    }
                    else
                    {
                        this.Flags &= ~0x04;
                    }
                }
            }

            public bool BottomRightInside
            {
                readonly get => (this.Flags & 0x08) == 0x08;
                set
                {
                    if (value)
                    {
                        this.Flags |= 0x08;
                    }
                    else
                    {
                        this.Flags &= ~0x08;
                    }
                }
            }

            public readonly int InsideFlags => (this.Flags & 0x0F);

            public readonly bool HasVertex1 => (this.Flags & 0x0F) > 0 && (this.Flags & 0x0F) < 0x0F;

            public readonly bool HasVertex2 => (this.Flags & 0x10) == 0x10;

            public readonly bool VertexConnectivity => (this.Flags & 0x20) == 0x20;

            public readonly bool HasLeftIntersection => (this.Flags & 0x01) != ((this.Flags >> 1) & 0x01);

            public readonly bool HasTopIntersection => ((this.Flags >> 1) & 0x01) != ((this.Flags >> 2) & 0x01);

            public readonly bool HasRightIntersection => ((this.Flags >> 2) & 0x01) != ((this.Flags >> 3) & 0x01);

            public readonly bool HasBottomIntersection => ((this.Flags >> 3) & 0x01) != (this.Flags & 0x01);
        }
        
        #endregion
    }
}
