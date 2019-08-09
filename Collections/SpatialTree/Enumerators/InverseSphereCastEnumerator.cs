// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         InverseSphereCastEnumerator.cs
// 
// Created:          05.08.2019  11:42
// Last modified:    09.08.2019  15:44
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

using UnityEngine;

namespace Unity_Tools.Collections.SpatialTree.Enumerators
{
    public sealed class InverseSphereCastEnumerator<T> : Spatial3DTreeExclusionEnumeratorBase<T> where T : class
    {
        private Vector3 center;

        private float sqrRadius;

        public InverseSphereCastEnumerator(Spatial3DTree<T> tree, Vector3 center, float radius) : base(tree)
        {
            this.center = center;
            sqrRadius = radius * radius;
        }

        public void Restart(Vector3 center, float radius)
        {
            this.center = center;
            sqrRadius = radius * radius;
            Reset();
        }

        /// <inheritdoc />
        protected override bool IsAabbNotFullyInside(Vector3 start, Vector3 end)
        {
            var a = start - center;
            var b = end - center;

            var xx = Mathf.Max(a.x * a.x, b.x * b.x);
            var yy = Mathf.Max(a.y * a.y, b.y * b.y);
            var zz = Mathf.Max(a.z * a.z, b.z * b.z);

            return xx + yy + zz <= sqrRadius;
        }

        /// <inheritdoc />
        protected override bool IsPointOutside(Vector3 point)
        {
            var x = point.x - center.x;
            var y = point.y - center.y;
            var z = point.z - center.z;

            return x * x + y * y + z * z > sqrRadius;
        }
    }
}