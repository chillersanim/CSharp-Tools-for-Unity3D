// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IPoint3DCollection.cs
// 
// Created:          16.08.2019  16:33
// Last modified:    25.10.2019  11:38
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
using JetBrains.Annotations;
using UnityEngine;
using Unity_Tools.Collections.SpatialTree;

namespace Unity_Tools.Core
{
    /// <summary>
    /// Interface for generic 3D collections.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface IPoint3DCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// The amount of items in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an item at the specified position to the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position.</param>
        void Add([NotNull] T item, Vector3 position);

        /// <summary>
        /// Iterates over all items that are within a bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns>Returns the found items.</returns>
        IEnumerable<T> BoundsCast(Bounds bounds);

        /// <summary>
        /// Clears this collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Evaluates whether the given item at the given position is part of this collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position.</param>
        /// <returns>Returns <c>true</c> if at the position the item was found, <c>false</c> otherwise.</returns>
        bool Contains([CanBeNull] T item, Vector3 position);

        /// <summary>
        /// Iterates over all items that are within a bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns>Returns the found items.</returns>
        IEnumerable<T> InverseBoundsCast(Bounds bounds);

        /// <summary>
        /// Iterates over all items that are within a shape.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns>Returns the found items.</returns>
        IEnumerable<T> InverseShapeCast(IShape shape);

        /// <summary>
        /// Iterates over all items that are within a given sphere.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>Returns the found items.</returns>
        IEnumerable<T> InverseSphereCast(Vector3 center, float radius);

        /// <summary>
        /// Moves the item within the collection. 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="from">The old position.</param>
        /// <param name="to">The new position.</param>
        /// <returns>Returns <c>true</c> if the item was moved successfully, <c>false</c> otherwise.</returns>
        /// <remarks>This operation is equal to the call of <see cref="Remove"/>(item, from) and then <see cref="Add"/>(item, to).</remarks>
        bool MoveItem([NotNull] T item, Vector3 from, Vector3 to);

        /// <summary>
        /// Removes the item at the given position from the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position.</param>
        /// <returns>Returns <c>true</c> if the item was successfully removed, <c>false</c> otherwise.</returns>
        bool Remove([CanBeNull] T item, Vector3 position);

        /// <summary>
        /// Iterates over all items that are within a shape.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns>Returns the found items.</returns>
        IEnumerable<T> ShapeCast(IShape shape);

        /// <summary>
        /// Iterates over all items that are within a given sphere.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>Returns the found items.</returns>
        IEnumerable<T> SphereCast(Vector3 center, float radius);
    }
}