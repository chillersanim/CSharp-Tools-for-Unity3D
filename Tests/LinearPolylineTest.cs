using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using Unity_Tools.Collections;
using Unity_Tools.Core;
using Unity_Tools.Polyline;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Unity_Tools.Tests
{
    public class LinearPolylineTest
    {
        [Test]
        public void CreationTest([Values(0, 1, 2, 3, 4, 5, 10, 100, 1000, 10000)] int pointAmount)
        {
            CreateTestData(pointAmount, out var points, out var length);

            var polyline = new LinearPolyline();
            Assert.AreEqual(polyline.Length, 0f, float.Epsilon);

            polyline = new LinearPolyline(points);
            Assert.AreEqual(polyline.Length, length, float.Epsilon);
        }

        [Test]
        public void AddTest([Values(0, 1, 2, 3, 4, 5, 10, 100, 1000, 10000)] int pointAmount)
        {
            CreateTestData(pointAmount, out var points, out var length);

            var polyline = new LinearPolyline();

            foreach (var p in points)
            {
                polyline.Add(p);
            }

            Assert.AreEqual(polyline.Length, length, float.Epsilon);
        }

        [Test]
        public void InsertionTest([Values(0, 1, 2, 3, 4, 5, 10, 100, 1000)] int pointAmount)
        {
            CreateTestData(pointAmount, out var points, out _);

            var polyline = new LinearPolyline();
            var randomPoints = new List<Vector3>(points);
            randomPoints.Shuffle();

            var insertedPoints = new List<Vector3>();

            while (randomPoints.Count > 0)
            {
                var point = randomPoints[randomPoints.Count - 1];
                randomPoints.RemoveAt(randomPoints.Count - 1);

                var index = Random.Range(0, insertedPoints.Count + 1);
                insertedPoints.Insert(index, point);
                polyline.Insert(index, point);
            }

            Assert.AreEqual(insertedPoints.Count, points.Length);
            Assert.AreEqual(polyline.Count, insertedPoints.Count);

            var length = 0.0;
            for (var i = 1; i < insertedPoints.Count; i++)
            {
                length += VectorMath.PreciseDistance(insertedPoints[i], insertedPoints[i - 1]);
            }

            if (pointAmount < 2 && Math.Abs(polyline.Length) < 1e-6f)
            {
                Assert.Pass();
            }

            Assert.AreEqual(polyline.Length, length, 1e-5, length.ToString(CultureInfo.InvariantCulture)); 
        }

        [Test]
        public void ReplacementTest([Values(0, 1, 2, 3, 4, 5, 10, 100, 1000)] int pointAmount)
        {
            CreateTestData(pointAmount, out var points, out _);
            CreateTestData(pointAmount, out var replacementPoints, out var replacementLength);

            var polyline = new LinearPolyline(points);

            var replacementOrder = CollectionUtil.Create(points.Length, 0, i => i + 1);
            replacementOrder.Shuffle();

            for (var i = 0; i < pointAmount; i++)
            {
                var index = replacementOrder[i];
                polyline[index] = replacementPoints[index];
            }

            if (pointAmount < 2 && Math.Abs(polyline.Length) < 1e-6f)
            {
                Assert.Pass();
            }

            Assert.AreEqual(polyline.Length / replacementLength, 1, 1e-5f, replacementLength.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void RemovalTest([Values(0, 1, 2, 3, 4, 5, 10, 100, 1000)] int pointAmount)
        {
            CreateTestData(pointAmount, out var points, out _);

            var polyline = new LinearPolyline(points);
            var currentPoints = new List<Vector3>(points);

            for (var i = 0; i < pointAmount; i++)
            {
                var index = Random.Range(0, currentPoints.Count);
                polyline.RemoveAt(index);
                currentPoints.RemoveAt(index);

                if (currentPoints.Count < 2)
                {
                    Assert.AreEqual(polyline.Length, 0, 1e-6);
                }
                else
                {
                    var length = 0f;
                    for (var j = 1; j < currentPoints.Count; j++)
                    {
                        length += (currentPoints[j] - currentPoints[j - 1]).magnitude;
                    }

                    Assert.AreEqual(polyline.Length / length, 1, 1e-5f, length.ToString(CultureInfo.InvariantCulture));
                }
            }
        }


        private void CreateTestData(int pointAmount, out Vector3[] points, out float length)
        {
            if (pointAmount == 0)
            {
                points = Array.Empty<Vector3>();
                length = 0;
                return;
            }

            points = new Vector3[pointAmount];
            var preciseLength = 0.0;

            var prev = Vector3.zero;
            points[0] = prev;

            for (var i = 1; i < pointAmount; i++)
            {
                var nextPoint = prev + Random.onUnitSphere * Random.Range(0, 10f);
                points[i] = nextPoint;
                preciseLength += (points[i] - points[i - 1]).magnitude;
            }

            length = (float) preciseLength;
        }
    }
}
