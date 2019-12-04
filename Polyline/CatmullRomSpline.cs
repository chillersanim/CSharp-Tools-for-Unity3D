using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity_Tools.Core.Pooling;
using Debug = UnityEngine.Debug;

namespace Unity_Tools.Polyline
{
    public sealed class CatmullRomSpline : IPolyline
    {
        private static readonly int Segmentation = 20;

        private float? length;

        private readonly List<Vector3> points;

        private readonly List<Vector3> tangents;

        private readonly List<float> segmentLengths;

        private float tension;

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
                UpdateTangents();
                UpdateSegmentLengths();
                length = null;
            }
        }

        public CatmullRomSpline(IList<Vector3> points) : this(points, 0.5f)
        {
        }

        public CatmullRomSpline(IList<Vector3> points, float tension)
        {
            this.points = new List<Vector3>(points);
            this.tangents = new List<Vector3>(points.Count);
            this.segmentLengths = new List<float>(points.Count);
            this.tension = tension;

            this.UpdateTangents();


            length = null;
        }

        public int Count => points.Count;

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

                UpdateTangents();

                if (index > 0 && index < points.Count - 2)
                {
                    segmentLengths[index] = GetSegmentLength(index);

                    if (index < points.Count - 3)
                    {
                        segmentLengths[index + 1] = GetSegmentLength(index + 1);
                    }
                }

                length = null;
            }
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            throw new System.NotImplementedException();
        }

        public float ClosestPosition(Vector3 point)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetDirectionAtPosition(float position)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetPointAtPosition(float position)
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
                position = 0;
            }

            if (position > Length)
            {
                position = Length;
            }

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

        private float CalculateLength()
        {
            if (points.Count < 2)
            {
                return 0f;
            }

            if (segmentLengths.Count != points.Count)
            {
                UpdateSegmentLengths();
            }

            var result = 0f;

            foreach (var l in segmentLengths)
            {
                result += l;
            }

            return result;
        }

        private Vector3 GetSegmentPoint(int segment, float u)
        {
            Debug.Assert(u >= 0f);
            Debug.Assert(u <= 1f);
            Debug.Assert(segment >= 0);
            Debug.Assert(segment < points.Count - 1);

            var p0 = points[segment];
            var p1 = points[segment + 1];
            var t0 = tangents[segment];
            var t1 = tangents[segment + 1];

            var p10 = p1 - p0;

            var c0 = p0;
            var c1 = t0;
            var c2 = 3 * p10 - 2 * t0 - t1;
            var c3 = t0 + t1 - 2 * p10;

            return u * (c1 + u * (c2 + u * c3)) + c0;
        }

        private void UpdateSegmentLengths()
        {
            segmentLengths.Clear();

            if (points.Count < 2)
            {
                return;
            }

            for (var i = 0; i < points.Count - 1; i++)
            {
                segmentLengths.Add(GetSegmentLength(i));
            }
        }

        private float GetSegmentLength(int segment)
        {
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

        private void UpdateTangents()
        {
            tangents.Clear();

            if (points.Count == 0)
            {
                return;
            }

            if(points.Count == 1)
            {
                tangents.Add(Vector3.forward);
                return;
            }

            Vector3 prev;
            var current = points[0];
            var next = points[1];

            tangents.Add((next - current) * tension);

            for (var i = 1; i < points.Count - 1; i++)
            {
                prev = current;
                current = next;
                next = points[i + 1];

                tangents.Add((next - prev) * tension);
            }

            tangents.Add((next - current) * tension);
        }
    }
}
