// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CameraBehaviour.cs
// 
// Created:          23.08.2019  12:18
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

#region usings

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Unity_Tools.Examples
{
    /// <summary>
    ///     The camera behaviour.
    /// </summary>
    [ExecuteInEditMode]
    public class CameraBehaviour : MonoBehaviour
    {
        /// <summary>
        ///     The absolute horizontal distance.
        /// </summary>
        public float AbsoluteHorizontalDistance;

        /// <summary>
        ///     The absolute vertical distance.
        /// </summary>
        public float AbsoluteVerticalDistance;

        /// <summary>
        ///     The allow camera collision.
        /// </summary>
        public bool AllowCameraCollision;

        /// <summary>
        ///     The allow X rotation.
        /// </summary>
        public bool AllowXRotation;

        /// <summary>
        ///     The allow Y rotation.
        /// </summary>
        public bool AllowYRotation;

        /// <summary>
        ///     The allow zoom.
        /// </summary>
        public bool AllowZoom;

        /// <summary>
        ///     The camera damping.
        /// </summary>
        public float CameraDamping;

        /// <summary>
        ///     The collision precission.
        /// </summary>
        public int CollisionPrecission;

        /// <summary>
        ///     The collision radius.
        /// </summary>
        public float CollisionRadius;

        /// <summary>
        ///     The current X rotation.
        /// </summary>
        public float CurrentXRotation;

        /// <summary>
        ///     The current Y rotation.
        /// </summary>
        public float CurrentYRotation;

        /// <summary>
        ///     The current zoom.
        /// </summary>
        public float CurrentZoom;

        /// <summary>
        ///     The ignore layer.
        /// </summary>
        public LayerMask LayerMask;

        /// <summary>
        ///     The lock cursor to center screen.
        /// </summary>
        public bool LockCursorToCenterScreen;

        /// <summary>
        ///     The main camera.
        /// </summary>
        public Camera MainCamera;

        /// <summary>
        ///     The max horizontal distance.
        /// </summary>
        public float MaxHorizontalDistance;

        /// <summary>
        ///     The max vertical distance.
        /// </summary>
        public float MaxVerticalDistance;

        /// <summary>
        ///     The max X rotation.
        /// </summary>
        public float MaxXRotation;

        /// <summary>
        ///     The max Y rotation.
        /// </summary>
        public float MaxYRotation;

        /// <summary>
        ///     The min horizontal distance.
        /// </summary>
        public float MinHorizontalDistance;

        /// <summary>
        ///     The min vertical distance.
        /// </summary>
        public float MinVerticalDistance;

        /// <summary>
        ///     The min X rotation.
        /// </summary>
        public float MinXRotation;

        /// <summary>
        ///     The min Y rotation.
        /// </summary>
        public float MinYRotation;

        /// <summary>
        ///     The previous avatar position.
        /// </summary>
        private Vector3 previousAvatarPosition;

        /// <summary>
        ///     The previous tab selection.
        /// </summary>
        public int previousTabSelection;

        /// <summary>
        ///     The restrict X rotation.
        /// </summary>
        public bool RestrictXRotation;

        /// <summary>
        ///     The restrict Y rotation.
        /// </summary>
        public bool RestrictYRotation;

        /// <summary>
        ///     The rotation speed.
        /// </summary>
        public float RotationSpeed;

        /// <summary>
        ///     The show collision calculation.
        /// </summary>
        public bool ShowCollisionCalculation;

        /// <summary>
        ///     The show cursor.
        /// </summary>
        public bool ShowCursor;

        // X Rotation

        /// <summary>
        ///     The triggered X rotation input axis names.
        /// </summary>
        public AxisSettings[] TriggeredXRotationInputAxisNames;

        /// <summary>
        ///     The triggered Y rotation input axis names.
        /// </summary>
        public AxisSettings[] TriggeredYRotationInputAxisNames;

        /// <summary>
        ///     The triggered zoom input axis names.
        /// </summary>
        public AxisSettings[] TriggeredZoomInputAxisNames;

        /// <summary>
        ///     The X rotation input axis names.
        /// </summary>
        public AxisSettings[] XRotationInputAxisNames;

        /// <summary>
        ///     The X trigger key code.
        /// </summary>
        public KeyCode XTriggerKeyCode;

        /// <summary>
        ///     The Y rotation input axis names.
        /// </summary>
        public AxisSettings[] YRotationInputAxisNames;

        /// <summary>
        ///     The Y trigger key code.
        /// </summary>
        public KeyCode YTriggerKeyCode;

        /// <summary>
        ///     The zoom input axis names.
        /// </summary>
        public AxisSettings[] ZoomInputAxisNames;

        /// <summary>
        ///     The zoom speed.
        /// </summary>
        public float ZoomSpeed;

        /// <summary>
        ///     The zoom trigger key code.
        /// </summary>
        public KeyCode ZoomTriggerKeyCode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CameraBehaviour" /> class.
        /// </summary>
        public CameraBehaviour()
        {
            MainCamera = null;

            ZoomSpeed = 1.0f;
            RotationSpeed = 4.0f;
            CameraDamping = 4.0f;

            LockCursorToCenterScreen = false;
            ShowCursor = true;

            AllowZoom = true;
            ZoomInputAxisNames = new AxisSettings[0];
            TriggeredZoomInputAxisNames = new AxisSettings[0];
            ZoomTriggerKeyCode = KeyCode.None;
            AbsoluteVerticalDistance = 1.0f;
            AbsoluteHorizontalDistance = 1.0f;
            CurrentZoom = 1.0f;
            MaxVerticalDistance = 4.0f;
            MinVerticalDistance = 1.0f;
            MaxHorizontalDistance = 2.0f;
            MinHorizontalDistance = 1.0f;

            AllowXRotation = true;

            XRotationInputAxisNames = new AxisSettings[0];
            TriggeredXRotationInputAxisNames = new AxisSettings[0];
            XTriggerKeyCode = KeyCode.None;
            CurrentXRotation = 0.0f;
            RestrictXRotation = true;
            MaxXRotation = 90.0f;
            MinXRotation = -30.0f;

            AllowYRotation = true;
            YRotationInputAxisNames = new AxisSettings[0];
            TriggeredYRotationInputAxisNames = new AxisSettings[0];
            YTriggerKeyCode = KeyCode.None;
            CurrentYRotation = 0.0f;
            RestrictYRotation = false;
            MaxYRotation = 0.0f;
            MinYRotation = 0.0f;

            AllowCameraCollision = true;
            CollisionRadius = 0.3f;
            CollisionPrecission = 1;
            ShowCollisionCalculation = false;
            LayerMask = new LayerMask();
        }

        /// <summary>
        ///     The get axis input.
        /// </summary>
        /// <param name="inputNames">
        ///     The input names.
        /// </param>
        /// <param name="triggeredInputNames">
        ///     The triggered input names.
        /// </param>
        /// <param name="triggerKey">
        ///     The trigger key.
        /// </param>
        /// <returns>
        ///     The <see cref="float" />.
        /// </returns>
        private static float GetAxisInput(
            IEnumerable<AxisSettings> inputNames,
            IEnumerable<AxisSettings> triggeredInputNames,
            KeyCode triggerKey)
        {
            if (!Application.isPlaying)
            {
                return 0f;
            }

            var result = 0f;

            foreach (var input in inputNames)
            {
                var invert = input.Invert ? -1 : 1;
                result += Input.GetAxis(input.AxisName) * invert;
            }

            if (Input.GetKey(triggerKey))
            {
                foreach (var triggeredInput in triggeredInputNames)
                {
                    var invert = triggeredInput.Invert ? -1 : 1;
                    result += Input.GetAxis(triggeredInput.AxisName) * invert;
                }
            }

            return result;
        }

        /// <summary>
        ///     The rotate Y.
        /// </summary>
        /// <param name="v">
        ///     The v.
        /// </param>
        /// <param name="angle">
        ///     The angle.
        /// </param>
        private static void RotateY(ref Vector3 v, float angle)
        {
            v = Quaternion.Euler(0.0f, angle, 0.0f) * v;
        }

        /// <summary>
        ///     The handle colission.
        /// </summary>
        /// <param name="dist">
        ///     The dist.
        /// </param>
        /// <param name="destination">
        ///     The destination.
        /// </param>
        private void HandleColission(float dist, ref Vector3 destination)
        {
            if (AllowCameraCollision)
            {
                RaycastHit hitInfo;

                if (Physics.SphereCast(
                    transform.position,
                    CollisionRadius,
                    MainCamera.transform.position - transform.position,
                    out hitInfo,
                    dist * 1.1f,
                    LayerMask))
                {
                    var d = hitInfo.distance;

                    destination = Vector3.Lerp(transform.position, destination, d / dist);
                    destination += hitInfo.normal * CollisionRadius;
                }
            }
        }

        /// <summary>
        ///     The handle rotation.
        /// </summary>
        private void HandleRotation()
        {
            if (AllowXRotation || AllowYRotation)
            {
                var addXRotation = GetAxisInput(
                    XRotationInputAxisNames,
                    TriggeredXRotationInputAxisNames,
                    XTriggerKeyCode);
                var addYRotation = GetAxisInput(
                    YRotationInputAxisNames,
                    TriggeredYRotationInputAxisNames,
                    YTriggerKeyCode);

                if (AllowXRotation && Math.Abs(addXRotation) > Mathf.Epsilon)
                {
                    CurrentXRotation += addXRotation * RotationSpeed;

                    if (RestrictXRotation)
                    {
                        if (CurrentXRotation < MinXRotation)
                        {
                            CurrentXRotation = MinXRotation;
                        }
                        else if (CurrentXRotation > MaxXRotation)
                        {
                            CurrentXRotation = MaxXRotation;
                        }
                    }
                }

                if (AllowYRotation && Math.Abs(addYRotation) > Mathf.Epsilon)
                {
                    CurrentYRotation += addYRotation * RotationSpeed;

                    if (RestrictYRotation)
                    {
                        if (CurrentYRotation < MinYRotation)
                        {
                            CurrentYRotation = MinYRotation;
                        }
                        else if (CurrentYRotation > MaxYRotation)
                        {
                            CurrentYRotation = MaxYRotation;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     The handle zoom.
        /// </summary>
        private void HandleZoom()
        {
            if (AllowZoom)
            {
                var addZoom = GetAxisInput(
                    ZoomInputAxisNames,
                    TriggeredZoomInputAxisNames,
                    ZoomTriggerKeyCode);

                if (Math.Abs(addZoom - 0.0f) > Mathf.Epsilon)
                {
                    CurrentZoom +=
                        addZoom * ZoomSpeed * (0.1f + CurrentZoom) / 1.1f;

                    if (CurrentZoom < 0.0f)
                    {
                        CurrentZoom = 0.0f;
                    }

                    if (CurrentZoom > 1.0f)
                    {
                        CurrentZoom = 1.0f;
                    }
                }
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

            if (Application.isPlaying)
            {
                Cursor.lockState = LockCursorToCenterScreen ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = ShowCursor;
            }

            MainCamera.transform.position += transform.position - previousAvatarPosition;
            previousAvatarPosition = transform.position;

            // Zoom
            HandleZoom();

            // Rotation
            HandleRotation();

            // Calculates the position of the camera
            var verticalZoomDistance = (MaxVerticalDistance - MinVerticalDistance) * CurrentZoom
                                       + MinVerticalDistance;
            var horizontalZoomDistance = (MaxHorizontalDistance - MinHorizontalDistance) * CurrentZoom
                                         + MinHorizontalDistance;

            var dist = Vector3.Distance(
                Vector3.zero,
                new Vector3(
                    Mathf.Sin(CurrentXRotation / 180.0f * Mathf.PI)
                    * (AllowZoom ? verticalZoomDistance : AbsoluteVerticalDistance),
                    Mathf.Cos(CurrentXRotation / 180.0f * Mathf.PI)
                    * (AllowZoom ? horizontalZoomDistance : AbsoluteHorizontalDistance),
                    0.0f));

            // Calculates the relative position for the camera without YRotation
            var absoluteVector = new Vector3(
                Mathf.Cos(CurrentXRotation / 180.0f * Mathf.PI) * dist,
                Mathf.Sin(CurrentXRotation / 180.0f * Mathf.PI) * dist,
                0.0f);
            RotateY(ref absoluteVector, CurrentYRotation);

            // Calculates the desired position
            var destination = absoluteVector + transform.position;

            // When camera collision is active, also check for collision
            HandleColission(dist, ref destination);

            MainCamera.transform.position = Vector3.Lerp(
                MainCamera.transform.position,
                destination,
                CameraDamping * Time.deltaTime * 5.0f);

            MainCamera.transform.LookAt(transform.position);

            if (Mathf.Abs(MainCamera.transform.position.x - transform.position.x) < 0.0001
                || Mathf.Abs(MainCamera.transform.position.z - transform.position.z) < 0.0001)
            {
                MainCamera.transform.rotation = Quaternion.Euler(
                    MainCamera.transform.rotation.eulerAngles.x,
                    CurrentYRotation - 90.0f,
                    MainCamera.transform.rotation.eulerAngles.z);
            }
        }
    }
}