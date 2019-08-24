using System.Collections;
using System.Collections.Generic;
using Unity_Tools.Collections;
using Unity_Tools.Core;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Point : MonoBehaviour
{
    private const float MaxSquareOffsetBeforeUpdate = 0.001f * 0.001f;

    public static readonly Spatial3DTree<Point> AllPoints = new Spatial3DTree<Point>();

    public Material ActiveMaterial, InactiveMaterial;

    public bool Active;

    private Vector3 storedPosition;

    private Renderer myRenderer;

    public void SetActive(bool active)
    {
        if (!Active && active)
        {
            Active = true;
            this.myRenderer.sharedMaterial = ActiveMaterial;
        }
        else if (Active && !active)
        {
            Active = false;
            this.myRenderer.sharedMaterial = InactiveMaterial;
        }
    }

    void Start()
    {
        Active = false;
        myRenderer = this.GetComponent<Renderer>();
        storedPosition = transform.position; 
        AllPoints.Add(this, storedPosition);
        CallProvider.AddPeriodicUpdateListener(this.PeriodicUpdate);
    }

    void OnDestroy()
    {
        AllPoints.Remove(this, storedPosition);
    }

    void PeriodicUpdate()
    {
        // Update position in spatial 3d tree
        if ((this.transform.position - storedPosition).sqrMagnitude > MaxSquareOffsetBeforeUpdate)
        {
            AllPoints.MoveItem(this, storedPosition, this.transform.position);
            storedPosition = this.transform.position;
        }
    }
}
