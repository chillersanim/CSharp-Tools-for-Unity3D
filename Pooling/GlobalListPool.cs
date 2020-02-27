// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         GlobalListPool.cs
// 
// Created:          29.01.2020  19:32
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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityTools.Pooling
{
    public static class GlobalListPool<T>
    {
        private static readonly ListPool<T> Pool = new ListPool<T>();

        public static int MaxListCapacity
        {
            get => Pool.MaxListCapacity;
            set => Pool.MaxListCapacity = value;
        }

        public static int MaxSize
        {
            get => Pool.MaxSize;
            set => Pool.MaxSize = value;
        }

        public static List<T> Get()
        {
            return Pool.Get();
        }

        public static List<T> Get(int minCapacity)
        {
            return Pool.Get(minCapacity);
        }

        public static void Put([NotNull] List<T> item)
        {
            Pool.Put(item);
        }
    }
}
