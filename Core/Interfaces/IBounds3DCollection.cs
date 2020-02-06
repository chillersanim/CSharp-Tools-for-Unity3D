// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IBounds3DCollection.cs
// 
// Created:          29.01.2020  19:27
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

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Tools.Core
{
    public interface IBounds3DCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// The amount of items in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an item with the specific bounds to the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="bounds">The bounds.</param>
        void Add([NotNull] T item, Bounds bounds);

        /// <summary>
        /// Clears this collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Evaluates whether the given item with the specific bounds is part of this collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns>Returns <c>true</c> if the item was found, <c>false</c> otherwise.</returns>
        bool Contains([CanBeNull] T item, Bounds bounds);

        /// <summary>
        /// Finds all items in the collection whose bounds intersect with the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns>Returns the found items.</returns>
        T[] FindInBounds(Bounds bounds);

        /// <summary>
        /// Finds all items in the collection whose bounds intersect with the given sphere.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>Returns the found items.</returns>
        T[] FindInRadius(Vector3 center, float radius);

        /// <summary>
        /// Changes the bounds of an item within the collection. 
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="oldBounds">The old bounds.</param>
        /// <param name="newBounds">The new bounds.</param>
        /// <returns>Returns <c>true</c> if the item bounds was changed successfully, <c>false</c> otherwise.</returns>
        /// <remarks>This operation is equal to the call of <see cref="Remove"/>(item, oldBounds) and then <see cref="Add"/>(item, newBounds).</remarks>
        bool MoveItem([NotNull] T item, Bounds oldBounds, Bounds newBounds);

        /// <summary>
        /// Removes the item with the given bounds from the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns>Returns <c>true</c> if the item was successfully removed, <c>false</c> otherwise.</returns>
        bool Remove([CanBeNull] T item, Bounds bounds);
    }
}
