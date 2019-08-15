// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         LinearPolyline.cs
// 
// Created:          13.08.2019  13:45
// Last modified:    15.08.2019  17:57
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
// 

using System;
using System.Collections;
using System.Collections.Generic;
using Unity_Tools.Core.Polyline;

namespace Assets.Unity_Tools.Core.Polyline
{
    [Serializable]
    public class LinearPolyline : IPolyline, IList<Vector3>, IReadOnlyList<Vector3>
    {
        /// <summary>
        /// Contain the distance from the start point to the point at the given index (distance along the polyline).
        /// </summary>
        [SerializeField]
        private readonly List<float> distances;

        [SerializeField]
        private readonly List<Vector3> points;

        public LinearPolyline()
        {
            this.points = new List<Vector3>();
            distances = new List<float>();
            distances.Add(0);
        }

        public LinearPolyline(params Vector3[] points)
        {
            this.points = new List<Vector3>(points);
            this.distances = new List<float>(points.Length);
            this.distances.Add(0);

            var length = 0f;
            for (var i = 1; i < points.Length; i++)
            {
                length += (points[i] - points[i - 1]).magnitude;
                this.distances.Add(length);
            }
        }

        public int Count => points.Count;

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
                float change;

                if (index == 0)
                {
                    distances.Insert(0, 0f);
                    change = (points[1] - point).magnitude;
                }
                else if (index == points.Count - 1)
                {
                    change = 0f;
                    var length = (point - points[index - 1]).magnitude;
                    var prevLength = distances[distances.Count - 1];
                    distances.Add(prevLength + length);
                }
                else
                {
                    var prev = points[index - 1];
                    var next = points[index + 1];
                    var prevLength = distances[index - 1];

                    var l0 = (point - prev).magnitude;
                    var l1 = (next - point).magnitude;
                    var lOld = distances[index] - prevLength;

                    distances.Insert(index, prevLength + l0);

                    change = l0 + l1 - lOld;
                }

                // Propagate changes of length
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
                float change;

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

        public Vector3 this[int index]
        {
            get => points[index];
            set
            {
                var old = points[index];
                points[index] = value;

                if (points.Count >= 2)
                {
                    float change;

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
                        change = 0f;
                    }
                    else
                    {
                        var prev = points[index - 1];
                        var next = points[index + 1];
                        var lp = distances[index - 1];
                        var ln = distances[index + 1];
                        var l0 = (value - prev).magnitude;
                        var l1 = (next - value).magnitude;

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

        public float Length => distances[distances.Count - 1];

        public Vector3 GetPointAtPosition(float position)
        {
            throw new NotImplementedException();
        }

        public Vector3 GetDirectionAtPosition(float position)
        {
            throw new NotImplementedException();
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            throw new NotImplementedException();
        }

        public float ClosestPosition(Vector3 point)
        {
            throw new NotImplementedException();
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
