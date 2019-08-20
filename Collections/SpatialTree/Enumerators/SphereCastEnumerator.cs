// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         SphereCastEnumerator.cs
// 
// Created:          12.08.2019  19:08
// Last modified:    20.08.2019  21:49
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

using Unity_Tools.Core;
using UnityEngine;

namespace Unity_Tools.Collections.SpatialTree.Enumerators
{
    /// <summary>
    ///     The sphere cast enumerator.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public sealed class SphereCastEnumerator<T> : Spatial3DTreeInclusionEnumeratorBase<T>
    {
        /// <summary>
        ///     The center.
        /// </summary>
        private Vector3 center;

        /// <summary>
        ///     The sqr radius.
        /// </summary>
        private float sqrRadius;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SphereCastEnumerator{T}" /> class.
        /// </summary>
        /// <param name="tree">
        ///     The tree.
        /// </param>
        /// <param name="center">
        ///     The center.
        /// </param>
        /// <param name="radius">
        ///     The radius.
        /// </param>
        public SphereCastEnumerator(Spatial3DTree<T> tree, Vector3 center, float radius)
            : base(tree)
        {
            this.center = center;
            sqrRadius = radius * radius;
        }

        /// <summary>
        ///     The restart.
        /// </summary>
        /// <param name="center">
        ///     The center.
        /// </param>
        /// <param name="radius">
        ///     The radius.
        /// </param>
        public void Restart(Vector3 center, float radius)
        {
            this.center = center;
            sqrRadius = radius * radius;
            Reset();
        }

        /// <inheritdoc />
        protected override bool IsAabbIntersecting(Vector3 start, Vector3 end)
        {
            var vectorToNearest = center.ClampComponents(start, end) - center;
            return vectorToNearest.sqrMagnitude <= sqrRadius;
        }

        /// <inheritdoc />
        protected override bool IsPointInside(Vector3 point)
        {
            var sqrDist = (point - center).sqrMagnitude;
            return sqrDist <= sqrRadius;
        }
    }
}