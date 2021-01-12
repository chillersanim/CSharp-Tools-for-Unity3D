// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Graph.cs
// 
// Created:          02.12.2019  14:44
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
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Unity_Tools.Pooling;

namespace Unity_Tools.Collections
{
    /// <summary>
    /// A graph class allows for graph queries using either directed or undirected graph representation.
    /// </summary>
    /// <typeparam name="T">The type of the node keys.</typeparam>
    /// <remarks>
    /// This graph implementation pools unused nodes.<br/>
    /// It is thus a good idea to release all outside references to a node when it has been removed from the graph, as the behavior of that node instance is undefined after removal.
    /// </remarks>
    public class Graph<T> : IEnumerable<IGraphNode<T>>
    {
        private static readonly IPool<GraphNode> GraphNodePool = new Pool<GraphNode>();

        private readonly Dictionary<T, GraphNode> itemToGraphNodes;

        private readonly Queue<GraphNode> searchCache;

        /// <inheritdoc/>
        /// <summary>
        /// Creates a graph instances that uses undirected connections.
        /// </summary>
        public Graph()
        {
            this.itemToGraphNodes = new Dictionary<T, GraphNode>();
            this.ConnectionType = GraphConnectionType.Undirected;
            this.searchCache = new Queue<GraphNode>();
        }

        /// <inheritdoc />
        /// <summary>
        /// Creates a graph instance that uses connections as indicated by <see cref="!:connectionType" />.
        /// </summary>
        /// <param name="connectionType">The connection type used in this graph.</param>
        public Graph(GraphConnectionType connectionType) : this()
        {
            ConnectionType = connectionType;
        }

        /// <summary>
        /// Defines how the connections are treated in this graph.
        /// </summary>
        public GraphConnectionType ConnectionType { get; }

        /// <summary>
        /// Gets the node that represents the key in this graph.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns the representing node.</returns>
        public IGraphNode<T> this[T key] => itemToGraphNodes[key];

        /// <inheritdoc/>
        public IEnumerator<IGraphNode<T>> GetEnumerator()
        {
            return itemToGraphNodes.Values.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the key as new node to the graph.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <exception cref="InvalidOperationException">An key cannot be added twice.</exception>
        public void Add(T key)
        {
            if (!TryAdd(key))
            {
                throw new InvalidOperationException("The key already exists in the graph.");
            }
        }

        /// <summary>
        /// Clears the graph.
        /// </summary>
        public void Clear()
        {
            foreach (var node in itemToGraphNodes.Values)
            {
                PoolNode(node);
            }

            itemToGraphNodes.Clear();
        }

        /// <summary>
        /// Evaluates whether the key is part of the graph.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns <c>true</c> if the key is in the graph, <c>false</c> otherwise.</returns>
        public bool Contains(T key)
        {
            return itemToGraphNodes.ContainsKey(key);
        }

        /// <summary>
        /// Find all matching nodes that are connected to the from node via allowed nodes.
        /// </summary>
        /// <param name="from">The start node.</param>
        /// <param name="canUseNode">Evaluates whether the node can be used as a path node.</param>
        /// <param name="match">Evaluates whether the node is a match.</param>
        /// <param name="output">The collection in which matching nodes get stored. This doesn't get cleared before use.</param>
        public void FindConnectedMatches(T from, Func<T, bool> canUseNode, Func<T, bool> match, ICollection<T> output)
        {
            if (!itemToGraphNodes.TryGetValue(from, out var fromNode))
            {
                throw new InvalidOperationException("The from key is not part of the graph.");
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output), "The output collection must not be null.");
            }

            PrepareForSearch();
            searchCache.Enqueue(fromNode);

            fromNode.Previous = fromNode;
            if (match(from))
            {
                output.Add(from);
            }

            while (searchCache.Count > 0)
            {
                var current = searchCache.Dequeue();

                foreach (var neighbor in current.Edges)
                {
                    if (neighbor.Previous != null)
                    {
                        continue;
                    }

                    neighbor.Previous = current;

                    if (match(neighbor.Key))
                    {
                        output.Add(neighbor.Key);
                    }

                    if (canUseNode(neighbor.Key))
                    {
                        searchCache.Enqueue(neighbor);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the shortest path along the graph between the from and the to node.
        /// </summary>
        /// <param name="from">The start node.</param>
        /// <param name="to">The goal node.</param>
        /// <param name="output">The collection in which the path gets stored. This doesn't get cleared before use.</param>
        /// <returns>Returns <c>true</c> if a path was found, <c>false</c> otherwise.</returns>
        public bool FindShortestPath(T from, T to, IList<T> output)
        {
            return FindShortestPath(from, to, _ => true, output);
        }

        /// <summary>
        /// Finds the shortest path along the graph between the from and the to node by only using nodes that match the canUseNode predicate.
        /// </summary>
        /// <param name="from">The start node.</param>
        /// <param name="to">The goal node.</param>
        /// <param name="canUseNode">Evaluates whether a node can be used as a path node.</param>
        /// <param name="output">The collection in which the path gets stored. This doesn't get cleared before use.</param>
        /// <returns>Returns <c>true</c> if a path was found, <c>false</c> otherwise.</returns>
        public bool FindShortestPath(T from, T to, Func<T, bool> canUseNode, IList<T> output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!itemToGraphNodes.TryGetValue(from, out var fromNode))
            {
                throw new InvalidOperationException("The from key is not part of the graph.");
            }

            if (Equals(from, to))
            {
                output.Add(from);
                return true;
            }

            PrepareForSearch();
            fromNode.Previous = fromNode;
            searchCache.Enqueue(fromNode);

            while (searchCache.Count > 0)
            {
                var current = searchCache.Dequeue();
                foreach (var neighbor in current.Edges)
                {
                    if (neighbor.Previous != null)
                    {
                        continue;
                    }

                    neighbor.Previous = current;

                    if (Equals(neighbor.Key, to))
                    {
                        current = neighbor;

                        while (current != fromNode)
                        {
                            output.Add(current.Key);
                            current = current.Previous;
                        }

                        output.Add(fromNode.Key);
                        return true;
                    }

                    if (canUseNode(neighbor.Key))
                    {
                        searchCache.Enqueue(neighbor);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Finds the shortest path along the graph between the from and any to node.
        /// </summary>
        /// <param name="from">The start node.</param>
        /// <param name="to">The available goal nodes.</param>
        /// <param name="output">The collection in which the path gets stored. This doesn't get cleared before use.</param>
        /// <returns>Returns <c>true</c> if a path was found, <c>false</c> otherwise.</returns>
        public bool FindShortestPathToNearest(T from, IList<T> to, IList<T> output)
        {
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (to.Count == 0)
            {
                throw new ArgumentException("Needs at least one goal key to find the path.", nameof(to));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (!itemToGraphNodes.TryGetValue(from, out var fromNode))
            {
                throw new InvalidOperationException("The from key is not part of the graph.");
            }

            if (to.Contains(from))
            {
                return true;
            }

            PrepareForSearch();
            fromNode.Previous = fromNode;
            searchCache.Enqueue(fromNode);

            while (searchCache.Count > 0)
            {
                var current = searchCache.Dequeue();
                foreach (var neighbor in current.Edges)
                {
                    if (neighbor.Previous != null)
                    {
                        continue;
                    }

                    neighbor.Previous = current;

                    if (to.Contains(neighbor.Key))
                    {
                        current = neighbor;

                        while (current != fromNode)
                        {
                            output.Add(current.Key);
                            current = current.Previous;
                        }

                        output.Add(fromNode.Key);
                        return true;
                    }
                    
                    searchCache.Enqueue(neighbor);
                }
            }

            return false;
        }

        /// <summary>
        /// Evaluates whether there exists and edge going from <see cref="from"/> to <see cref="to"/>.
        /// </summary>
        /// <param name="from">The from key.</param>
        /// <param name="to">The to key.</param>
        /// <returns>Returns <c>true</c> if the edge exists, <c>false</c> otherwise.</returns>
        public bool HasEdge(T from, T to)
        {
            if (!itemToGraphNodes.TryGetValue(from, out var fromNode))
            {
                return false;
            }

            foreach (var neighbor in fromNode.Edges)
            {
                if (Equals(neighbor.Key, to))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates an edge starting at the <see cref="from"/> key going to the <see cref="to"/> key.<br/>
        /// If any of those two key aren't part of the graph, a new node is created for them.
        /// </summary>
        /// <param name="from">The from key.</param>
        /// <param name="to">The to key.</param>
        public void MakeEdgeAndNodes(T from, T to)
        {
            if (!itemToGraphNodes.TryGetValue(from, out var fromNode))
            {
                fromNode = GraphNodePool.Get();
                fromNode.key = from;
                itemToGraphNodes.Add(from, fromNode);
            }

            if (!itemToGraphNodes.TryGetValue(to, out var toNode))
            {
                toNode = GraphNodePool.Get();
                toNode.key = to;
                itemToGraphNodes.Add(to, toNode);
            }

            if (fromNode.Edges.Contains(toNode))
            {
                return;
            }

            fromNode.Edges.Add(toNode);

            if (ConnectionType == GraphConnectionType.Undirected)
            {
                toNode.Edges.Add(fromNode);
            }
        }

        /// <summary>
        /// Tries to remove the <see cref="key"/> from the graph.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns <c>true</c> if the node was successfully removed, <c>false</c> otherwise.</returns>
        public bool Remove(T key)
        {
            if (!itemToGraphNodes.TryGetValue(key, out var node))
            {
                return false;
            }

            if (ConnectionType == GraphConnectionType.Undirected)
            {
                foreach (var neighbor in node.Edges)
                {
                    neighbor.Edges.Remove(node);
                }
            }
            else
            {
                foreach (var n in itemToGraphNodes.Values)
                {
                    n.Edges.Remove(node);
                }
            }
            
            itemToGraphNodes.Remove(key);
            PoolNode(node);

            return true;
        }

        /// <summary>
        /// Tries to remove the edge starting at <see cref="from"/> and going to <see cref="to"/>.
        /// </summary>
        /// <param name="from">The start of the edge.</param>
        /// <param name="to">The target of the edge.</param>
        /// <returns>Returns <c>true</c> if the edge was successfully removed, <c>false</c> otherwise.</returns>
        public bool RemoveEdge(T from, T to)
        {
            if (!itemToGraphNodes.TryGetValue(from, out var fromNode))
            {
                return false;
            }

            if (!itemToGraphNodes.TryGetValue(to, out var toNode))
            {
                return false;
            }

            var success = fromNode.Edges.Remove(toNode);
            if (success && ConnectionType == GraphConnectionType.Undirected)
            {
                toNode.Edges.Remove(fromNode);
            }

            return success;
        }

        /// <summary>
        /// Tries to add an key as new node to the graph and returns whether the operation succeeded.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Returns <c>true</c> when the key was successfully added to the graph, <c>false</c> otherwise.</returns>
        public bool TryAdd(T key)
        {
            if (this.itemToGraphNodes.ContainsKey(key))
            {
                return false;
            }

            var node = GraphNodePool.Get();
            node.key = key;
            itemToGraphNodes.Add(key, node);
            return true;
        }

        /// <summary>
        /// Tries to get the node that represents the key in this graph..
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="node">The representing node.</param>
        /// <returns>Return <c>true</c> if the key was part of the graph, <c>false</c> otherwise.</returns>
        public bool TryGetNode(T key, out IGraphNode<T> node)
        {
            if (itemToGraphNodes.TryGetValue(key, out var n))
            {
                node = n;
                return true;
            }

            node = null;
            return false;
        }

        /// <summary>
        /// Tries to create an edge starting at the <see cref="from"/> key going to the <see cref="to"/> key.<br/>
        /// If any of the two key aren't part of the graph, the operation fails and does nothing.
        /// </summary>
        /// <param name="from">The start of the new edge.</param>
        /// <param name="to">The target of the new edge.</param>
        /// <returns>Returns <c>true</c> if the new edge could be created, or <c>false</c> when any of the nodes are missing or the edge already exists.</returns>
        public bool TryMakeEdge(T from, T to)
        {
            if (!itemToGraphNodes.TryGetValue(from, out var fromNode))
            {
                return false;
            }

            if (!itemToGraphNodes.TryGetValue(to, out var toNode))
            {
                return false;
            }

            if (fromNode.Edges.Contains(toNode))
            {
                return false;
            }

            fromNode.Edges.Add(toNode);

            if (ConnectionType == GraphConnectionType.Undirected)
            {
                toNode.Edges.Add(fromNode);
            }

            return true;
        }

        /// <summary>
        /// Pools a node that is no longer used in this graph.
        /// </summary>
        /// <param name="node">The node.</param>
        private static void PoolNode([NotNull]GraphNode node)
        {
            node.Edges.Clear();
            node.key = default;
            node.Previous = null;
            GraphNodePool.Put(node);
        }

        /// <summary>
        /// Prepares the graph for a search operation.
        /// </summary>
        private void PrepareForSearch()
        {
            searchCache.Clear();

            foreach(var node in itemToGraphNodes.Values)
            {
                node.Previous = null;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Internal representation of the graph node.
        /// </summary>
        private sealed class GraphNode : IGraphNode<T>
        {
            public readonly Collection<GraphNode> Edges;

            public T key;

            public GraphNode Previous;

            public GraphNode()
            {
                this.Edges = new Collection<GraphNode>();
                this.Previous = null;
            }

            public GraphNode(T key) : this()
            {
                this.key = key;
            }

            public T Key => key;

            public IEnumerator<IGraphNode<T>> GetEnumerator()
            {
                return Edges.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool HasEdgeTo(IGraphNode<T> node)
            {
                if (!(node is GraphNode gn))
                {
                    return false;
                }

                return Edges.Contains(gn);
            }

            public int Count => Edges.Count;
        }
    }

    public enum GraphConnectionType
    {
        /// <summary>
        /// The connections between graph nodes are unidirectional.
        /// </summary>
        Directed,

        /// <summary>
        /// The connections between graph nodes are bidirectional.
        /// </summary>
        Undirected
    }

    /// <summary>
    /// Interface for graph nodes.
    /// </summary>
    /// <typeparam name="T">The type of the key associated with the node.</typeparam>
    public interface IGraphNode<T> : IReadOnlyCollection<IGraphNode<T>>
    {
        /// <summary>
        /// The key associated with this <see cref="IGraphNode{T}"/>.
        /// </summary>
        T Key { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IGraphNode{T}"/> has a connection to the <see cref="node"/>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Returns <c>true</c> if the connection exists, <c>false</c> otherwise.</returns>
        bool HasEdgeTo(IGraphNode<T> node);
    }
}
