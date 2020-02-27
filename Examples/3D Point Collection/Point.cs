// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Point.cs
// 
// Created:          23.08.2019  13:10
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

using UnityEngine;
using UnityTools.Collections;
using UnityTools.Components;

namespace UnityTools.Examples
{
    [RequireComponent(typeof(Renderer))]
    public class Point : MonoBehaviour
    {
        private const float MaxSquareOffsetBeforeUpdate = 0.001f * 0.001f;

        public static readonly Spatial3DTree<Point> AllPoints = new Spatial3DTree<Point>(true);

        public bool Active;

        public Material ActiveMaterial, InactiveMaterial;

        private Renderer myRenderer;

        private Vector3 storedPosition;

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

        void OnDestroy()
        {
            AllPoints.Remove(this, storedPosition);

            if (CallProvider.CanAccessInstance)
            {
                CallProvider.RemovePeriodicUpdateListener(this.PeriodicUpdate);
            }
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

        void Start()
        {
            Active = false;
            myRenderer = this.GetComponent<Renderer>();
            storedPosition = transform.position; 
            AllPoints.Add(this, storedPosition);
            CallProvider.AddPeriodicUpdateListener(this.PeriodicUpdate);
        }
    }
}
