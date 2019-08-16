// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         Spatial3DTreeTest.cs
// 
// Created:          10.08.2019  15:17
// Last modified:    15.08.2019  17:57
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

using Unity_Tools.Collections;
using Unity_Tools.Core;

namespace Unity_Tools.Tests
{
    public class Spatial3DTreeTest : I3DCollectionTest<string>
    {
        protected override I3DCollection<string> CreateInstance()
        {
            return new Spatial3DTree<string>();
        }

        protected override string GetItem(int i)
        {
            return i.ToString();
        }
    }
}
