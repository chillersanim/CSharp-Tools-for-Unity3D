// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PolynomialInterpolation2.cs
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
using UnityEngine;
using UnityTools.Pooling;

namespace UnityTools.Core
{
    /// <summary>
    /// An alternative polynomial interpolation, with explicit coefficients. Faster evaluation but much slower initialization.
    /// Degree k for k+1 data points.
    /// </summary>
    /// <remarks>
    /// Cost:
    ///  - Initial:     O(k^3)
    ///  - Eval:        O(k)
    /// </remarks>
    public class PolynomialInterpolation2 : IInterpolation<float>, IDisposable
    {
        /// <summary>
        /// The polynomial coefficients in order: f(t) = coeff[0] * t^k + coeff[1] * t^(k - 1) + ... + coeff[k-1] * t + coeff[k]
        /// </summary>
        private List<float> coeff;

        public PolynomialInterpolation2(params Vector2[] dataPoints) : this((IList<Vector2>)dataPoints)
        {
        }

        public PolynomialInterpolation2(IList<Vector2> dataPoints)
        {
            if (dataPoints == null)
            {
                throw new ArgumentNullException(nameof(dataPoints));
            }

            if (dataPoints.Count < 1)
            {
                throw new ArgumentException("At least one data point is required for a polynomial interpolation.", nameof(dataPoints));
            }

            this.coeff = GlobalListPool<float>.Get(dataPoints.Count);
            
            Prepare(dataPoints);
        }

        public void Dispose()
        {
            if (this.coeff == null)
            {
                return;
            }

            GlobalListPool<float>.Put(this.coeff);
            this.coeff = null;
        }

        public float Eval(float t)
        {
            if (this.coeff == null)
            {
                throw new ObjectDisposedException(null, "The polynomial interpolation container cannot be used after being disposed.");
            }

            var result = coeff[coeff.Count - 1];
            var tPowI = t;

            // Apply Horner scheme to efficiently evaluate the polynomial
            for (var i = coeff.Count - 2; i >= 0; i--)
            {
                result += tPowI * coeff[i];
                tPowI *= t;
            }

            return result;
        }

        private float GetNProductSum(IList<float> p, int n)
        {
            var result = 0f;

            var last = p.Count - 1;
            var indices = GlobalListPool<int>.Get(n);
            var values = GlobalListPool<float>.Get(n - 1);
            var steps = 0;      // Used for loop invariant to test implementation

            for (var k = 0; k < n; k++)
            {
                steps += k * (k + 1) / 2;
                indices.Add(k);

                if (k < n - 1)
                {
                    values.Add(p[k]);

                    if (k > 0)
                    {
                        values[k] *= values[k - 1];
                    }
                }
            }

            while (true)
            {
                Debug.Assert(steps > 0, "Loop invariant is broken, to many calculation steps needed for this calculation...");

                steps--;

                // For every index constellation, we need to sum up the products
                result += values[n - 2] * p[indices[n - 1]];

                // Shift last index until we reach the end
                if(indices[n - 1] < last)
                {
                    indices[n - 1]++;
                }
                else
                {
                    // Remember whether we were able to shift and index
                    var couldShift = false;

                    // Find first free index to shift
                    for (var i = n - 2; i >= 0; i--)
                    {
                        // If index can be shifted
                        if (indices[i] < indices[i + 1] - 1)
                        {
                            var prev = indices[i];

                            // Shift index and update precalculated values
                            for (var k = i; k < n; k++)
                            {
                                indices[k] = prev + k - i + 1;

                                if (k < n - 1)
                                {
                                    values[k] = p[indices[k]];

                                    if (k > 0)
                                    {
                                        values[k] *= values[k - 1];
                                    }
                                }
                            }

                            couldShift = true;
                            break;
                        }
                    }

                    if (!couldShift)
                    {
                        // If we reached the end, stop computing
                        break;
                    }
                }
            }

            Debug.Assert(steps == 0, "Loop invariant is broken, not enough calculation steps used for this calculation...");

            return result;
        }

        private void Prepare(IList<Vector2> p)
        {
            var cnt = p.Count;
            var tValues = GlobalListPool<float>.Get(cnt - 1);
            var w = PrepareWeights(p);
            
            var cof = 0f;

            for (var i = 0; i < cnt; i++)
            {
                coeff.Add(0f);
            }

            for (var i = 1; i < cnt; i++)
            {
                tValues.Add(p[i].x);
            }

            // First coefficient
            for (var i = 0; i < cnt; i++)
            {
                cof += p[i].y / w[i];
            }

            coeff[0] = cof;

            // Later coefficients
            for (var i = 0; i < cnt; i++)
            {
                var scale = p[i].y / w[i];

                for (var j = 1; j < cnt; j++)
                {
                    var sum = GetNProductSum(tValues, j);

                    coeff[j] += sum * scale;
                }

                if (i < cnt - 1)
                {
                    tValues[i] = p[i].x;
                }
            }
        }

        private List<float> PrepareWeights(IList<Vector2> p)
        {
            var cnt = p.Count;
            var weights = GlobalListPool<float>.Get(cnt);

            for (var i = 0; i < cnt; i++)
            {
                var ti = p[i].x;
                var wi = 1f;

                for (var j = 0; j < cnt; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    var tj = p[j].x;
                    wi *= ti - tj;
                }

                weights.Add(wi);
            }

            return weights;
        }

        ~PolynomialInterpolation2()
        {
            Dispose();
        }
    }
}
