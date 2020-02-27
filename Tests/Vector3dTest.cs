// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         Vector3dTest.cs
// 
// Created:          08.01.2020  11:48
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

using System;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityTools.Core;

namespace UnityTools.Tests
{
    /// <summary>
    /// Test class for vector3d tests.
    /// </summary>
    public class Vector3dTest
    {
        [NotNull]
        private readonly double[] values =
        {
            0, 1, -1, 2, -2, Math.PI, -Math.PI, double.NaN, double.NegativeInfinity,
            double.PositiveInfinity, 42.424242, -42.424242, 0.1, -0.1, 33.3333,
            -33.3333, 66.6666, -66.6666, 1234, -1234, 1337.42, -1337.42,
            double.Epsilon, -double.Epsilon
        };

        [Test]
        public void Addition()
        {
            for (var i = 0; i < this.values.Length; i++)
            {
                var l0 = this.values[i];
                var l1 = this.values[(i + 1) % this.values.Length];
                var l2 = this.values[(i + 2) % this.values.Length];

                for (var j = 0; j < this.values.Length; j++)
                {
                    var r0 = this.values[j];
                    var r1 = this.values[(j + 1) % this.values.Length];
                    var r2 = this.values[(j + 2) % this.values.Length];

                    var v0 = new Vector3d(l0, l1, l2);
                    var v1 = new Vector3d(r0, r1, r2);
                    var r = v0 + v1;

                    Assert.AreEqual(v0.X + v1.X, r.X);
                    Assert.AreEqual(v0.Y + v1.Y, r.Y);
                    Assert.AreEqual(v0.Z + v1.Z, r.Z);
                }
            }
        }

        [Test]
        public void Scale()
        {
            for (var i = 0; i < this.values.Length; i++)
            {
                var l0 = this.values[i];
                var l1 = this.values[(i + 1) % this.values.Length];
                var l2 = this.values[(i + 2) % this.values.Length];

                for (var j = 0; j < this.values.Length; j++)
                {
                    var scalar = this.values[j];

                    var vector = new Vector3d(l0, l1, l2);
                    var result0 = vector * scalar;
                    var result1 = scalar * vector;

                    Assert.AreEqual(vector.X * scalar, result0.X);
                    Assert.AreEqual(vector.Y * scalar, result0.Y);
                    Assert.AreEqual(vector.Z * scalar, result0.Z);

                    Assert.AreEqual(vector.X * scalar, result1.X);
                    Assert.AreEqual(vector.Y * scalar, result1.Y);
                    Assert.AreEqual(vector.Z * scalar, result1.Z);
                }
            }
        }

        [Test]
        public void Subtraction()
        {
            for (var i = 0; i < this.values.Length; i++)
            {
                var l0 = this.values[i];
                var l1 = this.values[(i + 1) % this.values.Length];
                var l2 = this.values[(i + 2) % this.values.Length];

                for (var j = 0; j < this.values.Length; j++)
                {
                    var r0 = this.values[j];
                    var r1 = this.values[(j + 1) % this.values.Length];
                    var r2 = this.values[(j + 2) % this.values.Length];

                    var v0 = new Vector3d(l0, l1, l2);
                    var v1 = new Vector3d(r0, r1, r2);
                    var r = v0 - v1;

                    Assert.AreEqual(v0.X - v1.X, r.X);
                    Assert.AreEqual(v0.Y - v1.Y, r.Y);
                    Assert.AreEqual(v0.Z - v1.Z, r.Z);
                }
            }
        }
    }
}
