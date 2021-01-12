// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IGraphNode.cs
// 
// Created:          18.11.2020  14:14
// Last modified:    18.11.2020  14:14
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

using System.Collections.Generic;

namespace UnityTools.Collections
{
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