// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         VolumeInverse.cs
// 
// Created:          27.01.2020  22:45
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

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityTools.Core
{
    /// <summary>
    /// ¬A
    /// </summary>
    public struct AreaInverse : IArea
    {
        private readonly IArea shape;


        /// <inheritdoc />
        public Bounds2 Bounds => this.shape.Bounds;

        /// <inheritdoc />
        public bool Inverted => !shape.Inverted;

        /// <summary>
        /// ¬A
        /// </summary>
        public AreaInverse(IArea shape)
        {
            this.shape = shape;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector2 point)
        {
            return !shape.ContainsPoint(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsRect(Vector2 start, Vector2 end)
        {
            return !shape.ContainsRect(start, end);
        }

        /// <inheritdoc />
        public bool Raycast(Vector2 orig, Vector2 dir, out float t, out Vector2 normal)
        {
            if (this.shape.Raycast(orig, dir, out t, out normal))
            {
                normal = -normal;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsRect(Vector2 start, Vector2 end)
        {
            return !shape.IntersectsRect(start, end);
        }
    }
}
