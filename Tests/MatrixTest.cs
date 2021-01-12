// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         MatrixTest.cs
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
using NUnit.Framework;
using Unity_Tools.Core;

namespace Unity_Tools.Tests
{
    public class MatrixTest
    {
        [Test]
        public void Addition()
        {
            for (var i = 1; i < 10; i++)
            {
                for (var j = 1; j < 10; j++)
                {
                    var data1 = TestData.GenerateTestDoubles(i * j, false, false, 7);
                    var data2 = TestData.GenerateTestDoubles(i * j, false, false, 19);

                    var m1 =MatrixFactory.Build(i, j, data1);
                    var m2 =MatrixFactory.Build(i, j, data2);

                    Assert.IsNotNull(m1);
                    Assert.IsNotNull(m2);
                    Assert.AreEqual(m1.Rows, i);
                    Assert.AreEqual(m1.Columns, j);
                    Assert.AreEqual(m2.Rows, i);
                    Assert.AreEqual(m2.Columns, j);

                    var m3 = m1 + m2;
                    var m4 = m2 + m1;

                    Assert.IsNotNull(m3);
                    Assert.IsNotNull(m4);
                    Assert.AreEqual(m3.Rows, i);
                    Assert.AreEqual(m3.Columns, j);
                    Assert.AreEqual(m4.Rows, i);
                    Assert.AreEqual(m4.Columns, j);
                    Assert.AreEqual(m3, m4);
                    Assert.IsFalse(ReferenceEquals(m3, m4));

                    for (var r = 0; r < i; r++)
                    {
                        for (var c = 0; c < j; c++)
                        {
                            var index = r * j + c;
                            var value = data1[index] + data2[index];
                            Assert.AreEqual(m3[r, c], value, 1e-9);
                        }
                    }
                }
            }
        }

        [Test]
        public void Generation()
        {
            for (var i = 1; i < 10; i++)
            {
                // *** Square matrices ***
                var diagData = TestData.GenerateTestDoubles(i);
                var diagonaleMatrix =MatrixFactory.Diagonal(diagData);
                Assert.IsTrue(diagonaleMatrix != null);
                Assert.IsTrue(diagonaleMatrix.Rows == i);
                Assert.IsTrue(diagonaleMatrix.Columns == i);

                var idMatrix =MatrixFactory.IdentityD(i, Math.PI);
                Assert.IsTrue(idMatrix != null);
                Assert.IsTrue(idMatrix.Rows == i);
                Assert.IsTrue(idMatrix.Columns == i);

                // Test square matrices
                for (var r = 0; r < i; r++)
                {
                    for (var c = 0; c < i; c++)
                    {
                        Assert.AreEqual(diagonaleMatrix[r, c], r == c ? diagData[r] : 0);
                        Assert.AreEqual(idMatrix[r, c], r == c ? Math.PI : 0, 1e-9);
                    }
                }

                // *** Non-square matrices ***
                for (var j = 1; j < 10; j++)
                {
                    var data = TestData.GenerateTestDoubles(i * j);
                    var matrix =MatrixFactory.Build(i, j, data);

                    Assert.IsTrue(matrix != null);
                    Assert.IsTrue(matrix.Rows == i);
                    Assert.IsTrue(matrix.Columns == j);

                    var zeroMatrix =MatrixFactory.ZeroD(i, j);
                    Assert.IsTrue(zeroMatrix != null);
                    Assert.IsTrue(zeroMatrix.Rows == i);
                    Assert.IsTrue(zeroMatrix.Columns == j);

                    var oneMatrix = MatrixFactory.OneD(i, j, Math.PI);
                    Assert.IsTrue(oneMatrix != null);
                    Assert.IsTrue(oneMatrix.Rows == i);
                    Assert.IsTrue(oneMatrix.Columns == j);

                    if (i == 4 && j == 4)
                    {
                        Assert.IsInstanceOf(typeof(Matrix4x4d), matrix);
                    }
                    
                    for (var r = 0; r < i; r++)
                    {
                        for (var c = 0; c < j; c++)
                        {
                            Assert.AreEqual(matrix[r, c], data[r * j + c], 1e-9);
                            Assert.AreEqual(zeroMatrix[r, c], 0, 1e-9);
                            Assert.AreEqual(oneMatrix[r, c], Math.PI, 1e-9);
                        }
                    }
                }
            }
        }

        [Test]
        public void Multiplication()
        {
            for (var i = 1; i < 10; i++)
            {
                for (var j = 1; j < 10; j++)
                {
                    for (var k = 1; k < 10; k++)
                    {
                        var data1 = TestData.GenerateTestDoubles(i * j, false, false, 7);
                        var data2 = TestData.GenerateTestDoubles(j * k, false, false, 19);

                        var m1 = MatrixFactory.Build(i, j, data1);
                        var m2 = MatrixFactory.Build(j, k, data2);

                        var m3 = m1 * m2;

                        Assert.IsNotNull(m3);
                        Assert.AreEqual(m3.Rows, i);
                        Assert.AreEqual(m3.Columns, k);

                        for (var r = 0; r < i; r++)
                        {
                            for (var c = 0; c < k; c++)
                            {
                                var value = m3[r, c];
                                var m1Row = m1.Row(r);
                                var m2Col = m2.Column(c);
                                Assert.AreEqual(value, m1Row.Dot(m2Col));
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void Scale()
        {
            for (var i = 1; i < 10; i++)
            {
                for (var j = 1; j < 10; j++)
                {
                    var data1 = TestData.GenerateTestDoubles(i * j, false, false, 7);
                    var data2 = TestData.GenerateTestDoubles(10, false, false, 19);

                    var m1 =MatrixFactory.Build(i, j, data1);

                    Assert.IsNotNull(m1);
                    Assert.AreEqual(m1.Rows, i);
                    Assert.AreEqual(m1.Columns, j);

                    for (var k = 0; k < data2.Length; k++)
                    {
                        var m2 = m1 * data2[k];
                        var m3 = data2[k] * m1;

                        Assert.IsNotNull(m2);
                        Assert.IsNotNull(m3);
                        Assert.AreEqual(m2.Rows, i);
                        Assert.AreEqual(m2.Columns, j);
                        Assert.AreEqual(m3.Rows, i);
                        Assert.AreEqual(m3.Columns, j);
                        Assert.AreEqual(m2, m3);
                        Assert.IsFalse(ReferenceEquals(m2, m3));

                        for (var r = 0; r < i; r++)
                        {
                            for (var c = 0; c < j; c++)
                            {
                                var index = r * j + c;
                                var value = data1[index] * data2[k];
                                Assert.AreEqual(m2[r, c], value, 1e-9);
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void Subtraction()
        {
            for (var i = 1; i < 10; i++)
            {
                for (var j = 1; j < 10; j++)
                {
                    var data1 = TestData.GenerateTestDoubles(i * j, false, false, 7);
                    var data2 = TestData.GenerateTestDoubles(i * j, false, false, 19);

                    var m1 =MatrixFactory.Build(i, j, data1);
                    var m2 =MatrixFactory.Build(i, j, data2);

                    Assert.IsNotNull(m1);
                    Assert.IsNotNull(m2);
                    Assert.AreEqual(m1.Rows, i);
                    Assert.AreEqual(m1.Columns, j);
                    Assert.AreEqual(m2.Rows, i);
                    Assert.AreEqual(m2.Columns, j);

                    var m3 = m1 - m2;
                    var m4 = m2 - m1;

                    Assert.IsNotNull(m3);
                    Assert.IsNotNull(m4);
                    Assert.AreEqual(m3.Rows, i);
                    Assert.AreEqual(m3.Columns, j);
                    Assert.AreEqual(m4.Rows, i);
                    Assert.AreEqual(m4.Columns, j);
                    Assert.IsFalse(ReferenceEquals(m3, m4));

                    for (var r = 0; r < i; r++)
                    {
                        for (var c = 0; c < j; c++)
                        {
                            var index = r * j + c;
                            Assert.AreEqual(m3[r, c], data1[index] - data2[index], 1e-9);
                            Assert.AreEqual(m4[r, c], data2[index] - data1[index], 1e-9);
                        }
                    }
                }
            }
        }
    }
}
