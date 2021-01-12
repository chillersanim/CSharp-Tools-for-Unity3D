using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityTools.Core
{
    public struct Rect : IArea
    {
        private readonly Vector2 center;

        private readonly Vector2 halfSize;

        private readonly float sin;

        private readonly float cos;

        private readonly float rotation;

        public Vector2 Center => this.center;

        public Vector2 Extend => this.halfSize;

        public Vector2 Size => this.halfSize * 2f;

        public float Rotation => this.rotation;

        public Rect(Vector2 center, Vector2 size)
        {
            this.center = center;
            this.halfSize = size.AbsComponents() / 2f;
            this.sin = 0f;
            this.cos = 1f;
            this.rotation = 0f;
        }

        public Rect(Vector2 center, Vector2 size, float rotation)
        {
            this.center = center;
            this.halfSize = size.AbsComponents() / 2f;
            this.rotation = rotation;
            
            this.sin = Mathf.Sin(rotation);
            this.cos = Mathf.Cos(rotation);
        }

        /// <inheritdoc />
        public Bounds2 Bounds
        {
            get
            {
                var sx = Mathf.Abs(this.cos * this.halfSize.x) + Mathf.Abs(this.sin * this.halfSize.y);
                var sy = Mathf.Abs(this.sin * this.halfSize.x) + Mathf.Abs(this.cos * this.halfSize.y);
                return new Bounds2(this.center, new Vector2(sx * 2f, sy * 2f));
            }
        }

        /// <inheritdoc />
        public bool Inverted => false;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsRect(Vector2 start, Vector2 end)
        {
            return this.ContainsPoint(start) &&
                   this.ContainsPoint(end) &&
                   this.ContainsPoint(new Vector2(start.x, end.y)) &&
                   this.ContainsPoint(new Vector2(end.x, start.y));
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(Vector2 point)
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
        public bool IntersectsRect(Vector2 start, Vector2 end)
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
        public bool Raycast(Vector2 orig, Vector2 dir, out float t, out Vector2 normal)
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
    }
}
