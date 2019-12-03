// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         SphereCastVisualization.cs
// 
// Created:          24.08.2019  14:36
// Last modified:    03.12.2019  08:37
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
using Unity_Tools.Collections.SpatialTree;

namespace Unity_Tools.Examples
{
    public class SphereCastVisualization : MonoBehaviour
    {
        public float Radius = 10;
        private List<Point> currentPoints;

        private HashSet<Point> previousPoints;

        private Stopwatch stopwatch = new Stopwatch();

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, Radius);

            Spatial3DTreeVisualizer.DrawTreeGizmos(Point.AllPoints);
        }

        void Start()
        {
            previousPoints = new HashSet<Point>();
            currentPoints = new List<Point>();
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
            Point.AllPoints.SphereCast(this.transform.position, Radius, currentPoints);
        
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
