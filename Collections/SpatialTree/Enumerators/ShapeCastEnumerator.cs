// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         ShapeCastEnumerator.cs
// 
// Created:          05.08.2019  11:45
// Last modified:    15.08.2019  17:56
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
// 

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Tools.Collections.SpatialTree.Enumerators
{
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
        protected override bool IsAabbIntersecting(Vector3 start, Vector3 end)
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