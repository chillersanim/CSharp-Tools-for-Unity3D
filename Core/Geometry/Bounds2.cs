using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityTools.Core
{
    public struct Bounds2 : IArea
    {
        private Vector2 min, max;

        public Vector2 Min => this.min;

        public Vector2 Max => this.max;

        public Vector2 Center => (this.min + this.max) / 2f;

        public Vector2 Size => this.max - this.min;

        public Vector2 Extends => (this.max - this.min) / 2f;

        public Bounds2(Vector2 center, Vector2 size)
        {
            var halfSize = size.AbsComponents() / 2f;
            this.min = center - halfSize;
            this.max = center + halfSize;
        }

        public static Bounds2 operator *(Bounds2 bounds, float scale)
        {
            if (scale < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "A bounding box cannot be scaled negatively.");
            }

            return new Bounds2(bounds.Center, bounds.Size * scale);
        }

        /// <inheritdoc />
        public Bounds2 Bounds => this;

        /// <inheritdoc />
        public bool Inverted => this.max.x < this.min.x || this.max.y < this.min.y;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsRect(Vector2 start, Vector2 end)
        {
            return this.Min.x < start.x && this.Min.y < start.y &&
                   this.Max.x > end.x && this.Max.y > end.y;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector2 point)
        {
            return point.x > this.Min.x && point.y > this.Min.y &&
                   point.x < this.Max.x && point.y < this.Max.y;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsRect(Vector2 start, Vector2 end)
        {
            return this.Min.x < end.x && this.Min.y < end.y &&
                   this.Max.x > start.x && this.Max.y > start.y;
        }

        /// <inheritdoc />
        public bool Raycast(Vector2 orig, Vector2 dir, out float t, out Vector2 normal)
        {
            t = float.PositiveInfinity;
            normal = Vector2.zero;

            // Left
            if (Math2D.LineYAxisIntersection(new Vector2(orig.x, orig.y - this.min.y), dir, out var ct, out var a))
            {
                if (0f <= ct && ct < t && this.min.y <= a && a <= this.max.y)
                {
                    t = ct;
                    normal = Vector2.left;
                }
            }

            // Right
            if (Math2D.LineYAxisIntersection(new Vector2(orig.x, orig.y - this.max.y), dir, out ct, out a))
            {
                if (0f <= ct && ct < t && this.min.y <= a && a <= this.max.y)
                {
                    t = ct;
                    normal = Vector2.right;
                }
            }

            // Bottom
            if (Math2D.LineXAxisIntersection(new Vector2(orig.x - this.min.x, orig.y), dir, out ct, out a))
            {
                if (0f <= ct && ct < t && this.min.x <= a && a <= this.max.x)
                {
                    t = ct;
                    normal = Vector2.down;
                }
            }

            // Top
            if (Math2D.LineXAxisIntersection(new Vector2(orig.x - this.max.x, orig.y), dir, out ct, out a))
            {
                if (0f <= ct && ct < t && this.min.x <= a && a <= this.max.x)
                {
                    t = ct;
                    normal = Vector2.up;
                }
            }

            return !float.IsPositiveInfinity(t);
        }

        public static Bounds2 FromMinMax(Vector2 min, Vector2 max)
        {
            return new Bounds2((min + max) / 2f, max - min);
        }
    }
}
