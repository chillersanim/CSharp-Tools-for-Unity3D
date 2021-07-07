using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityTools.Core;

namespace UnityTools.Experimental
{
    public class VoxelArea : IArea
    {
        private const float LeafSize = 0.03125f*4;

        private const float MinMargin = 0.1f;

        private Node root;

        private int depth;

        public Vector2 Min => this.root.Min;

        public Vector2 Max => this.root.Min + this.root.Size;

        /// <inheritdoc />
        public Bounds2 Bounds { get; }

        public bool Inverted
        {
            get
            {
                var node = this.root;

                while (node.Children[0] is Node childNode)
                {
                    node = childNode;
                }

                var leaf = (Leaf)node.Children[0];
                return leaf.IsInside(0);
            }
        }

        /// <inheritdoc />
        public bool ContainsRect(in Vector2 start, in Vector2 end)
        {
            return this.root.ContainsRect(start, end);
        }

        /// <inheritdoc />
        public bool ContainsPoint(in Vector2 point)
        {
            return this.root.ContainsPoint(point);
        }

        /// <inheritdoc />
        public bool IntersectsRect(in Vector2 start, in Vector2 end)
        {
            return this.root.IntersectsRect(start, end);
        }

        /// <inheritdoc />
        public bool Raycast(in Vector2 orig, in Vector2 dir, out float t, out Vector2 normal)
        {
            return this.root.Raycast(orig, dir, out t, out normal);
        }

        public VoxelArea()
        {
            this.root = new Node(-Vector2.one * LeafSize, Vector2.one * LeafSize * 2f, false);
            this.depth = 1;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(new Vector3(this.Min.x, 0f, this.Min.y), new Vector3(this.Min.x, 0f, this.Max.y));
            Gizmos.DrawLine(new Vector3(this.Min.x, 0f, this.Max.y), new Vector3(this.Max.x, 0f, this.Max.y));
            Gizmos.DrawLine(new Vector3(this.Max.x, 0f, this.Max.y), new Vector3(this.Max.x, 0f, this.Min.y));
            Gizmos.DrawLine(new Vector3(this.Max.x, 0f, this.Min.y), new Vector3(this.Min.x, 0f, this.Min.y));

            this.root.DrawGizmos();
        }

        public void Add<T>(T area) where T : IArea
        {
            if (!this.Inverted)
            {
                var bounds = area.Bounds;
                this.GrowToInclude(bounds.Min);
                this.GrowToInclude(bounds.Max);
            }
            
            AddToNode(this.root, area, this.depth);

            this.ShrinkToFit();
        }

        public void Clear()
        {
            var wasInverted = this.Inverted;
            this.root = new Node(-Vector2.one * LeafSize, Vector2.one * LeafSize * 2f, false);
            this.depth = 1;

            if (wasInverted)
            {
                this.root.Invert();
            }
        }

        public void Invert()
        {
            this.root.Invert();
        }

        public void Subtract<T>(T area) where T : IArea
        {
            if (this.Inverted)
            {
                var bounds = area.Bounds;
                this.GrowToInclude(bounds.Min);
                this.GrowToInclude(bounds.Max);
            }

            RemoveFromNode(this.root, area, this.depth);

            this.ShrinkToFit();
        }

        public void Intersect<T>(T area) where T : IArea
        {
            if (this.Inverted)
            {
                var bounds = area.Bounds;
                this.GrowToInclude(bounds.Min);
                this.GrowToInclude(bounds.Max);
            }

            IntersectWithNode(this.root, area, this.depth);

            this.ShrinkToFit();
        }

        public int GetLeafCount()
        {
            return GetLeafCount(this.root);
        }

        public int GetContentLeafCount()
        {
            return GetContentLeafCount(this.root);
        }

        public int GetNodeCount()
        {
            return GetNodeCount(this.root);
        }

        public static VoxelArea Area<T>(T area) where T : IArea
        {
            var result = new VoxelArea();
            result.Add(area);
            return result;
        }

        private void GrowToInclude(Vector2 point)
        {
            if (point.IsNaN() || point.IsInfinite())
            {
                throw new InvalidOperationException("Can't grow a polygon to fit a vector with NaN or infinity components.");
            }

            if (point.InRectMm(this.root.Min, this.root.Max))
            {
                return;
            }

            var inverted = this.Inverted;
            var hasData = false;

            for (var i = 0; i < 4; i++)
            {
                if (this.root.Children[i] is Leaf leaf)
                {
                    if (!inverted && leaf.InsideFlag != 0x00 || inverted && leaf.InsideFlag != 0x0F)
                    {
                        hasData = true;
                        break;
                    }
                }
                else
                {
                    hasData = true;
                    break;
                }
            }

            var min = this.root.Min;
            var size = this.root.Size;

            do
            {
                this.depth++;
                var hDir = min.x < point.x;
                var vDir = min.y < point.y;
                int rootIndex;

                if (vDir)
                {
                    if (hDir)
                    {
                        // Up right
                        rootIndex = 0;
                    }
                    else
                    {
                        // Up left
                        min.x -= size.x;
                        rootIndex = 2;
                    }
                }
                else
                {
                    if (hDir)
                    {
                        // Down right
                        min.y -= size.y;
                        rootIndex = 1;
                    }
                    else
                    {
                        // Down left
                        min -= size;
                        rootIndex = 3;
                    }
                }

                size *= 2f;

                if (hasData)
                {
                    var newRoot = new Node(min, size, inverted);
                    newRoot.Children[rootIndex] = this.root;
                    this.root.Parent = newRoot;
                    this.root = newRoot;
                }
            } while (!point.InRectMm(min, min + size));

            if (!hasData)
            {
                this.root = new Node(min, size, inverted);
            }
        }

        private void ShrinkToFit()
        {
            while (this.depth > 1)
            {
                var leafFlags = 0;

                for (var i = 0; i < 4; i++)
                {
                    if (this.root.Children[i] is Leaf leaf)
                    {
                        if (leaf.InsideFlag == 0 || leaf.InsideFlag == 0x0F)
                        {
                            leafFlags |= (1 << i);
                        }
                    }
                }

                Node node;

                if (leafFlags == 0x0E)
                {
                    // BL is node, TL, BR and TR are leafs
                    node = (Node)this.root.Children[0];
                }
                else if (leafFlags == 0x0D)
                {
                    // TL is node, BL, BR and TR are leafs
                    node = (Node)this.root.Children[1];
                }
                else if (leafFlags == 0x0B)
                {
                    // BR is node, BL, TL and TR are leafs
                    node = (Node)this.root.Children[2];
                }
                else if (leafFlags == 0x07)
                {
                    // TR is node, BL, TL and BR are leafs
                    node = (Node)this.root.Children[3];
                }
                else if (leafFlags == 0x0F)
                {
                    // All children are leafs, shrink BL to node
                    node = new Node(this.root.Children[0].Min, this.root.Children[0].Max, this.Inverted);
                }
                else
                {
                    return;
                }

                this.root = node;
                this.depth--;
            }
        }

        private static bool AddToNode<T>(Node node, T area, int depth) where T : IArea
        {
            var emptyFlag = 0;

            for (var i = 0; i < 4; i++)
            {
                var hasEmpty = false;

                if (node.Children[i] is Node childNode)
                {
                    hasEmpty = AddToNode(childNode, area, depth - 1);
                }
                else if (node.Children[i] is Leaf leaf)
                {
                    if (leaf.InsideFlag == 0x0F)
                    {
                        hasEmpty = false;
                    }
                    else if (depth > 0)
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
                            hasEmpty = AddToNode(childNode, area, depth - 1);
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
        }

        private static bool AddToLeaf<T>(Leaf leaf, T area) where T : IArea
        {
            if (leaf.InsideFlag == 0x0F)
            {
                return false;
            }

            var bl = leaf.Min;
            var tl = new Vector2(leaf.Min.x, leaf.Max.y);
            var br = new Vector2(leaf.Max.x, leaf.Min.y);
            var tr = leaf.Max;

            var blInOld = leaf.IsInside(0);
            var tlInOld = leaf.IsInside(1);
            var brInOld = leaf.IsInside(2);
            var trInOld = leaf.IsInside(3);

            var blAdd = area.ContainsPoint(bl);
            var tlAdd = area.ContainsPoint(tl);
            var brAdd = area.ContainsPoint(br);
            var trAdd = area.ContainsPoint(tr);

            var hasChange = false;
            hasChange |= AddToSide(leaf, area, blInOld, tlInOld, blAdd, tlAdd, bl, tl, Vector2.up, 0, 1, 1);
            hasChange |= AddToSide(leaf, area, tlInOld, trInOld, tlAdd, trAdd, tl, tr, Vector2.right, 1, 0, 1);
            hasChange |= AddToSide(leaf, area, trInOld, brInOld, trAdd, brAdd, tr, br, Vector2.down, 2, 1, -1);
            hasChange |= AddToSide(leaf, area, brInOld, blInOld, brAdd, blAdd, br, bl, Vector2.left, 3, 0, -1);
            hasChange |= !blInOld && blAdd || !tlInOld && blAdd || !trInOld && blAdd || !brInOld && blAdd;

            if (hasChange)
            {
                leaf.SetInside(0, blInOld || blAdd);
                leaf.SetInside(1, tlInOld || tlAdd);
                leaf.SetInside(2, brInOld || brAdd);
                leaf.SetInside(3, trInOld || trAdd);
                leaf.UpdateVertex();
            }

            return leaf.InsideFlag != 0x0F;
        }

        private static bool AddToSide<T>(Leaf leaf, T area, bool inOld0, bool inOld1, bool add0, bool add1,
            Vector2 pos0, Vector2 pos1, Vector2 dir01, int index, int vertexIndex, int compareSign) where T : IArea
        {
            if (!inOld0 && !inOld1)
            {
                if (add0 && !add1)
                {
                    if (area.Raycast(pos1, -dir01, out var t, out var normal))
                    {
                        leaf.Positions[index] = pos1[vertexIndex] - dir01[vertexIndex] * t;
                        leaf.SetNormal(index, normal);
                    }

                    return true;
                }

                if (!add0 && add1)
                {
                    if (area.Raycast(pos0, dir01, out var t, out var normal))
                    {
                        leaf.Positions[index] = pos0[vertexIndex] + dir01[vertexIndex] * t;
                        leaf.SetNormal(index, normal);
                    }

                    return true;
                }
            }
            else if (!inOld0 && inOld1)
            {
                if (!add0 && add1)
                {
                    if (area.Raycast(pos0, dir01, out var t, out var normal) )
                    {
                        var pos = pos0[vertexIndex] + dir01[vertexIndex] * t;

                        if (pos * compareSign < leaf.Positions[index] * compareSign)
                        {
                            leaf.Positions[index] = pos0[vertexIndex] + dir01[vertexIndex] * t;
                            leaf.SetNormal(index, normal);
                            return true;
                        }
                    }
                }
            }
            else if (inOld0 && !inOld1)
            {
                if (add0 && !add1)
                {
                    if (area.Raycast(pos1, -dir01, out var t, out var normal))
                    {
                        var pos = pos1[vertexIndex] - dir01[vertexIndex] * t;

                        if (pos * compareSign > leaf.Positions[index] * compareSign)
                        {
                            leaf.Positions[index] = pos;
                            leaf.SetNormal(index, normal);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool IntersectWithNode<T>(Node node, T area, int depth) where T : IArea
        {
            var contentFlag = 0;

            for (var i = 0; i < 4; i++)
            {
                var hasContent = false;

                if (node.Children[i] is Node childNode)
                {
                    hasContent = IntersectWithNode(childNode, area, depth - 1);
                }
                else if (node.Children[i] is Leaf leaf)
                {
                    if (leaf.InsideFlag == 0)
                    {
                        hasContent = false;
                    }
                    else if (depth > 0)
                    {
                        if (area.ContainsRect(leaf.Min, leaf.Max))
                        {
                            hasContent = true;
                        }
                        else if (area.IntersectsRect(leaf.Min, leaf.Max))
                        {
                            childNode = new Node(leaf.Min, leaf.Size, false);
                            node.Children[i] = childNode;
                            childNode.Parent = node;
                            hasContent = AddToNode(childNode, area, depth - 1);
                        }
                        else
                        {
                            leaf.InsideFlag = 0;
                            hasContent = false;
                        }
                    }
                    else
                    {
                        hasContent = IntersectWithLeaf(leaf, area);
                    }
                }

                contentFlag |= hasContent ? 1 << i : 0;
            }

            if (contentFlag == 0)
            {
                return false;
            }

            for (var i = 0; i < 4; i++)
            {
                var hasEmpty = ((contentFlag >> i) & 0x01) == 0x01;

                if (!hasEmpty && node.Children[i] is Node childNode)
                {
                    node.Children[i] = new Leaf(childNode.Min, childNode.Size, true);
                    node.Children[i].Parent = node;
                }
            }

            return true;
        }

        private static bool IntersectWithLeaf<T>(Leaf leaf, T area) where T : IArea
        {
            throw new NotImplementedException();
        }

        private static bool RemoveFromNode<T>(Node node, T area, int depth) where T : IArea
        {
            var contentFlags = 0;

            for (var i = 0; i < 4; i++)
            {
                var hasContent = false;

                if (node.Children[i] is Node childNode)
                {
                    hasContent = RemoveFromNode<T>(childNode, area, depth - 1);
                }
                else if (node.Children[i] is Leaf leaf)
                {
                    if (leaf.InsideFlag == 0)
                    {
                        hasContent = false;
                    }
                    else if (depth > 0)
                    {
                        if (area.ContainsRect(leaf.Min, leaf.Max))
                        {
                            leaf.InsideFlag = 0;
                            hasContent = false;
                        }
                        else if(area.IntersectsRect(leaf.Min, leaf.Max))
                        {
                            childNode = new Node(leaf.Min, leaf.Size, true);
                            childNode.UpdateVertices();
                            node.Children[i] = childNode;
                            childNode.Parent = node;
                            hasContent = RemoveFromNode<T>(childNode, area, depth - 1);
                        }
                        else
                        {
                            hasContent = true;
                        }
                    }
                    else
                    {
                        hasContent = RemoveFromLeaf(leaf, area);
                    }
                }

                contentFlags |= hasContent ? 1 << i : 0;
            }

            if (contentFlags == 0)
            {
                return false;
            }

            for (var i = 0; i < 4; i++)
            {
                var hasContent = ((contentFlags >> i) & 0x01) == 0x01;

                if (!hasContent && node.Children[i] is Node childNode)
                {
                    node.Children[i] = new Leaf(childNode.Min, childNode.Size);
                    node.Children[i].Parent = node;
                }
            }

            return true;
        }

        private static bool RemoveFromLeaf<T>(Leaf leaf, T area) where T : IArea
        {
            if (leaf.InsideFlag == 0)
            {
                return false;
            }

            var bl = leaf.Min;
            var tl = new Vector2(leaf.Min.x, leaf.Max.y);
            var br = new Vector2(leaf.Max.x, leaf.Min.y);
            var tr = leaf.Max;

            var blInOld = leaf.IsInside(0);
            var tlInOld = leaf.IsInside(1);
            var brInOld = leaf.IsInside(2);
            var trInOld = leaf.IsInside(3);

            var blRemove = area.ContainsPoint(bl);
            var tlRemove = area.ContainsPoint(tl);
            var brRemove = area.ContainsPoint(br);
            var trRemove = area.ContainsPoint(tr);

            var hasChange = false;
            hasChange |= RemoveFromSide(leaf, area, blInOld, tlInOld, blRemove, tlRemove, bl, tl, Vector2.up, 0, 1, 1);
            hasChange |= RemoveFromSide(leaf, area, tlInOld, trInOld, tlRemove, trRemove, tl, tr, Vector2.right, 1, 0, 1);
            hasChange |= RemoveFromSide(leaf, area, trInOld, brInOld, trRemove, brRemove, tr, br, Vector2.down, 2, 1, -1);
            hasChange |= RemoveFromSide(leaf, area, brInOld, blInOld, brRemove, blRemove, br, bl, Vector2.left, 3, 0, -1);
            hasChange |= blInOld && blRemove || tlInOld && tlRemove || trInOld && trRemove || brInOld && brRemove;

            if (hasChange)
            {
                leaf.SetInside(0, blInOld && !blRemove);
                leaf.SetInside(1, tlInOld && !tlRemove);
                leaf.SetInside(2, brInOld && !brRemove);
                leaf.SetInside(3, trInOld && !trRemove);
                leaf.UpdateVertex();
            }

            return leaf.InsideFlag != 0;
        }

        private static bool RemoveFromSide<T>(Leaf leaf, T area, bool inOld0, bool inOld1, bool remove0, bool remove1,
            Vector2 pos0, Vector2 pos1, Vector2 dir01, int index, int vertexIndex, int compareSign) where T : IArea
        {
            if (inOld0 && inOld1)
            {
                if (remove0 && !remove1)
                {
                    if (area.Raycast(pos1, -dir01, out var t, out var normal))
                    {
                        leaf.Positions[index] = pos1[vertexIndex] - dir01[vertexIndex] * t;
                        leaf.SetNormal(index, normal);
                    }

                    return true;
                }

                if (!remove0 && remove1)
                {
                    if (area.Raycast(pos0, dir01, out var t, out var normal))
                    {
                        leaf.Positions[index] = pos0[vertexIndex] + dir01[vertexIndex] * t;
                        leaf.SetNormal(index, normal);
                    }

                    return true;
                }
            }
            else if (!inOld0 && inOld1)
            {
                if (remove0 && !remove1)
                {
                    if (area.Raycast(pos1, -dir01, out var t, out var normal))
                    {
                        var pos = pos1[vertexIndex] - dir01[vertexIndex] * t;

                        if (pos * compareSign > leaf.Positions[index] * compareSign)
                        {
                            leaf.Positions[index] = pos;
                            leaf.SetNormal(index, normal);
                            return true;
                        }
                    }
                }
            }
            else if (inOld0 && !inOld1)
            {
                if (!remove0 && remove1)
                {
                    if (area.Raycast(pos0, dir01, out var t, out var normal))
                    {
                        var pos = pos0[vertexIndex] + dir01[vertexIndex] * t;

                        if (pos * compareSign < leaf.Positions[index] * compareSign)
                        {
                            leaf.Positions[index] = pos;
                            leaf.SetNormal(index, normal);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static int GetLeafCount(Node node)
        {
            var count = 0;

            for (var i = 0; i < 4; i++)
            {
                if (node.Children[i] is Node childNode)
                {
                    count += GetLeafCount(childNode);
                }
                else
                {
                    count++;
                }
            }

            return count;
        }

        private static int GetContentLeafCount(Node node)
        {
            var count = 0;

            for (var i = 0; i < 4; i++)
            {
                if (node.Children[i] is Node childNode)
                {
                    count += GetContentLeafCount(childNode);
                }
                else
                {
                    var leaf = (Leaf) node.Children[i];

                    if (leaf.InsideFlag != 0 && leaf.InsideFlag != 0x0F)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static int GetNodeCount(Node node)
        {
            var count = 1;

            for (var i = 0; i < 4; i++)
            {
                if (node.Children[i] is Node childNode)
                {
                    count += GetNodeCount(childNode);
                }
            }

            return count;
        }

        private interface INode
        {
            Node Parent { get; set; }

            Vector2 Min { get; }

            Vector2 Max { get; }

            bool ContainsPoint(Vector2 point);

            bool ContainsRect(Vector2 start, Vector2 end);

            bool IntersectsRect(Vector2 start, Vector2 end);

            bool Raycast(Vector2 orig, Vector2 dir, out float dist, out Vector2 normal);

            void DrawGizmos();

            void Invert();

            void Resize(Vector2 min, Vector2 size);
        }

        private class Node : INode
        {
            /// <summary>
            /// BL, TL, BR, TR
            /// </summary>
            [NotNull]
            [ItemNotNull]
            public INode[] Children { get; }

            /// <inheritdoc />
            public Node Parent { get; set; }

            /// <inheritdoc />
            public Vector2 Min { get; private set; }

            /// <inheritdoc />
            public Vector2 Size { get; private set; }

            public Vector2 Max => this.Min + this.Size;

            public Vector2 Center => this.Min + this.Size / 2f;

            public Node()
            {
                this.Parent = null;
                this.Min = Vector2.zero;
                this.Size = Vector2.zero;
                this.Children = new INode[4];

                for (var i = 0; i < 4; i++)
                {
                    this.Children[i] = new Leaf();
                }
            }

            public Node(Vector2 min, Vector2 size)
            {
                this.Parent = null;
                this.Min = Vector2.zero;
                this.Size = Vector2.zero;
                this.Children = new INode[4];

                for (var i = 0; i < 4; i++)
                {
                    this.Children[i] = new Leaf();
                    this.Children[i].Parent = this;
                }

                this.Resize(min, size);
            }

            public Node(Vector2 min, Vector2 size, bool invert) : this(min, size)
            {
                for (var i = 0; i < 4; i++)
                {
                    var child = (Leaf) this.Children[i];
                    child.InsideFlag = invert ? 0x0F : 0x00;
                }
            }

            /// <inheritdoc />
            public bool ContainsPoint(Vector2 point)
            {
                if (point.x < this.Center.x)
                {
                    if (point.y < this.Center.y)
                    {
                        return this.Children[0].ContainsPoint(point);
                    }

                    return this.Children[1].ContainsPoint(point);
                }

                if (point.y < this.Center.y)
                {
                    return this.Children[2].ContainsPoint(point);
                }

                return this.Children[3].ContainsPoint(point);
            }

            /// <inheritdoc />
            public bool ContainsRect(Vector2 start, Vector2 end)
            {
                for (var i = 0; i < 4; i++)
                {
                    var child = this.Children[i];
                    if (start.x < child.Max.x && start.y < child.Max.y && end.x >= child.Min.x &&
                        end.y >= child.Min.y)
                    {
                        if (!child.ContainsRect(start, end))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            /// <inheritdoc />
            public bool IntersectsRect(Vector2 start, Vector2 end)
            {
                for (var i = 0; i < 4; i++)
                {
                    var child = this.Children[i];
                    if (start.x < child.Max.x && start.y < child.Max.y && end.x >= child.Min.x &&
                        end.y >= child.Min.y)
                    {
                        if (child.IntersectsRect(start, end))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            /// <inheritdoc />
            public bool Raycast(Vector2 orig, Vector2 dir, out float dist, out Vector2 normal)
            {
                dist = float.PositiveInfinity;
                normal = Vector2.zero;

                if (dir.x >= 0)
                {
                    if (this.Max.x < orig.x)
                    {
                        return false;
                    }

                    if (dir.y >= 0)
                    {
                        if (this.Max.y < orig.y)
                        {
                            return false;
                        }

                        return this.Children[0].Raycast(orig, dir, out dist, out normal) ||
                               this.Children[1].Raycast(orig, dir, out dist, out normal) ||
                               this.Children[2].Raycast(orig, dir, out dist, out normal) ||
                               this.Children[3].Raycast(orig, dir, out dist, out normal);
                    }

                    if (orig.y < this.Min.y)
                    {
                        return false;
                    }

                    return this.Children[1].Raycast(orig, dir, out dist, out normal) ||
                           this.Children[3].Raycast(orig, dir, out dist, out normal) ||
                           this.Children[0].Raycast(orig, dir, out dist, out normal) ||
                           this.Children[2].Raycast(orig, dir, out dist, out normal);
                }

                if (orig.x < this.Min.x)
                {
                    return false;
                }

                if (dir.y >= 0)
                {
                    if (this.Max.y < orig.y)
                    {
                        return false;
                    }

                    return this.Children[2].Raycast(orig, dir, out dist, out normal) ||
                           this.Children[0].Raycast(orig, dir, out dist, out normal) ||
                           this.Children[3].Raycast(orig, dir, out dist, out normal) ||
                           this.Children[1].Raycast(orig, dir, out dist, out normal);
                }

                if (orig.y < this.Min.y)
                {
                    return false;
                }

                return this.Children[3].Raycast(orig, dir, out dist, out normal) ||
                       this.Children[2].Raycast(orig, dir, out dist, out normal) ||
                       this.Children[1].Raycast(orig, dir, out dist, out normal) ||
                       this.Children[0].Raycast(orig, dir, out dist, out normal);
            }

            /// <inheritdoc />
            public void DrawGizmos()
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(new Vector3(this.Min.x, 0f, this.Center.y), new Vector3(this.Max.x, 0f, this.Center.y));
                Gizmos.DrawLine(new Vector3(this.Center.x, 0f, this.Min.y), new Vector3(this.Center.x, 0f, this.Max.y));

                foreach (var child in this.Children)
                {
                    child.DrawGizmos();
                }
            }

            /// <inheritdoc />
            public void Invert()
            {
                for (var i = 0; i < 4; i++)
                {
                    this.Children[i].Invert();
                }
            }

            public void Resize(Vector2 min, Vector2 size)
            {
                Debug.Assert(size.x >= 0);
                Debug.Assert(size.y >= 0);

                if (this.Min == min && this.Size == size)
                {
                    return;
                }

                this.Min = min;
                this.Size = size;
                var extend = size / 2f;
                var center = this.Min + extend;

                this.Children[0].Resize(this.Min, extend);
                this.Children[1].Resize(new Vector2(min.x, center.y), extend);
                this.Children[2].Resize(new Vector2(center.x, min.y), extend);
                this.Children[3].Resize(center, extend);
            }

            public void UpdateVertices()
            {
                for (var i = 0; i < 4; i++)
                {
                    if (this.Children[i] is Node childNode)
                    {
                        childNode.UpdateVertices();
                    }
                    else if (this.Children[i] is Leaf leaf)
                    {
                        leaf.UpdateVertex();
                    }
                }
            }
        }

        private class Leaf : INode
        {
            /// <summary>
            /// Which vertex is inside? [BL:1, TL:2, BR:4, TR:8]
            /// </summary>
            public int InsideFlag { get; set; }

            /// <summary>
            /// Positions of the edge intersection in x or y coordinates [min, min + size]. [L:y, T:x, R:y, B:x]
            /// </summary>
            public float[] Positions { get; }

            /// <summary>
            /// Normals of the edge intersection. [L, T, R, B]
            /// </summary>
            public Vector2[] Normals { get; }

            public Vector2 Vertex { get; private set; }

            public Vector2 AltVertex { get; private set; }

            public bool HasAltVertex { get; private set; }

            /// <summary>
            /// If an alternate vertex is being used, determines how the vertex and altvertex are connected to the edges.<br/>
            /// <c>True</c>: Left and Top -> Vertex, Right and Bottom -> AltVertex
            /// <c>False</c>: Left and Bottom -> Vertex, Right and Top -> AltVertex
            /// </summary>
            public bool AltVertexType { get; private set; }

            /// <inheritdoc />
            public Node Parent { get; set; }

            /// <inheritdoc />
            public Vector2 Min { get; private set; }

            /// <inheritdoc />
            public Vector2 Size { get; private set; }
            
            public Vector2 Max => this.Min + this.Size;

            public Vector2 Center => this.Min + this.Size / 2f;

            public Vector2 PosLeft => new Vector2(this.Min.x, this.Positions[0]);

            public Vector2 PosTop => new Vector2(this.Positions[1], this.Max.y);

            public Vector2 PosRight => new Vector2(this.Max.x, this.Positions[2]);

            public Vector2 PosBottom => new Vector2(this.Positions[3], this.Min.y);

            public Vector2 NormLeft
            {
                get => this.Normals[0];
                set => this.Normals[0] = value;
            }

            public Vector2 NormTop
            {
                get => this.Normals[1];
                set => this.Normals[1] = value;
            }

            public Vector2 NormRight
            {
                get => this.Normals[2];
                set => this.Normals[2] = value;
            }

            public Vector2 NormBottom
            {
                get => this.Normals[3];
                set => this.Normals[3] = value;
            }

            public Leaf()
            {
                this.Parent = null;
                this.Min = Vector2.zero;
                this.Size = Vector2.zero;

                this.InsideFlag = 0;
                this.Normals = new Vector2[4];
                this.Positions = new float[4];
            }
             
            public Leaf(Vector2 min, Vector2 size, bool initialInside = false)
            {
                this.Parent = null;
                this.Min = min;
                this.Size = size;

                this.InsideFlag = initialInside ? 0x0F : 0;
                this.Normals = new Vector2[4];
                this.Positions = new float[4];
                this.Vertex = min + size / 2f;
            }

            /// <inheritdoc />
            public bool ContainsPoint(Vector2 point)
            {
                if (this.InsideFlag == 0 || this.InsideFlag == 0x0F)
                {
                    return this.InsideFlag == 0x0F;
                }

                var prevInside = this.IsInside(0);

                for (var i = 0; i < 4; i++)
                {
                    var nextInside = this.IsInside((i + 1) % 4);

                    if (prevInside != nextInside)
                    {
                        var v = this.GetVertexForEdge(i);
                        var p = this.GetPosition(i);

                        var dir = v - p;
                        var pointP = point - p;
                        var side = dir.x * pointP.y - dir.y * pointP.x;

                        if (!prevInside && side > 0f || prevInside && side < 0f)
                        {
                            return false;
                        }
                    }

                    prevInside = nextInside;
                }

                return true;
            }

            /// <inheritdoc />
            public bool ContainsRect(Vector2 start, Vector2 end)
            {
                if (this.InsideFlag == 0 || this.InsideFlag == 0x0F)
                {
                    return this.InsideFlag == 0x0F;
                }

                // Reduce the rect to the part inside this voxel
                start = Vector2.Max(start, this.Min);
                end = Vector2.Min(end, this.Max);

                var center = (start + end) / 2f;
                var prevInside = this.IsInside(0);

                // For every side with inside changes, test whether the center is on the inside and the rect doesn't intersect the vertex edge
                for (var i = 0; i < 4; i++)
                {
                    var nextInside = this.IsInside((i + 1) % 4);

                    if (prevInside != nextInside)
                    {
                        var v = this.GetVertexForEdge(i);
                        var p = this.GetPosition(i);

                        var dir = v - p;
                        var centerP = center - p;
                        var side = dir.x * centerP.y - dir.y * centerP.x;

                        if (!prevInside && side > 0f || prevInside && side < 0f)
                        {
                            return false;
                        }

                        if (Math2D.RayRectMmIntersection(p, dir, start, end, out var t) && t < 1f)
                        {
                            return false;
                        }
                    }

                    prevInside = nextInside;
                }

                return true;
            }

            /// <inheritdoc />
            public bool IntersectsRect(Vector2 start, Vector2 end)
            {
                if (this.InsideFlag == 0 || this.InsideFlag == 0x0F)
                {
                    return this.InsideFlag == 0x0F;
                }

                // Reduce the rect to the part inside this voxel
                start = Vector2.Max(start, this.Min);
                end = Vector2.Min(end, this.Max);

                var center = (start + end) / 2f;
                if (this.ContainsPoint(center))
                {
                    return true;
                }

                var prevInside = this.IsInside(0);

                // For every side with inside changes, test whether the rect intersects the vertex edge
                for (var i = 0; i < 4; i++)
                {
                    var nextInside = this.IsInside((i + 1) % 4);

                    if (prevInside != nextInside)
                    {
                        var v = this.GetVertexForEdge(i);
                        var p = this.GetPosition(i);
                        var dir = v - p;

                        if (Math2D.RayRectMmIntersection(p, dir, start, end, out var t) && t < 1f)
                        {
                            return true;
                        }
                    }

                    prevInside = nextInside;
                }

                return false;
            }

            /// <inheritdoc />
            public bool Raycast(Vector2 orig, Vector2 dir, out float dist, out Vector2 normal)
            {
                dist = float.PositiveInfinity;
                normal = Vector2.zero;

                if (this.InsideFlag == 0 || this.InsideFlag == 0x0F)
                {
                    return false;
                }

                var prevInside = this.IsInside(0);

                // For every side with inside changes, test whether the ray intersects the vertex edge
                for (var i = 0; i < 4; i++)
                {
                    var nextInside = this.IsInside((i + 1) % 4);

                    if (prevInside != nextInside)
                    {
                        var v = this.GetVertexForEdge(i);
                        var p = this.GetPosition(i);
                        var vDir = v - p;

                        if (Math2D.LineLineIntersection(orig, dir, v, vDir, out var t0, out var t1))
                        {
                            if (0 <= t0 && t0 < dist && 0 <= t1 && t1 <= 1)
                            {
                                dist = t0;
                                normal = prevInside ? new Vector2(-vDir.y, vDir.x) : new Vector2(vDir.y, -vDir.x);
                            }
                        }
                    }

                    prevInside = nextInside;
                }

                return !float.IsPositiveInfinity(dist);
            }

            public Vector2 GetVertexForEdge(int index)
            {
                switch (index)
                {
                    case 0: return this.Vertex;
                    case 1: return this.HasAltVertex && !this.AltVertexType ? this.AltVertex : this.Vertex;
                    case 2: return this.HasAltVertex ? this.AltVertex : this.Vertex;
                    case 3: return this.HasAltVertex && this.AltVertexType ? this.AltVertex : this.Vertex;
                    default:
                        throw new Exception("Internal error in Leaf.ContainsRect(), index out of bounds.");
                }
            }

            public Vector2 GetPosition(int index)
            {
                switch (index)
                {
                    case 0:
                        return this.PosLeft;
                    case 1:
                        return this.PosTop;
                    case 2:
                        return this.PosRight;
                    case 3:
                        return this.PosBottom;
                    default:
                        throw new Exception("Internal error in Leaf.GetPosition(), index out of bounds.");
                }
            }

            /// <inheritdoc />
            public void Invert()
            {
                this.InsideFlag = ~this.InsideFlag & 0x0F;

                for (var i = 0; i < 4; i++)
                {
                    this.Normals[i] = -this.Normals[i];
                }
            }

            public void SetNormal(int index, Vector2 normal)
            {
                switch (index)
                {
                    case 0:
                        this.NormLeft = normal;
                        break;
                    case 1:
                        this.NormTop = normal;
                        break;
                    case 2:
                        this.NormRight = normal;
                        break;
                    case 3:
                        this.NormBottom = normal;
                        break;
                    default:
                        throw new Exception("Internal error in Leaf.GetNormal(), index out of bounds.");
                }
            }

            public bool IsInside(int index)
            {
                Debug.Assert(index >= 0 && index < 4);
                return (this.InsideFlag & (1 << index)) != 0;
            }

            public void SetInside(int index, bool value)
            {
                Debug.Assert(index >= 0 && index < 4);

                if (value)
                {
                    this.InsideFlag |= 1 << index;
                }
                else
                {
                    this.InsideFlag &= ~(1 << index);
                }
            }
            
            /// <inheritdoc />
            public void DrawGizmos()
            {
                var blIn = this.IsInside(0);
                var tlIn = this.IsInside(1);
                var brIn = this.IsInside(2);
                var trIn = this.IsInside(3);

                Gizmos.color = Color.blue;

                if (blIn != tlIn)
                {
                    var point = this.PosLeft;
                    var tan = point + this.NormLeft * LeafSize * 0.1f;
                    Gizmos.DrawLine(new Vector3(point.x, 0f, point.y), new Vector3(tan.x, 0f, tan.y));
                }

                if (tlIn != trIn)
                {
                    var point = this.PosTop;
                    var tan = point + this.NormLeft * LeafSize * 0.1f;
                    Gizmos.DrawLine(new Vector3(point.x, 0f, point.y), new Vector3(tan.x, 0f, tan.y));
                }

                if (trIn != brIn)
                {
                    var point = this.PosRight;
                    var tan = point - this.NormLeft * LeafSize * 0.1f;
                    Gizmos.DrawLine(new Vector3(point.x, 0f, point.y), new Vector3(tan.x, 0f, tan.y));
                }

                if (brIn != blIn)
                {
                    var point = this.PosBottom;
                    var tan = point - this.NormLeft * LeafSize * 0.1f;
                    Gizmos.DrawLine(new Vector3(point.x, 0f, point.y), new Vector3(tan.x, 0f, tan.y));
                }

                var baseSize = 0.06f;
                Gizmos.color = blIn ? Color.green : Color.red;
                Gizmos.DrawWireCube(new Vector3(this.Min.x, 0f, this.Min.y), Vector3.one * LeafSize * baseSize);

                Gizmos.color = tlIn ? Color.green : Color.red;
                Gizmos.DrawWireCube(new Vector3(this.Min.x, 0f, this.Max.y), Vector3.one * LeafSize * baseSize * 1.1f);

                Gizmos.color = brIn ? Color.green : Color.red;
                Gizmos.DrawWireCube(new Vector3(this.Max.x, 0f, this.Min.y), Vector3.one * LeafSize * baseSize * 1.2f);

                Gizmos.color = trIn ? Color.green : Color.red;
                Gizmos.DrawWireCube(new Vector3(this.Max.x, 0f, this.Max.y), Vector3.one * LeafSize * baseSize * 1.3f);

                if (this.InsideFlag != 0)
                {
                    var size = this.InsideFlag != 0x0F ? LeafSize * 0.15f : LeafSize * 0.07f;
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireSphere(new Vector3(this.Vertex.x, 0f, this.Vertex.y), size);

                    if (this.HasAltVertex)
                    {
                        Gizmos.DrawWireSphere(new Vector3(this.AltVertex.x, 0f, this.AltVertex.y), size); 
                    }
                }
            }

            /// <inheritdoc />
            public void Resize(Vector2 min, Vector2 size)
            {
                Debug.Assert(size.x >= 0);
                Debug.Assert(size.y >= 0);

                this.Min = min;
                this.Size = size;
            }

            public void UpdateVertex()
            {
                this.HasAltVertex = false;

                switch (this.InsideFlag)
                {
                    case 0:     // ¬BL ¬TL ¬BR ¬TR  (No vertex)
                        this.Vertex = this.Center;
                        break;
                    case 1:     //  BL ¬TL ¬BR ¬TR
                        this.Vertex = this.GetVertex(this.PosLeft, this.NormLeft, this.PosBottom, this.NormBottom);
                        break;
                    case 2:     // ¬BL  TL ¬BR ¬TR 
                        this.Vertex = this.GetVertex(this.PosLeft, this.NormLeft, this.PosTop, this.NormTop);
                        break;
                    case 3:     //  BL  TL ¬BR ¬TR
                        this.Vertex = this.GetVertex(this.PosTop, this.NormTop, this.PosBottom, this.NormBottom);
                        break;
                    case 4:     // ¬BL ¬TL  BR ¬TR
                        this.Vertex = this.GetVertex(this.PosRight, this.NormRight, this.PosBottom, this.NormBottom);
                        break;
                    case 5:     //  BL ¬TL  BR ¬TR
                        this.Vertex = this.GetVertex(this.PosLeft, this.NormLeft, this.PosRight, this.NormRight);
                        break;
                    case 6:     // ¬BL  TL  BR ¬TR
                        this.GetVertexAll();
                        break;
                    case 7:     //  BL  TL  BR ¬TR
                        this.Vertex = this.GetVertex(this.PosTop, this.NormTop, this.PosRight, this.NormRight);
                        break;
                    case 8:     // ¬BL ¬TL ¬BR  TR
                        this.Vertex = this.GetVertex(this.PosTop, this.NormTop, this.PosRight, this.NormRight);
                        break;
                    case 9:     //  BL ¬TL ¬BR  TR
                        this.GetVertexAll();
                        break;
                    case 10:    // ¬BL  TL ¬BR  TR
                        this.Vertex = this.GetVertex(this.PosLeft, this.NormLeft, this.PosRight, this.NormRight);
                        break;
                    case 11:    //  BL  TL ¬BR  TR
                        this.Vertex = this.GetVertex(this.PosRight, this.NormRight, this.PosBottom, this.NormBottom);
                        break;
                    case 12:    // ¬BL ¬TL  BR  TR
                        this.Vertex = this.GetVertex(this.PosTop, this.NormTop, this.PosBottom, this.NormBottom);
                        break;
                    case 13:    //  BL ¬TL  BR  TR
                        this.Vertex = this.GetVertex(this.PosLeft, this.NormLeft, this.PosTop, this.NormTop);
                        break;
                    case 14:    // ¬BL  TL  BR  TR
                        this.Vertex = this.GetVertex(this.PosLeft, this.NormLeft, this.PosBottom, this.NormBottom);
                        break;
                    case 15:    //  BL  TL  BR  TR  (No vertex)
                        this.Vertex = this.Center;
                        break;
                }
            }

            private Vector2 GetVertex(Vector2 p0, Vector2 n0, Vector2 p1, Vector2 n1)
            {
                var v = GetCrossingPoint(p0, n0, p1, n1);

                // If v is outside bounds, put it in the middle of the points
                if (!IsInBounds(v, this.Min, this.Max))
                {
                    v = v.ClampComponents(this.Min, this.Max);
                }

                return v;
            }

            private static Vector2 GetCrossingPoint(Vector2 p0, Vector2 n0, Vector2 p1, Vector2 n1)
            {
                var cross = n0.x * n1.y - n0.y * n1.x;

                // If normals are nearly collinear
                if (Mathf.Abs(cross) < 1e-2f)
                {
                    return (p0 + p1) / 2f;
                }

                var pn0x = p0.x * n0.x;
                var pn0y = p0.y * n0.y;
                var pn1x = p1.x * n1.x;
                var pn1y = p1.y * n1.y;

                // For num stability, divide by the larger value
                if (Mathf.Abs(n0.x) > Mathf.Abs(n1.x))
                {
                    var vy = (pn1y + pn1x - n1.x * (pn0x + pn0y) / n0.x) / (n1.y - (n1.x * n0.y) / n0.x);
                    var vx = (pn0x + pn0y - n0.y * vy) / n0.x;
                    return new Vector2(vx, vy);
                }
                else
                {
                    var vy = (pn0y + pn0x - n0.x * (pn1x + pn1y) / n1.x) / (n0.y - (n0.x * n1.y) / n1.x);
                    var vx = (pn1x + pn1y - n1.y * vy) / n1.x;
                    return new Vector2(vx, vy);
                }
            }

            private static bool IsInBounds(Vector2 v, Vector2 min, Vector2 max)
            {
                return v.x >= min.x && v.y >= min.y && v.x <= max.x && v.y <= max.y;
            }

            private void GetVertexAll()
            {
                var pl = this.PosLeft;
                var pt = this.PosTop;
                var pr = this.PosRight;
                var pb = this.PosBottom;

                var nl = this.NormLeft;
                var nt = this.NormTop;
                var nr = this.NormRight;
                var nb = this.NormBottom;

                // Get all possible vertices
                var vBl = GetCrossingPoint(pb, nb, pl, nl);
                var vTl = GetCrossingPoint(pt, nt, pl, nl);
                var vTr = GetCrossingPoint(pt, nt, pr, nr);
                var vBr = GetCrossingPoint(pb, nb, pr, nr);

                // Try to decide based on intersection
                Math2D.LineLineIntersection(pl, vBl - pl, pt, vTr - pt, out var tPlVbl);
                Math2D.LineLineIntersection(pb, vBl - pb, pr, vTr - pr, out var tPbVTr);

                if (tPlVbl > 0f && tPlVbl < 1f || tPbVTr > 0f && tPbVTr < 1f)
                {
                    this.Vertex = vTl;
                    this.AltVertex = vBr;
                    this.HasAltVertex = true;
                    this.AltVertexType = true;
                    return;
                }

                Math2D.LineLineIntersection(pl, vTl - pl, pb, vBr - pb, out var tPlVTl);
                Math2D.LineLineIntersection(pt, vTl - pt, pr, vBr - pr, out var tPbVBr);

                if (tPlVTl > 0f && tPlVTl < 1f || tPbVBr > 0f && tPbVBr < 1f)
                {
                    this.Vertex = vBl;
                    this.AltVertex = vTr;
                    this.HasAltVertex = true;
                    this.AltVertexType = false;
                    return;
                }

                // Try to decide based on out of bounds
                var min = this.Min;
                var max = this.Max;

                var inBoundsBl = IsInBounds(vBl, min, max);
                var inBoundsTl = IsInBounds(vTl, min, max);
                var inBoundsTr = IsInBounds(vTr, min, max);
                var inBoundsBr = IsInBounds(vBr, min, max);
                
                if (inBoundsBl && inBoundsTr && !(inBoundsTl && inBoundsBr))
                {
                    this.Vertex = vBl;
                    this.AltVertex = vTr;
                    this.HasAltVertex = true;
                    this.AltVertexType = true;
                    return;
                }
                
                if (inBoundsTl && inBoundsBr && !(inBoundsBl && inBoundsTr))
                {
                    this.Vertex = vTl;
                    this.AltVertex = vBr;
                    this.HasAltVertex = true;
                    this.AltVertexType = false;
                    return;
                }

                // Simply take a default selection
                this.Vertex = inBoundsBl ? vBl : (pl + pb) / 2f;
                this.AltVertex = inBoundsTr ? vTr : (pt + pr) / 2f;
                this.HasAltVertex = true;
                this.AltVertexType = true;
            }
        }
    }
}
