// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         GlobalPool.cs
// 
// Created:          16.08.2019  15:26
// Last modified:    25.10.2019  11:38
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

using JetBrains.Annotations;

namespace Unity_Tools.Core.Pooling
{
    public static class GlobalPool<T> where T : class, new()
    {
        private static readonly Pool<T> Pool = new Pool<T>();

        public static int MaxSize
        {
            get => Pool.MaxSize;
            set => Pool.MaxSize = value;
        }

        public static T Get()
        {
            return Pool.Get();
        }

        public static void Put([NotNull] T item)
        {
            Pool.Put(item);
        }
    }
}
