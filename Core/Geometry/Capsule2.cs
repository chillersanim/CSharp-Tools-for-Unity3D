using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityTools.Core
{
    /// <summary>
    /// Represents the 2D stadium shape.<br/>
    /// Duo to unity's naming convention, it's called Capsule2 instead of Stadium.
    /// </summary>
    public struct Capsule2 : IArea
    {
        private Vector2 center;

        private float rectHalfHeight;

        private float radius;

        private float sin;

        private float cos;

        private float rotation;

        public readonly Vector2 Center => this.center;

        public readonly float Height => (this.rectHalfHeight + this.radius) * 2f;

        public readonly float Radius => this.radius;

        public readonly float Rotation => this.rotation;

        public readonly Vector2 BottomHalfCircleCenter => new Vector2(this.center.x - this.sin * this.rectHalfHeight, this.center.y - this.cos * this.rectHalfHeight);

        public readonly Vector2 TopHalfCircleCenter => new Vector2(this.center.x + this.sin * this.rectHalfHeight, this.center.y + this.cos * this.rectHalfHeight);

        public Capsule2(in Vector2 center, in float height, in float radius)
        {
            this.center = center;
            this.radius = Mathf.Abs(radius);
            this.rectHalfHeight = Mathf.Abs(height) / 2f;
            this.rectHalfHeight = Mathf.Max(0f, this.rectHalfHeight - this.radius);
            this.sin = 0f;
            this.cos = 1f;
            this.rotation = 0f;
        }

        public Capsule2(in Vector2 center, in float height, in float radius, in float rotation)
        {
            this.center = center;
            this.radius = Mathf.Abs(radius);
            this.rectHalfHeight = Mathf.Abs(height) / 2f;
            this.rectHalfHeight = Mathf.Max(0f, this.rectHalfHeight - this.radius);
            this.sin = Mathf.Sin(rotation);
            this.cos = Mathf.Cos(rotation);
            this.rotation = rotation;
        }

        private Capsule2(in Vector2 center, in float rectHalfHeight, in float radius, in float sin, in float cos, in float rotation)
        {
            this.center = center;
            this.rectHalfHeight = rectHalfHeight;
            this.radius = radius;
            this.sin = sin;
            this.cos = cos;
            this.rotation = rotation;
        }

        /// <inheritdoc />
        public readonly Bounds2 Bounds
        {
            get
            {
                if (this.Height < 1e-5f)
                {
                    return new Bounds2(in this.center, new Vector2(this.radius * 2f, this.radius * 2f));
                }

                var sinH = this.sin * this.rectHalfHeight;
                var cosH = this.cos * this.rectHalfHeight;
                var size = new Vector2((this.radius + Mathf.Abs(sinH)) * 2f, (this.radius + Mathf.Abs(cosH)) * 2f);

                return new Bounds2(in this.center, in size);
            }
        }

        /// <inheritdoc />
        public readonly bool Inverted => false;

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
        public readonly bool ContainsPoint(in Vector2 point)
        {
            var sinH = this.sin * this.rectHalfHeight;
            var cosH = this.cos * this.rectHalfHeight;

            var c1 = new Vector2(this.center.x - sinH, this.center.y + cosH);
            var c2 = new Vector2(this.center.x + sinH, this.center.y - cosH);
            var r2 = this.radius * this.radius;

            // Test for end circles
            if ((point - c1).sqrMagnitude <= r2)
            {
                return true;
            }

            if (this.Height < 1e-5f)
            {
                return false;
            }

            if ((point - c2).sqrMagnitude <= r2)
            {
                return true;
            }

            // Test for inner rect
            var sinR = this.sin * this.radius;
            var cosR = this.cos * this.radius;

            var bl = new Vector2(c2.x - cosR, c2.y - sinR);
            var tl = new Vector2(c1.x - cosR, c1.y - sinR);
            var tr = new Vector2(c1.x + cosR, c1.y + sinR);
            var br = new Vector2(c2.x + cosR, c2.y + sinR);

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
        public readonly bool IntersectsRect(in Vector2 start, in Vector2 end)
        {
            var sinH = this.sin * this.rectHalfHeight;
            var cosH = this.cos * this.rectHalfHeight;

            var c1 = new Vector2(this.center.x - sinH, this.center.y + cosH);
            var c2 = new Vector2(this.center.x + sinH, this.center.y - cosH);
            var r2 = this.radius * this.radius;

            // Test for end circles
            var x = Mathf.Clamp(c1.x, start.x, end.x) - c1.x;
            var y = Mathf.Clamp(c1.y, start.y, end.y) - c1.y;
            var sqrDist = x + y;

            if (sqrDist * sqrDist <= r2)
            {
                return true;
            }

            if (this.Height < 1e-5f)
            {
                return false;
            }

            x = Mathf.Clamp(c2.x, start.x, end.x) - c2.x;
            y = Mathf.Clamp(c2.y, start.y, end.y) - c2.y;
            sqrDist = x + y;

            if (sqrDist * sqrDist <= r2)
            {
                return true;
            }

            var sinR = this.sin * this.radius;
            var cosR = this.cos * this.radius;

            var bl = new Vector2(c2.x - cosR, c2.y - sinR);
            var tl = new Vector2(c1.x - cosR, c1.y - sinR);
            var tr = new Vector2(c1.x + cosR, c1.y + sinR);
            var br = new Vector2(c2.x + cosR, c2.y + sinR);
            var side = tl - bl;

            if (bl.InRectMm(start, end) || tl.InRectMm(start, end) ||
                tr.InRectMm(start, end) || br.InRectMm(start, end))
            {
                return true;
            }

            // Left
            if (Math2D.RayRectMmIntersection(bl, side, start, end, out var t))
            {
                if (0f <= t && t <= 1f)
                {
                    return true;
                }
            }

            // Right
            if (Math2D.RayRectMmIntersection(br, side, start, end, out t))
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
            var startInside = this.ContainsPoint(in orig);
            t = startInside ? -1f : float.PositiveInfinity;
            normal = Vector2.zero;

            var sinH = this.sin * this.rectHalfHeight;
            var cosH = this.cos * this.rectHalfHeight;

            var c1 = new Vector2(this.center.x - sinH, this.center.y + cosH);
            var c2 = new Vector2(this.center.x + sinH, this.center.y - cosH);

            // Test for end circles
            if (this.RaycastCircle(in c1, in orig, in dir, out var tc0, out var nc0))
            {
                t = tc0;
                normal = nc0;
            }

            if (this.Height < 1e-5f)
            {
                return !startInside && !float.IsInfinity(t) || startInside && t >= 0f;
            }

            if (this.RaycastCircle(in c2, in orig, in dir, out var tc1, out var nc1))
            {
                if (!startInside && tc1 < t || startInside && tc1 > t)
                {
                    t = tc1;
                    normal = nc1;
                }
            }

            // Test for inner rect

            var sinR = this.sin * this.radius;
            var cosR = this.cos * this.radius;

            var bl = new Vector2(c2.x - cosR, c2.y - sinR);
            var tl = new Vector2(c1.x - cosR, c1.y - sinR);
            var br = new Vector2(c2.x + cosR, c2.y + sinR);
            var side = tl - bl;

            // Left
            if (Math2D.LineLineIntersection(in bl, in side, in orig, in dir, out var t0, out var ct))
            {
                if (0f <= t0 && t0 <= 1f && 0f <= ct)
                {
                    t = ct;
                    normal = this.cos * Vector2.left + this.sin * Vector2.down;
                }
            }

            // Right
            if (Math2D.LineLineIntersection(br, side, orig, dir, out t0, out ct))
            {
                if (0f <= t0 && t0 <= 1f && 0f <= ct && ct < t)
                {
                    t = ct;
                    normal = this.cos * Vector2.right + this.sin * Vector2.up;
                }
            }
            

            return !startInside && !float.IsInfinity(t) || startInside && t >= 0f;
        }

        public readonly bool GetLineIntersections(in Vector2 p0, in Vector2 p1, out float t0, out float t1)
        {
            var d = p1 - p0;
            var l = d.magnitude;
            d /= l;

            var min = float.MaxValue;
            var max = float.MinValue;

            // Test for end circles
            var sinH = this.sin * this.rectHalfHeight;
            var cosH = this.cos * this.rectHalfHeight;

            var c1 = new Vector2(this.center.x - sinH, this.center.y + cosH);
            var c2 = new Vector2(this.center.x + sinH, this.center.y - cosH);

            if (this.GetLineCircleIntersection(in c1, in p0, in d, out t0, out t1))
            {
                min = t0;
                max = t1;
            }

            if (this.Height < 1e-5)
            {
                t0 /= l;
                t1 /= l;

                return t0 < 1f && t1 > 0f;
            }

            if (this.GetLineCircleIntersection(in c2, in p0, in d, out t0, out t1))
            {
                if (t0 < min)
                {
                    min = t0;
                }

                if (t1 > max)
                {
                    max = t1;
                }
            }

            // Test for inner rect
            var sinR = this.sin * this.radius;
            var cosR = this.cos * this.radius;

            var bl = new Vector2(c2.x - cosR, c2.y - sinR);
            var tl = new Vector2(c1.x - cosR, c1.y - sinR);
            var br = new Vector2(c2.x + cosR, c2.y + sinR);
            var side = tl - bl;
            
            // Left
            if (Math2D.LineLineIntersection(in bl, in side, in p0, in d, out t0, out t1))
            {
                if (0f <= t0 && t0 <= 1f)
                {
                    if (t1 < min)
                    {
                        min = t1;
                    }

                    if (t1 > max)
                    {
                        max = t1;
                    }
                }
            }

            // Right
            if (Math2D.LineLineIntersection(in br, in side, in p0, in d, out t0, out t1))
            {
                if (0f <= t0 && t0 <= 1f)
                {
                    if (t1 < min)
                    {
                        min = t1;
                    }

                    if (t1 > max)
                    {
                        max = t1;
                    }
                }
            }

            t0 = min / l;
            t1 = max / l;

            return t0 < 1f && t1 > 0f;
        }

        public readonly void DrawGizmos()
        {
            var sinH = this.sin * this.rectHalfHeight;
            var cosH = this.cos * this.rectHalfHeight;

            var c1 = new Vector2(this.center.x - sinH, this.center.y + cosH);
            var c2 = new Vector2(this.center.x + sinH, this.center.y - cosH);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(this.Bounds.Center, this.Bounds.Size);

            // End circles
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(c1, this.radius);

            if (this.Height < 1e-5f)
            {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(c2, this.radius);

            // Inner rect
            var sinR = this.sin * this.radius;
            var cosR = this.cos * this.radius;

            var bl = new Vector2(c2.x - cosR, c2.y - sinR);
            var tl = new Vector2(c1.x - cosR, c1.y - sinR);
            var tr = new Vector2(c1.x + cosR, c1.y + sinR);
            var br = new Vector2(c2.x + cosR, c2.y + sinR);

            Gizmos.color = Color.blue;

            for (var i = 0; i < 6; i++)
            {
                Gizmos.DrawWireSphere(Vector2.Lerp(bl, tl, i / 5f), this.radius / 3f);
                Gizmos.DrawWireSphere(Vector2.Lerp(br, tr, i / 5f), this.radius / 3f);
            }
        }

        public static Capsule2 FromCircles(in Vector2 center0, in Vector2 center1, in float radius)
        {
            var center = (center0 + center1) / 2f;
            var line = center1 - center0;
            var length = line.magnitude;

            if (length < 1e-5f)
            {
                return new Capsule2(in center, length / 2f, in radius, 0f, 1f, 0f);
            }

            var cos = line.y / length;
            var sin = -line.x / length;
            var rotation = Vector2.SignedAngle(Vector2.left, line);

            return new Capsule2(in center, length / 2f, in radius, in sin, in cos, in rotation);
        }

        private readonly bool RaycastCircle(in Vector2 c, in Vector2 orig, in Vector2 dir, out float t, out Vector2 normal)
        {
            t = float.PositiveInfinity;

            var delta = orig - c;
            var deltaDir = dir.x * delta.x + dir.y * delta.y;
            var sqDir = dir.x * dir.x + dir.y * dir.y;
            var sqDelta = delta.x * delta.x + delta.y * delta.y;
            var gamma = deltaDir * deltaDir - sqDir * (sqDelta - this.Radius * this.Radius);

            if (gamma < 0)
            {
                normal = Vector2.zero;
                return false;
            }

            var sqrGamma = Mathf.Sqrt(gamma); 
            var t0 = (-deltaDir + sqrGamma) / sqDir;
            var t1 = (-deltaDir - sqrGamma) / sqDir;

            if (0f <= t0)
            {
                t = t0;
            }

            if (0f <= t1 && t1 < t)
            {
                t = t1;
            }

            if (float.IsPositiveInfinity(t))
            {
                normal = Vector2.zero;
                return false;
            }

            normal = (orig + t * dir - c) / this.Radius;
            return true;
        }

        private readonly bool GetLineCircleIntersection(in Vector2 c, in Vector2 p, in Vector2 d, out float t0, out float t1)
        {
            var delta = p - c;
            var delta2 = Vector2.Dot(delta, delta);
            var d2 = Vector2.Dot(d, d);
            var r = this.Radius;
            var r2 = r * r;
            var deltaD = Vector2.Dot(delta, d);
            var deltaD2 = deltaD * deltaD;

            var gamma = deltaD2 - d2 * (delta2 - r2);

            if (gamma <= float.Epsilon)
            {
                t0 = t1 = float.NaN;
                return false;
            }

            var sqrGamma = Mathf.Sqrt(gamma);
            t0 = (-deltaD - sqrGamma) / d2;
            t1 = (-deltaD + sqrGamma) / d2;

            if (t0 > t1)
            {
                var tmp = t0;
                t0 = t1;
                t1 = tmp;
            }

            return true;
        }
    }
}
