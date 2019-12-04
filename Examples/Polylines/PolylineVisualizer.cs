using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity_Tools.Polyline;

public class PolylineVisualizer : MonoBehaviour
{
    [SerializeField]
    private PolylineType type;

    [Range(1, 100)]
    [SerializeField] private uint samples = 20;

    [Range(0,1)]
    [SerializeField] private float tension = 0.5f;

    [SerializeField]
    private List<Vector3> points;

    private IPolyline polyline;

    [SerializeField]
    private float length;

    public PolylineVisualizer()
    {
        type = PolylineType.Linear;
        polyline = new LinearPolyline();
        points = new List<Vector3>();
    }

    void OnEnable()
    {
        this.polyline = null;
        OnValidate();
    }

    void OnValidate()
    {
        if (type == PolylineType.Linear)
        {
            if (!(polyline is LinearPolyline lp) || lp.Count != points.Count)
            {
                polyline = new LinearPolyline(points);
            }
            else
            {
                for (var i = 0; i < points.Count; i++)
                {
                    lp[i] = points[i];
                }
            }
        }
        else if (type == PolylineType.CatmullRom)
        {
            if (!(polyline is CatmullRomSpline crs) || crs.Count != points.Count)
            {
                polyline = new CatmullRomSpline(points, tension);
            }
            else
            {
                for (var i = 0; i < points.Count; i++)
                {
                    crs[i] = points[i];
                }

                crs.Tension = tension;
            }
        }

        this.length = polyline.Length;
    }

    void OnDrawGizmos()
    {
        if (polyline == null)
        { 
            return;
        }

        var prev = polyline.GetPointAtPosition(0f);
        var smpls = samples * points.Count;

        Gizmos.color = Color.white;
        for (var i = 1; i <= smpls; i++)
        {
            var cur = polyline.GetPointAtPosition(i * length / (float)smpls);
            Gizmos.DrawLine(prev, cur);
            prev = cur;
        }

        Gizmos.color = Color.green;
        for (var i = 0; i < points.Count; i++)
        {
            Gizmos.DrawWireCube(points[i], Vector3.one * 0.02f);
        }
    }
}

public enum PolylineType
{
    Linear,

    CatmullRom
}
