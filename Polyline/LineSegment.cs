// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         LineSegment.cs
// 
// Created:          16.08.2019  16:33
// Last modified:    25.10.2019  11:38
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
using Unity_Tools.Core;

namespace Unity_Tools.Polyline
{
    [Serializable]
    public class LineSegment : IPolyline
    {
        [SerializeField]
        private Vector3 end;

        [SerializeField]
        private float length;

        [SerializeField]
        private Vector3 start;

        public LineSegment()
        {
            this.start = Vector3.zero;
            this.end = Vector3.zero;
            this.length = 0f;
        }

        public LineSegment(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
            this.length = Vector3.Distance(start, end);
        }

        public Vector3 End
        {
            get => end;
            set
            {
                end = value;
                length = Vector3.Distance(start, end);
            }
        }

        public Vector3 Start
        {
            get => start;
            set
            {
                start = value;
                length = Vector3.Distance(start, end);
            }
        }

        /// <inheritdoc/>
        public float Length => length;

        /// <inheritdoc/>
        public Vector3 GetPointAtPosition(float position)
        {
            if (position < 0 || position > length)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "The position must be non-negative and less than or equal to the length.");
            }

            var relative = position / length;
            return (end - start) * relative;
        }

        /// <inheritdoc/>
        public Vector3 GetDirectionAtPosition(float position)
        {
            if (position < 0 || position > length)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "The position must be non-negative and less than or equal to the length.");
            }

            return (end - start) / length;
        }

        /// <inheritdoc/>
        public Vector3 ClosestPoint(Vector3 point)
        {
            return Math3D.ClosestPointOnLineSegment(point, start, end);
        }

        /// <inheritdoc/>
        public float ClosestPosition(Vector3 point)
        {
            return Mathf.Clamp(Math3D.RelativeClosestPositionToLine(point, start, end), 0, length);
        }
    }
}
