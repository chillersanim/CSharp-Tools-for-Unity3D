// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         GlobalListPool.cs
// 
// Created:          20.08.2019  12:39
// Last modified:    03.12.2019  08:37
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

namespace Unity_Tools.Core.Pooling
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
