// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IPoint3DCollection.cs
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
using UnityTools.Core;

namespace UnityTools.Collections
{
    /// <summary>
    /// Interface for generic 3D collections.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface IPoint3DCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets a value indicating whether duplicate elements are detected and duplicate entries are prevented when adding or moving items (Two item entries are duplicates if item and position are equal)
        /// </summary>
        bool AllowsDuplicates { get; }

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
        /// Moves the item from the old position to the new position. 
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
        /// Returns an enumerator that iterates through all items that are inside the given shape.
        /// </summary>
        /// <typeparam name="TShape">The type of the shape to cast for.</typeparam>
        /// <param name="shape">The shape to use.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}"/> that provides access to all resulting items.</returns>
        IEnumerable<T> ShapeCast<TShape>(TShape shape) where TShape : IVolume;

        /// <summary>
        /// Populates the output with all items that are inside the given shape.
        /// </summary>
        /// <typeparam name="TShape">The type of the shape to cast for.</typeparam>
        /// <param name="shape">The shape to use.</param>
        /// <param name="output">The output in which to store the shape cast result.</param>
        void ShapeCast<TShape>(TShape shape, IList<T> output) where TShape : IVolume;
    }
}