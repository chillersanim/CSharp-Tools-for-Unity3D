// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         SphereCastEnumerator.cs
// 
// Created:          05.08.2019  11:41
// Last modified:    09.08.2019  15:54
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

using UnityEngine;

namespace Unity_Tools.Collections.SpatialTree.Enumerators
{
    /// <summary>
    ///     The sphere cast enumerator.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public sealed class SphereCastEnumerator<T> : Spatial3DTreeInclusionEnumeratorBase<T>
        where T : class
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
            var x = Mathf.Clamp(center.x, start.x, end.x) - center.x;
            var y = Mathf.Clamp(center.y, start.y, end.y) - center.y;
            var z = Mathf.Clamp(center.z, start.z, end.z) - center.z;

            return x * x + y * y + z * z <= sqrRadius;
        }

        /// <inheritdoc />
        protected override bool IsPointInside(Vector3 point)
        {
            var x = point.x - center.x;
            var y = point.y - center.y;
            var z = point.z - center.z;

            return x * x + y * y + z * z <= sqrRadius;
        }
    }
}