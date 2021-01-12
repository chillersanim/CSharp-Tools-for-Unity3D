using UnityEngine;

namespace UnityTools.Core
{
    public static class Math2D
    {
        /// <summary>
        /// Given a line <b>orig + t * dir</b>, finds the point on that line segment that is closest to the given <c>point</c>.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="orig">A point on the line.</param>
        /// <param name="dir">The direction of the line.</param>
        /// <returns>Returns the point on the line that is closest to the given <c>point</c>.</returns>
        public static Vector2 ClosestPointOnLine(Vector2 point, Vector2 orig, Vector2 dir)
        {
            return orig + ClosestPositionOnLine(point, orig, dir) * dir;
        }

        /// <summary>
        /// Given a line segment going from <c>start</c> to <c>end</c>, finds the point on that line segment that is closest to the given <c>point</c>.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="start">The start of the line segment.</param>
        /// <param name="end">The end of the line segment.</param>
        /// <returns>Returns the point on the line segment that is closest to the given <c>point</c>.</returns>
        public static Vector2 ClosestPointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            var dir = end - start;
            return start + Mathf.Clamp01(ClosestPositionOnLine(point, start, dir)) * dir;
        }

        /// <summary>
        /// Given a line <c>orig + t * dir</c>, calculates the <c>t</c> at which the line is closest to the <c>point</c>.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="orig">A point on the line.</param>
        /// <param name="dir">The direction of the line.</param>
        /// <returns>Returns the <c>t</c> at which the line is closest to the <c>point</c>.</returns>
        public static float ClosestPositionOnLine(Vector2 point, Vector2 orig, Vector2 dir)
        {
            var abx = dir.x - orig.x;
            var aby = dir.y - orig.y;

            return -(abx * (orig.x - point.x) + aby * (orig.y - point.y)) / (abx * abx + aby * aby);
        }

        /// <summary>
        /// Given an axis aligned rectangle defined by <c>min</c> and <c>max</c>, evaluates whether the <c>point</c> is inside.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="min">The minimum of the axis aligned rectangle (Bottom left corner).</param>
        /// <param name="max">The maximum of the axis aligned rectangle (Top right corner).</param>
        /// <returns>Returns <c>true</c> if the point is inside the rect or on its boundary, <c>false</c> otherwise.</returns>
        public static bool InRectMm(this Vector2 point, Vector2 min, Vector2 max)
        {
            return min.x <= point.x && min.y <= point.y && point.x <= max.x && point.y <= max.y;
        }

        /// <summary>
        /// Given an axis aligned rectangle defined by center and size, evaluates whether the point is inside.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="center">The center of the axis aligned rectangle.</param>
        /// <param name="size">The size of the axis aligned rectangle.</param>
        /// <returns>Returns <c>true</c> if the point is inside the rect or on its boundary, <c>false</c> otherwise.</returns>
        public static bool InRectCs(this Vector2 point, Vector2 center, Vector2 size)
        {
            var halfSize = size.AbsComponents() / 2f;
            return InRectMm(point, center - halfSize, center + halfSize);
        }
        
        /// <summary>
        /// Given two lines <b>orig0 + t0 * dir0</b> and <b>orig1 + t1 * dir1</b>, calculates at which <c>t0</c> they intersect.
        /// </summary>
        /// <param name="orig0">A point on the first line.</param>
        /// <param name="dir0">The direction of the first line.</param>
        /// <param name="orig1">A point on the second line.</param>
        /// <param name="dir1">The direction of the second line.</param>
        /// <param name="t0">Returns the value of <c>t0</c> at which the intersection occurs.</param>
        /// <returns>Returns <c>true</c> if the lines intersect, <c>false</c> otherwise.</returns>
        public static bool LineLineIntersection(Vector2 orig0, Vector2 dir0, Vector2 orig1, Vector2 dir1, out float t0)
        {
            var b = dir0.x * dir1.y - dir0.y * dir1.x;

            if (Mathf.Abs(b) <= Mathf.Epsilon)
            {
                t0 = float.PositiveInfinity;
                return false;
            }

            var x10 = orig1.x - orig0.x;
            var y10 = orig1.y - orig0.y;
            var a0 = x10 * dir1.y - y10 * dir1.x;

            t0 = a0 / b;
            return true;
        }

        /// <summary>
        /// Given two lines <b>orig0 + t0 * dir0</b> and <b>orig1 + t1 * dir1</b>, calculates at which <c>t0</c> and <c>t1</c> they intersect.
        /// </summary>
        /// <param name="orig0">A point on the first line.</param>
        /// <param name="dir0">The direction of the first line.</param>
        /// <param name="orig1">A point on the second line.</param>
        /// <param name="dir1">The direction of the second line.</param>
        /// <param name="t0">Returns the value of <c>t0</c> at which the intersection occurs.</param>
        /// <param name="t1">Returns the value of <c>t1</c> at which the intersection occurs.</param>
        /// <returns>Returns <c>true</c> if the lines intersect, <c>false</c> otherwise.</returns>
        public static bool LineLineIntersection(Vector2 orig0, Vector2 dir0, Vector2 orig1, Vector2 dir1, out float t0, out float t1)
        {
            var b = dir0.x * dir1.y - dir0.y * dir1.x;

            if (Mathf.Abs(b) <= Mathf.Epsilon)
            {
                t0 = float.PositiveInfinity;
                t1 = float.PositiveInfinity;
                return false;
            }

            var x10 = orig1.x - orig0.x;
            var y10 = orig1.y - orig0.y;
            var a0 = x10 * dir1.y - y10 * dir1.x;
            var a1 = x10 * dir0.y - y10 * dir0.x;

            t0 = a0 / b;
            t1 = a1 / b;
            return true;
        }
        
        /// <summary>
        /// Given a line <b>orig + t * dir</b>, calculates at which <c>t</c> that line intersects with the x axis.
        /// </summary>
        /// <param name="orig">A point on the line.</param>
        /// <param name="dir">The direction of the line.</param>
        /// <param name="t">Returns the value of <c>t</c> at which the intersection occurs.</param>
        /// <returns>Returns <c>true</c> if the line intersects with the x axis, <c>false</c> otherwise.</returns>
        public static bool LineXAxisIntersection(Vector2 orig, Vector2 dir, out float t)
        {
            if (Mathf.Abs(dir.y) <= Mathf.Epsilon)
            {
                t = float.PositiveInfinity;
                return false;
            }

            t = -orig.y / dir.y;
            return true;
        }

        /// <summary>
        /// Given a line <b>orig + t * dir</b>, calculates at which <c>t</c> and <c>x</c> that line intersects with the x axis.
        /// </summary>
        /// <param name="orig">A point on the line.</param>
        /// <param name="dir">The direction of the line.</param>
        /// <param name="t">Returns the value of <c>t</c> at which the intersection occurs.</param>
        /// <param name="x">Returns the <c>x</c> value on the x axis at which the intersection occurs.</param>
        /// <returns>Returns <c>true</c> if the line intersects with the x axis, <c>false</c> otherwise.</returns>
        public static bool LineXAxisIntersection(Vector2 orig, Vector2 dir, out float t, out float x)
        {
            if (Mathf.Abs(dir.y) <= Mathf.Epsilon)
            {
                t = float.PositiveInfinity;
                x = float.PositiveInfinity;
                return false;
            }

            t = -orig.y / dir.y;
            x = orig.x + dir.x * t;
            return true;
        }

        /// <summary>
        /// Given a line <b>orig + t * dir</b>, calculates at which <c>t</c> that line intersects with the y axis.
        /// </summary>
        /// <param name="orig">A point on the line.</param>
        /// <param name="dir">The direction of the line.</param>
        /// <param name="t">Returns the value of <c>t</c> at which the intersection occurs.</param>
        /// <returns>Returns <c>true</c> if the line intersects with the y axis, <c>false</c> otherwise.</returns>
        public static bool LineYAxisIntersection(Vector2 orig, Vector2 dir, out float t)
        {
            if (Mathf.Abs(dir.x) <= Mathf.Epsilon)
            {
                t = float.PositiveInfinity;
                return false;
            }

            t = -orig.x / dir.x;
            return true;
        }

        /// <summary>
        /// Given a line <b>orig + t * dir</b>, calculates at which <c>t</c> and <c>y</c> that line intersects with the y axis.
        /// </summary>
        /// <param name="orig">A point on the line.</param>
        /// <param name="dir">The direction of the line.</param>
        /// <param name="t">Returns the value of <c>t</c> at which the intersection occurs.</param>
        /// <param name="y">Returns the <c>y</c> value on the y axis at which the intersection occurs.</param>
        /// <returns>Returns <c>true</c> if the line intersects with the y axis, <c>false</c> otherwise.</returns>
        public static bool LineYAxisIntersection(Vector2 orig, Vector2 dir, out float t, out float y)
        {
            if (Mathf.Abs(dir.x) <= Mathf.Epsilon)
            {
                t = float.PositiveInfinity;
                y = float.PositiveInfinity;
                return false;
            }

            t = -orig.x / dir.x;
            y = orig.y + dir.y * t;
            return true;
        }

        /// <summary>
        /// Given a ray <c>orig + t * dir</c> and an axis aligned rectangle defined by <c>min</c> and <c>max</c>, calculates if and where an intersection occurs.
        /// </summary>
        /// <param name="orig">The starting point of the ray.</param>
        /// <param name="dir">The direction of the ray.</param>
        /// <param name="min">The minimum of the axis aligned rectangle (Bottom left corner).</param>
        /// <param name="max">The maximum of the axis aligned rectangle (Top right corner).</param>
        /// <param name="t">Returns the closest <c>t</c> for which an intersection occurs.</param>
        /// <returns>Returns <c>true</c> if an intersection occurs, <c>false</c> otherwise.</returns>
        public static bool RayRectMmIntersection(Vector2 orig, Vector2 dir, Vector2 min, Vector2 max, out float t)
        {
            t = float.PositiveInfinity;

            // Left
            if (LineYAxisIntersection(new Vector2(orig.x - min.x, orig.y), dir, out var ct, out var a))
            {
                if (0f <= ct && min.y <= a && a <= max.y)
                {
                    t = ct;
                }
            }

            // Right
            if (LineYAxisIntersection(new Vector2(orig.x - max.x, orig.y), dir, out ct, out a))
            {
                if (0f <= ct && ct < t && min.y <= a && a <= max.y)
                {
                    t = ct;
                }
            }

            // Bottom
            if (LineXAxisIntersection(new Vector2(orig.x, orig.y - min.y), dir, out ct, out a))
            {
                if (0f <= ct && ct < t && min.x <= a && a <= max.x)
                {
                    t = ct;
                }
            }

            // Top
            if (LineXAxisIntersection(new Vector2(orig.x, orig.y - max.y), dir, out ct, out a))
            {
                if (0f <= ct && ct < t && min.x <= a && a <= max.x)
                {
                    t = ct;
                }
            }

            return !float.IsPositiveInfinity(t);
        }

        /// <summary>
        /// Given a ray <c>orig + t * dir</c> and an axis aligned rectangle defined by <c>center</c> and <c>size</c>, calculates if and where an intersection occurs.
        /// </summary>
        /// <param name="orig">The starting point of the ray.</param>
        /// <param name="dir">The direction of the ray.</param>
        /// <param name="center">The center of the axis aligned rectangle (Bottom left corner).</param>
        /// <param name="size">The size of the axis aligned rectangle (Top right corner).</param>
        /// <param name="t">Returns the closest <c>t</c> for which an intersection occurs.</param>
        /// <returns>Returns <c>true</c> if an intersection occurs, <c>false</c> otherwise.</returns>
        public static bool RayRectCsIntersection(Vector2 orig, Vector2 dir, Vector2 center, Vector2 size,
            out float t)
        {
            var halfSize = size.AbsComponents() / 2f;
            return RayRectMmIntersection(orig, dir, center - halfSize, center + halfSize, out t);
        }
    }
}
