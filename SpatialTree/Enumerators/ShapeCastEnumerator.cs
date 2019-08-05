// Copyright © 2019 Jasper Ermatinger

#region usings

using System;
using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Unity_Collections.SpatialTree.Enumerators
{
    #region Usings

    #endregion

    /// <summary>
    ///     The shape cast enumerator.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public sealed class ShapeCastEnumerator<T> : Spatial3DTreeInclusionEnumeratorBase<T>
        where T : class
    {
        /// <summary>
        ///     The shape.
        /// </summary>
        [NotNull] private IShape shape;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShapeCastEnumerator{T}" /> class.
        /// </summary>
        /// <param name="tree">
        ///     The tree.
        /// </param>
        /// <param name="shape">
        ///     The shape.
        /// </param>
        public ShapeCastEnumerator(Spatial3DTree<T> tree, IShape shape)
            : base(tree)
        {
            this.shape = shape ?? throw new ArgumentNullException();
        }

        /// <summary>
        ///     The restart.
        /// </summary>
        /// <param name="shape">
        ///     The shape.
        /// </param>
        public void Restart(IShape shape)
        {
            this.shape = shape ?? throw new ArgumentNullException();
            Reset();
        }

        /// <inheritdoc />
        protected override bool IsAabbInside(Vector3 start, Vector3 end)
        {
            return shape.IntersectsAabb(start, end);
        }

        /// <inheritdoc />
        protected override bool IsPointInside(Vector3 point)
        {
            return shape.ContainsPoint(point);
        }
    }
}