// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         Spatial3DTreeTest.cs
// 
// Created:          12.08.2019  19:04
// Last modified:    05.02.2020  19:40
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

using UnityTools.Core;
using UnityEngine;
using UnityTools.Collections;

namespace UnityTools.Tests
{
    public class Spatial3DTreeTest : I3DCollectionTest<string>
    {
        protected override IPoint3DCollection<string> CreateInstance(Vector3 origin, Vector3 size, int capacity)
        {
            return new Spatial3DTree<string>();
        }

        protected override string GetItem(int i)
        {
            return i.ToString();
        }

        protected override void TestIntegrity(IPoint3DCollection<string> collection)
        {
        }
    }
}
