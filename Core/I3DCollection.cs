using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Tools.Core
{
    /// <summary>
    /// Interface for generic 3D collections.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface I3DCollection<T> : IEnumerable<T> where T : class
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
        /// Finds all items in the collection that are within a given sphere.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>Returns the found items.</returns>
        T[] FindInRadius(Vector3 center, float radius);

        /// <summary>
        /// Finds all items in the collection that are within a axis aligned bounding box.
        /// </summary>
        /// <param name="center">The center of the aabb.</param>
        /// <param name="size">The size of the aabb.</param>
        /// <returns>Returns the found items.</returns>
        T[] FindInAabb(Vector3 center, Vector3 size);
    }
}