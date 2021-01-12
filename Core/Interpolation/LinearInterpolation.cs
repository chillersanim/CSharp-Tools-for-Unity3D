// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         LinearInterpolation.cs
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
using Debug = System.Diagnostics.Debug;

namespace Unity_Tools.Core
{
    public class LinearInterpolation : IInterpolation<float>, IDisposable
    {
        private List<Vector2> points;

        public LinearInterpolation(params Vector2[] dataPoints) : this((IList<Vector2>)dataPoints)
        {
        }

        public LinearInterpolation(IList<Vector2> dataPoints)
        {
            if (dataPoints == null)
            {
                throw new ArgumentNullException(nameof(dataPoints));
            }

            if (dataPoints.Count == 0)
            {
                throw new ArgumentException("At least one data point is required.", nameof(dataPoints));
            }

            this.points = GlobalListPool<Vector2>.Get(dataPoints.Count);

            this.points.AddRange(dataPoints);
            this.points.Sort((a, b) => a.x.CompareTo(b.x));

            this.Range = new Interval<float>(points[0].x, points[points.Count - 1].x);
        }

        public Interval<float> Range { get; }

        public void Dispose()
        {
            if (points == null)
            {
                return;
            }

            // Allow the internal list to be reused
            GlobalListPool<Vector2>.Put(points);

            points = null;
        }

        public float Eval(float t)
        {
            if (points == null)
            {
                throw new ObjectDisposedException("The linear interpolation has already been disposed.");
            }

            if (points.Count == 1)
            {
                return points[0].y;
            }

            if (t < Range)
            {
                return points[0].y;
            }

            if (t > Range)
            {
                return points[points.Count - 1].y;
            }

            var segment = GetSegment(t);

            Debug.Assert(segment >= 0 && segment <= points.Count - 2);

            var p0 = points[segment];
            var p1 = points[segment + 1];

            return p0.y + (p1.y - p0.y) * ((t - p0.x) / (p1.x - p0.x));
        }

        private int GetSegment(float t)
        {
            Debug.Assert(Range.Contains(t));
            return points.BinarySearchLocation(t, (p, v) => p.x.CompareTo(v));
        }

        ~LinearInterpolation()
        {
            Dispose();
        }
    }
}
