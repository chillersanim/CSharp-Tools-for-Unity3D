// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         I3DCollectionTest.cs
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

using System.Collections.Generic;
using System.Diagnostics;
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
        public void AabbCastPerformanceTest([NUnit.Framework.Range(0, 100000, 100000)]
            int pointAmount)
        {
            GenerateTestCollection(pointAmount, out var instance, out var items, out var origin, out var size);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var cnt = 0;

            for (var i = 0; i < 100; i++)
            {
                var sphere = new Aabb(RandomInAabb(origin, size), RandomInUnitAabb() * size.magnitude);

                foreach (var item in instance.ShapeCast(sphere))
                {
                    cnt++;
                }
            }

            stopwatch.Stop();
            Assert.Pass($"Sphere cast time: {stopwatch.ElapsedMilliseconds / 100f} ms ({stopwatch.ElapsedTicks / 100f} ticks) Count: {cnt}\n");
        }

        [Test]
        public void AabbCastTest([NUnit.Framework.Range(0, 1000, 250)] int pointAmount)
        {
            GenerateTestCollection(pointAmount, out var instance, out var items, out var origin, out var size);

            for (var i = 0; i < 10; i++)
            {
                var aabb = new Aabb(RandomInAabb(origin, size), RandomInUnitAabb() * 100f);

                var castResult = instance.ShapeCast(aabb).ToArray();
                var reference = items.Where(item => aabb.ContainsPoint(item.Value))
                    .ToDictionary(item => item.Key, item => item.Value);

                TestForSetEquality(castResult, reference);
            }
        }

        [Test]
        public void InsertionPerformanceTest([NUnit.Framework.Range(0, 10000, 10000)]int pointAmount)
        {
            GenerateTestData(pointAmount, out var items, out var origin, out var size);
            var collection = CreateInstance(origin, size, pointAmount);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var item in items)
            {
                collection.Add(item.Key, item.Value);
            }

            stopwatch.Stop();

            Assert.Pass($"Insertion time: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks} ticks)\n");
        }

        [Test]
        public void InsertionTest([NUnit.Framework.Range(0, 1000, 250)]int pointAmount)
        {
            GenerateTestCollection(pointAmount, out var instance, out var items, out _, out _);
            var dict = items.ToDictionary(item => item.Key, item => item.Value);

            Assert.AreEqual(items.Count, instance.Count);
            var enumerator = instance.GetEnumerator();
            Assert.NotNull(enumerator, "enumerator != null");

            // Test that the content of the tree matches the items added
            var instanceItems = enumerator.ToArray();
            TestForSetEquality(instanceItems, dict);
            Assert.IsTrue(items.All(item => instance.Contains(item.Key, item.Value)));
            TestIntegrity(instance);
        }

        [Test]
        public void MoveTest([NUnit.Framework.Range(0, 1000, 250)]int pointAmount, [Values(0.1f, 0.5f, 0.9f, 1f)]float changePercent)
        {
            GenerateTestCollection(pointAmount, out var instance, out var items, out var origin, out var size);
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
            TestIntegrity(instance);
        }

        [Test]
        public void RemovalPerformanceTest([NUnit.Framework.Range(0, 10000, 10000)]int pointAmount)
        {
            GenerateTestData(pointAmount, out var items, out var origin, out var size);
            var collection = CreateInstance(origin, size, pointAmount);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var item in items)
            {
                collection.Remove(item.Key, item.Value);
            }

            stopwatch.Stop();

            Assert.Pass($"Removal time: {stopwatch.ElapsedMilliseconds} ms ({stopwatch.ElapsedTicks} ticks)\n");
        }

        [Test]
        public void RemovalTest([NUnit.Framework.Range(0, 1000, 250)]int pointAmount)
        {
            GenerateTestCollection(pointAmount, out var instance, out var items, out _, out _);

            Assert.AreEqual(items.Count, instance.Count);
            var enumerator = instance.GetEnumerator();
            Assert.NotNull(enumerator, "enumerator != null");

            foreach (var item in items)
            {
                Assert.IsTrue(instance.Remove(item.Key, item.Value));
            }

            TestIntegrity(instance);
        }

        [Test]
        public void SphereCastPerformanceTest([NUnit.Framework.Range(0, 100000, 100000)]
            int pointAmount)
        {
            GenerateTestCollection(pointAmount, out var instance, out var items, out var origin, out var size);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var cnt = 0;

            for (var i = 0; i < 100; i++)
            {
                var sphere = new Sphere(RandomInAabb(origin, size), Random.Range(0.1f, size.magnitude));

                foreach (var item in instance.ShapeCast(sphere))
                {
                    cnt++;
                }
            }

            stopwatch.Stop();
            Assert.Pass($"Sphere cast time: {stopwatch.ElapsedMilliseconds / 100f} ms ({stopwatch.ElapsedTicks / 100f} ticks) Count: {cnt}\n");
        }

        [Test]
        public void SphereCastTest([NUnit.Framework.Range(0, 1000, 250)] int pointAmount)
        {
            GenerateTestCollection(pointAmount, out var instance, out var items, out var origin, out var size);

            for (var i = 0; i < 10; i++)
            {
                var sphere = new Sphere(RandomInAabb(origin, size), Random.Range(0.1f, size.magnitude));

                var castResult = instance.ShapeCast(sphere).ToArray();
                var reference = items.Where(item => sphere.ContainsPoint(item.Value))
                    .ToDictionary(item => item.Key, item => item.Value);

                TestForSetEquality(castResult, reference); 
            }
        }

        protected abstract IPoint3DCollection<T> CreateInstance(Vector3 origin, Vector3 size, int capacity);

        protected void GenerateTestCollection(int pointAmount, out IPoint3DCollection<T> instance,
            out IList<KeyValuePair<T, Vector3>> items, out Vector3 origin, out Vector3 size)
        {
            GenerateTestData(pointAmount, out items, out origin, out size);

            instance = CreateInstance(origin, size, pointAmount);
            Assert.NotNull(instance, "instance != null");

            for (var i = 0; i < pointAmount; i++)
            {
                instance.Add(items[i].Key, items[i].Value); 
            }
        }

        protected void GenerateTestData(int pointAmount, out IList<KeyValuePair<T, Vector3>> items, out Vector3 origin, out Vector3 size)
        {
            var randomSeed = 0;
            Random.InitState(randomSeed);

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
            }
        }

        protected abstract T GetItem(int i);

        protected Vector3 RandomInAabb(Vector3 origin, Vector3 halfSize)
        {
            var x = Random.value * 2f - 1f;
            var y = Random.value * 2f - 1f;
            var z = Random.value * 2f - 1f;

            return new Vector3(x, y, z).ScaleComponents(halfSize) + origin;
        }

        protected Vector3 RandomInUnitAabb()
        {
            var x = Random.value * 2f - 1f;
            var y = Random.value * 2f - 1f;
            var z = Random.value * 2f - 1f;

            return new Vector3(x, y, z);
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

        protected abstract void TestIntegrity(IPoint3DCollection<T> collection);
    }
}
