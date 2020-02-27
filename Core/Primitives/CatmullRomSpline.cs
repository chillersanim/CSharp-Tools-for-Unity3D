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

            length = null;
        }

        /// <summary>
        /// The alpha 
        /// </summary>
        public float Alpha
        {
            get => alpha;
            set
            {
                if (Mathf.Approximately(alpha, value))
                {
                    return;
                }

                alpha = value;
                segments.Clear();
                length = null;
            }
        }

        public int Count => points.Count;

        public bool IsReadOnly => false;

        public Vector3 this[int index]
        {
            get => points [index];
            set
            {
                var sqDist = (points[index] - value).sqrMagnitude;
                if(Mathf.Approximately(sqDist, 0))
                {
                    return;
                }

                points[index] = value;

                if (points.Count >= 2 && segments.Count > 0)
                {
                    var start = Mathf.Max(0, index - 2);
                    var end = Mathf.Min(segments.Count - 1, index + 1);

                    UpdateSegmentsLocally(start, end);
                    UpdateSegmentLengthsLocally(start, end);
                    length = null;
                }
            }
        }

        public IReadOnlyList<float> SegmentLengths => segmentLengths.AsReadOnly();

        public IReadOnlyList<Vector3> Tangents => tangents.AsReadOnly();

        public float Tension
        {
            get => tension;
            set
            {
                if (Mathf.Approximately(tension, value))
                {
                    return; 
                }

                tension = value;
                segments.Clear();
                length = null;
            }
        }

        public float Length
        {
            get
            {
                if (!length.HasValue)
                {
                    length = CalculateLength();
                }

                return length.Value;
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
            return points.Contains(point);
        }

        public void CopyTo(Vector3[] array, int arrayIndex)
        {
            points.CopyTo(array, arrayIndex);
        }

        public int IndexOf(Vector3 point)
        {
            return points.IndexOf(point);
        }

        public void Insert(int index, Vector3 point)
        {
            if (index < 0 || index > this.points.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            this.points.Insert(index, point);

            if (points.Count >= 2 && segments.Count > 0)
            {
                var start = Mathf.Max(0, index - 2);
                var end = Mathf.Min(segments.Count - 1, index + 1);

                UpdateSegmentsLocally(start, end);
                UpdateSegmentLengthsLocally(start, end);
                length = null;
            }
        }

        public bool Remove(Vector3 point)
        {
            for (var i = 0; i < points.Count; i++)
            {
                if (points[i] == point)
                {
                    RemoveAt(i);
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

            if (points.Count >= 2 && segments.Count > 0)
            {
                var start = Mathf.Max(0, index - 2);
                var end = Mathf.Min(segments.Count - 1, index + 1);

                UpdateSegmentsLocally(start, end);
                UpdateSegmentLengthsLocally(start, end);
                length = null;
            }
            else
            {
                segments.Clear();
                segmentLengths.Clear();
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
                return points[0];
            }

            var minDist = float.PositiveInfinity;
            var minPos = 0f;
            var minSeg = 0;

            for (var i = 0; i < segments.Count; i++)
            {
                var dist = GetSegmentDistance(i, point, out var pos);

                if (dist < minDist)
                {
                    minDist = dist;
                    minPos = pos;
                    minSeg = i;
                }
            }

            if (minSeg < 0)
            {
                return points[0];
            }

            return GetSegmentPoint(minSeg, minPos);
        }

        public float ClosestPosition(Vector3 point)
        {
            if (points.Count < 2)
            {
                return 0f;
            }

            var minDist = float.PositiveInfinity;
            var minPos = 0f;
            var offset = 0f;

            for (var i = 0; i < segments.Count; i++)
            {
                var dist = GetSegmentDistance(i, point, out var pos);
                var segLength = segmentLengths[i];

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
            if (points.Count == 0)
            {
                return Vector3.zero;
            }

            if (points.Count == 1)
            {
                return points[0];
            }

            if (position < 0)
            {
                return points[0];
            }

            if (position > Length)
            {
                return points[points.Count - 1];
            }

            if (segments.Count == 0)
            {
                UpdateSegments();
                UpdateSegmentLengths();
            }

            Debug.Assert(segments.Count == points.Count - 1);
            Debug.Assert(segmentLengths.Count == segments.Count);

            for (var i = 0; i < segmentLengths.Count; i++)
            {
                var l = segmentLengths[i];
                if (l >= position)
                {
                    return GetSegmentPoint(i, position / l);
                }

                position -= l;
            }

            return points[points.Count - 1];
        }

        public Vector3 GetTangent(float position)
        {
            if (points.Count < 2)
            {
                return Vector3.forward;
            }

            position = Mathf.Clamp(position, 0f, Length);

            if (segments.Count == 0)
            {
                UpdateSegments();
                UpdateSegmentLengths();
            }

            Debug.Assert(segments.Count == points.Count - 1);
            Debug.Assert(segmentLengths.Count == segments.Count);

            for (var i = 0; i < segments.Count; i++)
            {
                var l = segmentLengths[i];
                if (l >= position)
                {
                    return GetSegmentTangent(i, position / l);
                }

                position -= l;
            }

            return GetSegmentTangent(segmentLengths.Count - 1, 1f);
        }

        public float GetCurvature(float position)
        {
            if (points.Count < 2)
            {
                return float.NaN;
            }

            position = Mathf.Clamp(position, 0f, Length);

            if (segments.Count == 0)
            {
                UpdateSegments();
                UpdateSegmentLengths();
            }

            Debug.Assert(segments.Count == points.Count - 1);
            Debug.Assert(segmentLengths.Count == segments.Count);

            for (var i = 0; i < segments.Count; i++)
            {
                var l = segmentLengths[i];
                if (l >= position)
                {
                    return GetSegmentCurvature(i, position / l);
                }

                position -= l;
            }

            return GetSegmentCurvature(segmentLengths.Count - 1, 1f);
        }

        private float CalculateLength()
        {
            if (points.Count < 2)
            {
                return 0f;
            }

            if (segments.Count == 0)
            {
                UpdateSegments();
                UpdateSegmentLengths();
            }

            var result = 0f;

            foreach (var l in segmentLengths)
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
            Debug.Assert(segment < segments.Count);


            // Source: https://en.wikipedia.org/wiki/Curvature#Space_curves
            var s = segments[segment];
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
            Debug.Assert(segment < segments.Count);

            // TODO: This code is slow, not very precise and can produce false results in some cases, needs to be replaced with solid implementation

            var s = segments[segment];
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
            Debug.Assert(points.Count >= 2);
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < segments.Count);

            var result = 0f;
            var prev = points[segment];
            var segLength = 1f / Segmentation;

            for (var i = 1; i < Segmentation; i++)
            {
                var cur = GetSegmentPoint(segment, i * segLength);
                result += Vector3.Distance(prev, cur);
                prev = cur;
            }

            var last = points[segment + 1];
            result += Vector3.Distance(prev, last);

            return result;
        }

        private Vector3 GetSegmentPoint(int segment, float t)
        {
            Debug.Assert(t >= 0f);
            Debug.Assert(t <= 1f);
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < segments.Count);

            var s = segments[segment];
            var pos = t * (t * (t * s.a + s.b) + s.c) + s.d;

            return pos;
        }

        private Vector3 GetSegmentTangent(int segment, float t)
        {
            Debug.Assert(t >= 0f);
            Debug.Assert(t <= 1f);
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < segments.Count);

            var s = segments[segment];
            var tan = t * (t * 3f * s.a + 2f * s.b) + s.c;

            return tan;
        }

        private void UpdateSegmentLengths()
        {
            Debug.Assert(segments.Count == Mathf.Max(0, points.Count - 1));

            if (points.Count < 2)
            {
                segmentLengths.Clear();
                return;
            }

            while (segmentLengths.Count < segments.Count)
            {
                segmentLengths.Add(0f);
            }

            while (segmentLengths.Count > segments.Count)
            {
                segmentLengths.RemoveAt(segmentLengths.Count - 1);
            }

            UpdateSegmentLengthsLocally(0, segments.Count - 1);
        }

        private void UpdateSegmentLengthsLocally(int start, int end)
        {
            Debug.Assert(points.Count >= 2);
            Debug.Assert(segments.Count == points.Count - 1);
            Debug.Assert(segmentLengths.Count == segments.Count);
            Debug.Assert(start >= 0);
            Debug.Assert(end < segments.Count);
            Debug.Assert(start <= end);

            for (var i = start; i <= end; i++)
            {
                segmentLengths[i] = GetSegmentLength(i);
            }
        }

        private void UpdateSegments()
        {
            if (points.Count < 2)
            {
                segments.Clear();
                return;
            }

            while (tangents.Count > points.Count)
            {
                tangents.RemoveAt(tangents.Count - 1);
            }

            while (tangents.Count < points.Count)
            {
                tangents.Add(Vector3.zero);
            }

            while (segments.Count > points.Count - 1)
            {
                segments.RemoveAt(segments.Count - 1);
            }

            while (segments.Count < points.Count - 1)
            {
                segments.Add(new Segment());
            }

            UpdateSegmentsLocally(0, segments.Count - 1);
        }

        private void UpdateSegmentsLocally(int start, int end)
        {
            Debug.Assert(points.Count >= 2);
            Debug.Assert(tangents.Count == points.Count);
            Debug.Assert(segments.Count == points.Count - 1);
            Debug.Assert(start >= 0);
            Debug.Assert(end < segments.Count);
            Debug.Assert(start <= end);

            // Cache values so they only need to be calculated once (helps with segment updates for >= 2 segments)
            // As values have to be stored anyways, the cache size wont really impact the execution speed
            var p0 = start == 0 ? Vector3.zero : points[start - 1];
            var p1 = points[start];
            var p2 = points[start + 1];

            var t01 = start == 0 ? 0f : Mathf.Pow(Vector3.Distance(p0, p1), alpha);
            var t12 = Mathf.Pow(Vector3.Distance(p1, p2), alpha);
            
            var p10 = p1 - p0;
            var p20 = p2 - p0;
            var p21 = p2 - p1;
            var tau = 1f - tension;

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

                if (i == points.Count - 2)      // Last segment
                {
                    p3 = p2;
                    p31 = Vector3.zero; // Won't need these values anymore, but need to be set
                    p32 = Vector3.zero;
                    t23 = t12;
                    m1 = p21 * tau;
                }
                else
                {
                    p3 = points[i + 2];
                    p31 = p3 - p1;
                    p32 = p3 - p2;
                    t23 = Mathf.Pow(Vector3.Distance(p2, p3), alpha);
                    m1 = tau * (p21 + t12 * (p32 / t23 - p31 / (t12 + t23)));
                }

                tangents[i] = m0;
                tangents[i + 1] = m1;

                // Calculate segment values, tangents mustn't be normalized here
                var a = -2f * p21 + m0 + m1;        // Use -2 * p21 as
                var b = 3f * p21 - m0 - m0 - m1;
                var c = m0;
                var d = p1;

                segments[i] = new Segment(a, b, c, d);

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
            return GetEnumerator();
        }
    }
}

