// Solution:         Unity Tools
// Project:          UnityTools_Tests
// Filename:         TestData.cs
// 
// Created:          08.01.2020  11:50
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

namespace Unity_Tools.Tests
{
    public static class TestData
    {
        [NotNull]
        private static readonly double[] Data =
        {
            0, 1, 2, 3, 4, 5, -1, -2, -3, -4, -5, double.PositiveInfinity,
            double.NegativeInfinity, double.NaN, 0.1, 0.2, 0.3, 0.4, 0.5, -0.1,
            -0.2, -0.3, -0.4, -0.5, Math.PI, -Math.PI, Math.E, -Math.E, 100,
            -100, 1000, -1000, 10000, -10000, 100000, -100000, 1 / 3d, -1 / 3d,
            1 / 7d, -1 / 7d, 42.42424242, -42.42424242,
            33.3333333333333333333333333, -33.3333333333333333333333333,
            66.6666666666666666666666666, -66.6666666666666666666666666,
            double.PositiveInfinity, double.NegativeInfinity, double.NaN
        };

        [NotNull]
        public static double[] GenerateTestDoubles(
            int size,
            bool allowNaN = false,
            bool allowInf = true,
            int offset = 0)
        {
            if (size < 0)
            {
                size = -size;
            }

            if (size == 0)
            {
                return new double[0];
            }

            var result = new double[size];
            var dataIndex = offset;

            for (var i = 0; i < size; i++)
            {
                if (!allowInf)
                {
                    while (double.IsInfinity(Data[dataIndex]))
                    {
                        dataIndex = (dataIndex + 1) % Data.Length;
                    }
                }

                if (!allowNaN)
                {
                    while (double.IsNaN(Data[dataIndex]))
                    {
                        dataIndex = (dataIndex + 1) % Data.Length;
                    }
                }

                result[i] = Data[dataIndex];
                dataIndex = (dataIndex + 1) % Data.Length;
            }

            return result;
        }
    }
}