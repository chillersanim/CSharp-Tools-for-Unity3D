// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         InverseAabbCastEnumerator.cs
// 
// Created:          05.08.2019  11:42
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

using UnityEngine;

namespace Unity_Tools.Collections.SpatialTree.Enumerators
{
    public sealed class InverseAabbCastEnumerator<T> : Spatial3DTreeExclusionEnumeratorBase<T> where T : class
    {
        private Vector3 min, max;

        public InverseAabbCastEnumerator(Spatial3DTree<T> tree, Vector3 center, Vector3 size) : base(tree)
        {
            var halfSize = size / 2f;
            this.min = center - halfSize;
            this.max = center + halfSize;
        }

        public void Restart(Vector3 center, Vector3 size)
        {
            var halfSize = size / 2f;
            this.min = center - halfSize;
            this.max = center + halfSize;
            Reset();
        }

        protected override bool IsAabbNotFullyInside(Vector3 start, Vector3 end)
        {
            return min.x < start.x && min.y < start.y && min.z < start.z ||
                   max.x > end.x && max.y > end.y && max.z > end.z;
        }

        protected override bool IsPointOutside(Vector3 point)
        {
            return point.x < min.x && point.y < min.y && point.y < min.z ||
                   point.x > max.x && point.y > max.y && point.z > max.z;
        }
    }
}