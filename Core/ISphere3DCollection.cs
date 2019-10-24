// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         ISphere3DCollection.cs
// 
// Created:          24.08.2019  23:15
// Last modified:    25.08.2019  15:59
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

namespace Unity_Tools.Core
{
    public interface ISphere3DCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// The amount of items in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an item with the specific position and radius to the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position.</param>
        /// <param name="radius">The radius.</param>
        void Add([NotNull] T item, Vector3 position, float radius);

        /// <summary>
        /// Clears this collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Evaluates whether the given item with the specific position and radius is part of this collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>Returns <c>true</c> if the item was found, <c>false</c> otherwise.</returns>
        bool Contains([CanBeNull] T item, Vector3 position, float radius);

        /// <summary>
        /// Changes the position and radius of an item within the collection. 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="oldPosition">The old position.</param>
        /// <param name="oldRadius">The old radius.</param>
        /// <param name="newPosition">The new position.</param>
        /// <param name="newRadius">The new radius.</param>
        /// <returns>Returns <c>true</c> if the item position and radius was changed successfully, <c>false</c> otherwise.</returns>
        /// <remarks>This operation is equal to the call of <see cref="Remove"/>(item, oldPosition, oldRadius) and then <see cref="Add"/>(item, newPosition, newRadius).</remarks>
        bool MoveAndResizeItem([NotNull] T item, Vector3 oldPosition, float oldRadius, Vector3 newPosition, float newRadius);

        /// <summary>
        /// Removes the item with the given position and radius from the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="position">The position.</param>
        /// <param name="radius">The radius.</param>
        /// <returns>Returns <c>true</c> if the item was successfully removed, <c>false</c> otherwise.</returns>
        bool Remove([CanBeNull] T item, Vector3 position, float radius);

        /// <summary>
        /// Finds all items in the collection whose bounds intersect with the given sphere.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>Returns the found items.</returns>
        T[] FindInRadius(Vector3 center, float radius);

        /// <summary>
        /// Finds all items in the collection whose bounds intersect with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns>Returns the found items.</returns>
        T[] FindInBounds(Bounds bounds);
    }
}
