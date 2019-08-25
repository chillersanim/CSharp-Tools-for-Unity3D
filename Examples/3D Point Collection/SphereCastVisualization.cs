using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity_Tools;
using Unity_Tools.Collections.SpatialTree;
using Unity_Tools.Collections.SpatialTree.Enumerators;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class SphereCastVisualization : MonoBehaviour
{
    public float Radius = 10;
    
    private HashSet<Point> previousPoints;

    private List<Point> currentPoints;

    private Stopwatch stopwatch = new Stopwatch();

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, Radius);

        Spatial3DTreeVisualizer.DrawTreeGizmos(Point.AllPoints);
    }
}
