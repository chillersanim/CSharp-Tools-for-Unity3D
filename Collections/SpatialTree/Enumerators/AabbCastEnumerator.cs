// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         AabbCastEnumerator.cs
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
    public sealed class AabbCastEnumerator<T> : Spatial3DTreeInclusionEnumeratorBase<T> where T : class
    {
        private Vector3 min, max;

        public AabbCastEnumerator(Spatial3DTree<T> tree, Vector3 min, Vector3 max) : base(tree)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Starts over the enumerator, allows for enumerator reuse
        /// </summary>
        /// <param name="min">The min vertex of the aabb.</param>
        /// <param name="max">The max vertex of the aabb.</param>
        public void Restart(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
            Reset();
        }

        /// <inheritdoc />
        protected override bool IsAabbIntersecting(Vector3 start, Vector3 end)
        {
            return min.x <= end.x && min.y <= end.y && min.z <= end.z &&
                   max.x >= start.x && max.y >= start.y && max.z >= start.z;
        }

        /// <inheritdoc />
        protected override bool IsPointInside(Vector3 point)
        {
            return point.x >= min.x && point.y >= min.y && point.z >= min.z &&
                   point.x <= max.x && point.y <= max.y && point.z <= max.z;
        }
    }
}