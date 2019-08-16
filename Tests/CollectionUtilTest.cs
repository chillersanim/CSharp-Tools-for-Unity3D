// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         CollectionUtilTest.cs
// 
// Created:          15.08.2019  18:18
// Last modified:    16.08.2019  16:56
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

using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Unity_Tools.Collections;

namespace Unity_Tools.Tests
{
    public class CollectionUtilTest
    {
        [Test]
        public void BinarySearchLocationTest([Values(0, 1, 2, 3, 4, 5, 10, 100, 1000)]
            int itemAmount)
        {
            var items = new List<float>(itemAmount);

            // Create values between -5 and 5
            for (var i = 0; i < itemAmount; i++)
            {
                items.Add(Random.value * 10 - 5);
            }

            items.Sort();

            // Test for any values between -7.5 and 7.5
            for (var i = 0; i < 10; i++)
            {
                var valueToSearch = Random.value * 15 - 7.5f;
                var index = items.BinarySearchLocation(valueToSearch);
                TestBinarySearchLocationResult(items, index, valueToSearch);
            }

            // Test for specific items in the values range
            if (items.Count > 0)
            {
                for (var i = 0; i < 10; i++)
                {
                    var valueToSearch = items[Random.Range(0, items.Count)];
                    var index = items.BinarySearchLocation(valueToSearch);
                    TestBinarySearchLocationResult(items, index, valueToSearch);
                }
            }

            Assert.AreEqual(0, items.BinarySearchLocation(float.NegativeInfinity));
            Assert.AreEqual(items.Count, items.BinarySearchLocation(float.PositiveInfinity));
        }

        private void TestBinarySearchLocationResult(List<float> items, int index, float value)
        {
            if (items.Count == 0)
            {
                Assert.AreEqual(index, 0);
            }
            else if (index == 0)
            {
                Assert.LessOrEqual(value, items[0]);
            }
            else if (index == items.Count)
            {
                Assert.Less(items[index - 1], value);
            }
            else
            {
                Assert.LessOrEqual(value, items[index]);
                Assert.Less(items[index - 1], value);
            }
        }
    }
}
