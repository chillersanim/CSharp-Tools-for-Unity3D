// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         I3DCollectionTest.cs
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity_Tools.Collections;
using Unity_Tools.Core;
using UnityEngine;

namespace Unity_Tools.Tests
{
    public abstract class I3DCollectionTest<T>
    {
        [Test]
        public void InsertionTest([NUnit.Framework.Range(0, 1000, 250)]int pointAmount)
        {
            GenerateTestData(pointAmount, out var instance, out var items, out _, out _);
            var dict = items.ToDictionary(item => item.Key, item => item.Value);

            Assert.AreEqual(items.Count, instance.Count);
            var enumerator = instance.GetEnumerator();
            Assert.NotNull(enumerator, "enumerator != null");

            // Test that the content of the tree matches the items added
            var instanceItems = enumerator.ToArray();
            TestForSetEquality(instanceItems, dict);
            Assert.IsTrue(items.All(item => instance.Contains(item.Key, item.Value)));
        }

        [Test]
        public void MoveTest([NUnit.Framework.Range(0, 1000, 250)]int pointAmount, [Values(0.1f, 0.5f, 0.9f, 1f)]float changePercent)
        {
            GenerateTestData(pointAmount, out var instance, out var items, out var origin, out var size);
            var dict = items.ToDictionary(item => item.Key, item => item.Value);

            Assert.AreEqual(items.Count, instance.Count);
            var enumerator = instance.GetEnumerator();
            Assert.NotNull(enumerator, "enumerator != null");

            var skip = Mathf.RoundToInt(1f / changePercent);
            var halfSize = size / 2f;
            for (var i = 0; i < items.Count; i += skip)
            {
                var item = items[i];
                var newPos = RandomInAabb(origin, halfSize);
                items[i] = new KeyValuePair<T, Vector3>(item.Key, newPos);
                dict[item.Key] = newPos;
                Assert.IsTrue(instance.MoveItem(item.Key, item.Value, newPos));
            }

            // Test that the content of the tree matches the items added
            var instanceItems = enumerator.ToArray();
            TestForSetEquality(instanceItems, dict);
            Assert.IsTrue(items.All(item => instance.Contains(item.Key, item.Value)));
        }

        [Test]
        public void RemovalTest([NUnit.Framework.Range(0, 1000, 250)]int pointAmount)
        {
            GenerateTestData(pointAmount, out var instance, out var items, out _, out _);

            Assert.AreEqual(items.Count, instance.Count);
            var enumerator = instance.GetEnumerator();
            Assert.NotNull(enumerator, "enumerator != null");

            foreach (var item in items)
            {
                Assert.IsTrue(instance.Remove(item.Key, item.Value));
            }
        }

        [Test]
        public void SphereCastTest([NUnit.Framework.Range(0, 1000, 250)] int pointAmount)
        {
            GenerateTestData(pointAmount, out var instance, out var items, out var origin, out var size);

            for (var i = 0; i < 10; i++)
            {
                var center = RandomInAabb(origin, size);
                var radius = Random.Range(0.1f, size.magnitude);
                var sqrRadius = radius * radius;

                var castResult = instance.FindInRadius(center, radius);
                var reference = items.Where(item => (item.Value - center).sqrMagnitude <= sqrRadius)
                    .ToDictionary(item => item.Key, item => item.Value);

                TestForSetEquality(castResult, reference);
            }
        }

        [Test]
        public void AabbCastTest([NUnit.Framework.Range(0, 1000, 250)] int pointAmount)
        {
            GenerateTestData(pointAmount, out var instance, out var items, out var origin, out var size);

            for (var i = 0; i < 10; i++)
            {
                var aabbOrigin = RandomInAabb(origin, size);
                var aabbSize = RandomInUnitAabb() * 100f;

                var castResult = instance.FindInBounds(new Bounds(aabbOrigin, aabbSize));
                var reference = items.Where(item => item.Value.IsInAabb(aabbOrigin, aabbSize))
                    .ToDictionary(item => item.Key, item => item.Value);

                TestForSetEquality(castResult, reference);
            }
        }

        protected Vector3 RandomInUnitAabb()
        {
            var x = Random.value * 2f - 1f;
            var y = Random.value * 2f - 1f;
            var z = Random.value * 2f - 1f;

            return new Vector3(x, y, z);
        }

        protected Vector3 RandomInAabb(Vector3 origin, Vector3 halfSize)
        {
            var x = Random.value * 2f - 1f;
            var y = Random.value * 2f - 1f;
            var z = Random.value * 2f - 1f;

            return new Vector3(x, y, z).ScaleComponents(halfSize) + origin;
        }

        protected void GenerateTestData(int pointAmount, out IPoint3DCollection<T> instance,
            out IList<KeyValuePair<T, Vector3>> items, out Vector3 origin, out Vector3 size)
        {
            var randomSeed = 0;
            Random.InitState(randomSeed);

            instance = CreateInstance();
            Assert.NotNull(instance, "instance != null");

            origin = RandomInUnitAabb() * 100f;
            size = RandomInUnitAabb().AbsComponents() * 100f;
            var halfSize = size / 2f;

            items = new List<KeyValuePair<T, Vector3>>();

            for (var i = 0; i < pointAmount; i++)
            {
                var vector = RandomInAabb(origin, halfSize);
                Assert.True(vector.IsInAabb(origin, size), $"{vector}.IsInAabb({origin}, {size})");
                var item = GetItem(i);
                Assert.NotNull(item, "item != null");
                items.Add(new KeyValuePair<T, Vector3>(item, vector));
                instance.Add(item, vector);
            }
        }

        protected void TestForSetEquality(IList<T> items, Dictionary<T, Vector3> reference)
        {
            Assert.IsTrue(items.Count == reference.Count);

            foreach (var item in items)
            {
                Assert.IsTrue(reference.ContainsKey(item));
                reference.Remove(item);
            }

            Assert.IsTrue(reference.Count == 0);
        }

        protected abstract IPoint3DCollection<T> CreateInstance();

        protected abstract T GetItem(int i);
    }
}
