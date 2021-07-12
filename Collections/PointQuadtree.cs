// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PointOctree.cs
// 
// Created:          21.01.2020  18:42
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
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityTools.Core;
using UnityTools.Pooling;
using Debug = UnityEngine.Debug;

namespace UnityTools.Collections
{
    public class PointQuadtree<T> : IPoint2DCollection<T>
    {
        /// <summary>
        /// The index offset for the first child index
        /// </summary>
        private const int ChildIndexOffset = 2;

        /// <summary>
        /// The default capacity of a leaf content array
        /// </summary>
        private const int DefaultLeafCapacity = 16;

        /// <summary>
        /// The initial array size for the leafs container.
        /// </summary>
        private const int InitialLeafCapacity = 64;

        /// <summary>
        /// The initial array size for the nodes container.
        /// </summary>
        private const int InitialNodeCapacity = 16;

        /// <summary>
        /// The amount of different leaf content array sizes that are cached
        /// </summary>
        private const int LeafCacheHierarchySize = 5;

        /// <summary>
        /// The maximum depth of the tree before simply expanding leaf capacity on adding items
        /// </summary>
        private const int MaxDepth = 16;

        /// <summary>
        /// The threshold of node items before a node gets collapsed into a leaf
        /// </summary>
        private const int NodeCollapseCount = 12;

        /// <summary>
        /// The amount of values per node
        /// </summary>
        private const int NodeSize = 6;

        /// <summary>
        /// Contains cached cast path arrays for inclusion and exclusion casts
        /// </summary>
        private static readonly ConcurrentBag<CastPathEntry[]> CastPathCache;

        /// <summary>
        /// Contains cached leaf arrays of increasing size (ContentCache[i].Length = defaultLeafCapacity^i; 0 <= i < defaultCacheHierarchySize)
        /// </summary>
        private static readonly ConcurrentBag<ItemEntry[]>[] ContentCache;

        /// <summary>
        /// Field indicating whether duplicate elements are detected and duplicate entries are prevented when adding or moving items (Two item entries are duplicates if item and position are equal)
        /// </summary>
        private readonly bool allowDuplicates;

        private int leafCount;

        /// <summary>
        /// Contains the leaf containers
        /// </summary>
        private QuadtreeLeaf[] leafs;

        private Vector2 min, size;

        private int nodeCount;

        /// <summary>
        /// Contains nodes as sequence of their values, every node starts at index * NodeSize. (index): Parent; (index+1): Item count; (index + [2, NodeSize-1]): Child index
        /// </summary>
        private int[] nodes;

        private int version;

        static PointQuadtree()
        {
            ContentCache = new ConcurrentBag<ItemEntry[]>[LeafCacheHierarchySize];
            CastPathCache = new ConcurrentBag<CastPathEntry[]>();

            for (var i = 0; i < LeafCacheHierarchySize; i++)
            {
                ContentCache[i] = new ConcurrentBag<ItemEntry[]>();
            }
        }

        public PointQuadtree(Vector2 center, Vector2 size, bool allowDuplicates = false)
        {
            this.nodes = new int[InitialNodeCapacity * NodeSize];
            this.leafs = new QuadtreeLeaf[InitialLeafCapacity];

            this.allowDuplicates = allowDuplicates;
            this.nodeCount = 1;
            this.leafCount = 0;
            this.version = 0;

            this.size = size;
            this.min = center - size / 2f;

            for (var i = 0; i < NodeSize; i++)
            {
                this.nodes[i] = 0;
            }
        }

        public PointQuadtree(Vector2 center, Vector2 size, int capacity, bool allowDuplicates = false)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            var minLeafsRequired = capacity / DefaultLeafCapacity + 1;

            this.nodes = new int[minLeafsRequired * NodeSize];
            this.leafs = new QuadtreeLeaf[minLeafsRequired];

            this.allowDuplicates = allowDuplicates;
            this.nodeCount = 1;
            this.leafCount = 0;
            this.version = 0;

            this.size = size;
            this.min = center - size / 2f;

            for (var i = 0; i < NodeSize; i++)
            {
                this.nodes[i] = 0;
            }
        }

        /// <inheritdoc/>
        public bool AllowsDuplicates => this.allowDuplicates;

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            var startVersion = this.version;

            for (var i = 0; i < this.leafCount; i++)
            {
                var leaf = this.leafs[i];
                for(var j = 0; j < leaf.Count; j++)
                {
                    if (startVersion != this.version)
                    {
                        throw new InvalidOperationException("The octree was modified after the enumerator was created.");
                    }

                    yield return leaf.Content[j].item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public int Count => this.nodes[1];

        /// <inheritdoc/>
        public void Add(in T item, in Vector2 position)
        {
            if (position.x < this.min.x || position.y < this.min.y ||
                position.x > this.min.x + this.size.x || position.y > this.min.y + this.size.y )
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            this.version++;

            this.AddToNode(in item, in position, 0, 0, this.min, this.size / 2f, !this.allowDuplicates);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.version++;

            for (var i = 0; i < this.leafCount; i++)
            {
                this.CacheContentArray(this.leafs[i].Content);
            }

            this.nodeCount = 1;
            this.leafCount = 0;

            for(var i = 0; i < NodeSize; i++)
            {
                this.nodes[i] = 0;
            }
        }

        /// <inheritdoc/>
        public bool Contains(in T item, in Vector2 position)
        {
            if (position.x < this.min.x || position.y < this.min.y ||
                position.x > this.min.x + this.size.x || position.y > this.min.y + this.size.y)
            {
                return false;
            }

            var index = 0;
            var halfSize = this.size / 2f;
            var localMin = this.min;

            while (true)
            {
                var childIndex = 0;
                if (position.x > localMin.x + halfSize.x)
                {
                    childIndex += 2;
                    localMin.x += halfSize.x;
                }

                if (position.y > localMin.y + halfSize.y)
                {
                    childIndex += 1;
                    localMin.y += halfSize.y;
                }

                var nextIndex = this.nodes[index + childIndex + ChildIndexOffset]; 

                if (nextIndex == 0)
                {
                    return false;
                } 

                if (nextIndex > 0)
                {
                    // Prepare for going one layer deeper
                    halfSize /= 2f;

                    // The node has a sub node for that area, restart search for that sub node
                    index = nextIndex;
                    continue;
                }

                // The node has a leaf for that area
                var leafIndex = -(nextIndex + 1);
                ref var leaf = ref this.leafs[leafIndex];

                // Test if the item is in the leaf
                for (var i = 0; i < leaf.Count; i++)
                {
                    if (leaf.Content[i].Position == position && Equals(leaf.Content[i].item, item))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<T> ShapeCast<TShape>(TShape shape) where TShape : IArea
        {
            CastPathEntry[] path;

            if (!CastPathCache.TryTake(out path))
            {
                path = new CastPathEntry[MaxDepth + 1];
            }

            var startVersion = this.version;
            var pathDepth = 0;
            var localMin = this.min;
            var halfSize = this.size / 2f;

            var index = 0;
            var childIndex = ChildIndexOffset - 1;
            var fullyInside = shape.ContainsRect(this.min, this.min + this.size);

            while (pathDepth >= 0)
            {
                childIndex++;

                if (index < 0)
                {
                    var leaf = this.leafs[-(index + 1)];
                    for (var i = 0; i < leaf.Count; i++)
                    {
                        if (fullyInside || shape.ContainsPoint(leaf.Content[i].Position))
                        {
                            if (startVersion != this.version)
                            {
                                CastPathCache.Add(path);
                                throw new InvalidOperationException("The enumerator has been modified since the last step and cannot be used anymore.");
                            }

                            yield return leaf.Content[i].item;
                        }
                    }
                }
                else
                {
                    if (childIndex < NodeSize)
                    {
                        if (this.nodes[index + childIndex] == 0)
                        {
                            // Child is empty
                            continue;
                        }

                        var isInside = fullyInside;
                        var childFullyInside = fullyInside;
                        var childMin = localMin;

                        if (!fullyInside)
                        {
                            // Child aabb only matters if it is not fully inside, otherwise we won't need it for sub nodes
                            var quadrantIndex = childIndex - ChildIndexOffset;

                            if ((quadrantIndex & 2) == 2)
                            {
                                childMin.x += halfSize.x;
                            }

                            if ((quadrantIndex & 1) == 1)
                            {
                                childMin.y += halfSize.y;
                            }

                            var childMax = childMin + halfSize;
                            isInside = shape.IntersectsRect(childMin, childMax);

                            if (isInside)
                            {
                                childFullyInside = shape.ContainsRect(childMin, childMax);
                            }
                        }

                        if (isInside)
                        {
                            path[pathDepth] = new CastPathEntry(index, childIndex, fullyInside, localMin, halfSize);
                            pathDepth++;
                            index = this.nodes[index + childIndex];
                            childIndex = ChildIndexOffset - 1;
                            fullyInside = childFullyInside;
                            localMin = childMin;
                            halfSize /= 2f;
                        }

                        continue;
                    }
                }

                // Go one layer up
                pathDepth--;
                if (pathDepth < 0) break;

                var c = path[pathDepth];
                index = c.Index;
                childIndex = c.ChildIndex;
                fullyInside = c.Flag;
                localMin = c.LocalMin;
                halfSize = c.HalfSize;
            }

            CastPathCache.Add(path);
        }

        public void ShapeCast<TShape>(in TShape shape, IList<T> output) where TShape : IArea
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!CastPathCache.TryTake(out var path))
            {
                path = new CastPathEntry[MaxDepth + 1];
            }

            var pathDepth = 0;
            var localMin = this.min;
            var halfSize = this.size / 2f;

            var index = 0;
            var childIndex = ChildIndexOffset - 1;
            var fullyInside = shape.ContainsRect(in this.min, this.min + this.size);

            while (pathDepth >= 0)
            {
                childIndex++;

                if (index < 0)
                {
                    ref var leaf = ref this.leafs[-(index + 1)];

                    for (var i = 0; i < leaf.Count; i++)
                    {
                        ref var item = ref leaf.Content[i];
                        
                        if (fullyInside || shape.ContainsPoint(in item.Position))
                        {
                            output.Add(item.item);
                        }
                    }
                }
                else
                {
                    if (childIndex < NodeSize)
                    {
                        if (this.nodes[index + childIndex] == 0)
                        {
                            // Child is empty
                            continue;
                        }

                        var isInside = fullyInside;
                        var childFullyInside = fullyInside;
                        var childMin = localMin;

                        if (!fullyInside)
                        {
                            // Child aabb only matters if it is not fully inside, otherwise we won't need it for sub nodes
                            var quadrantIndex = childIndex - ChildIndexOffset;

                            if ((quadrantIndex & 2) == 2)
                            {
                                childMin.x += halfSize.x;
                            }

                            if ((quadrantIndex & 1) == 1)
                            {
                                childMin.y += halfSize.y;
                            }

                            var childMax = childMin + halfSize;
                            isInside = shape.IntersectsRect(in childMin, in childMax);

                            if (isInside)
                            {
                                childFullyInside = shape.ContainsRect(in childMin, in childMax);
                            }
                        }

                        if (isInside)
                        {
                            path[pathDepth] = new CastPathEntry(in index, in childIndex, in fullyInside, in localMin, in halfSize);
                            pathDepth++;
                            index = this.nodes[index + childIndex];
                            childIndex = ChildIndexOffset - 1;
                            fullyInside = childFullyInside;
                            localMin = childMin;
                            halfSize /= 2f;
                        }

                        continue;
                    }
                }

                // Go one layer up
                pathDepth--;
                if (pathDepth < 0) break;

                ref var c = ref path[pathDepth];
                index = c.Index;
                childIndex = c.ChildIndex;
                fullyInside = c.Flag;
                localMin = c.LocalMin;
                halfSize = c.HalfSize;
            }

            CastPathCache.Add(path);
        }
        
        /// <inheritdoc/>
        public bool MoveItem(in T item, in Vector2 @from, in Vector2 to)
        {
            if (@from.x < this.min.x || @from.y < this.min.y ||
                @from.x > this.min.x + this.size.x || @from.y > this.min.y + this.size.y)
            {
                return false;
            }

            if (to.x < this.min.x || to.y < this.min.y ||
                to.x > this.min.x + this.size.x || to.y > this.min.y + this.size.y )
            {
                return false;
            }

            this.version++;

            var index = 0;
            var halfSize = this.size / 2f;
            var localMin = this.min;

            while (true)
            {
                var childIndex = 0;
                if (@from.x > localMin.x + halfSize.x)
                {
                    childIndex += 2;
                    localMin.x += halfSize.x;
                }

                if (@from.y > localMin.y + halfSize.y)
                {
                    childIndex += 1;
                    localMin.y += halfSize.y;
                }

                var nextIndex = this.nodes[index + childIndex + ChildIndexOffset];

                if (nextIndex == 0)
                {
                    return false; 
                }

                if (nextIndex > 0)
                {
                    // Prepare for going one layer deeper
                    halfSize /= 2f;

                    // The node has a sub node for that area, restart search for that sub node
                    index = nextIndex;
                    continue;
                }

                // The node has a leaf for that area
                var leafIndex = -(nextIndex + 1);
                ref var leaf = ref this.leafs[leafIndex];

                // Test if the item is in the leaf
                for (var i = 0; i < leaf.Count; i++)
                {
                    if (leaf.Content[i].Position == @from && Equals(leaf.Content[i].item, item))
                    {
                        if (to.x < localMin.x || to.y < localMin.y||
                            to.x > localMin.x + halfSize.x || to.y > localMin.y + halfSize.y)
                        {
                            // The target area is not in this leaf, remove item
                            if (leaf.Count > 1)
                            {
                                if (i < leaf.Count - 1)
                                {
                                    // If not last item, replace with last to prevent empty spots
                                    leaf.Content[i] = leaf.Content[leaf.Count - 1];
                                }

                                leaf.Content[leaf.Count - 1] = default;
                                this.leafs[leafIndex] = new QuadtreeLeaf(in leaf.Parent, leaf.Content, leaf.Count - 1);
                            }
                            else
                            {
                                var content = leaf.Content;
                                this.RemoveLeaf(in leafIndex);
                                this.CacheContentArray(content);
                                this.nodes[index + childIndex + ChildIndexOffset] = 0;
                            }

                            // Remove the item from the item count of all containing nodes
                            var removalIndex = index;
                            this.nodes[removalIndex + 1]--;
                            while (removalIndex != 0)
                            {
                                removalIndex = this.nodes[removalIndex];
                                this.nodes[removalIndex + 1]--;
                            }

                            if (this.nodes[index + 1] < NodeCollapseCount)
                            {
                                this.CollapseNode(in index); 
                            }

                            // Add item again with the new position
                            this.AddToNode(in item, in to, 0, 0, this.min, this.size / 2f, !this.allowDuplicates);
                            return true;
                        }

                        leaf.Content[i] = new ItemEntry(in item, in to);
                        return true;
                    }
                }

                return false;
            }
        }

        /// <inheritdoc/>
        public bool Remove(in T item, in Vector2 position)
        {
            if (position.x < this.min.x || position.y < this.min.y ||
                position.x > this.min.x + this.size.x || position.y > this.min.y + this.size.y)
            {
                return false;
            }

            this.version++;

            var index = 0;
            var halfSize = this.size / 2f;
            var localMin = this.min;

            while (true)
            {
                var childIndex = 0;
                if (position.x > localMin.x + halfSize.x)
                {
                    childIndex += 1;
                    localMin.x += halfSize.x;
                }

                if (position.y > localMin.y + halfSize.y)
                {
                    childIndex += 1;
                    localMin.y += halfSize.y;
                }

                var nextIndex = this.nodes[index + childIndex + ChildIndexOffset];

                if (nextIndex == 0)
                {
                    return false;
                }

                if (nextIndex > 0)
                {
                    // Prepare for going one layer deeper
                    halfSize /= 2f;

                    // The node has a sub node for that area, restart search for that sub node
                    index = nextIndex;
                    continue;
                }

                // The node has a leaf for that area
                var leafIndex = -(nextIndex + 1);
                ref var leaf = ref this.leafs[leafIndex];

                // Test if the item is in the leaf
                for (var i = 0; i < leaf.Count; i++)
                {
                    if (leaf.Content[i].Position == position && Equals(leaf.Content[i].item, item))
                    {
                        if (leaf.Count > 1)
                        {
                            if (i < leaf.Count - 1)
                            {
                                // If not last item, replace with last to prevent empty spots
                                leaf.Content[i] = leaf.Content[leaf.Count - 1];
                            }

                            leaf.Content[leaf.Count - 1] = default;
                            this.leafs[leafIndex] = new QuadtreeLeaf(leaf.Parent, leaf.Content, leaf.Count - 1);
                        }
                        else
                        {
                            var content = leaf.Content;
                            this.RemoveLeaf(in leafIndex);
                            this.CacheContentArray(content);
                            this.nodes[index + childIndex + ChildIndexOffset] = 0;
                        }

                        // Remove the item from the item count of all containing nodes
                        var removalIndex = index;
                        this.nodes[removalIndex + 1]--;
                        while (removalIndex != 0)
                        {
                            removalIndex = this.nodes[removalIndex];
                            this.nodes[removalIndex + 1]--;
                        }

                        if (this.nodes[index + 1] < NodeCollapseCount)
                        {
                            this.CollapseNode(in index);
                        }

                        return true;
                    }
                }

                return false;
            }
        }

        public void TransformTo(in Vector2 center, in Vector2 size)
        {
            var newMin = center - size / 2f;

            if (this.Count == 0)
            {
                this.min = newMin;
                this.size = size;
                return;
            }

            if (this.min == newMin && this.size == size)
            {
                return;
            }

            var items = ConcurrentGlobalListPool<ItemEntry>.Get(this.Count);
            
            for (var i = 0; i < this.leafCount; i++)
            {
                ref var leaf = ref this.leafs[i];
                for (var j = 0; j < leaf.Count; j++)
                {
                    ref var entry = ref leaf.Content[j];
                    items.Add(entry);
                }
            }

            this.Clear();

            this.min = newMin;
            this.size = size;

            foreach (var item in items)
            {
                this.Add(in item.item, in item.Position);
            }

            ConcurrentGlobalListPool<ItemEntry>.Put(items);
        }

        /// <summary>
        /// Editor only functionality to verify implementation.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public void TestIntegrity()
        {
#if UNITY_EDITOR
            var untestedNodes = this.nodeCount - 1;
            var untestedLeafs = this.leafCount;
            var usedContentArrays = new HashSet<ItemEntry[]>();

            // Test that all nodes and leafes are reachable from the root node
            var stack = new Stack<int>();
            stack.Push(0);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                for(var i = ChildIndexOffset; i < NodeSize; i++)
                {
                    var reference = this.nodes[current + i];

                    if (reference > 0)
                    {
                        Debug.Assert(reference < this.nodeCount * NodeSize, "Child node index is out of bounds.");
                        Debug.Assert(this.nodes[reference] == current, "Parent index of child node doesn't match with actual index of parent.");
                        stack.Push(reference);
                        untestedNodes--;
                    }
                    else if (reference < 0)
                    {
                        var leafIndex = -(reference + 1);
                        Debug.Assert(leafIndex < this.leafCount, "Child leaf index is out of bounds.");
                        Debug.Assert(this.leafs[leafIndex].Parent == current,
                            "Parent index of child leaf doesn't match with actual index of parent.");
                        Debug.Assert(this.leafs[leafIndex].Count > 0, "Empty leaf.");
                        Debug.Assert(this.leafs[leafIndex].Content != null);
                        Debug.Assert(this.leafs[leafIndex].Content.Length >= this.leafs[leafIndex].Count,
                            "Leaf children out of bounds.");
                        Debug.Assert(Mathf.IsPowerOfTwo(this.leafs[leafIndex].Content.Length / InitialNodeCapacity),
                            "Leaf content array has the wrong size.");
                        Debug.Assert(!usedContentArrays.Contains(this.leafs[leafIndex].Content),
                            "The leaf content array is already used by another leaf.");
                        usedContentArrays.Add(this.leafs[leafIndex].Content);
                        untestedLeafs--;
                    }
                }
            }

            Debug.Assert(untestedNodes == 0, "There exist unreferenced nodes.");
            Debug.Assert(untestedLeafs == 0, "There exist unreferenced leafs.");
            Debug.Assert(usedContentArrays.Count == this.leafCount);
#endif
        }

        private int AddLeaf(in int parent, in ItemEntry item)
        {
            Debug.Assert(parent >= 0);
            Debug.Assert(parent % NodeSize == 0);
            Debug.Assert(parent < this.nodeCount * NodeSize);
            Debug.Assert(this.leafCount >= 0);
            Debug.Assert(this.leafCount <= this.leafs.Length);

            if (this.leafCount == this.leafs.Length)
            {
                // Grow the array to accomodate more leafs
                var newLeafs = new QuadtreeLeaf[this.leafs.Length * 2];
                this.leafs.CopyTo(newLeafs, 0);
                this.leafs = newLeafs;
            }


            if (!ContentCache[0].TryTake(out var leafContent))
            {
                leafContent = new ItemEntry[DefaultLeafCapacity];
            }

            leafContent[0] = item;
            this.leafs[this.leafCount] = new QuadtreeLeaf(in parent, leafContent, 1);
            this.leafCount++;

            return -this.leafCount;  // Leafs have negative indexing starting at -1
        }

        private int AddLeaf(in int parent, List<ItemEntry> items)
        {
            Debug.Assert(parent >= 0);
            Debug.Assert(parent % NodeSize == 0);
            Debug.Assert(parent < this.nodeCount * NodeSize);
            Debug.Assert(this.leafCount >= 0);
            Debug.Assert(this.leafCount <= this.leafs.Length);

            if (this.leafCount == this.leafs.Length)
            {
                // Grow the array to accomodate more leafs
                var newLeafs = new QuadtreeLeaf[this.leafs.Length * 2];
                this.leafs.CopyTo(newLeafs, 0);
                this.leafs = newLeafs;
            }
            
            var cacheIndex = 0;
            var cacheSize = InitialNodeCapacity;
            while (cacheSize < items.Count)
            {
                cacheSize *= 2;
                cacheIndex++;
            }

            if (!ContentCache[cacheIndex].TryTake(out var leafContent))
            {
                leafContent = new ItemEntry[cacheSize];
            }

            for (var i = 0; i < items.Count; i++)
            {
                leafContent[i] = items[i];
            }

            this.leafs[this.leafCount] = new QuadtreeLeaf(in parent, leafContent, items.Count);
            this.leafCount++;

            return -this.leafCount;  // Leafs have negative indexing starting at -1
        }

        private int AddNode(in int parent)
        {
            Debug.Assert(parent >= 0);
            Debug.Assert(parent % NodeSize == 0);
            Debug.Assert(parent < this.nodeCount * NodeSize);
            Debug.Assert(this.nodeCount >= 0);
            Debug.Assert(this.nodeCount * NodeSize <= this.nodes.Length);

            var index = this.nodeCount * NodeSize;

            if(index == this.nodes.Length)
            {
                // Grow the array to accomodate more nodes
                var newNodes = new int[this.nodes.Length * 2];
                this.nodes.CopyTo(newNodes, 0);
                this.nodes = newNodes;
            }

            this.nodes[index] = parent;  // The first value is the parent
            this.nodes[index + 1] = 0;   // The new node has 0 items
            for (var i = ChildIndexOffset; i < NodeSize; i++)
            {
                // Initialize child references
                this.nodes[index + i] = 0;
            }

            this.nodeCount++;
            return index;
        }

        private void AddToNode (in T item, in Vector2 position, int index, int depth, Vector2 localMin, Vector2 halfSize, in bool testForDuplicate)
        {
            while (true)
            {
                var childIndex = 0;
                if (position.x > localMin.x + halfSize.x)
                {
                    childIndex += 2;    
                    localMin.x += halfSize.x;
                }

                if (position.y > localMin.y + halfSize.y)
                {
                    childIndex += 1;    
                    localMin.y += halfSize.y;
                }

                var nextIndex = this.nodes[index + childIndex + ChildIndexOffset];
                this.nodes[index + 1]++; // Increase the node child count

                if (nextIndex == 0)
                {
                    // The node doesn't have a sub node or leaf for that area
                    var newLeafIndex = this.AddLeaf(in index, new ItemEntry(in item, in position));
                    this.nodes[index + childIndex + ChildIndexOffset] = newLeafIndex;
                    break;
                }

                if (nextIndex > 0)
                {
                    // Prepare for going one layer deeper
                    halfSize /= 2f;
                    depth++;

                    // The node has a sub node for that area, restart search for that sub node
                    index = nextIndex;
                    continue;
                }

                // The node has a leaf for that area
                var leafIndex = -(nextIndex + 1);
                ref var leaf = ref this.leafs[leafIndex];

                // Make sure the item isn't already part of the octree
                if (testForDuplicate)
                {
                    for (var i = 0; i < leaf.Count; i++)
                    {
                        // If the item is already part of the tree, don't add again
                        if (leaf.Content[i].Position == position && Equals(leaf.Content[i].item, item))
                        {
                            // Revert the item count because the item wasn't actually added
                            this.nodes[index + 1]--;
                            while (index != 0)
                            {
                                index = this.nodes[index];
                                this.nodes[index + 1]--;
                            }

                            return;
                        }
                    }
                }

                // If the leaf has the capacity to store the item, just add it
                if (leaf.Count < leaf.Content.Length)
                {
                    // Add the item to the leaf content and update the leaf count
                    leaf.Content[leaf.Count] = new ItemEntry(in item, in position);
                    this.leafs[leafIndex] = new QuadtreeLeaf(in leaf.Parent, leaf.Content, leaf.Count + 1);
                    break;
                }

                // If we can go deeper, replace the leaf with a node
                if (depth < MaxDepth)
                {
                    // Prepare for going one layer deeper
                    halfSize /= 2f;
                    depth++;

                    var content = leaf.Content;
                    var cnt = leaf.Count;

                    this.RemoveLeaf(in leafIndex);                      // Remove the leaf
                    nextIndex = this.AddNode(in index);                 // Create a new node
                    this.nodes[index + childIndex + ChildIndexOffset] = nextIndex;  // Replace the leaf reference with the node reference
                    index = nextIndex;                          // Continue with the current item

                    for(var i = 0; i < cnt; i++)
                    {
                        ref var contentItem = ref content[i];
                        this.AddToNode(contentItem.item, contentItem.Position, index, depth, localMin, halfSize, false);
                    }

                    this.CacheContentArray(content);
                }
                else
                {
                    // The max depth has been reached, expand the leaf
                    var newContent = this.ExpandContentArray(leaf.Content);
                    newContent[leaf.Count] = new ItemEntry(in item, in position);
                    this.leafs[leafIndex] = new QuadtreeLeaf(in leaf.Parent, newContent, leaf.Count + 1);
                    break;
                }
            }
        }

        private void CacheContentArray(ItemEntry[] array)
        {
            Debug.Assert(array != null);
            Debug.Assert(array.Length % DefaultLeafCapacity == 0);

            // Clean the array to get rid of potential references
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = default;
            }

            var currentCacheSize = DefaultLeafCapacity;
            for (var i = 0; i < LeafCacheHierarchySize; i++)
            {
                // Find cache to which this leaf container belongs
                if (array.Length == currentCacheSize)
                {
                    ContentCache[i].Add(array);
                    break;
                }

                currentCacheSize *= 2;
            }
        }

        private void CollapseNode(in int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < this.nodeCount * NodeSize);
            Debug.Assert(index % NodeSize == 0);
            Debug.Assert(this.nodes[index + 1] < NodeCollapseCount);

            for(var i = ChildIndexOffset; i < NodeSize; i++)
            {
                Debug.Assert(this.nodes[index + i] <= 0, "If a node has few enough items to be collapsed, it can't be that the node contains child nodes.");
            }

            if (index == 0)
            {
                // Don't collapse root
                return;
            }
            
            // Node is bellow collapse threshold -> collapse into leaf
            var itemCache = GlobalListPool<ItemEntry>.Get(this.nodes[index + 1]);

            for (var i = ChildIndexOffset; i < NodeSize; i++)
            {
                var reference = this.nodes[index + i];
                this.nodes[index + i] = 0;

                if (reference < 0)
                {
                    var leafIndex = -(reference + 1);
                    ref var leaf = ref this.leafs[leafIndex];

                    for (var j = 0; j < leaf.Count; j++)
                    {
                        itemCache.Add(leaf.Content[j]);
                    }

                    this.CacheContentArray(leaf.Content);
                    this.RemoveLeaf(in leafIndex);
                }
            }

            var parent = this.nodes[index];
            var newLeafIndex = this.AddLeaf(in parent, itemCache);

            for (var i = ChildIndexOffset; i < NodeSize; i++)
            { 
                if (this.nodes[parent + i] == index)
                {
                    this.nodes[parent + i] = newLeafIndex;
                } 
            }

            this.RemoveNode(in index);
            GlobalListPool<ItemEntry>.Put(itemCache);

            if (parent == this.nodeCount * NodeSize)
            {
                // In case parent was the last node and was used to replace the current node
            }

            if (this.nodes[parent + 1] < NodeCollapseCount)
            {
                this.CollapseNode(in parent);
            }
        }

        private ItemEntry[] ExpandContentArray(ItemEntry[] array)
        {
            Debug.Assert(array != null);
            Debug.Assert(array.Length % DefaultLeafCapacity == 0);

            ItemEntry[] result = null;

            var currentCacheSize = DefaultLeafCapacity;
            for (var i = 0; i < LeafCacheHierarchySize; i++)
            {
                // Find cache to which this leaf container belongs
                if (array.Length == currentCacheSize)
                {
                    ContentCache[i].Add(array);
                    i++;

                    // If there is a cache for larger content arrays
                    if(i < LeafCacheHierarchySize)
                    {
                        ContentCache[i].TryTake(out result);
                    }

                    break;
                }

                currentCacheSize *= 2;
            }

            if (result == null)
            {
                result = new ItemEntry[array.Length * 2];
            }

            array.CopyTo(result, 0);

            // Clean the old array to get rid of potential references
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = default;
            }

            return result;
        }

        private void RemoveLeaf(in int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < this.leafCount);
            Debug.Assert(this.leafs[index].Parent < this.nodeCount * NodeSize);

            this.leafCount--;

            if (index < this.leafCount)
            {
                this.leafs[index] = this.leafs[this.leafCount];
                this.leafs[this.leafCount] = default;

                var parentIndex = this.leafs[index].Parent;
                var leafReference = -(this.leafCount + 1);
                for (var i = ChildIndexOffset; i < NodeSize; i++)
                {
                    if (this.nodes[parentIndex + i] == leafReference)
                    {
                        // Update reference to the moved entry
                        this.nodes[parentIndex + i] = -(index + 1);
                        break;
                    }
                }
            }
            else
            {
                this.leafs[index] = default;
            }
        }

        private void RemoveNode(in int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < this.nodeCount * NodeSize);
            Debug.Assert(index % NodeSize == 0);
            Debug.Assert(index != 0, "Can't remove root node");

            this.nodeCount--;
            var lastIndex = this.nodeCount * NodeSize;
            if (index < lastIndex)
            {
                // Replace with the last node to prevent empty entries
                for (var i = 0; i < NodeSize; i++)
                {
                    this.nodes[index + i] = this.nodes[lastIndex + i];
                }

                // Update child reference of parent
                var parentIndex = this.nodes[index];
                for (var i = ChildIndexOffset; i < NodeSize; i++)
                {
                    if (this.nodes[parentIndex + i] == lastIndex)
                    {
                        // Update reference to the moved entry
                        this.nodes[parentIndex + i] = index;
                        break;
                    }
                }

                // Update parent references of children
                for (var i = ChildIndexOffset; i < NodeSize; i++)
                {
                    var child = this.nodes[index + i];
                    if (child > 0)
                    {
                        this.nodes[child] = index;
                    }
                    else if (child < 0)
                    {
                        var leafIndex = -(child + 1);
                        var leaf = this.leafs[leafIndex];
                        this.leafs[leafIndex] = new QuadtreeLeaf(in index, leaf.Content, in leaf.Count);
                    }
                }
            }
        }

        ~PointQuadtree()
        {
            this.Clear();
        }

        private readonly struct CastPathEntry
        {
            public readonly int Index;

            public readonly int ChildIndex;

            public readonly bool Flag;

            public readonly Vector2 LocalMin;

            public readonly Vector2 HalfSize;

            public CastPathEntry(in int index, in int childIndex, in bool flag, in Vector2 localMin, in Vector2 halfSize)
            {
                this.Index = index;
                this.ChildIndex = childIndex;
                this.Flag = flag;
                this.LocalMin = localMin;
                this.HalfSize = halfSize;
            }
        }

        private readonly struct ItemEntry
        {
            public readonly T item;

            public readonly Vector2 Position;

            public ItemEntry(in T item, in Vector2 position)
            {
                this.item = item;
                this.Position = position;
            }
        }

        private readonly struct QuadtreeLeaf
        {
            public readonly int Parent;

            public readonly ItemEntry[] Content;

            public readonly int Count;

            public QuadtreeLeaf(in int parent, ItemEntry[] content, in int count)
            {
                this.Parent = parent;
                this.Content = content;
                this.Count = count;
            }
        }
    }
}
