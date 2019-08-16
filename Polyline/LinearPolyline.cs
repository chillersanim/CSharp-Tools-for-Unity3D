using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity_Tools.Core;

namespace Unity_Tools.Polyline
{
    [Serializable]
    public class LinearPolyline : IPolyline, IList<Vector3>, IReadOnlyList<Vector3>
    {
        [SerializeField]
        private readonly List<Vector3> points;

        /// <summary>
        /// Contain the distance from the start point to the point at the given index (distance along the polyline).
        /// </summary>
        [SerializeField]
        private readonly List<double> distances;

        public float Length => (float)distances[distances.Count - 1];

        public int Count => points.Count;

        public LinearPolyline()
        {
            this.points = new List<Vector3>();
            distances = new List<double>();
            distances.Add(0);
        }

        public LinearPolyline(params Vector3[] points)
        {
            this.points = new List<Vector3>(points);
            this.distances = new List<double>(points.Length);
            
            RecalculateLength();
        }

        public Vector3 GetPointAtPosition(float position)
        {
            if (points.Count == 0)
            {
                throw new InvalidOperationException(
                    "The polyline has no points, thus cannot get a point at a position.");
            }

            if (position < 0 || position > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            if (points.Count == 1)
            {
                return points[0];
            }

            var index = distances.BinarySearch(position);
            if (index == points.Count - 1)
            {
                index--;
            }

            var from = points[index];
            var to = points[index + 1];
            var localPosition = (position - distances[index]) / Length;

            return (to - from) * (float)localPosition;
        }

        public Vector3 GetDirectionAtPosition(float position)
        {
            if (points.Count == 0)
            {
                throw new InvalidOperationException(
                    "The polyline has no points, thus cannot get a point at a position.");
            }

            if (position < 0 || position > Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            if (points.Count == 1)
            {
                return points[0];
            }

            var index = distances.BinarySearch(position);
            if (index == points.Count - 1)
            {
                index--;
            }

            var from = points[index];
            var to = points[index + 1];
            var fromDist = distances[index];
            var toDist = distances[index + 1];

            return (to - from) / (float)(toDist - fromDist);
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            if (points.Count == 0)
            {
                throw new InvalidOperationException(
                    "The polyline has no points, thus cannot get a point at a position.");
            }

            if (points.Count == 1)
            {
                return points[0];
            }

            var minSqrDist = float.PositiveInfinity;
            var minDistPoint = Vector3.zero;

            for (var i = 1; i < points.Count; i++)
            {
                var from = points[i - 1];
                var to = points[i];

                var segmentPoint = Math3D.ClosestPointOnLineSegment(point, from, to);
                var sqrDist = (segmentPoint - point).sqrMagnitude;

                if(sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    minDistPoint = segmentPoint;
                }
            }

            return minDistPoint;
        }

        public float ClosestPosition(Vector3 point)
        {
            if (points.Count == 0)
            {
                throw new InvalidOperationException(
                    "The polyline has no points, thus cannot get a point at a position.");
            }

            if (points.Count == 1)
            {
                return 0f;
            }

            var minSqrDist = float.PositiveInfinity;
            var minDistPoint = Vector3.zero;
            var minDistIndex = -1;

            for (var i = 1; i < points.Count; i++)
            {
                var from = points[i - 1];
                var to = points[i];

                var segmentPoint = Math3D.ClosestPointOnLineSegment(point, from, to);
                var sqrDist = (segmentPoint - point).sqrMagnitude;

                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    minDistPoint = segmentPoint;
                    minDistIndex = i;
                }
            }

            var distanceToStart = distances[minDistIndex - 1];
            var start = points[minDistIndex - 1];
            return (float) (distanceToStart + (minDistPoint - start).magnitude);
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            return points.GetEnumerator();
        }

        public void Add(Vector3 point)
        {
            points.Add(point);

            var count = points.Count;
            if (count > 1)
            {
                var length = Length + (point - points[count - 2]).magnitude;
                distances.Add(length);
            }
        }

        public void Clear()
        {
            this.points.Clear();
            this.distances.Clear();
        }

        public void CopyTo(Vector3[] array, int arrayIndex)
        {
            points.CopyTo(array, arrayIndex);
        }

        public bool Remove(Vector3 point)
        {
            var index = points.IndexOf(point);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void Insert(int index, Vector3 point)
        {
            points.Insert(index, point);
            
            if (points.Count >= 2)
            {
                double change;

                if (index == 0)
                {
                    distances.Insert(0, 0f);
                    change = VectorMath.PreciseDistance(points[1], point); 
                }
                else if (index == points.Count - 1)
                {
                    change = 0.0;
                    var length = VectorMath.PreciseDistance(point, points[index - 1]);
                    var prevLength = distances[distances.Count - 1];
                    distances.Add(prevLength + length);
                }
                else
                {
                    var prev = points[index - 1];
                    var next = points[index + 1];
                    var prevLength = distances[index - 1];

                    var l0 = VectorMath.PreciseDistance(point, prev);
                    var l1 = VectorMath.PreciseDistance(next, point);
                    var lOld = distances[index] - prevLength;

                    distances.Insert(index, prevLength + l0);

                    change = l0 + l1 - lOld;
                }

                // Propagate the distance change along subsequent distances
                for (var i = index + 1; i < distances.Count; i++)
                {
                    distances[i] += change;
                }
            }
        }

        public void RemoveAt(int index)
        {
            if (points.Count < 3)
            {
                if (distances.Count == 2)
                {
                    distances.RemoveAt(1);
                }

                distances[0] = 0f;
            }
            else
            {
                double change;

                if (index == 0)
                {
                    change = -distances[1];
                    distances.RemoveAt(1);
                }
                else if (index == points.Count - 1)
                {
                    change = 0f;
                    distances.RemoveAt(index);
                }
                else
                {
                    var lp = distances[index - 1];
                    var ln = distances[index + 1];
                    var prev = points[index - 1];
                    var next = points[index + 1];
                    var npDist = (next - prev).magnitude;
                    var newDist = lp + npDist;

                    distances.RemoveAt(index);
                    distances[index] = newDist;
                    change = newDist - ln;
                }

                // Propagate changes of length
                for (var i = index + 1; i < distances.Count; i++)
                {
                    distances[i] += change;
                }
            }

            points.RemoveAt(index);
        }

        /// <summary>
        /// Many insertions, removals or replacements can lead to the length value loosing precision.<br/>
        /// If precision is important, call this method before, in order to make the length precise again.
        /// </summary>
        public void RecalculateLength()
        {
            var length = 0.0;

            distances.Clear();
            distances.Add(0.0f);

            for (var i = 1; i < points.Count; i++)
            {
                length += (points[i] - points[i - 1]).magnitude;
                distances.Add(length);
            }
        }

        public Vector3 this[int index]
        {
            get => points[index];
            set
            {
                var old = points[index];
                points[index] = value;

                if (points.Count >= 2)
                {
                    double change;

                    if (index == 0)
                    {
                        var next = points[1];
                        var length = (next - value).magnitude;
                        change = length - distances[1];
                    }
                    else if (index == points.Count - 1)
                    {
                        var prev = points[index - 1];
                        distances[index] = (value - prev).magnitude + distances[index - 1];
                        change = 0.0;
                    }
                    else
                    {
                        var prev = points[index - 1];
                        var next = points[index + 1];
                        var lp = distances[index - 1];
                        var ln = distances[index + 1];
                        double l0 = (value - prev).magnitude;
                        double l1 = (next - value).magnitude;

                        distances[index] = l0 + lp;
                        change = l0 + l1 - (ln - lp);
                    }

                    for (var i = index + 1; i < distances.Count; i++)
                    {
                        distances[i] += change;
                    }
                }
            }
        }

        #region Hidden

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection<Vector3>.Contains(Vector3 point)
        {
            return points.Contains(point);
        }

        bool ICollection<Vector3>.IsReadOnly => false;

        int IList<Vector3>.IndexOf(Vector3 point)
        {
            return points.IndexOf(point);
        }

        #endregion
    }
}
