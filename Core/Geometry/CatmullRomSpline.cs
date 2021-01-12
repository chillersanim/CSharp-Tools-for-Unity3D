// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CatmullRomSpline.cs
// 
// Created:          29.01.2020  19:23
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools.Core
{
    public sealed class CatmullRomSpline : IPolyline, IList<Vector3>
    {
        private static readonly int Segmentation = 20;

        private readonly List<Vector3> points;

        private readonly List<float> segmentLengths;

        private readonly List<Segment> segments;

        private readonly List<Vector3> tangents;

        private float alpha;

        private float? length;

        private float tension;

        /// <summary>
        /// Initializes a new instance of the CatmullRomSpline type using the provided points as initial data set and a given alpha and tension.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="tension">The tension</param>
        public CatmullRomSpline(IEnumerable<Vector3> points, float alpha = 0.5f, float tension = 0f)
        {
            this.points = new List<Vector3>(points);
            this.tangents = new List<Vector3>(this.points.Count);
            this.segments = new List<Segment>(this.points.Count - 1);
            this.segmentLengths = new List<float>(this.points.Count - 1);
            this.alpha = alpha;
            this.tension = tension;

            this.length = null;
        }

        /// <summary>
        /// The alpha 
        /// </summary>
        public float Alpha
        {
            get => this.alpha;
            set
            {
                if (Mathf.Approximately(this.alpha, value))
                {
                    return;
                }

                this.alpha = value;
                this.segments.Clear();
                this.length = null;
            }
        }

        public int Count => this.points.Count;

        public bool IsReadOnly => false;

        public Vector3 this[int index]
        {
            get => this.points [index];
            set
            {
                var sqDist = (this.points[index] - value).sqrMagnitude;
                if(Mathf.Approximately(sqDist, 0))
                {
                    return;
                }

                this.points[index] = value;

                if (this.points.Count >= 2 && this.segments.Count > 0)
                {
                    var start = Mathf.Max(0, index - 2);
                    var end = Mathf.Min(this.segments.Count - 1, index + 1);

                    this.UpdateSegmentsLocally(start, end);
                    this.UpdateSegmentLengthsLocally(start, end);
                    this.length = null;
                }
            }
        }

        public IReadOnlyList<float> SegmentLengths => this.segmentLengths.AsReadOnly();

        public IReadOnlyList<Vector3> Tangents => this.tangents.AsReadOnly();

        public float Tension
        {
            get => this.tension;
            set
            {
                if (Mathf.Approximately(this.tension, value))
                {
                    return; 
                }

                this.tension = value;
                this.segments.Clear();
                this.length = null;
            }
        }

        public float Length
        {
            get
            {
                if (!this.length.HasValue)
                {
                    this.length = this.CalculateLength();
                }

                return this.length.Value;
            }
        }

        public void Add(Vector3 point)
        {
            this.Insert(this.points.Count, point);
        }

        public void Clear()
        {
            this.points.Clear();
            this.segments.Clear();
            this.segmentLengths.Clear();
            this.tangents.Clear();
            this.length = 0f;
        }

        public bool Contains(Vector3 point)
        {
            return this.points.Contains(point);
        }

        public void CopyTo(Vector3[] array, int arrayIndex)
        {
            this.points.CopyTo(array, arrayIndex);
        }

        public void GetSegmentData(List<Vector3> output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (this.points.Count < 2)
            {
                return;
            }

            if (this.segments.Count == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

            if (output.Capacity < output.Count + 4 * this.segments.Count)
            {
                output.Capacity = output.Count + 4 * this.segments.Count;
            }

            for (var i = 0; i < this.segments.Count; i++)
            {
                output.Add(this.segments[i].a);
                output.Add(this.segments[i].b);
                output.Add(this.segments[i].c);
                output.Add(this.segments[i].d);
            }
        }

        public Vector3[] GetSegmentData()
        {
            if (this.points.Count < 2)
            {
                return Array.Empty<Vector3>();
            }

            if (this.segments.Count == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

            var result = new Vector3[4 * this.segments.Count];

            for (var i = 0; i < this.segments.Count; i++)
            {
                result[4 * i + 0] = this.segments[i].a;
                result[4 * i + 1] = this.segments[i].b;
                result[4 * i + 2] = this.segments[i].c;
                result[4 * i + 3] = this.segments[i].d;
            }

            return result;
        }

        public int IndexOf(Vector3 point)
        {
            return this.points.IndexOf(point);
        }

        public void Insert(int index, Vector3 point)
        {
            if (index < 0 || index > this.points.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.points.Insert(index, point);

            if (this.points.Count >= 2 && this.segments.Count > 0)
            {
                var start = Mathf.Max(0, index - 2);
                var end = Mathf.Min(this.segments.Count - 1, index + 1);

                this.UpdateSegmentsLocally(start, end);
                this.UpdateSegmentLengthsLocally(start, end);
                this.length = null;
            }
        }

        public bool Remove(Vector3 point)
        {
            for (var i = 0; i < this.points.Count; i++)
            {
                if (this.points[i] == point)
                {
                    this.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.points.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.points.RemoveAt(index);

            if (this.points.Count >= 2 && this.segments.Count > 0)
            {
                var start = Mathf.Max(0, index - 2);
                var end = Mathf.Min(this.segments.Count - 1, index + 1);

                this.UpdateSegmentsLocally(start, end);
                this.UpdateSegmentLengthsLocally(start, end);
                this.length = null;
            }
            else
            {
                this.segments.Clear();
                this.segmentLengths.Clear();
            }
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            if(this.points.Count == 0)
            {
                throw new InvalidOperationException("The catmull rom spline needs at least one point to determine the closest point.");
            }

            if(this.points.Count == 1)
            {
                return this.points[0];
            }

            var minDist = float.PositiveInfinity;
            var minPos = 0f;
            var minSeg = 0;

            for (var i = 0; i < this.segments.Count; i++)
            {
                var dist = this.GetSegmentDistance(i, point, out var pos);

                if (dist < minDist)
                {
                    minDist = dist;
                    minPos = pos;
                    minSeg = i;
                }
            }

            if (minSeg < 0)
            {
                return this.points[0];
            }

            return this.GetSegmentPoint(minSeg, minPos);
        }

        public float ClosestPosition(Vector3 point)
        {
            if (this.points.Count < 2)
            {
                return 0f;
            }

            var minDist = float.PositiveInfinity;
            var minPos = 0f;
            var offset = 0f;

            for (var i = 0; i < this.segments.Count; i++)
            {
                var dist = this.GetSegmentDistance(i, point, out var pos);
                var segLength = this.segmentLengths[i];

                if (dist < minDist)
                {
                    minDist = dist;
                    minPos = offset + pos * segLength;
                }

                offset += segLength;
            }

            return minPos;
        }

        public Vector3 GetPoint(float position)
        {
            if (this.points.Count == 0)
            {
                return Vector3.zero;
            }

            if (this.points.Count == 1)
            {
                return this.points[0];
            }

            if (position < 0)
            {
                return this.points[0];
            }

            if (position > this.Length)
            {
                return this.points[this.points.Count - 1];
            }

            if (this.segments.Count == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

            Debug.Assert(this.segments.Count == this.points.Count - 1);
            Debug.Assert(this.segmentLengths.Count == this.segments.Count);

            for (var i = 0; i < this.segmentLengths.Count; i++)
            {
                var l = this.segmentLengths[i];
                if (l >= position)
                {
                    return this.GetSegmentPoint(i, position / l);
                }

                position -= l;
            }

            return this.points[this.points.Count - 1];
        }

        public Vector3 GetTangent(float position)
        {
            if (this.points.Count < 2)
            {
                return Vector3.forward;
            }

            position = Mathf.Clamp(position, 0f, this.Length);

            if (this.segments.Count == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

            Debug.Assert(this.segments.Count == this.points.Count - 1);
            Debug.Assert(this.segmentLengths.Count == this.segments.Count);

            for (var i = 0; i < this.segments.Count; i++)
            {
                var l = this.segmentLengths[i];
                if (l >= position)
                {
                    return this.GetSegmentTangent(i, position / l);
                }

                position -= l;
            }

            return this.GetSegmentTangent(this.segmentLengths.Count - 1, 1f);
        }

        public float GetCurvature(float position)
        {
            if (this.points.Count < 2)
            {
                return float.NaN;
            }

            position = Mathf.Clamp(position, 0f, this.Length);

            if (this.segments.Count == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

            Debug.Assert(this.segments.Count == this.points.Count - 1);
            Debug.Assert(this.segmentLengths.Count == this.segments.Count);

            for (var i = 0; i < this.segments.Count; i++)
            {
                var l = this.segmentLengths[i];
                if (l >= position)
                {
                    return this.GetSegmentCurvature(i, position / l);
                }

                position -= l;
            }

            return this.GetSegmentCurvature(this.segmentLengths.Count - 1, 1f);
        }

        public Vector3 GetForward(float position)
        {
            return this.GetTangent(position).normalized;
        }

        public Vector3 GetBackward(float position)
        {
            return -this.GetForward(position);
        }

        public Vector3 GetRight(float position)
        {
            var tangent = this.GetTangent(position);

            if (tangent.x + tangent.z < 1e-6)
            {
                return Vector3.right;
            }

            return new Vector3(-tangent.z, 0, tangent.x).normalized;
        }

        public Vector3 GetLeft(float position)
        {
            return -this.GetRight(position);
        }

        public Vector3 GetUp(float position)
        {
            var tangent = this.GetTangent(position);

            if (tangent.x + tangent.z < 1e-6)
            {
                return tangent.y >= 1e-6 ? Vector3.forward : Vector3.up;
            }

            var right = new Vector3(-tangent.z, 0, tangent.x);
            return Vector3.Cross(tangent, right).normalized;
        }

        public Vector3 GetDown(float position)
        {
            return -this.GetUp(position);
        }

        private float CalculateLength()
        {
            if (this.points.Count < 2)
            {
                return 0f;
            }

            if (this.segments.Count == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

            var result = 0f;

            foreach (var l in this.segmentLengths)
            {
                result += l;
            }

            return result;
        }

        private float GetSegmentCurvature(int segment, float t)
        {
            Debug.Assert(t >= 0f);
            Debug.Assert(t <= 1f);
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < this.segments.Count);


            // Source: https://en.wikipedia.org/wiki/Curvature#Space_curves
            var s = this.segments[segment];
            var tan = t * (t * 3f * s.a + 2f * s.b) + s.c;
            var tanD = t * 6f * s.a + 2f * s.b;

            var x = tanD.z * tan.y - tanD.y * tan.z;
            var y = tanD.x * tan.z - tanD.z * tan.x;
            var z = tanD.y * tan.x - tanD.x * tan.y;

            var sqTanMag = tan.magnitude;

            return Mathf.Sqrt(x * x + y * y + z * z) / (sqTanMag * sqTanMag * sqTanMag);
        }

        private float GetSegmentDistance(int segment, Vector3 point, out float position)
        {
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < this.segments.Count);

            // TODO: This code is slow, not very precise and can produce false results in some cases, needs to be replaced with solid implementation

            var s = this.segments[segment];
            var minSqDist = float.PositiveInfinity;
            var minIndex = -1;

            // Broad phase
            for (var i = 0; i <= 20; i++)
            {
                var t = i / 20f;
                var pos = t * (t * (t * s.a + s.b) + s.c) + s.d;
                var sqDist = (point - pos).sqrMagnitude;

                if (sqDist < minSqDist)
                {
                    minSqDist = sqDist;
                    minIndex = i;
                }
            }
            
            var from = minIndex > 0 ? (minIndex - 1) / 20f : 0f;
            var to = minIndex < 20 ? (minIndex + 1) / 20f : 1f;
            var span = to - from;

            // Narrow phase
            for (var i = 0; i <= 20; i++)
            {
                var t = (i * span) / 20f + from;
                var pos = t * (t * (t * s.a + s.b) + s.c) + s.d;
                var sqDist = (point - pos).sqrMagnitude;

                if (sqDist < minSqDist)
                {
                    minSqDist = sqDist;
                    minIndex = i;
                }
            }

            position = (minIndex * span) / 20f + from;
            return minSqDist;
        }

        private float GetSegmentLength(int segment)
        {
            Debug.Assert(this.points.Count >= 2);
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < this.segments.Count);

            var result = 0f;
            var prev = this.points[segment];
            var segLength = 1f / Segmentation;

            for (var i = 1; i < Segmentation; i++)
            {
                var cur = this.GetSegmentPoint(segment, i * segLength);
                result += Vector3.Distance(prev, cur);
                prev = cur;
            }

            var last = this.points[segment + 1];
            result += Vector3.Distance(prev, last);

            return result;
        }

        private Vector3 GetSegmentPoint(int segment, float t)
        {
            Debug.Assert(t >= 0f);
            Debug.Assert(t <= 1f);
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < this.segments.Count);

            var s = this.segments[segment];
            var pos = t * (t * (t * s.a + s.b) + s.c) + s.d;

            return pos;
        }

        private Vector3 GetSegmentTangent(int segment, float t)
        {
            Debug.Assert(t >= 0f);
            Debug.Assert(t <= 1f);
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < this.segments.Count);

            var s = this.segments[segment];
            var tan = t * (t * 3f * s.a + 2f * s.b) + s.c;

            return tan;
        }

        private void UpdateSegmentLengths()
        {
            Debug.Assert(this.segments.Count == Mathf.Max(0, this.points.Count - 1));

            if (this.points.Count < 2)
            {
                this.segmentLengths.Clear();
                return;
            }

            while (this.segmentLengths.Count < this.segments.Count)
            {
                this.segmentLengths.Add(0f);
            }

            while (this.segmentLengths.Count > this.segments.Count)
            {
                this.segmentLengths.RemoveAt(this.segmentLengths.Count - 1);
            }

            this.UpdateSegmentLengthsLocally(0, this.segments.Count - 1);
        }

        private void UpdateSegmentLengthsLocally(int start, int end)
        {
            Debug.Assert(this.points.Count >= 2);
            Debug.Assert(this.segments.Count == this.points.Count - 1);
            Debug.Assert(this.segmentLengths.Count == this.segments.Count);
            Debug.Assert(start >= 0);
            Debug.Assert(end < this.segments.Count);
            Debug.Assert(start <= end);

            for (var i = start; i <= end; i++)
            {
                this.segmentLengths[i] = this.GetSegmentLength(i);
            }
        }

        private void UpdateSegments()
        {
            if (this.points.Count < 2)
            {
                this.segments.Clear();
                return;
            }

            while (this.tangents.Count > this.points.Count)
            {
                this.tangents.RemoveAt(this.tangents.Count - 1);
            }

            while (this.tangents.Count < this.points.Count)
            {
                this.tangents.Add(Vector3.zero);
            }

            while (this.segments.Count > this.points.Count - 1)
            {
                this.segments.RemoveAt(this.segments.Count - 1);
            }

            while (this.segments.Count < this.points.Count - 1)
            {
                this.segments.Add(new Segment());
            }

            this.UpdateSegmentsLocally(0, this.segments.Count - 1);
        }

        private void UpdateSegmentsLocally(int start, int end)
        {
            Debug.Assert(this.points.Count >= 2);
            Debug.Assert(this.tangents.Count == this.points.Count);
            Debug.Assert(this.segments.Count == this.points.Count - 1);
            Debug.Assert(start >= 0);
            Debug.Assert(end < this.segments.Count);
            Debug.Assert(start <= end);

            // Cache values so they only need to be calculated once (helps with segment updates for >= 2 segments)
            // As values have to be stored anyways, the cache size wont really impact the execution speed
            var p0 = start == 0 ? Vector3.zero : this.points[start - 1];
            var p1 = this.points[start];
            var p2 = this.points[start + 1];

            var t01 = start == 0 ? 0f : Mathf.Pow(Vector3.Distance(p0, p1), this.alpha);
            var t12 = Mathf.Pow(Vector3.Distance(p1, p2), this.alpha);
            
            var p10 = p1 - p0;
            var p20 = p2 - p0;
            var p21 = p2 - p1;
            var tau = 1f - this.tension;

            for (var i = start; i <= end; i++)
            {
                Vector3 m0, m1;
                Vector3 p31, p32;
                Vector3 p3;
                float t23;

                if (i == 0)     // First segment
                {
                    m0 = p21 * tau;
                }
                else
                {
                    m0 = tau * (p21 + t12 * (p10 / t01 - p20 / (t01 + t12)));
                }

                if (i == this.points.Count - 2)      // Last segment
                {
                    p3 = p2;
                    p31 = Vector3.zero; // Won't need these values anymore, but need to be set
                    p32 = Vector3.zero;
                    t23 = t12;
                    m1 = p21 * tau;
                }
                else
                {
                    p3 = this.points[i + 2];
                    p31 = p3 - p1;
                    p32 = p3 - p2;
                    t23 = Mathf.Pow(Vector3.Distance(p2, p3), this.alpha);
                    m1 = tau * (p21 + t12 * (p32 / t23 - p31 / (t12 + t23)));
                }

                this.tangents[i] = m0;
                this.tangents[i + 1] = m1;

                // Calculate segment values, tangents mustn't be normalized here
                var a = -2f * p21 + m0 + m1;        // Use -2 * p21 as
                var b = 3f * p21 - m0 - m0 - m1;
                var c = m0;
                var d = p1;

                this.segments[i] = new Segment(a, b, c, d);

                // Shift cached values for next segment
                p1 = p2;
                p2 = p3;
                t01 = t12;
                t12 = t23;
                p10 = p21;
                p20 = p31;
                p21 = p32;
            }
        }

        private struct Segment
        {
            public readonly Vector3 a;

            public readonly Vector3 b;

            public readonly Vector3 c;

            public readonly Vector3 d;

            public Segment(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
            }
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return this.points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

