// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PolynomialInterpolation.cs
// 
// Created:          27.01.2020  22:49
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
using System.Collections.Generic;
using Unity_Tools.Pooling;
using UnityEngine;

namespace Unity_Tools.Core
{
    /// <summary>
    /// A polynomial interpolation using uses a lagrange interpolation polynomial with barycentric interpolation formula for efficient point evaluation.
    /// Degree k for k+1 data points, .
    /// </summary>
    /// <remarks>
    /// Cost:
    ///  - Initial:     O(k^2)
    ///  - Eval:        O(k)
    /// </remarks>
    public class PolynomialInterpolation : IInterpolation<float>, IDisposable
    {
        /// <summary>
        /// Stores the data points and their precalculated weights (p.X, p.Y, weight)
        /// </summary>
        private List<Vector3> data;

        public PolynomialInterpolation(params Vector2[] dataPoints) : this((IList<Vector2>)dataPoints)
        {
        }

        public PolynomialInterpolation(IList<Vector2> dataPoints)
        {
            if (dataPoints == null)
            {
                throw new ArgumentNullException(nameof(dataPoints));
            }

            if (dataPoints.Count < 1)
            {
                throw new ArgumentException("At least one data point is required for a polynomial interpolation.", nameof(dataPoints));
            }

            this.data = GlobalListPool<Vector3>.Get(dataPoints.Count);
            
            // Point order doesn't matter, so don't sort input
            PrepareData(dataPoints);
        }

        public void Dispose()
        {
            if (this.data == null)
            {
                return;
            }

            GlobalListPool<Vector3>.Put(this.data);
            this.data = null;
        }

        public float Eval(float t)
        {
            if (this.data == null)
            {
                throw new ObjectDisposedException(null, "The polynomial interpolation container cannot be used after being disposed.");
            }

            var c = data.Count;
            var a = 0f;
            var b = 0f;

            for (var i = 0; i < c; i++)
            {
                var xyw = data[i];
                var wDeltaT = xyw.z / (t - xyw.x);
                a += wDeltaT * xyw.y;
                b += wDeltaT;
            }

            return a / b;
        }

        private void PrepareData(IList<Vector2> points)
        {
            var cnt = points.Count;

            for (var i = 0; i < cnt; i++)
            {
                var p = points[i];
                var ti = p.x;
                var wi = 1f;

                for (var j = 0; j < cnt; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    var tj = points[j].x;
                    wi *= ti - tj;
                }

                data.Add(new Vector3(p.x, p.y, wi));
            }
        }

        ~PolynomialInterpolation()
        {
            Dispose();
        }
    }
}
