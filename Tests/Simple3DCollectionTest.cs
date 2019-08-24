// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         Simple3DCollectionTest.cs
// 
// Created:          12.08.2019  19:04
// Last modified:    20.08.2019  21:50
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

using Unity_Tools.Collections;
using Unity_Tools.Core;

namespace Unity_Tools.Tests
{
    public class Simple3DCollectionTest : I3DCollectionTest<string>
    {
        protected override IPoint3DCollection<string> CreateInstance()
        {
            return new Simple3DCollection<string>();
        }

        protected override string GetItem(int i)
        {
            return i.ToString();
        }
    }
}
