using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityTools.Core
{
    [Serializable]
    public struct Bounds2 : IArea, IEquatable<Bounds2>
    {
        public static readonly Bounds2 zero = new Bounds2(Vector2.zero, Vector2.zero);

        [SerializeField]
        private Vector2 min, max;

        public Vector2 Min
        {
            readonly get => this.min;
            set
            {
                this.min = value;
                this.max = Vector2.Max(this.min, this.max);
            }
        }

        public Vector2 Max
        {
            readonly get => this.max;
            set
            {
                this.max = value;
                this.min = Vector2.Min(this.min, this.max);
            }
        }

        public Vector2 Center
        {
            readonly get => (this.min + this.max) / 2f;
            set
            {
                var offset = value - this.Center;
                this.min += offset;
                this.max += offset;
            }
        }

        public Vector2 Size
        {
            readonly get => this.max - this.min;
            set
            {
                var change = (value.AbsComponents() - this.Size) / 2f;
                this.min -= change;
                this.max += change;
            }
        }

        public Vector2 Extends
        {
            readonly get => (this.max - this.min) / 2f;
            set => this.Size = value * 2f;
        }

        /// <inheritdoc />
        public readonly Bounds2 Bounds => this;

        /// <inheritdoc />
        public readonly bool Inverted => this.max.x < this.min.x || this.max.y < this.min.y;

        public Bounds2(in Vector2 center, in Vector2 size)
        {
            var halfSize = size.AbsComponents() / 2f;
            this.min = center - halfSize;
            this.max = center + halfSize;
        }
        
        public readonly Vector2 ClosestPointInBounds(in Vector2 point)
        {
            return point.ClampComponents(in this.min, in this.max);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ContainsRect(in Vector2 start, in Vector2 end)
        {
            return this.Min.x < start.x && this.Min.y < start.y &&
                   this.Max.x > end.x && this.Max.y > end.y;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ContainsPoint(in Vector2 point)
        {
            return point.x > this.Min.x && point.y > this.Min.y &&
                   point.x < this.Max.x && point.y < this.Max.y;
        }

        public void Encapsulate(in Vector2 point)
        {
            if (point.x < this.min.x)
            {
                this.min.x = point.x;
            }

            if (point.y < this.min.y)
            {
                this.min.y = point.y;
            }

            if (point.x > this.max.x)
            {
                this.max.x = point.x;
            }

            if (point.y > this.max.y)
            {
                this.max.y = point.y;
            }
        }

        public void Encapsulate(in Bounds2 bounds)
        {
            if (bounds.min.x < this.min.x)
            {
                this.min.x = bounds.min.x;
            }

            if (bounds.min.y < this.min.y)
            {
                this.min.y = bounds.min.y;
            }

            if (bounds.max.x > this.max.x)
            {
                this.max.x = bounds.max.x;
            }

            if (bounds.max.y > this.max.y)
            {
                this.max.y = bounds.max.y;
            }
        }

        /// <inheritdoc />
        public bool Equals(Bounds2 other)
        {
            return this.min.Equals(other.min) && this.max.Equals(other.max);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Bounds2 other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.min.GetHashCode() * 397) ^ this.max.GetHashCode();
            }
        }
        
        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IntersectsRect(in Vector2 start, in Vector2 end)
        {
            return this.min.x < end.x && this.min.y < end.y &&
                   this.max.x > start.x && this.max.y > start.y;
        }

        public readonly bool Intersects(in Bounds2 other)
        {
            return this.min.x < other.max.x && this.min.y < other.max.y &&
                   this.max.x > other.min.x && this.max.y > other.min.y;
        }

        /// <inheritdoc />
        public readonly bool Raycast(in Vector2 orig, in Vector2 dir, out float t, out Vector2 normal)
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

        public static Bounds2 FromMinMax(in Vector2 min, in Vector2 max)
        {
            return new Bounds2{min = min, max = max};
        }

        public static Bounds2 operator *(in Bounds2 bounds, in float scale)
        {
            if (scale < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scale), "A bounding box cannot be scaled negatively.");
            }

            return new Bounds2(bounds.Center, bounds.Size * scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Bounds(in Bounds2 bounds)
        {
            return new Bounds(bounds.Center, bounds.Size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Bounds2(in Bounds bounds)
        {
            return new Bounds2(bounds.center, bounds.size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Bounds2 lhs, in Bounds2 rhs)
        {
            return lhs.min == rhs.min && lhs.max == rhs.max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Bounds2 lhs, in Bounds2 rhs)
        {
            return lhs.min != rhs.min || lhs.max != rhs.max;
        }
    }
}
