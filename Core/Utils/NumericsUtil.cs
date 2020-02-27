// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         NumericsUtil.cs
// 
// Created:          29.01.2020  19:31
// Last modified:    05.02.2020  19:39
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
using UnityEngine;

namespace UnityTools.Core
{
    public static class NumericsUtil
    {
        public static Func<float, float> ApproximateDerivative(this Func<float, float> f, float epsilon = 0.01f)
        {
            return t =>
            {
                var a = f(t - epsilon);
                var b = f(t + epsilon);

                return (b - a) / (2f * epsilon);
            };
        }

        public static Func<double, double> ApproximateDerivative(this Func<double, double> f, double epsilon = 0.0001)
        {
            return t =>
            {
                var a = f(t - epsilon);
                var b = f(t + epsilon);

                return (b - a) / (2.0 * epsilon);
            };
        }

        public static Func<float, Vector2> ApproximateDerivative(this Func<float, Vector2> f, float epsilon = 0.01f)
        {
            return t =>
            {
                var a = f(t - epsilon);
                var b = f(t + epsilon);

                return (b - a) / (2f * epsilon);
            };
        }

        public static Func<float, Vector3> ApproximateDerivative(this Func<float, Vector3> f, float epsilon = 0.01f)
        {
            return t =>
            {
                var a = f(t - epsilon);
                var b = f(t + epsilon);

                return (b - a) / (2f * epsilon);
            };
        }

        public static Func<float, Vector4> ApproximateDerivative(this Func<float, Vector4> f, float epsilon = 0.01f)
        {
            return t =>
            {
                var a = f(t - epsilon);
                var b = f(t + epsilon);

                return (b - a) / (2f * epsilon);
            };
        }
    }
}
