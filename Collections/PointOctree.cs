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
using System.Collections.Generic;
using System.Diagnostics;
using Unity_Tools.Core;
using Unity_Tools.Pooling;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Unity_Tools.Collections
{
    public class PointOctree<T> : IPoint3DCollection<T>
    {
        /// <summary>
        /// The index offset for the first child index
        /// </summary>
        private const int ChildIndexOffset = 2;

        /// <summary>
        /// The default capacity of a leaf content array
        /// </summary>
        private const int DefaultLeafCapacity = 64;

        /// <summary>
        /// The initial array size for the leafs container.
        /// </summary>
        private const int InitialLeafCapacity = 64;

        /// <summary>
        /// The initial array size for the nodes container.
        /// </summary>
        private const int InitialNodeCapacity = 64;

        /// <summary>
        /// The amount of different leaf content array sizes that are cached
        /// </summary>
        private const int LeafCacheHierarchySize = 3;

        /// <summary>
        /// The maximum depth of the tree before simply expanding leaf capacity on adding items
        /// </summary>
        private const int MaxDepth = 16;

        /// <summary>
        /// The threshold of node items before a node gets collapsed into a leaf
        /// </summary>
        private const int NodeCollapseCount = 32;

        /// <summary>
        /// The amount of values per node
        /// </summary>
        private const int NodeSize = 10;

        /// <summary>
        /// Contains cached cast path arrays for inclusion and exclusion casts
        /// </summary>
        private static readonly List<CastPathEntry[]> CastPathCache;

        /// <summary>
        /// Contains cached leaf arrays of increasing size (ContentCache[i].Length = defaultLeafCapacity^i; 0 <= i < defaultCacheHierarchySize)
        /// </summary>
        private static readonly List<ItemEntry[]>[] ContentCache;

        /// <summary>
        /// Field indicating whether duplicate elements are detected and duplicate entries are prevented when adding or moving items (Two item entries are duplicates if item and position are equal)
        /// </summary>
        private readonly bool allowDuplicates;

        private int leafCount;

        /// <summary>
        /// Contains the leaf containers
        /// </summary>
        private OctreeLeaf[] leafs;

        private Vector3 min, size;

        private int nodeCount;

        /// <summary>
        /// Contains nodes as sequence of their values, every node starts at index * NodeSize. (index): Parent; (index+1): Item count; (index + [2, NodeSize-1]): Child index
        /// </summary>
        private int[] nodes;

        private int version;

        static PointOctree()
        {
            ContentCache = new List<ItemEntry[]>[LeafCacheHierarchySize];
            CastPathCache = new List<CastPathEntry[]>();

            for (var i = 0; i < LeafCacheHierarchySize; i++)
            {
                ContentCache[i] = new List<ItemEntry[]>();
            }
        }

        public PointOctree(Vector3 center, Vector3 size, bool allowDuplicates = false)
        {
            this.nodes = new int[InitialNodeCapacity * NodeSize];
            this.leafs = new OctreeLeaf[InitialLeafCapacity];

            this.allowDuplicates = allowDuplicates;
            this.nodeCount = 1;
            this.leafCount = 0;
            this.version = 0;

            this.size = size;
            this.min = center - size / 2f;

            for (var i = 0; i < NodeSize; i++)
            {
                nodes[i] = 0;
            }
        }

        public PointOctree(Vector3 center, Vector3 size, int capacity, bool allowDuplicates = false)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            var minLeafsRequired = capacity / DefaultLeafCapacity + 1;

            this.nodes = new int[minLeafsRequired * NodeSize];
            this.leafs = new OctreeLeaf[minLeafsRequired];

            this.allowDuplicates = allowDuplicates;
            this.nodeCount = 1;
            this.leafCount = 0;
            this.version = 0;

            this.size = size;
            this.min = center - size / 2f;

            for (var i = 0; i < NodeSize; i++)
            {
                nodes[i] = 0;
            }
        }

        /// <inheritdoc/>
        public bool AllowsDuplicates => allowDuplicates;

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            var startVersion = version;

            for (var i = 0; i < leafCount; i++)
            {
                var leaf = leafs[i];
                for(var j = 0; j < leaf.Count; j++)
                {
                    if (startVersion != version)
                    {
                        throw new InvalidOperationException("The octree was modified after the enumerator was created.");
                    }

                    yield return leaf.Content[j].item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public int Count => nodes[1];

        /// <inheritdoc/>
        public void Add(T item, Vector3 position)
        {
            if (position.x < min.x || position.y < min.y || position.z < min.z ||
                position.x > min.x + size.x || position.y > min.y + size.y || position.z > min.z + size.z)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            version++;

            AddToNode(item, position, 0, 0, min, size / 2f, !allowDuplicates);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.version++;

            for (var i = 0; i < leafCount; i++)
            {
                CacheContentArray(leafs[i].Content);
            }

            this.nodeCount = 1;
            this.leafCount = 0;

            for(var i = 0; i < NodeSize; i++)
            {
                nodes[i] = 0;
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item, Vector3 position)
        {
            if (position.x < min.x || position.y < min.y || position.z < min.z ||
                position.x > min.x + size.x || position.y > min.y + size.y || position.z > min.z + size.z)
            {
                return false;
            }

            var index = 0;
            var halfSize = size / 2f;
            var localMin = min;

            while (true)
            {
                var childIndex = 0;
                if (position.x > localMin.x + halfSize.x)
                {
                    childIndex += 4;
                    localMin.x += halfSize.x;
                }

                if (position.y > localMin.y + halfSize.y)
                {
                    childIndex += 2;
                    localMin.y += halfSize.y;
                }

                if (position.z > localMin.z + halfSize.z)
                {
                    childIndex += 1;
                    localMin.z += halfSize.z;
                }

                var nextIndex = nodes[index + childIndex + ChildIndexOffset]; 

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
                var leaf = leafs[leafIndex];

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
        public IEnumerable<T> ShapeCast<TShape>(TShape shape) where TShape : IVolume
        {
            if (!CastPathCache.TryExtractLast(out var path))
            {
                path = new CastPathEntry[MaxDepth + 1];
            }

            var startVersion = version;
            var pathDepth = 0;
            var localMin = min;
            var halfSize = size / 2f;

            var index = 0;
            var childIndex = ChildIndexOffset - 1;
            var fullyInside = shape.ContainsAabb(min, min + size);

            while (pathDepth >= 0)
            {
                childIndex++;

                if (index < 0)
                {
                    var leaf = leafs[-(index + 1)];
                    for (var i = 0; i < leaf.Count; i++)
                    {
                        if (fullyInside || shape.ContainsPoint(leaf.Content[i].Position))
                        {
                            if (startVersion != version)
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
                        if (nodes[index + childIndex] == 0)
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

                            if ((quadrantIndex & 4) == 4)
                            {
                                childMin.x += halfSize.x;
                            }

                            if ((quadrantIndex & 2) == 2)
                            {
                                childMin.y += halfSize.y;
                            }

                            if ((quadrantIndex & 1) == 1)
                            {
                                childMin.z += halfSize.z;
                            }

                            var childMax = childMin + halfSize;
                            isInside = shape.IntersectsAabb(childMin, childMax);

                            if (isInside)
                            {
                                childFullyInside = shape.ContainsAabb(childMin, childMax);
                            }
                        }

                        if (isInside)
                        {
                            path[pathDepth] = new CastPathEntry(index, childIndex, fullyInside, localMin, halfSize);
                            pathDepth++;
                            index = nodes[index + childIndex];
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

        /// <inheritdoc/>
        public bool MoveItem(T item, Vector3 @from, Vector3 to)
        {
            if (@from.x < min.x || @from.y < min.y || @from.z < min.z ||
                @from.x > min.x + size.x || @from.y > min.y + size.y || @from.z > min.z + size.z)
            {
                return false;
            }

            if (to.x < min.x || to.y < min.y || to.z < min.z ||
                to.x > min.x + size.x || to.y > min.y + size.y || to.z > min.z + size.z)
            {
                return false;
            }

            this.version++;

            var index = 0;
            var halfSize = size / 2f;
            var localMin = min;

            while (true)
            {
                var childIndex = 0;
                if (@from.x > localMin.x + halfSize.x)
                {
                    childIndex += 4;
                    localMin.x += halfSize.x;
                }

                if (@from.y > localMin.y + halfSize.y)
                {
                    childIndex += 2;
                    localMin.y += halfSize.y;
                }

                if (@from.z > localMin.z + halfSize.z)
                {
                    childIndex += 1;
                    localMin.z += halfSize.z;
                }

                var nextIndex = nodes[index + childIndex + ChildIndexOffset];

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
                var leaf = leafs[leafIndex];

                // Test if the item is in the leaf
                for (var i = 0; i < leaf.Count; i++)
                {
                    if (leaf.Content[i].Position == @from && Equals(leaf.Content[i].item, item))
                    {
                        if (to.x < localMin.x || to.y < localMin.y || to.z < localMin.z ||
                            to.x > localMin.x + halfSize.x || to.y > localMin.y + halfSize.y ||
                            to.z > localMin.z + halfSize.z)
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
                                leafs[leafIndex] = new OctreeLeaf(leaf.Parent, leaf.Content, leaf.Count - 1);
                            }
                            else
                            {
                                var content = leaf.Content;
                                RemoveLeaf(leafIndex);
                                CacheContentArray(content);
                                nodes[index + childIndex + ChildIndexOffset] = 0;
                            }

                            // Remove the item from the item count of all containing nodes
                            var removalIndex = index;
                            nodes[removalIndex + 1]--;
                            while (removalIndex != 0)
                            {
                                removalIndex = nodes[removalIndex];
                                nodes[removalIndex + 1]--;
                            }

                            if (nodes[index + 1] < NodeCollapseCount)
                            {
                                CollapseNode(index); 
                            }

                            // Add item again with the new position
                            AddToNode(item, to, 0, 0, min, size / 2f, !allowDuplicates);
                            return true;
                        }

                        leaf.Content[i] = new ItemEntry(item, to);
                        return true;
                    }
                }

                return false;
            }
        }

        /// <inheritdoc/>
        public bool Remove(T item, Vector3 position)
        {
            if (position.x < min.x || position.y < min.y || position.z < min.z ||
                position.x > min.x + size.x || position.y > min.y + size.y || position.z > min.z + size.z)
            {
                return false;
            }

            this.version++;

            var index = 0;
            var halfSize = size / 2f;
            var localMin = min;

            while (true)
            {
                var childIndex = 0;
                if (position.x > localMin.x + halfSize.x)
                {
                    childIndex += 4;
                    localMin.x += halfSize.x;
                }

                if (position.y > localMin.y + halfSize.y)
                {
                    childIndex += 2;
                    localMin.y += halfSize.y;
                }

                if (position.z > localMin.z + halfSize.z)
                {
                    childIndex += 1;
                    localMin.z += halfSize.z;
                }

                var nextIndex = nodes[index + childIndex + ChildIndexOffset];

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
                var leaf = leafs[leafIndex];

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
                            leafs[leafIndex] = new OctreeLeaf(leaf.Parent, leaf.Content, leaf.Count - 1);
                        }
                        else
                        {
                            var content = leaf.Content;
                            RemoveLeaf(leafIndex);
                            CacheContentArray(content);
                            nodes[index + childIndex + ChildIndexOffset] = 0;
                        }

                        // Remove the item from the item count of all containing nodes
                        var removalIndex = index;
                        nodes[removalIndex + 1]--;
                        while (removalIndex != 0)
                        {
                            removalIndex = nodes[removalIndex];
                            nodes[removalIndex + 1]--;
                        }

                        if (nodes[index + 1] < NodeCollapseCount)
                        {
                            CollapseNode(index);
                        }

                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Editor only functionality to verify implementation.
        /// </summary>
        [Conditional("UNITY_EDITOR")]
        public void TestIntegrity()
        {
#if UNITY_EDITOR
            var untestedNodes = nodeCount - 1;
            var untestedLeafs = leafCount;
            var usedContentArrays = new HashSet<ItemEntry[]>();

            // Test that all nodes and leafes are reachable from the root node
            var stack = new Stack<int>();
            stack.Push(0);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                for(var i = ChildIndexOffset; i < NodeSize; i++)
                {
                    var reference = nodes[current + i];

                    if (reference > 0)
                    {
                        Debug.Assert(reference < nodeCount * NodeSize, "Child node index is out of bounds.");
                        Debug.Assert(nodes[reference] == current, "Parent index of child node doesn't match with actual index of parent.");
                        stack.Push(reference);
                        untestedNodes--;
                    }
                    else if (reference < 0)
                    {
                        var leafIndex = -(reference + 1);
                        Debug.Assert(leafIndex < leafCount, "Child leaf index is out of bounds.");
                        Debug.Assert(leafs[leafIndex].Parent == current,
                            "Parent index of child leaf doesn't match with actual index of parent.");
                        Debug.Assert(leafs[leafIndex].Count > 0, "Empty leaf.");
                        Debug.Assert(leafs[leafIndex].Content != null);
                        Debug.Assert(leafs[leafIndex].Content.Length >= leafs[leafIndex].Count,
                            "Leaf children out of bounds.");
                        Debug.Assert(Mathf.IsPowerOfTwo(leafs[leafIndex].Content.Length / InitialNodeCapacity),
                            "Leaf content array has the wrong size.");
                        Debug.Assert(!usedContentArrays.Contains(leafs[leafIndex].Content),
                            "The leaf content array is already used by another leaf.");
                        usedContentArrays.Add(leafs[leafIndex].Content);
                        untestedLeafs--;
                    }
                }
            }

            Debug.Assert(untestedNodes == 0, "There exist unreferenced nodes.");
            Debug.Assert(untestedLeafs == 0, "There exist unreferenced leafs.");
            Debug.Assert(usedContentArrays.Count == leafCount);
#endif
        }

        private int AddLeaf(int parent, ItemEntry item)
        {
            Debug.Assert(parent >= 0);
            Debug.Assert(parent % NodeSize == 0);
            Debug.Assert(parent < nodeCount * NodeSize);
            Debug.Assert(leafCount >= 0);
            Debug.Assert(leafCount <= leafs.Length);

            if (leafCount == leafs.Length)
            {
                // Grow the array to accomodate more leafs
                var newLeafs = new OctreeLeaf[leafs.Length * 2];
                leafs.CopyTo(newLeafs, 0);
                leafs = newLeafs;
            }

            ItemEntry[] leafContent;
            var cache = ContentCache[0];
            if (cache.Count > 0)
            {
                // Extract the item container from the cache
                leafContent = cache[cache.Count - 1];
                cache.RemoveAt(cache.Count - 1);
            }
            else
            {
                // Cache is empty, create a new container
                leafContent = new ItemEntry[DefaultLeafCapacity];
            }

            leafContent[0] = item;
            leafs[leafCount] = new OctreeLeaf(parent, leafContent, 1);
            leafCount++;

            return -leafCount;  // Leafs have negative indexing starting at -1
        }

        private int AddLeaf(int parent, List<ItemEntry> items)
        {
            Debug.Assert(parent >= 0);
            Debug.Assert(parent % NodeSize == 0);
            Debug.Assert(parent < nodeCount * NodeSize);
            Debug.Assert(leafCount >= 0);
            Debug.Assert(leafCount <= leafs.Length);

            if (leafCount == leafs.Length)
            {
                // Grow the array to accomodate more leafs
                var newLeafs = new OctreeLeaf[leafs.Length * 2];
                leafs.CopyTo(newLeafs, 0);
                leafs = newLeafs;
            }
            
            var cacheIndex = 0;
            var cacheSize = InitialNodeCapacity;
            while (cacheSize < items.Count)
            {
                cacheSize *= 2;
                cacheIndex++;
            }

            ItemEntry[] leafContent;
            var cache = ContentCache[cacheIndex];
            if (cache.Count > 0)
            {
                // Extract the item container from the cache
                leafContent = cache[cache.Count - 1];
                cache.RemoveAt(cache.Count - 1);
            }
            else
            {
                // Cache is empty, create a new container
                leafContent = new ItemEntry[cacheSize];
            }

            for (var i = 0; i < items.Count; i++)
            {
                leafContent[i] = items[i];
            }

            leafs[leafCount] = new OctreeLeaf(parent, leafContent, items.Count);
            leafCount++;

            return -leafCount;  // Leafs have negative indexing starting at -1
        }

        private int AddNode(int parent)
        {
            Debug.Assert(parent >= 0);
            Debug.Assert(parent % NodeSize == 0);
            Debug.Assert(parent < nodeCount * NodeSize);
            Debug.Assert(nodeCount >= 0);
            Debug.Assert(nodeCount * NodeSize <= nodes.Length);

            var index = nodeCount * NodeSize;

            if(index == nodes.Length)
            {
                // Grow the array to accomodate more nodes
                var newNodes = new int[nodes.Length * 2];
                nodes.CopyTo(newNodes, 0);
                nodes = newNodes;
            }
            
            nodes[index] = parent;  // The first value is the parent
            nodes[index + 1] = 0;   // The new node has 0 items
            for (var i = ChildIndexOffset; i < NodeSize; i++)
            {
                // Initialize child references
                nodes[index + i] = 0;
            }

            nodeCount++;
            return index;
        }

        private void AddToNode (T item, Vector3 position, int index, int depth, Vector3 localMin, Vector3 halfSize, bool testForDuplicate)
        {
            while (true)
            {
                var childIndex = 0;
                if (position.x > localMin.x + halfSize.x)
                {
                    childIndex += 4;    
                    localMin.x += halfSize.x;
                }

                if (position.y > localMin.y + halfSize.y)
                {
                    childIndex += 2;    
                    localMin.y += halfSize.y;
                }

                if (position.z > localMin.z + halfSize.z)
                {
                    childIndex += 1;
                    localMin.z += halfSize.z;
                }

                var nextIndex = nodes[index + childIndex + ChildIndexOffset];
                nodes[index + 1]++; // Increase the node child count

                if (nextIndex == 0)
                {
                    // The node doesn't have a sub node or leaf for that area
                    var newLeafIndex = AddLeaf(index, new ItemEntry(item, position));
                    nodes[index + childIndex + ChildIndexOffset] = newLeafIndex;
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
                var leaf = leafs[leafIndex];

                // Make sure the item isn't already part of the octree
                if (testForDuplicate)
                {
                    for (var i = 0; i < leaf.Count; i++)
                    {
                        // If the item is already part of the tree, don't add again
                        if (leaf.Content[i].Position == position && Equals(leaf.Content[i].item, item))
                        {
                            // Revert the item count because the item wasn't actually added
                            nodes[index + 1]--;
                            while (index != 0)
                            {
                                index = nodes[index];
                                nodes[index + 1]--;
                            }

                            return;
                        }
                    }
                }

                // If the leaf has the capacity to store the item, just add it
                if (leaf.Count < leaf.Content.Length)
                {
                    // Add the item to the leaf content and update the leaf count
                    leaf.Content[leaf.Count] = new ItemEntry(item, position);
                    leafs[leafIndex] = new OctreeLeaf(leaf.Parent, leaf.Content, leaf.Count + 1);
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

                    RemoveLeaf(leafIndex);                      // Remove the leaf
                    nextIndex = AddNode(index);                 // Create a new node
                    nodes[index + childIndex + ChildIndexOffset] = nextIndex;  // Replace the leaf reference with the node reference
                    index = nextIndex;                          // Continue with the current item

                    for(var i = 0; i < cnt; i++)
                    {
                        AddToNode(content[i].item, content[i].Position, index, depth, localMin, halfSize, false);
                    }

                    CacheContentArray(content);
                }
                else
                {
                    // The max depth has been reached, expand the leaf
                    var newContent = ExpandContentArray(leaf.Content);
                    newContent[leaf.Count] = new ItemEntry(item, position);
                    leafs[leafIndex] = new OctreeLeaf(leaf.Parent, newContent, leaf.Count + 1);
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

        private void CollapseNode(int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < nodeCount * NodeSize);
            Debug.Assert(index % NodeSize == 0);
            Debug.Assert(nodes[index + 1] < NodeCollapseCount);

            for(var i = ChildIndexOffset; i < NodeSize; i++)
            {
                Debug.Assert(nodes[index + i] <= 0, "If a node has few enough items to be collapsed, it can't be that the node contains child nodes.");
            }

            if (index == 0)
            {
                // Don't collapse root
                return;
            }
            
            // Node is bellow collapse threshold -> collapse into leaf
            var itemCache = GlobalListPool<ItemEntry>.Get(nodes[index + 1]);

            for (var i = ChildIndexOffset; i < NodeSize; i++)
            {
                var reference = nodes[index + i];
                nodes[index + i] = 0;

                if (reference < 0)
                {
                    var leafIndex = -(reference + 1);
                    var leaf = leafs[leafIndex];

                    for (var j = 0; j < leaf.Count; j++)
                    {
                        itemCache.Add(leaf.Content[j]);
                    }

                    CacheContentArray(leaf.Content);
                    RemoveLeaf(leafIndex);
                }
            }

            var parent = nodes[index];
            var newLeafIndex = AddLeaf(parent, itemCache);

            for (var i = ChildIndexOffset; i < NodeSize; i++)
            { 
                if (nodes[parent + i] == index)
                {
                    nodes[parent + i] = newLeafIndex;
                } 
            }
             
            RemoveNode(index);
            GlobalListPool<ItemEntry>.Put(itemCache);

            if (parent == nodeCount * NodeSize)
            {
                // In case parent was the last node and was used to replace the current node
            }

            if (nodes[parent + 1] < NodeCollapseCount)
            {
                CollapseNode(parent);
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
                        var cache = ContentCache[i];
                        var index = cache.Count - 1;
                        if (index >= 0)
                        {
                            // If the cache is not empty
                            result = cache[index];
                            cache.RemoveAt(index);
                        }
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

        private void RemoveLeaf(int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < leafCount);
            Debug.Assert(leafs[index].Parent < nodeCount * NodeSize);

            leafCount--;

            if (index < leafCount)
            {
                leafs[index] = leafs[leafCount];
                leafs[leafCount] = default;

                var parentIndex = leafs[index].Parent;
                var leafReference = -(leafCount + 1);
                for (var i = ChildIndexOffset; i < NodeSize; i++)
                {
                    if (nodes[parentIndex + i] == leafReference)
                    {
                        // Update reference to the moved entry
                        nodes[parentIndex + i] = -(index + 1);
                        break;
                    }
                }
            }
            else
            {
                leafs[index] = default;
            }
        }

        private void RemoveNode(int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < nodeCount * NodeSize);
            Debug.Assert(index % NodeSize == 0);
            Debug.Assert(index != 0, "Can't remove root node");

            nodeCount--;
            var lastIndex = nodeCount * NodeSize;
            if (index < lastIndex)
            {
                // Replace with the last node to prevent empty entries
                for (var i = 0; i < NodeSize; i++)
                {
                    nodes[index + i] = nodes[lastIndex + i];
                }

                // Update child reference of parent
                var parentIndex = nodes[index];
                for (var i = ChildIndexOffset; i < NodeSize; i++)
                {
                    if (nodes[parentIndex + i] == lastIndex)
                    {
                        // Update reference to the moved entry
                        nodes[parentIndex + i] = index;
                        break;
                    }
                }

                // Update parent references of children
                for (var i = ChildIndexOffset; i < NodeSize; i++)
                {
                    var child = nodes[index + i];
                    if (child > 0)
                    {
                        nodes[child] = index;
                    }
                    else if (child < 0)
                    {
                        var leafIndex = -(child + 1);
                        var leaf = leafs[leafIndex];
                        leafs[leafIndex] = new OctreeLeaf(index, leaf.Content, leaf.Count);
                    }
                }
            }
        }

        ~PointOctree()
        {
            Clear();
        }

        private struct CastPathEntry
        {
            public readonly int Index;

            public readonly int ChildIndex;

            public readonly bool Flag;

            public readonly Vector3 LocalMin;

            public readonly Vector3 HalfSize;

            public CastPathEntry(int index, int childIndex, bool flag, Vector3 localMin, Vector3 halfSize)
            {
                this.Index = index;
                this.ChildIndex = childIndex;
                this.Flag = flag;
                this.LocalMin = localMin;
                this.HalfSize = halfSize;
            }
        }

        private struct ItemEntry
        {
            public readonly T item;

            public readonly Vector3 Position;

            public ItemEntry(T item, Vector3 position)
            {
                this.item = item;
                this.Position = position;
            }
        }

        private struct OctreeLeaf
        {
            public readonly int Parent;

            public readonly ItemEntry[] Content;

            public readonly int Count;

            public OctreeLeaf(int parent, ItemEntry[] content, int count)
            {
                this.Parent = parent;
                this.Content = content;
                this.Count = count;
            }
        }
    }
}
