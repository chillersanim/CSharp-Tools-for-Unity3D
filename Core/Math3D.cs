// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Math3D.cs
// 
// Created:          12.08.2019  19:04
// Last modified:    25.08.2019  15:59
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
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Tools.Core
{
    /// <summary>
    ///     The math 3 d.
    /// </summary>
    public static class Math3D
    {
        /// <summary>
        ///     Converts cartesian coordiantes to sphericals
        /// </summary>
        /// <param name="v">
        ///     The cartesian coordinates
        /// </param>
        /// <param name="azimuth">
        ///     The output value for azimuth in degrees
        /// </param>
        /// <param name="elevation">
        ///     The output value for elevation in degrees
        /// </param>
        /// <param name="radius">
        ///     The output value for radius
        /// </param>
        /// <param name="normalized">
        ///     Optional: Set to <c>true</c> if you know that <see cref="v" /> is normalized
        /// </param>
        public static void CartesianToSpherical(
            Vector3 v,
            out float azimuth,
            out float elevation,
            out float radius)
        {
            if (Math.Abs(v.x) < float.Epsilon)
                v.x = Mathf.Epsilon;
            radius = Mathf.Sqrt((v.x * v.x) + (v.y * v.y) + (v.z * v.z));
            azimuth = Mathf.Atan(v.z / v.x);
            if (v.x < 0)
                azimuth += Mathf.PI;
            elevation = Mathf.Asin(v.y / radius);

            azimuth *= Mathf.Rad2Deg;
            elevation *= Mathf.Rad2Deg;
        }

        /// <summary>
        ///     Calculates the nearest point from point <see cref="p" /> to the axis aligned bounding box from <see cref="min" />
        ///     to <see cref="max" />
        /// </summary>
        /// <param name="p">
        ///     The point
        /// </param>
        /// <param name="min">
        ///     The starting point of the AABB
        /// </param>
        /// <param name="max">
        ///     The end point of the AABB
        /// </param>
        /// <returns>
        ///     The nearest point in the AABB
        /// </returns>
        public static Vector3 ClosestPointInAabb(Vector3 p, Vector3 min, Vector3 max)
        {
            return p.ClampComponents(min, max);
        }

        /// <summary>
        ///     Calculates the nearest point from point <see cref="p" /> to a sphere with center <see cref="c" /> and radius
        ///     <see cref="r" />
        /// </summary>
        /// <param name="p">
        ///     The point
        /// </param>
        /// <param name="c">
        ///     The center of the sphere
        /// </param>
        /// <param name="r">
        ///     The radius of the sphere
        /// </param>
        /// <returns>
        ///     Returns the nearest point in the sphere to the point
        /// </returns>
        public static Vector3 ClosestPointInSphere(Vector3 p, Vector3 c, float r)
        {
            return Vector3.ClampMagnitude(p - c, r);
        }

        /// <summary>
        ///     Calculates the nearest point of point <see cref="p" /> to the line going through <see cref="a" /> and
        ///     <see cref="b" />
        /// </summary>
        /// <param name="p">
        ///     The point
        /// </param>
        /// <param name="a">
        ///     The first point on the line
        /// </param>
        /// <param name="b">
        ///     The second point on the line
        /// </param>
        /// <returns>
        ///     The nearest point on the line
        /// </returns>
        public static Vector3 ClosestPointOnLine(Vector3 p, Vector3 a, Vector3 b)
        {
            return a + RelativeClosestPositionToLine(p, a, b) * (b - a);
        }

        /// <summary>
        ///     Calculates the nearest point of point <see cref="p" /> to the line segment from <see cref="start" /> to
        ///     <see cref="end" />
        /// </summary>
        /// <param name="p">
        ///     The point
        /// </param>
        /// <param name="start">
        ///     start point of the line segment
        /// </param>
        /// <param name="end">
        ///     The end point of the line segment
        /// </param>
        /// <returns>
        ///     The nearest point on the line segment to the point
        /// </returns>
        public static Vector3 ClosestPointOnLineSegment(Vector3 p, Vector3 start, Vector3 end)
        {
            return start + Mathf.Clamp01(RelativeClosestPositionToLine(p, start, end)) * (end - start);
        }

        /// <summary>
        ///     Calculates the nearest point of point <see cref="p" /> to the ray with <see cref="start" /> and
        ///     <see cref="direction" />
        /// </summary>
        /// <param name="p">
        ///     The point
        /// </param>
        /// <param name="start">
        ///     The start point of the ray
        /// </param>
        /// <param name="direction">
        ///     The direction of the ray
        /// </param>
        /// <returns>
        ///     The closest position on the ray to the point
        /// </returns>
        public static Vector3 ClosestPointOnRay(Vector3 p, Vector3 start, Vector3 direction)
        {
            return start + Mathf.Max(0, RelativeClosestPositionToLine(p, start, start + direction)) * direction;
        }

        /// <summary>
        ///     Calculates the exact averaged polygon normal of p0 any non complex polygon (no self intersection)<br />
        ///     SLOW: If the polygon is planar or nearly planar, consider using <see cref="PlanarPolygonArea" /> instead.
        /// </summary>
        /// <param name="v">
        ///     The v forming the polygon
        /// </param>
        /// <returns>
        ///     Returns the average normal
        /// </returns>
        public static Vector3 ExactAveragedPolygonNormal(params Vector3[] v)
        {
            return ExactAveragedPolygonNormal((IList<Vector3>) v);
        }

        public static Vector3 ExactAveragedPolygonNormal(IList<Vector3> v)
        {
            var n = v.Count;

            if (n < 3)
            {
                return Vector3.zero;
            }

            var result = Vector3.zero;

            for (var i = 1; i < n - 1; i++)
            {
                result += Vector3.Cross(v[i] - v[i - 1], v[i] - v[i + 1]).normalized;
            }

            result += Vector3.Cross(v[0] - v[n], v[0] - v[1]).normalized;
            result += Vector3.Cross(v[n - 1] - v[n - 2], v[n - 1] - v[0]).normalized;

            return result / n;
        }
        
        /// <summary>
        ///     The furthest position in aabb.
        /// </summary>
        /// <param name="p">
        ///     The p.
        /// </param>
        /// <param name="min">
        ///     The min.
        /// </param>
        /// <param name="max">
        ///     The max.
        /// </param>
        /// <returns>
        ///     The <see cref="Vector3" />.
        /// </returns>
        public static Vector3 FurthestPositionInAabb(Vector3 p, Vector3 min, Vector3 max)
        {
            var x = Mathf.Abs(min.x - p.x) > Mathf.Abs(max.x - p.x) ? min.x : max.x;
            var y = Mathf.Abs(min.y - p.y) > Mathf.Abs(max.y - p.y) ? min.y : max.y;
            var z = Mathf.Abs(min.z - p.z) > Mathf.Abs(max.z - p.z) ? min.z : max.z;

            return new Vector3(x, y, z);
        }

        /// <summary>
        ///     Determines whether two spheres are intersecting
        /// </summary>
        /// <param name="c0">
        ///     The center of the first sphere
        /// </param>
        /// <param name="r0">
        ///     The radius of the first sphere
        /// </param>
        /// <param name="c1">
        ///     The center or the second sphere
        /// </param>
        /// <param name="r1">
        ///     The radius of the second sphere
        /// </param>
        /// <returns>
        ///     <c>True</c> when the spheres intersect, otherwise <c>false</c>.
        /// </returns>
        public static bool IsIntersectingSphereSphere(Vector3 c0, float r0, Vector3 c1, float r1)
        {
            var radius = r0 + r1;
            return (c1 - c0).sqrMagnitude < radius * radius;
        }

        /// <summary>
        ///     Determines whether three points are lying on the same line.
        /// </summary>
        /// <param name="v0">
        ///     The v0.
        /// </param>
        /// <param name="v1">
        ///     The v1.
        /// </param>
        /// <param name="v2">
        ///     The v2.
        /// </param>
        /// <param name="maxError">The maximum deviation from zero of the triangle area spanned by v0, v1, v2.</param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool IsLinearExtension(Vector3 v0, Vector3 v1, Vector3 v2, float maxError = 1e-5f)
        {
            var cross = Vector3.Cross(v1 - v0, v2 - v0);
            return cross.sqrMagnitude <= maxError * maxError;
        }

        /// <summary>
        ///     Determines whether a point lies inside a triangle or not
        /// </summary>
        /// <param name="p0">
        ///     The first point of the triangle
        /// </param>
        /// <param name="p1">
        ///     The second point of the triangle
        /// </param>
        /// <param name="p2">
        ///     The thirth point of the triangle
        /// </param>
        /// <param name="point">
        ///     The point
        /// </param>
        /// <returns>
        ///     <c>True</c> when the point lies inside the triangle, otherwise <c>false</c>
        /// </returns>
        public static bool IsPointInTriangle(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 point)
        {
            var bary = PointToBarycentric(p0, p1, p2, point);

            // Check if point is in triangle
            return bary.x >= 0 && bary.y >= 0 && bary.x + bary.y < 1;
        }

        /// <summary>
        /// Determines whether a polygon is convex.
        /// </summary>
        /// <param name="polygon">The polygon. (Needs at least 3 vertices.)</param>
        /// <returns>Returns <c>true</c> if the polygon is convex, <c>false</c> otherwise.</returns>
        public static bool IsPolygonConvex(IList<Vector3> polygon)
        {
            if (polygon.Count < 3)
            {
                throw new ArgumentException("A polygon cannot have less than 3 vertices.");
            }

            var changeX = 0;
            var changeY = 0;
            var changeZ = 0;

            var currentPoint = polygon[1];
            var v0 = currentPoint - polygon[0];
            var prevSignX = Math.Sign(v0.x);
            var prevSignY = Math.Sign(v0.y);
            var prevSignZ = Math.Sign(v0.z);

            for (var i = 1; i < polygon.Count; i++)
            {
                var nextIndex = i < polygon.Count - 1 ? i + 1 : 0;
                var nextPoint = polygon[nextIndex];
                var v = nextPoint - currentPoint;
                currentPoint = nextPoint;

                var signX = Math.Sign(v.x);
                var signY = Math.Sign(v.y);
                var signZ = Math.Sign(v.z);

                if (prevSignX != signX)
                {
                    changeX++;
                }

                if (prevSignY != signY)
                {
                    changeY++;
                }

                if(prevSignZ != signZ)
                {
                    changeZ++;
                }

                prevSignX = signX;
                prevSignY = signY;
                prevSignZ = signZ;
            }

            return changeX <= 2 && changeY <= 2 && changeZ <= 2;
        }

        /// <summary>
        /// Determines whether a polygon is convex.
        /// </summary>
        /// <param name="vertices">The vertices of the polygon.</param>
        /// <param name="indices">The indices that make up the polygon.</param>
        /// <returns>Returns <c>true</c> if the polygon is convex, <c>false</c> otherwise.</returns>
        public static bool IsPolygonConvex(IList<Vector3> vertices, IList<int> indices)
        {
            if (indices.Count < 3)
            {
                throw new ArgumentException("A polygon cannot have less than 3 vertices.");
            }

            var changeX = 0;
            var changeY = 0;
            var changeZ = 0;

            var currentPoint = vertices[indices[1]];
            var v0 = currentPoint - vertices[indices[0]];
            var prevSignX = Math.Sign(v0.x);
            var prevSignY = Math.Sign(v0.y);
            var prevSignZ = Math.Sign(v0.z);

            for (var i = 1; i < indices.Count; i++)
            {
                var nextIndex = i < indices.Count - 1 ? i + 1 : 0;
                var nextPoint = vertices[indices[nextIndex]];
                var v = nextPoint - currentPoint;
                currentPoint = nextPoint;

                var signX = Math.Sign(v.x);
                var signY = Math.Sign(v.y);
                var signZ = Math.Sign(v.z);

                if (prevSignX != signX)
                {
                    changeX++;
                }

                if (prevSignY != signY)
                {
                    changeY++;
                }

                if (prevSignZ != signZ)
                {
                    changeZ++;
                }

                prevSignX = signX;
                prevSignY = signY;
                prevSignZ = signZ;
            }

            return changeX <= 2 && changeY <= 2 && changeZ <= 2;
        }

        /// <summary>
        ///     The line plane intersection.
        /// </summary>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <param name="direction">
        ///     The direction.
        /// </param>
        /// <param name="planeNormal">
        ///     The plane normal.
        /// </param>
        /// <param name="planeDistance">
        ///     The plane distance.
        /// </param>
        /// <returns>
        ///     The <see cref="float?" />.
        /// </returns>
        public static float? LinePlaneIntersection(
            Vector3 start,
            Vector3 direction,
            Vector3 planeNormal,
            float planeDistance)
        {
            if (!Mathf.Approximately(planeNormal.sqrMagnitude, 1))
            {
                planeNormal = planeNormal.normalized;
            }

            if (Mathf.Approximately(Vector3.Dot(direction, planeNormal), 0))
            {
                return null;
            }

            return (planeDistance - Vector3.Dot(start, planeNormal)) / Vector3.Dot(direction, planeNormal);
        }

        /// <summary>
        ///     Calculates the area size of p0 polygon with given v and normal
        /// </summary>
        /// <param name="v">
        ///     The polygon v, in order of how they are connected
        /// </param>
        /// <param name="normale">
        ///     The normal
        /// </param>
        /// <returns>
        ///     The area size
        /// </returns>
        public static float PlanarPolygonArea(IList<Vector3> v)
        {
            var vectorSum = Vector3.zero;
            var v0 = v[0];

            for (var i = 1; i < v.Count - 1; i++)
            {
                var v1 = v[i] - v0;
                var v2 = v[i + 1] - v0;
                vectorSum += 0.5f * Vector3.Cross(v1, v2);
            }

            return vectorSum.magnitude;
        }

        /// <summary>
        ///     Calculates the polygon normal of p0 planar polygon.
        /// </summary>
        /// <param name="v">
        ///     The v forming the polygon
        /// </param>
        /// <returns>
        ///     The normalized normal
        /// </returns>
        public static Vector3 PlanarPolygonNormal(params Vector3[] v)
        {
            return PlanarPolygonNormal((IList<Vector3>) v);
        }

        /// <summary>
        ///     Calculates the polygon normal of p0 planar polygon.
        /// </summary>
        /// <param name="v">
        ///     The v forming the polygon
        /// </param>
        /// <returns>
        ///     The normalized normal
        /// </returns>
        public static Vector3 PlanarPolygonNormal(IList<Vector3> v, bool isConvex = false)
        {
            if (v.Count < 3)
            {
                return Vector3.zero;
            }

            if (isConvex || v.Count == 3)
            {
                return TriangleNormal(v[0], v[1], v[2]);
            }

            var cross = Vector3.zero;
            var prev = v[v.Count - 1];
            var cur = v[0];
            var next = v[1];

            for (var i = 0; i < v.Count; i++)
            {
                cross += Vector3.Cross(cur - prev, next - cur).normalized;

                prev = cur;
                cur = next;
                next = v[(i + 1) % v.Count];
            }

            return cross.normalized;
        }

        /// <summary>
        ///     Calculates the barycentric coordinates of a given <see cref="point" /> on the triangle defined by <see cref="p0" />
        ///     , <see cref="p1" /> and <see cref="p2" />
        /// </summary>
        /// <param name="p0">
        ///     The first triangle point
        /// </param>
        /// <param name="p1">
        ///     The second triangle point
        /// </param>
        /// <param name="p2">
        ///     The thirth triangle point
        /// </param>
        /// <param name="point">
        ///     The point
        /// </param>
        /// <returns>
        ///     The barycentric coordinates of the point as <see cref="Vector2" /> where <see cref="Vector2.x" /> represents
        ///     <c>u</c> and <see cref="Vector2.y" /> represents <c>v</c>
        /// </returns>
        public static Vector3 PointToBarycentric(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 point)
        {
            var v0 = p1 - p0;
            var v1 = p2 - p0;
            var v2 = point - p0;

            var d00 = Vector3.Dot(v0, v0);
            var d01 = Vector3.Dot(v0, v1);
            var d11 = Vector3.Dot(v1, v1);
            var d20 = Vector3.Dot(v2, v0);
            var d21 = Vector3.Dot(v2, v1);
            var denom = d00 * d11 - d01 * d01;

            var v = (d11 * d20 - d01 * d21) / denom;
            var w = (d00 * d21 - d01 * d20) / denom;
            var u = 1.0f - v - w;

            return new Vector3(u, v, w);
        }

        /// <summary>
        ///     The polyline length.
        /// </summary>
        /// <param name="polyline">
        ///     The polyline.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float PolylineLength(this IList<(Vector3 point, Vector3)> polyline)
        {
            var length = 0f;

            for (var i = 1; i < polyline.Count; i++)
            {
                length += (polyline[i].point - polyline[i - 1].point).magnitude;
            }

            return length;
        }

        /// <summary>
        ///     The polyline segment.
        /// </summary>
        /// <param name="polyline">
        ///     The polyline.
        /// </param>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <param name="size">
        ///     The size.
        /// </param>
        /// <returns>
        ///     The <see cref="(Vector3 point, Vector3 normal)[]" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public static (Vector3 point, Vector3 normal)[] PolylineSegment(
            this IList<(Vector3 point, Vector3 normal)> polyline,
            float start,
            float size)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), "Start cannot be negative.");
            }

            if (size < 0)
            {
                return Array.Empty<(Vector3, Vector3)>();
            }

            var result = new List<(Vector3 point, Vector3 normal)>();
            var foundStart = false;

            for (var i = 1; i < polyline.Count; i++)
            {
                var lineSize = (polyline[i].point - polyline[i - 1].point).magnitude;

                if (!foundStart)
                {
                    if (lineSize < start)
                    {
                        start -= lineSize;
                    }
                    else
                    {
                        var relativePosition = start / (polyline[i].point - polyline[i - 1].point).magnitude;
                        var position = polyline[i - 1].point
                                       + (polyline[i].point - polyline[i - 1].point) * relativePosition;
                        var normal = Vector3.Lerp(polyline[i - 1].normal, polyline[i].normal, relativePosition);
                        result.Add((position, normal));
                        size += start;
                        foundStart = true;
                    }
                }

                if (foundStart)
                {
                    if (lineSize < size)
                    {
                        result.Add((polyline[i].point, polyline[i].normal));
                        size -= lineSize;
                    }
                    else
                    {
                        var relativePosition = size / (polyline[i].point - polyline[i - 1].point).magnitude;
                        var position = polyline[i - 1].point
                                       + (polyline[i].point - polyline[i - 1].point) * relativePosition;
                        var normal = Vector3.Lerp(polyline[i - 1].normal, polyline[i].normal, relativePosition);
                        result.Add((position, normal));
                        break;
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        ///     Calculates the area size of p0 quadrilateral (polygon with four edges)
        /// </summary>
        /// <param name="p0">
        ///     The first position
        /// </param>
        /// <param name="p1">
        ///     The second position
        /// </param>
        /// <param name="p2">
        ///     The third position
        /// </param>
        /// <param name="p3">
        ///     The fourth
        /// </param>
        /// <returns>
        ///     The area size
        /// </returns>
        public static float QuadrilateralArea(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return 0.5f * Vector3.Cross(p2 - p0, p3 - p1).magnitude;
        }

        public static bool RayTriangleIntersect(
            Vector3 orig, Vector3 dir,
            Vector3 v0, Vector3 v1, Vector3 v2, 
            out Vector3 hit)
        {
            var normal = TriangleNormal(v0, v1, v2);

            if (!RayPlaneIntersection(orig, dir, normal, v0, out hit))
            {
                return false;
            }

            if (!IsPointInTriangle(v0, v1, v2, hit))
            {
                return false;
            }

            return true;
        }

        public static bool RayPlaneIntersection(Vector3 orig, Vector3 dir, Vector3 normal, Vector3 planeOrig, out Vector3 hit)
        {
            // Code adapted from https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-plane-and-ray-disk-intersection
            // Not verified

            var denom = Vector3.Dot(normal, dir);
            if (!Mathf.Approximately(denom, 0f))
            {
                var p0l0 = planeOrig - orig;
                var t = Vector3.Dot(p0l0, normal) / denom;
                hit = orig + t * dir;
                return (t >= 0);
            }

            hit = Vector3.zero;
            return false;
        }

        /// <summary>
        ///     Calculates the relative position of the closest position of point <see cref="p" /> to the line <see cref="b" />-
        ///     <see cref="a" />
        /// </summary>
        /// <param name="p">
        ///     The point
        /// </param>
        /// <param name="a">
        ///     The first point on the line
        /// </param>
        /// <param name="b">
        ///     The second point on the line
        /// </param>
        /// <returns>
        ///     The relative position of the closest point.<br />
        ///     The return value relates to the following: [Closest Point] = <see cref="a" /> + [Return Value] * (<see cref="b" />-
        ///     <see cref="a" />)
        /// </returns>
        public static float RelativeClosestPositionToLine(Vector3 p, Vector3 a, Vector3 b)
        {
            var abx = b.x - a.x;
            var aby = b.y - a.y;
            var abz = b.z - a.z;

            return -(abx * (a.x - p.x) + aby * (a.y - p.y) + abz * (a.z - p.z)) / (abx * abx + aby * aby + abz * abz);
        }

        /// <summary>
        ///     Calculates the signed volume of the tetrahedron defined by a triangle and topped of by the origin.
        /// </summary>
        /// <param name="p1">
        ///     The first point of the triangle.
        /// </param>
        /// <param name="p2">
        ///     The second point of the triangle.
        /// </param>
        /// <param name="p3">
        ///     The thirth point of the triangle.
        /// </param>
        /// <returns>
        ///     Returns the calculated volume.
        /// </returns>
        public static float SignedVolumeOfTetrahedron(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var v321 = p3.x * p2.y * p1.z;
            var v231 = p2.x * p3.y * p1.z;
            var v312 = p3.x * p1.y * p2.z;
            var v132 = p1.x * p3.y * p2.z;
            var v213 = p2.x * p1.y * p3.z;
            var v123 = p1.x * p2.y * p3.z;
            return 1.0f / 6.0f * (-v321 + v231 + v312 - v132 - v213 + v123);
        }

        /// <summary>
        ///     Calculates the volume of a sphere segment<br />
        ///     Eg. how much volume of a sphere is bellow the given height
        /// </summary>
        /// <param name="radius">
        ///     The radius
        /// </param>
        /// <param name="height">
        ///     The height from the lowest point of the sphere
        /// </param>
        /// <returns>
        ///     The segment volume.
        /// </returns>
        public static float SphereSegmentVolume(float radius, float height)
        {
            return height * height * height * Mathf.PI * (3 * radius - height) / 3f;
        }

        /// <summary>
        ///     The sphere segment volume clamped.
        /// </summary>
        /// <param name="radius">
        ///     The radius.
        /// </param>
        /// <param name="height">
        ///     The height.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float SphereSegmentVolumeClamped(float radius, float height)
        {
            if (radius < 0)
            {
                radius = -radius;
            }

            return SphereSegmentVolume(radius, Mathf.Clamp(height, 0, 2 * radius));
        }

        /// <summary>
        ///     Converts spherical coordinates to cartesians
        /// </summary>
        /// <param name="azimuth">
        ///     The Y-axis rotation in degrees
        /// </param>
        /// <param name="elevation">
        ///     The upwards angle in degrees, clamped to [-PI/2, PI/2]
        /// </param>
        /// <param name="radius">
        ///     The radius or length or the vector
        /// </param>
        /// <returns>
        ///     The cartesian vector
        /// </returns>
        public static Vector3 SphericalToCartesian(float azimuth, float elevation)
        {
            azimuth *= Mathf.Deg2Rad;
            elevation *= Mathf.Deg2Rad;

            var a = Mathf.Cos(elevation);

            var x = a * Mathf.Cos(azimuth);
            var y = Mathf.Sin(elevation);
            var z = a * Mathf.Sin(azimuth);

            return new Vector3(x, y, z);
        }

        /// <summary>
        ///     Translates a set of points by a given offset.
        /// </summary>
        /// <param name="points">
        ///     The points
        /// </param>
        /// <param name="offset">
        ///     The offset
        /// </param>
        public static void TranslatePoints(this IList<Vector3> points, Vector3 offset)
        {
            for (var i = 0; i < points.Count; i++)
            {
                points[i] = points[i] + offset;
            }
        }

        /// <summary>
        ///     Calculates the area size of an triangle
        /// </summary>
        /// <param name="p0">
        ///     The first position
        /// </param>
        /// <param name="p1">
        ///     The second position
        /// </param>
        /// <param name="p2">
        ///     The third position
        /// </param>
        /// <returns>
        ///     The area size
        /// </returns>
        public static float TriangleArea(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return 0.5f * Vector3.Cross(p0 - p1, p2 - p1).magnitude;
        }

        public static Vector3 TriangleCenter(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return 1f / 3f * (p0 + p1 + p2);
        }

        /// <summary>
        ///     Calculates the triangle normal
        /// </summary>
        /// <param name="p0">
        ///     The first position
        /// </param>
        /// <param name="p1">
        ///     The second position
        /// </param>
        /// <param name="p2">
        ///     The third position
        /// </param>
        /// <returns>
        ///     Returns the triangle normal
        /// </returns>
        public static Vector3 TriangleNormal(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            return Vector3.Cross(p1 - p0, p2 - p1).normalized;
        }

        /// <summary>
        ///     Calculates the volume of the given mesh.
        /// </summary>
        /// <param name="mesh">
        ///     The mesh, assumed to be watertight.
        /// </param>
        /// <returns>
        ///     Returns the volume of the mesh.
        /// </returns>
        public static float VolumeOfMesh([NotNull] Mesh mesh)
        {
            return VolumeOfMesh(mesh.vertices, mesh.triangles);
        }

        /// <summary>
        ///     The volume of mesh.
        /// </summary>
        /// <param name="vertices">
        ///     The vertices.
        /// </param>
        /// <param name="triangles">
        ///     The triangles.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        public static float VolumeOfMesh([NotNull] IList<Vector3> vertices, [NotNull] IList<int> triangles)
        {
            float volume = 0;
            for (var i = 0; i < triangles.Count; i += 3)
            {
                var p1 = vertices[triangles[i + 0]];
                var p2 = vertices[triangles[i + 1]];
                var p3 = vertices[triangles[i + 2]];

                volume += SignedVolumeOfTetrahedron(p1, p2, p3);
            }

            return Mathf.Abs(volume);
        }

        public static bool IsPointInSphere(Vector3 itemPosition, Vector3 center, float radius)
        {
            return (itemPosition - center).sqrMagnitude <= radius * radius;
        }
    }
}