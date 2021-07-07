using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityTools.Core
{
    [Serializable]
    public struct Rect : IArea
    {
        public static readonly Rect zero = new Rect(Vector2.zero, Vector2.zero);

        private Vector2 center;

        private Vector2 halfSize;

        private float sin;

        private float cos;

        private float rotation;

        public readonly Vector2 Center => this.center;

        public readonly Vector2 Extend => this.halfSize;

        public readonly Vector2 Size => this.halfSize * 2f;

        public readonly float Rotation => this.rotation;

        /// <inheritdoc />
        public readonly Bounds2 Bounds
        {
            get
            {
                var sx = Mathf.Abs(this.cos * this.halfSize.x) + Mathf.Abs(this.sin * this.halfSize.y);
                var sy = Mathf.Abs(this.sin * this.halfSize.x) + Mathf.Abs(this.cos * this.halfSize.y);
                return new Bounds2(this.center, new Vector2(sx * 2f, sy * 2f));
            }
        }

        /// <inheritdoc />
        public readonly bool Inverted => false;

        public Rect(in Vector2 center, in Vector2 size)
        {
            this.center = center;
            this.halfSize = size.AbsComponents() / 2f;
            this.sin = 0f;
            this.cos = 1f;
            this.rotation = 0f;
        }

        public Rect(in Vector2 center, in Vector2 size, in float rotation)
        {
            this.center = center;
            this.halfSize = size.AbsComponents() / 2f;
            this.rotation = rotation;
            
            this.sin = Mathf.Sin(rotation);
            this.cos = Mathf.Cos(rotation);
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ContainsRect(in Vector2 start, in Vector2 end)
        {
            return this.ContainsPoint(start) &&
                   this.ContainsPoint(end) &&
                   this.ContainsPoint(new Vector2(start.x, end.y)) &&
                   this.ContainsPoint(new Vector2(end.x, start.y));
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ContainsPoint(in Vector2 point)
        {
            var sinX = this.sin * this.halfSize.x;
            var sinY = this.sin * this.halfSize.y;
            var cosX = this.cos * this.halfSize.x;
            var cosY = this.cos * this.halfSize.y;

            var bl = this.center + new Vector2(-cosX + sinY, -sinX - cosY);
            var tl = this.center + new Vector2(-cosX - sinY, -sinX + cosY);
            var tr = this.center + new Vector2(cosX - sinY, sinX + cosY);
            var br = this.center + new Vector2(cosX + sinY, sinX - cosY);

            var tlbl = tl - bl;
            var trtl = tr - tl;
            var brtr = br - tr;
            var blbr = bl - br;

            var sideLeft = tlbl.x * (point.y - bl.y) - tlbl.y * (point.x - bl.x);
            var sideTop = trtl.x * (point.y - tl.y) - trtl.y * (point.x - tl.x);
            var sideRight = brtr.x * (point.y - tr.y) - brtr.y * (point.x - tr.x);
            var sideBottom = blbr.x * (point.y - br.y) - blbr.y * (point.x - br.x);

            return sideLeft <= 0 && sideTop <= 0 && sideRight <= 0 && sideBottom <= 0;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IntersectsRect(in Vector2 start, in Vector2 end)
        {
            var sinX = this.sin * this.halfSize.x;
            var sinY = this.sin * this.halfSize.y;
            var cosX = this.cos * this.halfSize.x;
            var cosY = this.cos * this.halfSize.y;

            var bl = this.center + new Vector2(-cosX + sinY, -sinX - cosY);
            var tl = this.center + new Vector2(-cosX - sinY, -sinX + cosY);
            var tr = this.center + new Vector2(cosX - sinY, sinX + cosY);
            var br = this.center + new Vector2(cosX + sinY, sinX - cosY);

            if (bl.InRectMm(start, end) || tl.InRectMm(start, end) ||
                tr.InRectMm(start, end) || br.InRectMm(start, end))
            {
                return true;
            }

            // Left
            if (Math2D.RayRectMmIntersection(bl, tl - bl, start, end, out var t))
            {
                if (0f <= t && t <= 1f)
                {
                    return true;
                }
            }

            // Top
            if (Math2D.RayRectMmIntersection(tl, tr - tl, start, end, out t))
            {
                if (0f <= t && t <= 1f)
                {
                    return true;
                }
            }

            // Right
            if (Math2D.RayRectMmIntersection(tr, br - tr, start, end, out t))
            {
                if (0f <= t && t <= 1f)
                {
                    return true;
                }
            }

            // Bottom
            if (Math2D.RayRectMmIntersection(br, bl - br, start, end, out t))
            {
                if (0f <= t && t <= 1f)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public readonly bool Raycast(in Vector2 orig, in Vector2 dir, out float t, out Vector2 normal)
        {
            t = float.PositiveInfinity;
            normal = Vector2.zero;

            var sinX = this.sin * this.halfSize.x;
            var sinY = this.sin * this.halfSize.y;
            var cosX = this.cos * this.halfSize.x;
            var cosY = this.cos * this.halfSize.y;

            var bl = this.center + new Vector2(-cosX + sinY, -sinX - cosY);
            var tl = this.center + new Vector2(-cosX - sinY, -sinX + cosY);
            var tr = this.center + new Vector2(cosX - sinY, sinX + cosY);
            var br = this.center + new Vector2(cosX + sinY, sinX - cosY);

            // Left
            if (Math2D.LineLineIntersection(bl, tl - bl, orig, dir, out var t0, out var ct))
            {
                if (0f <= t0 && t0 <= 1f && 0f <= ct)
                {
                    t = ct;
                    normal = this.cos * Vector2.left + this.sin * Vector2.down;
                }
            }

            // Top
            if (Math2D.LineLineIntersection(tl, tr - tl, orig, dir, out t0, out ct))
            {
                if (0f <= t0 && t0 <= 1f && 0f <= ct && ct < t)
                {
                    t = ct;
                    normal = this.cos * Vector2.up + this.sin * Vector2.left;
                }
            }

            // Right
            if (Math2D.LineLineIntersection(tr, br - tr, orig, dir, out t0, out ct))
            {
                if (0f <= t0 && t0 <= 1f && 0f <= ct && ct < t)
                {
                    t = ct;
                    normal = this.cos * Vector2.right + this.sin * Vector2.up;
                }
            }

            // Bottom
            if (Math2D.LineLineIntersection(br, bl - br, orig, dir, out t0, out ct))
            {
                if (0f <= t0 && t0 <= 1f && 0f <= ct && ct < t)
                {
                    t = ct;
                    normal = this.cos * Vector2.down + this.sin * Vector2.right;
                }
            }

            return !float.IsPositiveInfinity(t);
        }

        public static Rect FromMinMax(in Vector2 min, in Vector2 max)
        {
            return new Rect((min + max) / 2f, max - min);
        }

        public static Rect Span(in Vector2 p0, in Vector2 p1)
        {
            return new Rect(Vector2.Min(p0, p1), Vector2.Max(p0, p1));
        }
    }
}
