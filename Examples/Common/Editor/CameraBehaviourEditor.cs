// Solution:         Unity Tools
// Project:          UnityTools.Editor
// Filename:         CameraBehaviourEditor.cs
// 
// Created:          23.08.2019  12:19
// Last modified:    05.02.2020  19:40
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

using Unity_Tools.Examples;
using UnityEditor;
using UnityEngine;

#endregion

namespace Assets.Unity_Tools.Examples.Common.Editor
{
    /// <summary>
    ///     The camera behaviour editor.
    /// </summary>
    [CustomEditor(typeof(CameraBehaviour))]
    [CanEditMultipleObjects]
    public class CameraBehaviourEditor : UnityEditor.Editor
    {
        /// <summary>
        ///     The absoulte horizontal distance prop.
        /// </summary>
        private SerializedProperty AbsoulteHorizontalDistanceProp;

        /// <summary>
        ///     The absoulte vertical distance prop.
        /// </summary>
        private SerializedProperty AbsoulteVerticalDistanceProp;

        // Camera Collision
        /// <summary>
        ///     The allow camera collision prop.
        /// </summary>
        private SerializedProperty AllowCameraCollisionProp;

        // X Rotation
        /// <summary>
        ///     The allow x rotation prop.
        /// </summary>
        private SerializedProperty AllowXRotationProp;

        // Y Rotation
        /// <summary>
        ///     The allow y rotation prop.
        /// </summary>
        private SerializedProperty AllowYRotationProp;

        // Zoom
        /// <summary>
        ///     The allow zoom prop.
        /// </summary>
        private SerializedProperty AllowZoomProp;

        /// <summary>
        ///     The camera damping prop.
        /// </summary>
        private SerializedProperty CameraDampingProp;

        /// <summary>
        ///     The collision precission prop.
        /// </summary>
        private SerializedProperty CollisionPrecissionProp;

        /// <summary>
        ///     The collision radius prop.
        /// </summary>
        private SerializedProperty CollisionRadiusProp;

        /// <summary>
        ///     The current x rotation prop.
        /// </summary>
        private SerializedProperty CurrentXRotationProp;

        /// <summary>
        ///     The current y rotation prop.
        /// </summary>
        private SerializedProperty CurrentYRotationProp;

        /// <summary>
        ///     The current zoom prop.
        /// </summary>
        private SerializedProperty CurrentZoomProp;

        /// <summary>
        ///     The layer mask prop.
        /// </summary>
        private SerializedProperty LayerMaskProp;

        /// <summary>
        ///     The lock cursor to center screen prop.
        /// </summary>
        private SerializedProperty LockCursorToCenterScreenProp;

        // Main Camera
        /// <summary>
        ///     The main camera prop.
        /// </summary>
        private SerializedProperty MainCameraProp;

        /// <summary>
        ///     The max horizontal distance prop.
        /// </summary>
        private SerializedProperty MaxHorizontalDistanceProp;

        /// <summary>
        ///     The max vertical distance prop.
        /// </summary>
        private SerializedProperty MaxVerticalDistanceProp;

        /// <summary>
        ///     The max x rotation prop.
        /// </summary>
        private SerializedProperty MaxXRotationProp;

        /// <summary>
        ///     The max y rotation prop.
        /// </summary>
        private SerializedProperty MaxYRotationProp;

        /// <summary>
        ///     The min horizontal distance prop.
        /// </summary>
        private SerializedProperty MinHorizontalDistanceProp;

        /// <summary>
        ///     The min vertical distance prop.
        /// </summary>
        private SerializedProperty MinVerticalDistanceProp;

        /// <summary>
        ///     The min x rotation prop.
        /// </summary>
        private SerializedProperty MinXRotationProp;

        /// <summary>
        ///     The min y rotation prop.
        /// </summary>
        private SerializedProperty MinYRotationProp;

        /// <summary>
        ///     The previous tab prop.
        /// </summary>
        private SerializedProperty previousTabProp;

        /// <summary>
        ///     The restrict x rotation prop.
        /// </summary>
        private SerializedProperty RestrictXRotationProp;

        /// <summary>
        ///     The restrict y rotation prop.
        /// </summary>
        private SerializedProperty RestrictYRotationProp;

        /// <summary>
        ///     The rotation speed prop.
        /// </summary>
        private SerializedProperty RotationSpeedProp;

        /// <summary>
        ///     The show collision calculation prop.
        /// </summary>
        private SerializedProperty ShowCollisionCalculationProp;

        /// <summary>
        ///     The show cursor prop.
        /// </summary>
        private SerializedProperty ShowCursorProp;

        /// <summary>
        ///     The triggered x rotation input axis names prop.
        /// </summary>
        private SerializedProperty TriggeredXRotationInputAxisNamesProp;

        /// <summary>
        ///     The triggered y rotation input axis names prop.
        /// </summary>
        private SerializedProperty TriggeredYRotationInputAxisNamesProp;

        /// <summary>
        ///     The triggered zoom input axis names prop.
        /// </summary>
        private SerializedProperty TriggeredZoomInputAxisNamesProp;

        /// <summary>
        ///     The x rotation input axis names prop.
        /// </summary>
        private SerializedProperty XRotationInputAxisNamesProp;

        /// <summary>
        ///     The x trigger key code prop.
        /// </summary>
        private SerializedProperty XTriggerKeyCodeProp;

        /// <summary>
        ///     The y rotation input axis names prop.
        /// </summary>
        private SerializedProperty YRotationInputAxisNamesProp;

        /// <summary>
        ///     The y trigger key code prop.
        /// </summary>
        private SerializedProperty YTriggerKeyCodeProp;

        /// <summary>
        ///     The zoom input axis names prop.
        /// </summary>
        private SerializedProperty ZoomInputAxisNamesProp;

        /// <summary>
        ///     The zoom speed prop.
        /// </summary>
        private SerializedProperty ZoomSpeedProp;

        /// <summary>
        ///     The zoom trigger key code prop.
        /// </summary>
        private SerializedProperty ZoomTriggerKeyCodeProp;

        /// <summary>
        ///     The on inspector gui.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            // General settings
            EditorGUILayout.PropertyField(
                MainCameraProp,
                new GUIContent("Main Camera", "The camera that should be used."));

            GUILayout.Space(8.0f);

            EditorGUILayout.Slider(
                ZoomSpeedProp,
                0.1f,
                50.0f,
                new GUIContent("Zoom Speed", "Define how fast you can zoom."));
            EditorGUILayout.Slider(
                RotationSpeedProp,
                0.1f,
                50.0f,
                new GUIContent("Rotation Speed", "Define how fast you can rotate."));
            EditorGUILayout.Slider(
                CameraDampingProp,
                0.5f,
                50.0f,
                new GUIContent("Camera Damping", "Define how much damping the camera gets. Large values = less damping."));

            GUILayout.Space(8.0f);

            LockCursorToCenterScreenProp.boolValue = EditorGUILayout.Toggle(
                new GUIContent("Lock cursor", "Lock the cursor to the center of the screen"),
                LockCursorToCenterScreenProp.boolValue);

            if (!LockCursorToCenterScreenProp.boolValue)
            {
                ShowCursorProp.boolValue = EditorGUILayout.Toggle(
                    new GUIContent("Show cursor", "Should the cursor be visible in the game?"),
                    ShowCursorProp.boolValue);
            }

            // Tabs
            GUILayout.Space(24.0f);
            previousTabProp.intValue = GUILayout.Toolbar(
                previousTabProp.intValue,
                new[]
                {
                    new GUIContent("Zoom", "Edit the camera zoom settings"),
                    new GUIContent("X Rotation", "Edit the settings for the X rotation of the camera"),
                    new GUIContent("Y Rotation", "Edit the settings for the Y rotation of the camera"),
                    new GUIContent("Collision", "Edit the camera collision settings")
                });
            GUILayout.Space(8.0f);

            // Zoom
            switch (previousTabProp.intValue)
            {
                // Zoom
                case 0:
                {
                    AllowZoomProp.boolValue = EditorGUILayout.Toggle(
                        new GUIContent("Allow Zoom", "Can the player zoom the camera?"),
                        AllowZoomProp.boolValue);
                    GUILayout.Space(8);

                    if (AllowZoomProp.boolValue)
                    {
                        EditorGUILayout.Slider(
                            CurrentZoomProp,
                            0.0f,
                            1.0f,
                            new GUIContent(
                                "Current Zoom",
                                "The current zoomlevel between max distance and min distance"));

                        GUILayout.Space(8);
                        EditorGUILayout.PropertyField(
                            ZoomInputAxisNamesProp,
                            new GUIContent(
                                "Input Axis",
                                "Insert the name of the input axis that should be used for zoom"),
                            true);
                        EditorGUILayout.PropertyField(
                            TriggeredZoomInputAxisNamesProp,
                            new GUIContent(
                                "Triggered Input Axis",
                                "Insert the name of the input axis that should be used for zoom only when the trigger key is pressed"),
                            true);
                        EditorGUILayout.PropertyField(
                            ZoomTriggerKeyCodeProp,
                            new GUIContent(
                                "Trigger Keycode",
                                "Select which key need to be pressed, so that the \"Triggered Input Axis\" get used. Leave empty if not used."));

                        GUILayout.Space(8);
                        EditorGUILayout.LabelField(
                            new GUIContent("Vertical Distance", "Defines the vertical distances used for zoom."));
                        MinVerticalDistanceProp.floatValue = Mathf.Max(
                            EditorGUILayout.FloatField(
                                new GUIContent("Min", "Defines the minimum vertical distance of the camera"),
                                MinVerticalDistanceProp.floatValue),
                            0.0f);
                        MaxVerticalDistanceProp.floatValue = Mathf.Max(
                            MaxVerticalDistanceProp.floatValue,
                            MinVerticalDistanceProp.floatValue + 0.1f);
                        MaxVerticalDistanceProp.floatValue = Mathf.Max(
                            EditorGUILayout.FloatField(
                                new GUIContent("Max", "Defines the maximum vertical distance of the camera"),
                                MaxVerticalDistanceProp.floatValue),
                            0.0f);
                        MinVerticalDistanceProp.floatValue = Mathf.Min(
                            MaxVerticalDistanceProp.floatValue - 0.1f,
                            MinVerticalDistanceProp.floatValue);

                        GUILayout.Space(8);
                        EditorGUILayout.LabelField(
                            new GUIContent("Horizontal Distance", "Defines the horizontal distances used for zoom."));
                        MinHorizontalDistanceProp.floatValue = Mathf.Max(
                            EditorGUILayout.FloatField(
                                new GUIContent("Min", "Defines the minimum horizontal distance of the camera"),
                                MinHorizontalDistanceProp.floatValue),
                            0.0f);
                        MaxHorizontalDistanceProp.floatValue = Mathf.Max(
                            MaxHorizontalDistanceProp.floatValue,
                            MinHorizontalDistanceProp.floatValue + 0.1f);
                        MaxHorizontalDistanceProp.floatValue = Mathf.Max(
                            EditorGUILayout.FloatField(
                                new GUIContent("Max", "Defines the maximum horizontal distance of the camera"),
                                MaxHorizontalDistanceProp.floatValue),
                            0.0f);
                        MinHorizontalDistanceProp.floatValue = Mathf.Min(
                            MaxHorizontalDistanceProp.floatValue - 0.1f,
                            MinHorizontalDistanceProp.floatValue);
                    }
                    else
                    {
                        AbsoulteVerticalDistanceProp.floatValue = Mathf.Max(
                            EditorGUILayout.FloatField(
                                new GUIContent("Vertical Distance", "Set the absolute vertical distance of the camera"),
                                AbsoulteVerticalDistanceProp.floatValue),
                            0.0f);
                        AbsoulteHorizontalDistanceProp.floatValue = Mathf.Max(
                            EditorGUILayout.FloatField(
                                new GUIContent(
                                    "Horizontal Distance",
                                    "Set the absolute horizontal distance of the camera"),
                                AbsoulteHorizontalDistanceProp.floatValue),
                            0.0f);
                    }

                    break;
                }

                // X Rotation
                case 1:
                {
                    AllowXRotationProp.boolValue = EditorGUILayout.Toggle(
                        new GUIContent("Allow X Rotation", "Can the player rotate the camera around the X axis?"),
                        AllowXRotationProp.boolValue);
                    GUILayout.Space(8);

                    EditorGUILayout.Slider(
                        CurrentXRotationProp,
                        0.0f,
                        360.0f,
                        new GUIContent("Current X Rotation", "Define the actual X rotation of the camera"));
                    GUILayout.Space(8);

                    if (AllowXRotationProp.boolValue)
                    {
                        EditorGUILayout.PropertyField(
                            XRotationInputAxisNamesProp,
                            new GUIContent(
                                "Input Axis",
                                "Insert the name of the input axis that should be used for X rotation"),
                            true);
                        EditorGUILayout.PropertyField(
                            TriggeredXRotationInputAxisNamesProp,
                            new GUIContent(
                                "Triggered Input Axis",
                                "Insert the name of the input axis that should be used for X rotation only when the trigger key is pressed"),
                            true);
                        EditorGUILayout.PropertyField(
                            XTriggerKeyCodeProp,
                            new GUIContent(
                                "Trigger Keycode",
                                "Select which key need to be pressed, so that the \"Triggered Input Axis\" get used. Leave empty if not used."));

                        GUILayout.Space(8);
                        RestrictXRotationProp.boolValue = EditorGUILayout.Toggle(
                            new GUIContent(
                                "Restrict X Rotation",
                                "Restrict the rotation around the X axis to the min and max values"),
                            RestrictXRotationProp.boolValue);

                        GUILayout.Space(8);
                        if (RestrictXRotationProp.boolValue)
                        {
                            MinXRotationProp.floatValue = EditorGUILayout.FloatField(
                                new GUIContent("Min", "Min X rotation"),
                                MinXRotationProp.floatValue);
                            MaxXRotationProp.floatValue = Mathf.Max(
                                MaxXRotationProp.floatValue,
                                MinXRotationProp.floatValue + 0.1f);
                            MaxXRotationProp.floatValue = EditorGUILayout.FloatField(
                                new GUIContent("Max", "Max X rotation"),
                                MaxXRotationProp.floatValue);
                            MinXRotationProp.floatValue = Mathf.Min(
                                MaxXRotationProp.floatValue - 0.1f,
                                MinXRotationProp.floatValue);
                        }
                    }

                    break;
                }

                // Y Rotation
                case 2:
                {
                    AllowYRotationProp.boolValue = EditorGUILayout.Toggle(
                        new GUIContent("Allow Y Rotation", "Can the player rotate the camera around the Y axis?"),
                        AllowYRotationProp.boolValue);
                    GUILayout.Space(8);

                    EditorGUILayout.Slider(
                        CurrentYRotationProp,
                        0.0f,
                        360.0f,
                        new GUIContent("Current Y Rotation", "Define the actual Y rotation of the camera"));
                    GUILayout.Space(8);

                    if (AllowYRotationProp.boolValue)
                    {
                        EditorGUILayout.PropertyField(
                            YRotationInputAxisNamesProp,
                            new GUIContent(
                                "Input Axis",
                                "Insert the name of the input axis that should be used for Y rotation"),
                            true);
                        EditorGUILayout.PropertyField(
                            TriggeredYRotationInputAxisNamesProp,
                            new GUIContent(
                                "Triggered Input Axis",
                                "Insert the name of the input axis that should be used for Y rotation only when the trigger key is pressed"),
                            true);
                        EditorGUILayout.PropertyField(
                            YTriggerKeyCodeProp,
                            new GUIContent(
                                "Trigger Keycode",
                                "Select which key need to be pressed, so that the \"Triggered Input Axis\" get used. Leave empty if not used."));

                        GUILayout.Space(8);
                        RestrictYRotationProp.boolValue = EditorGUILayout.Toggle(
                            new GUIContent(
                                "Restrict Y Rotation",
                                "Restrict the rotation around the Y axis to the min and max values"),
                            RestrictYRotationProp.boolValue);

                        GUILayout.Space(8);
                        if (RestrictYRotationProp.boolValue)
                        {
                            MinYRotationProp.floatValue = EditorGUILayout.FloatField(
                                new GUIContent("Min", "Min Y rotation"),
                                MinYRotationProp.floatValue);
                            MaxYRotationProp.floatValue = Mathf.Max(
                                MaxYRotationProp.floatValue,
                                MinYRotationProp.floatValue + 0.1f);
                            MaxYRotationProp.floatValue = EditorGUILayout.FloatField(
                                new GUIContent("Max", "Max Y rotation"),
                                MaxYRotationProp.floatValue);
                            MinYRotationProp.floatValue = Mathf.Min(
                                MaxYRotationProp.floatValue - 0.1f,
                                MinYRotationProp.floatValue);
                        }
                    }

                    break;
                }

                // Camera Collision
                case 3:
                {
                    AllowCameraCollisionProp.boolValue = EditorGUILayout.Toggle(
                        new GUIContent("Allow Camera Collision", "Should the camera avoid collision with obstacles?"),
                        AllowCameraCollisionProp.boolValue);
                    GUILayout.Space(8);

                    if (AllowCameraCollisionProp.boolValue)
                    {
                        CollisionRadiusProp.floatValue = Mathf.Max(
                            EditorGUILayout.FloatField(
                                new GUIContent("Collision Radius", "The radius for the collision detection"),
                                CollisionRadiusProp.floatValue),
                            0.01f);

                        GUILayout.Space(8);
                        EditorGUILayout.IntSlider(
                            CollisionPrecissionProp,
                            1,
                            10,
                            new GUIContent(
                                "Collision Precission",
                                "Amount of substeps that should be used for the collision detection."));

                        GUILayout.Space(8);
                        EditorGUILayout.PropertyField(
                            LayerMaskProp,
                            new GUIContent(
                                "Layer Mask",
                                "Which layers should be considered for the collision detection"),
                            true);

                        GUILayout.Space(8);
                        ShowCollisionCalculationProp.boolValue = EditorGUILayout.Toggle(
                            new GUIContent("Visualize", "Should the collision calculation be shown in the editor?"),
                            ShowCollisionCalculationProp.boolValue);
                    }

                    break;
                }
            }

            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        ///     The on enable.
        /// </summary>
        private void OnEnable()
        {
            // Initialize overview

            // General settings
            MainCameraProp = serializedObject.FindProperty("MainCamera");

            ZoomSpeedProp = serializedObject.FindProperty("ZoomSpeed");
            RotationSpeedProp = serializedObject.FindProperty("RotationSpeed");
            CameraDampingProp = serializedObject.FindProperty("CameraDamping");

            LockCursorToCenterScreenProp = serializedObject.FindProperty("LockCursorToCenterScreen");
            ShowCursorProp = serializedObject.FindProperty("ShowCursor");

            // Zoom
            AllowZoomProp = serializedObject.FindProperty("AllowZoom");
            ZoomInputAxisNamesProp = serializedObject.FindProperty("ZoomInputAxisNames");
            TriggeredZoomInputAxisNamesProp = serializedObject.FindProperty("TriggeredZoomInputAxisNames");
            ZoomTriggerKeyCodeProp = serializedObject.FindProperty("ZoomTriggerKeyCode");
            AbsoulteVerticalDistanceProp = serializedObject.FindProperty("AbsoluteVerticalDistance");
            AbsoulteHorizontalDistanceProp = serializedObject.FindProperty("AbsoluteHorizontalDistance");
            CurrentZoomProp = serializedObject.FindProperty("CurrentZoom");
            MaxVerticalDistanceProp = serializedObject.FindProperty("MaxVerticalDistance");
            MinVerticalDistanceProp = serializedObject.FindProperty("MinVerticalDistance");
            MaxHorizontalDistanceProp = serializedObject.FindProperty("MaxHorizontalDistance");
            MinHorizontalDistanceProp = serializedObject.FindProperty("MinHorizontalDistance");

            // X Rotation
            AllowXRotationProp = serializedObject.FindProperty("AllowXRotation");
            RestrictXRotationProp = serializedObject.FindProperty("RestrictXRotation");
            XRotationInputAxisNamesProp = serializedObject.FindProperty("XRotationInputAxisNames");
            TriggeredXRotationInputAxisNamesProp =
                serializedObject.FindProperty("TriggeredXRotationInputAxisNames");
            XTriggerKeyCodeProp = serializedObject.FindProperty("XTriggerKeyCode");
            CurrentXRotationProp = serializedObject.FindProperty("CurrentXRotation");
            MaxXRotationProp = serializedObject.FindProperty("MaxXRotation");
            MinXRotationProp = serializedObject.FindProperty("MinXRotation");

            // Y Rotation
            AllowYRotationProp = serializedObject.FindProperty("AllowYRotation");
            RestrictYRotationProp = serializedObject.FindProperty("RestrictYRotation");
            YRotationInputAxisNamesProp = serializedObject.FindProperty("YRotationInputAxisNames");
            TriggeredYRotationInputAxisNamesProp =
                serializedObject.FindProperty("TriggeredYRotationInputAxisNames");
            YTriggerKeyCodeProp = serializedObject.FindProperty("YTriggerKeyCode");
            CurrentYRotationProp = serializedObject.FindProperty("CurrentYRotation");
            MaxYRotationProp = serializedObject.FindProperty("MaxYRotation");
            MinYRotationProp = serializedObject.FindProperty("MinYRotation");

            // Camera Collision
            AllowCameraCollisionProp = serializedObject.FindProperty("AllowCameraCollision");
            CollisionRadiusProp = serializedObject.FindProperty("CollisionRadius");
            CollisionPrecissionProp = serializedObject.FindProperty("CollisionPrecission");
            ShowCollisionCalculationProp = serializedObject.FindProperty("ShowCollisionCalculation");
            LayerMaskProp = serializedObject.FindProperty("LayerMask");

            previousTabProp = serializedObject.FindProperty("previousTabSelection");
        }
    }
}