// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         VolumeEmpty.cs
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

using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityTools.Core
{
    /// <summary>
    /// The empty volume containing nothing.
    /// </summary>
    public struct VolumeEmpty : IVolume
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector3 point)
        {
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsAabb(Vector3 start, Vector3 end)
        {
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsAabb(Vector3 start, Vector3 end)
        {
            return false;
        }
    }
}
