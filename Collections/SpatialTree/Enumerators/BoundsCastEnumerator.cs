// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         BoundsCastEnumerator.cs
// 
// Created:          24.08.2019  23:15
// Last modified:    25.08.2019  15:58
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
    public sealed class BoundsCastEnumerator<T> : Spatial3DTreeInclusionEnumeratorBase<T>
    {
        private Vector3 min, max;

        public BoundsCastEnumerator(Spatial3DTree<T> tree, Bounds bounds) : base(tree)
        {
            this.min = bounds.min;
            this.max = bounds.max;
        }

        /// <summary>
        /// Starts over the enumerator, allows for enumerator reuse
        /// </summary>
        public void Restart(Bounds bounds)
        {
            this.min = bounds.min;
            this.max = bounds.max;
            Reset();
        }

        /// <inheritdoc />
        protected override bool IsBoundsIntersecting(Vector3 start, Vector3 end)
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