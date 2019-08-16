// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         Simple3DCollectionTest.cs
// 
// Created:          10.08.2019  16:01
<<<<<<< HEAD
// Last modified:    16.08.2019  16:31
=======
// Last modified:    15.08.2019  17:57
>>>>>>> refs/remotes/origin/master
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
    public class Simple3DCollectionTest : I3DCollectionTest<string>
    {
        protected override I3DCollection<string> CreateInstance()
        {
            return new Simple3DCollection<string>();
        }

        protected override string GetItem(int i)
        {
            return i.ToString();
        }
    }
}
