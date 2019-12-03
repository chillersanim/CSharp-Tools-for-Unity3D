// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IPool.cs
// 
// Created:          16.08.2019  15:56
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
namespace Unity_Tools.Core.Pooling
{
    public interface IPool<T> where T : class
    {
        int MaxSize { get; set; }

        T Get();

        void Put(T item);
    }
}
