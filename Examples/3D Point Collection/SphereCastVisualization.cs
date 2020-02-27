// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         SphereCastVisualization.cs
// 
// Created:          23.08.2019  13:05
// Last modified:    05.02.2020  19:39
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

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityTools.Core;

namespace UnityTools.Examples
{
    public class SphereCastVisualization : MonoBehaviour
    {
        public float Radius = 10f;

        public double CastTime = 0.0;

        public bool DrawTree = true;

        public bool DrawShape = true;

        public Color ShapeColor = Color.red;

        private readonly List<Point> currentPoints = new List<Point>();

        private readonly HashSet<Point> previousPoints = new HashSet<Point>();

        private Stopwatch stopwatch = new Stopwatch();

        void OnDrawGizmos()
        {
            if (DrawTree)
            {
                Spatial3DTreeVisualizer.DrawTreeGizmos(Point.AllPoints);
            }

            if (DrawShape)
            {
                var center0 = this.transform.position - Vector3.right * Radius;
                var center1 = this.transform.position + Vector3.right * Radius;
                 
                Gizmos.color = ShapeColor;
                //Gizmos.DrawWireSphere(center0, Radius);
                //Gizmos.DrawWireCube(center1, Vector3.one * Radius * 2f);

                Gizmos.DrawLine(this.transform.position, this.transform.position + (this.transform.right - this.transform.up*2) * 100);
                Gizmos.DrawLine(this.transform.position, this.transform.position - (this.transform.right - this.transform.up*2) * 100);
                Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.Cross(this.transform.right - this.transform.up * 2, this.transform.forward));
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                Radius = Mathf.Clamp(Radius + 10 * Time.deltaTime, 1, 80);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                Radius = Mathf.Clamp(Radius - 10 * Time.deltaTime, 1, 80);
            }

            currentPoints.Clear();

            var center0 = this.transform.position - Vector3.right * Radius;
            var center1 = this.transform.position + Vector3.right * Radius;
            var shape = new VolumePlane(Vector3.Cross(this.transform.right - this.transform.up * 2, this.transform.forward), this.transform.position);

            stopwatch.Restart();
            currentPoints.AddRange(Point.AllPoints.ShapeCast(shape));
            stopwatch.Stop();

            CastTime = stopwatch.Elapsed.TotalMilliseconds / 1000.0;

            previousPoints.ExceptWith(currentPoints);

            foreach (var pp in previousPoints)
            {
                pp.SetActive(false);
            }

            previousPoints.Clear();

            foreach (var cp in currentPoints)
            {
                cp.SetActive(true);
                previousPoints.Add(cp); 
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                foreach (var point in currentPoints)
                {
                    var rb = point.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddExplosionForce(10f, this.transform.position, Radius, 0, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}
