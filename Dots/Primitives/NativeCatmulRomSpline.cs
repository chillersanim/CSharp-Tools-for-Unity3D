using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace UnityTools.Dots.Primitives
{
    [BurstCompile]
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeCatmulRomSpline
    {
        #region Constants

        private const int Segmentation = 20;

        private const float Epsilon = 1e-6f;

        #endregion

        #region Private Fields

        private NativeList<float3> points;

        private NativeList<float> segmentLengths;

        private NativeList<Segment> segments;

        private NativeList<float3> tangents;

        private float alpha;

        private float length;

        private float tension;

        private bool HasLength => this.length <= -0.5f;

        private Allocator allocator;

        #endregion
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the CatmullRomSpline type.
        /// </summary>
        public NativeCatmulRomSpline(Allocator allocator)
        {
            this.points = new NativeList<float3>(allocator);
            this.tangents = new NativeList<float3>(allocator);
            this.segments = new NativeList<Segment>(allocator);
            this.segmentLengths = new NativeList<float>(allocator);
            this.alpha = 0.5f;
            this.tension = 0.0f;

            this.allocator = allocator;

            this.length = -1f;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The alpha 
        /// </summary>
        public float Alpha
        {
            get => this.alpha;
            set
            {
                if (math.abs(this.alpha - value) < Epsilon)
                {
                    return;
                }

                this.alpha = value;
                this.segments.Clear();
                this.length = -1f;
            }
        }

        public int Count => this.points.Length;

        public float3 this[int index]
        {
            get => this.points [index];
            set
            {
                this.points[index] = value;

                if (this.points.Length >= 2 && this.segments.Length > 0)
                {
                    var start = math.max(0, index - 2);
                    var end = math.min(this.segments.Length - 1, index + 1);

                    UpdateSegmentsLocally(start, end);
                    UpdateSegmentLengthsLocally(start, end);
                    this.length = -1;
                }
            }
        }

        public float Tension
        {
            get => this.tension;
            set
            {
                if (math.abs(this.tension - value) < Epsilon)
                {
                    return; 
                }

                this.tension = value;
                this.segments.Clear();
                this.length = -1f;
            }
        }

        public float Length
        {
            get
            {
                if (!this.HasLength)
                {
                    this.length = CalculateLength();
                }

                return this.length;
            }
        }

        #endregion

        #region Public Methods

        public void Add(float3 point)
        {
            var index = this.points.Length - 1;
            this.points.Add(point);

            if (this.segments.Length == 0)
            {
                return;
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.points.Length != this.segments.Length + 2)
                throw new Exception("Segments were out of sync with points.");
#endif

            var start = math.max(0, index - 2);

            UpdateSegmentsLocally(start, this.segments.Length - 1);
            UpdateSegmentLengthsLocally(start, this.segments.Length - 1);
            this.length = -1f;
        }

        public void Clear()
        {
            this.points.Clear();
            this.segments.Clear();
            this.segmentLengths.Clear();
            this.tangents.Clear();
            this.length = 0f;
        }

        public void Insert(int index, float3 point)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (index < 0 || index > this.points.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
#endif

            this.points.Add(default);

            for (var i = this.points.Length - 2; i >= index; i--)
            {
                this.points[i + 1] = this.points[i];
            }

            this.points[index] = point;

            if (this.segments.Length == 0)
            {
                return;
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.points.Length != this.segments.Length + 2)
                throw new Exception("Segments were out of sync with points.");
#endif

            var start = math.max(0, index - 2);
            var end = math.min(this.segments.Length - 1, index + 1);

            UpdateSegmentsLocally(start, end);
            UpdateSegmentLengthsLocally(start, end);
            this.length = -1f;
        }

        public void RemoveAt(int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (index < 0 || index >= this.points.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
#endif

            this.points.RemoveAt(index);

            if (this.segments.Length == 0)
            {
                return;
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.points.Length != this.segments.Length)
                throw new Exception("Segments were out of sync with points.");
#endif

            if (this.points.Length >= 2)
            {
                var start = math.max(0, index - 2);
                var end = math.min(this.segments.Length - 1, index + 1);

                UpdateSegmentsLocally(start, end);
                UpdateSegmentLengthsLocally(start, end);
                this.length = -1f;
            }
            else
            {
                this.segments.Clear();
                this.segmentLengths.Clear();
                this.length = 0f;
            }
        }

        public float3 ClosestPoint(float3 point)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.points.Length == 0)
                throw new InvalidOperationException("The native catmull rom spline needs at least one point to determine the closest point.");
#endif

            if(this.points.Length == 1)
            {
                return this.points[0];
            }

            var minDist = float.PositiveInfinity;
            var minPos = 0f;
            var minSeg = 0;

            for (var i = 0; i < this.segments.Length; i++)
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

        public float ClosestPosition(float3 point)
        {
            if (this.points.Length < 2)
            {
                return 0f;
            }

            var minDist = float.PositiveInfinity;
            var minPos = 0f;
            var offset = 0f;

            for (var i = 0; i < this.segments.Length; i++)
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

        public float3 GetPoint(float position)
        {
            if (this.points.Length == 0)
            {
                return float3.zero;
            }

            if (this.points.Length == 1)
            {
                return this.points[0];
            }

            if (position < 0)
            {
                return this.points[0];
            }

            if (position > this.Length)
            {
                return this.points[this.points.Length - 1];
            }

            if (this.segments.Length == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.segments.Length != this.points.Length - 1)
                throw new Exception("Segments were out of sync with points.");
            if(this.segmentLengths.Length != this.segments.Length)
                throw new Exception("Segments lengths were out of sync with segments.");
#endif

            for (var i = 0; i < this.segmentLengths.Length; i++)
            {
                var l = this.segmentLengths[i];
                if (l >= position)
                {
                    return this.GetSegmentPoint(i, position / l);
                }

                position -= l;
            }

            return this.points[this.points.Length - 1];
        }

        public float3 GetTangent(float position)
        {
            if (this.points.Length < 2)
            {
                return new float3(0, 0, 1);
            }

            position = math.clamp(position, 0f, this.Length);

            if (this.segments.Length == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.segments.Length != this.points.Length - 1)
                throw new Exception("Segments were out of sync with points.");
            if(this.segmentLengths.Length != this.segments.Length)
                throw new Exception("Segments lengths were out of sync with segments.");
#endif

            for (var i = 0; i < this.segments.Length; i++)
            {
                var l = this.segmentLengths[i];
                if (l >= position)
                {
                    return this.GetSegmentTangent(i, position / l);
                }

                position -= l;
            }

            return this.GetSegmentTangent(this.segmentLengths.Length - 1, 1f);
        }

        #endregion
        
        #region Private Methods

        private float CalculateLength()
        {
            if (this.points.Length < 2)
            {
                return 0f;
            }

            if (this.segments.Length == 0)
            {
                this.UpdateSegments();
                this.UpdateSegmentLengths();
            }

            var result = 0f;

            for (var i = 0; i < this.segmentLengths.Length; i++)
            {
                result += this.segmentLengths[i];
            }

            return result;
        }

        private float GetSegmentDistance(int segment, float3 point, out float position)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (segment < 0 || segment >= this.segments.Length)
                throw new ArgumentOutOfRangeException(nameof(segment));
#endif

            // TODO: This code is slow, not very precise and can produce false results in some cases, needs to be replaced with solid implementation

            var s = this.segments[segment];
            var minSqDist = float.PositiveInfinity;
            var minIndex = -1;

            // Broad phase
            for (var i = 0; i <= 20; i++)
            {
                var t = i / 20f;
                var pos = t * (t * (t * s.a + s.b) + s.c) + s.d;
                var sqDist = math.lengthsq(point - pos);

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
                var sqDist = math.lengthsq(point - pos);

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
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.points.Length < 2)
                throw new InvalidOperationException("Can't get segment length if less than 2 points were defined.");
            if(segment < 0 ||segment >= this.segments.Length)
                throw new ArgumentOutOfRangeException(nameof(segment));
#endif

            var result = 0f;
            var prev = this.points[segment];
            var segLength = 1f / Segmentation;

            for (var i = 1; i < Segmentation; i++)
            {
                var cur = this.GetSegmentPoint(segment, i * segLength);
                result += math.distance(prev, cur);
                prev = cur;
            }

            var last = this.points[segment + 1];
            result += math.distance(prev, last);

            return result;
        }

        private float3 GetSegmentPoint(int segment, float t)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (t < 0 || t > 1)
                throw new ArgumentOutOfRangeException(nameof(t));
            if (segment < 0 || segment >= this.segments.Length)
                throw new ArgumentOutOfRangeException(nameof(segment));
#endif

            var s = this.segments[segment];
            var pos = t * (t * (t * s.a + s.b) + s.c) + s.d;

            return pos;
        }

        private float3 GetSegmentTangent(int segment, float t)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (t < 0 || t > 1)
                throw new ArgumentOutOfRangeException(nameof(t));
            if (segment < 0 || segment >= this.segments.Length)
                throw new ArgumentOutOfRangeException(nameof(segment));
#endif

            var s = this.segments[segment];
            var tan = t * (t * 3f * s.a + 2f * s.b) + s.c;

            return tan;
        }

        private void UpdateSegmentLengths()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.segments.Length != math.max(0, this.points.Length - 1))
                throw new Exception("Segments were out of sync with points.");
#endif

            if (this.points.Length < 2)
            {
                this.segmentLengths.Clear();
                return;
            }

            this.segmentLengths.ResizeUninitialized(this.segments.Length);
            this.UpdateSegmentLengthsLocally(0, this.segments.Length - 1);
        }

        private void UpdateSegmentLengthsLocally(int start, int end)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.points.Length < 2)
                throw new Exception("Not enough points provided to update the segment lengths.");
            if(this.segments.Length != this.points.Length - 1)
                throw new Exception("Segments were out of sync with points.");
            if(this.segmentLengths.Length != this.segments.Length)
                throw new Exception("Segments lengths were out of sync with segments.");
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (end >= this.segments.Length)
                throw new ArgumentOutOfRangeException(nameof(end));
            if(start > end)
                throw new ArgumentException("Start cannot be larger than end.");
#endif

            for (var i = start; i <= end; i++)
            {
                this.segmentLengths[i] = this.GetSegmentLength(i);
            }
        }

        private void UpdateSegments()
        {
            if (this.points.Length < 2)
            {
                this.segments.Clear();
                return;
            }

            this.tangents.ResizeUninitialized(this.points.Length);
            this.segments.ResizeUninitialized(this.points.Length - 1);

            this.UpdateSegmentsLocally(0, this.segments.Length - 1);
        }

        private void UpdateSegmentsLocally(int start, int end)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(this.points.Length < 2)
                throw new Exception("Not enough points provided to update the segment lengths.");
            if(this.segments.Length != this.points.Length - 1)
                throw new Exception("Segments were out of sync with points.");
            if(this.segmentLengths.Length != this.segments.Length)
                throw new Exception("Segments lengths were out of sync with segments.");
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start));
            if (end >= this.segments.Length)
                throw new ArgumentOutOfRangeException(nameof(end));
            if(start > end)
                throw new ArgumentException("Start cannot be larger than end.");
#endif
            // Cache values so they only need to be calculated once (helps with segment updates for >= 2 segments)
            // As values have to be stored anyways, the cache size wont really impact the execution speed
            var p0 = start == 0 ? float3.zero : this.points[start - 1];
            var p1 = this.points[start];
            var p2 = this.points[start + 1];

            var t01 = start == 0 ? 0f : math.pow(math.distance(p0, p1), this.alpha);
            var t12 = math.pow(math.distance(p1, p2), this.alpha);
            
            var p10 = p1 - p0;
            var p20 = p2 - p0;
            var p21 = p2 - p1;
            var tau = 1f - this.tension;

            for (var i = start; i <= end; i++)
            {
                float3 m0, m1;
                float3 p31, p32;
                float3 p3;
                float t23;

                if (i == 0)     // First segment
                {
                    m0 = p21 * tau;
                }
                else
                {
                    m0 = tau * (p21 + t12 * (p10 / t01 - p20 / (t01 + t12)));
                }

                if (i == this.points.Length - 2)      // Last segment
                {
                    p3 = p2;
                    p31 = float3.zero; // Won't need these values anymore, but need to be set
                    p32 = float3.zero;
                    t23 = t12;
                    m1 = p21 * tau;
                }
                else
                {
                    p3 = this.points[i + 2];
                    p31 = p3 - p1;
                    p32 = p3 - p2;
                    t23 = math.pow(math.distance(p2, p3), this.alpha);
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

        #endregion

        #region Internal Data Structures

        [BurstCompile]
        [StructLayout(LayoutKind.Sequential)]
        private readonly struct Segment
        {
            public readonly float3 a;

            public readonly float3 b;

            public readonly float3 c;

            public readonly float3 d;

            public Segment(float3 a, float3 b, float3 c, float3 d)
            {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
            }
        }

        #endregion
    }
}
