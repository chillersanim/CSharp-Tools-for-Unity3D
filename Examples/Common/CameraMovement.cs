// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CameraMovement.cs
// 
// Created:          23.08.2019  12:18
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

#region usings

using UnityTools.Core;
using UnityEngine;

#endregion

namespace UnityTools.Examples
{
    /// <summary>
    ///     The camera movement.
    /// </summary>
    public class CameraMovement : MonoBehaviour
    {
        /// <summary>
        ///     The grab layer mask.
        /// </summary>
        public LayerMask GrabLayerMask;

        /// <summary>
        ///     The grab trigger key.
        /// </summary>
        public KeyCode GrabTriggerKey;

        /// <summary>
        ///     The horizontal forward axis.
        /// </summary>
        public AxisSettings HorizontalForwardAxis;

        /// <summary>
        ///     The horizontal right axis.
        /// </summary>
        public AxisSettings HorizontalRightAxis;

        /// <summary>
        ///     The main camera.
        /// </summary>
        public Camera MainCamera;

        /// <summary>
        ///     The max.
        /// </summary>
        public Vector3 Max;

        /// <summary>
        ///     The min.
        /// </summary>
        public Vector3 Min;

        /// <summary>
        ///     The movement speed.
        /// </summary>
        [Range(0.1f, 100f)] public float MovementSpeed = 5;

        /// <summary>
        ///     The show boundary.
        /// </summary>
        public bool ShowBoundary = true;

        /// <summary>
        ///     The vertical axis.
        /// </summary>
        public AxisSettings VerticalAxis;

        /// <summary>
        ///     The zoom movement impact.
        /// </summary>
        [Range(0f, 1f)] public float ZoomMovementImpact = 0.5f;

        /// <summary>
        ///     The grab distance.
        /// </summary>
        private Plane grabPlane;

        private Vector3 prevMousePos;

        /// <summary>
        ///     The get axis.
        /// </summary>
        /// <param name="axis">
        ///     The axis.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        private float GetAxis(AxisSettings axis)
        {
            if (string.IsNullOrWhiteSpace(axis.AxisName))
            {
                return 0f;
            }

            float value;

            try
            {
                value = Input.GetAxis(axis.AxisName);
            }
            catch
            {
                return 0f;
            }

            if (axis.Invert)
            {
                value = -value;
            }

            return value;
        }

        /// <summary>
        ///     The on draw gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!ShowBoundary)
            {
                return;
            }

            var min = Vector3.Min(Min, Max);
            var max = Vector3.Max(Min, Max);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube((max - min) / 2f + min, max - min);
        }

        /// <summary>
        ///     The on enable.
        /// </summary>
        private void OnEnable()
        {
            if (MainCamera == null)
            {
                MainCamera = Camera.main;
            }
        }

        /// <summary>
        ///     The update.
        /// </summary>
        private void Update()
        {
            if (MainCamera == null)
            {
                return;
            }

            var min = Vector3.Min(Min, Max);
            var max = Vector3.Max(Min, Max);
            var zoom = (MainCamera.transform.position - transform.position).magnitude;

            var movement = Vector3.zero;
            var zoomImpact = zoom * ZoomMovementImpact;

            var hfa = GetAxis(HorizontalForwardAxis);
            var hra = GetAxis(HorizontalRightAxis);
            var va = GetAxis(VerticalAxis);

            var yRotation = Quaternion.Euler(0, MainCamera.transform.rotation.eulerAngles.y, 0);
            var forward = yRotation * Vector3.forward;
            var right = yRotation * Vector3.right;

            movement += forward * hfa * Time.deltaTime * MovementSpeed * zoomImpact;
            movement += right * hra * Time.deltaTime * MovementSpeed * zoomImpact;
            movement += Vector3.up * va * Time.deltaTime * MovementSpeed * zoomImpact;

            if (Input.GetKeyDown(GrabTriggerKey))
            {
                var mousePosition = Input.mousePosition;
                var ray = MainCamera.ScreenPointToRay(mousePosition);
                var cameraForward = MainCamera.transform.forward;
                prevMousePos = mousePosition;

                if (Physics.Raycast(ray, out var hit, GrabLayerMask.value))
                {
                    grabPlane = new Plane(-cameraForward, hit.point);
                }
                else
                {
                    var point = cameraForward * zoom;
                    grabPlane = new Plane(-cameraForward, point);
                }
            }

            if (Input.GetKey(GrabTriggerKey))
            {
                var mousePosition = Input.mousePosition;
                var ray = MainCamera.ScreenPointToRay(mousePosition);
                var prevRay = MainCamera.ScreenPointToRay(prevMousePos);
                grabPlane.Raycast(ray, out var dist);
                grabPlane.Raycast(prevRay, out var prevDist);
                var grabPosition = ray.origin + ray.direction * dist;
                var prevGrabPosition = prevRay.origin + prevRay.direction * prevDist;
                movement += prevGrabPosition - grabPosition;
                prevMousePos = mousePosition;
            }

            var newPos = transform.position + movement;
            newPos = newPos.ClampComponents(min, max);
            transform.position = newPos;
        }
    }
}