// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CallProvider.cs
// 
// Created:          29.01.2020  19:28
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity_Tools.Core;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

#endregion

namespace Unity_Tools.Components
{
    /// <summary>
    ///     The call provider.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class CallProvider : SingletonBehaviour<CallProvider>
    {
#if UNITY_EDITOR

        private CallController editorOnlyUpdateController = new CallController();

        private CallController onGizmosController = new CallController();

        private CallController fixedUpdateController = new CallController();

        private CallController onGuiController = new CallController();

        private CallController updateController = new CallController();

#else

        private readonly CallController fixedUpdateController = new CallController();

        private readonly CallController onGuiController = new CallController();

        private readonly CallController updateController = new CallController();

#endif

        private readonly List<Action> periodicUpdateListeners = new List<Action>();

        private readonly List<Action> puToAdd = new List<Action>();

        private readonly List<Action> puToRemove = new List<Action>();

        private readonly Stopwatch periodicStopwatch = new Stopwatch();

        private int periodicUpdateIndex = 0;

        private float maxPeriodicUpdateDuration;

        public static float MaxPeriodicUpdateDuration
        {
            get => Instance.maxPeriodicUpdateDuration;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value))
                {
                    throw new ArgumentException("The maximum periodic update duration cannot be NaN or infinite. For unbounded updates, use the update callback instead of the periodic update callback.");
                }

                Instance.maxPeriodicUpdateDuration = Mathf.Max(value, 0.0001f);
            }
        }

        /// <summary>
        ///     Adds a listener to the on editor only update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        [Conditional("UNITY_EDITOR")]
        public static void AddEditorOnlyUpdateListener(Action listener)
        {
#if UNITY_EDITOR
            Instance.editorOnlyUpdateController.AddListener(listener);
#endif
        }

        /// <summary>
        ///     Adds a listener to the on fixed update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddFixedUpdateListener(Action listener)
        {
            Instance.fixedUpdateController.AddListener(listener);
        }

        /// <summary>
        ///     Adds a listener to the on draw gizmos callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        [Conditional("UNITY_EDITOR")]
        public static void AddOnGizmosListener(Action listener)
        {
#if UNITY_EDITOR
            Instance.onGizmosController.AddListener(listener);
#endif
        }

        /// <summary>
        ///     Adds a listener to the on draw gui callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddOnGuiListener(Action listener)
        {
            Instance.onGuiController.AddListener(listener);
        }

        /// <summary>
        ///     Adds a listener to the on update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddUpdateListener(Action listener)
        {
            Instance.updateController.AddListener(listener);
        }

        /// <summary>
        ///     Removes a listener from the on editor only update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        [Conditional("UNITY_EDITOR")]
        public static void RemoveEditorOnlyUpdateListener(Action listener)
        {
#if UNITY_EDITOR
            Instance.editorOnlyUpdateController.RemoveListener(listener);
#endif
        }

        /// <summary>
        ///     Removes a listener from the on fixed update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void RemoveFixedUpdateListener(Action listener)
        {
            Instance.fixedUpdateController.RemoveListener(listener);
        }

        /// <summary>
        ///     Removes a listener from the on draw gizmos callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        [Conditional("UNITY_EDITOR")]
        public static void RemoveOnGizmosListener(Action listener)
        {
#if UNITY_EDITOR
            Instance.onGizmosController.RemoveListener(listener);
#endif
        }

        /// <summary>
        ///     Removes a listener from the on draw gui callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void RemoveOnGuiListener(Action listener)
        {
            Instance.onGuiController.RemoveListener(listener);
        }

        /// <summary>
        ///     Removes a listener from the on update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void RemoveUpdateListener(Action listener)
        {
            Instance.updateController.RemoveListener(listener);
        }

        /// <summary>
        ///     Adds a listener to the periodic update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddPeriodicUpdateListener(Action listener)
        {
            if (Instance.puToRemove.Contains(listener))
            {
                Instance.puToRemove.Remove(listener);
            }

            if (!Instance.puToAdd.Contains(listener))
            {
                Instance.puToAdd.Add(listener);
            }
        }

        /// <summary>
        ///     Removes a listener from the periodic update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void RemovePeriodicUpdateListener(Action listener)
        {
            if (Instance.puToAdd.Contains(listener))
            {
                Instance.puToAdd.Remove(listener);
            }

            if (!Instance.puToRemove.Contains(listener))
            {
                Instance.puToRemove.Add(listener);
            }
        }

#if UNITY_EDITOR

        /// <summary>
        ///     The on enable.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (editorOnlyUpdateController.Count > 0)
            {
                editorOnlyUpdateController = new CallController();
            }

            if (onGuiController.Count > 0)
            {
                onGuiController = new CallController();
            }

            if (onGizmosController.Count > 0)
            {
                onGizmosController = new CallController();
            }

            if (fixedUpdateController.Count > 0)
            {
                fixedUpdateController = new CallController();
            }

            if (updateController.Count > 0)
            {
                updateController = new CallController();
            }

            if (!Application.isPlaying)
            {
                EditorApplication.update += UpdateTick;
                EditorApplication.update += FixedUpdateTick;
            }
        }

        /// <summary>
        ///     The on disable.
        /// </summary>
        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                EditorApplication.update -= UpdateTick;
                EditorApplication.update -= FixedUpdateTick;
            }
        }

        /// <summary>
        ///     The on draw gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            onGizmosController.Invoke();
        }

#endif

        /// <summary>
        ///     The fixed update.
        /// </summary>
        private void FixedUpdate()
        {
            if (Application.isPlaying)
            {
                FixedUpdateTick();
            }
        }

        /// <summary>
        ///     The fixed update tick.
        /// </summary>
        private void FixedUpdateTick()
        {
            fixedUpdateController.Invoke();
        }

        /// <summary>
        ///     The on gui.
        /// </summary>
        private void OnGUI()
        {
            onGuiController.Invoke();
        }

        /// <summary>
        ///     The update.
        /// </summary>
        private void Update()
        {
            if (Application.isPlaying)
            {
                UpdateTick();
            }
        }

        /// <summary>
        ///     The update tick.
        /// </summary>
        private void UpdateTick()
        {
            updateController.Invoke();
            PeriodicUpdateTick();

#if UNITY_EDITOR

            if (!Application.isPlaying)
            {
                editorOnlyUpdateController.Invoke();
            }

#endif
        }

        private void PeriodicUpdateTick()
        {
            foreach (var itemToRemove in puToRemove)
            {
                var oldIndex = periodicUpdateListeners.IndexOf(itemToRemove);
                if (oldIndex >= 0)
                {
                    periodicUpdateListeners.RemoveAt(oldIndex);
                    if (oldIndex < periodicUpdateIndex)
                    {
                        // To make sure that we don't skip items duo to removal, we need to adapt our current index.
                        periodicUpdateIndex--;
                    }
                    else if (oldIndex == periodicUpdateIndex && periodicUpdateIndex == periodicUpdateListeners.Count)
                    {
                        // We were at the last item, the next one is the first
                        periodicUpdateIndex = 0;
                    }
                }
            }

            foreach (var itemToAdd in puToAdd)
            {
                if (!periodicUpdateListeners.Contains(itemToAdd))
                {
                    periodicUpdateListeners.Add(itemToAdd);
                }
            }

            puToRemove.Clear();
            uToAdd.Clear();
            puToAdd.Clear();

            foreach (var action in UpdateListeners)
            {
                action();
            }
            
            var itemCount = PeriodicUpdateListener.Count;
            if (itemCount > 0)
            {
                var periodicUpdateCalls = Mathf.Min(MaxPeriodicCallsPerUpdate, itemCount);

                for (var i = 0; i < periodicUpdateCalls; i++)
                {
                    var index = (i + currentPeriodicOffset) % itemCount;
                    PeriodicUpdateListener[index].Invoke();
                }

                currentPeriodicOffset = (currentPeriodicOffset + periodicUpdateCalls) % itemCount;
            }

            while (periodicStopwatch.Elapsed.TotalMilliseconds < MaxPeriodicUpdateDuration)
            {
                try
                {
                    // Errors in a periodic update call shouldn't affect behavior of other listeners
                    periodicUpdateListeners[index].Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError("A periodic update listener threw an exception while being executed.\n" + e);
                }

                index = (index + 1) % periodicUpdateListeners.Count;

                if (index == periodicUpdateIndex)
                {
                    // We went through all items
                    break;
                }
            }

            periodicUpdateIndex = index;
            periodicStopwatch.Stop();
        }
    }
}