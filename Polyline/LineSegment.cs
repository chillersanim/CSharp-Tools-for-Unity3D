using System;
using UnityEngine;
using Unity_Tools.Core;

namespace Unity_Tools.Polyline
{
    [Serializable]
    public class LineSegment : IPolyline
    {
        [SerializeField]
        private Vector3 start;

        [SerializeField]
        private Vector3 end;

        [SerializeField]
        private float length;

        public Vector3 Start
        {
            get => start;
            set
            {
                start = value;
                length = Vector3.Distance(start, end);
            }
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

        /// <inheritdoc/>
        public float Length => length;

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
