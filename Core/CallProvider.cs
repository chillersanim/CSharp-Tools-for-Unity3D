// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CallProvider.cs
// 
// Created:          12.08.2019  19:04
// Last modified:    20.08.2019  21:49
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

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unity_Tools.Core
{
    /// <summary>
    ///     The call provider.
    /// </summary>
    [ExecuteInEditMode]
    public sealed class CallProvider : SingletonBehaviour<CallProvider>
    {
        public static int MaxPeriodicCallsPerUpdate = 250;

        /// <summary>
        ///     The editor only update listeners.
        /// </summary>
        private readonly List<Action> EditorOnlyUpdateListeners = new List<Action>();

        /// <summary>
        ///     The eou to add.
        /// </summary>
        private readonly List<Action> eouToAdd = new List<Action>();

        /// <summary>
        ///     The eou to remove.
        /// </summary>
        private readonly List<Action> eouToRemove = new List<Action>();

        /// <summary>
        ///     The fixed update listeners.
        /// </summary>
        private readonly List<Action> FixedUpdateListeners = new List<Action>();

        /// <summary>
        ///     The fu to add.
        /// </summary>
        private readonly List<Action> fuToAdd = new List<Action>();

        /// <summary>
        ///     The fu to remove.
        /// </summary>
        private readonly List<Action> fuToRemove = new List<Action>();

        /// <summary>
        ///     The ogi to add.
        /// </summary>
        private readonly List<Action> ogiToAdd = new List<Action>();

        /// <summary>
        ///     The ogi to remove.
        /// </summary>
        private readonly List<Action> ogiToRemove = new List<Action>();

        /// <summary>
        ///     The ogu to add.
        /// </summary>
        private readonly List<Action> oguToAdd = new List<Action>();

        /// <summary>
        ///     The ogu to remove.
        /// </summary>
        private readonly List<Action> oguToRemove = new List<Action>();

        /// <summary>
        ///     The on gizmos listeners.
        /// </summary>
        private readonly List<Action> OnGizmosListeners = new List<Action>();

        /// <summary>
        ///     The on gui listeners.
        /// </summary>
        private readonly List<Action> OnGuiListeners = new List<Action>();

        /// <summary>
        ///     The update listeners.
        /// </summary>
        private readonly List<Action> UpdateListeners = new List<Action>();

        /// <summary>
        /// The periodic update listener
        /// </summary>
        private readonly List<Action> PeriodicUpdateListener = new List<Action>();

        /// <summary>
        ///     The u to add.
        /// </summary>
        private readonly List<Action> uToAdd = new List<Action>();

        /// <summary>
        ///     The u to add.
        /// </summary>
        private readonly List<Action> puToAdd = new List<Action>();

        /// <summary>
        ///     The u to remove.
        /// </summary>
        private readonly List<Action> uToRemove = new List<Action>();

        /// <summary>
        ///     The u to remove.
        /// </summary>
        private readonly List<Action> puToRemove = new List<Action>();

        private int currentPeriodicOffset = 0;

        /// <summary>
        ///     Adds a listener to the on editor only update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddEditorOnlyUpdateListener(Action listener)
        {
            if (!Instance.eouToAdd.Contains(listener))
            {
                Instance.eouToAdd.Add(listener);
            }
        }

        /// <summary>
        ///     Adds a listener to the on fixed update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddFixedUpdateListener(Action listener)
        {
            if (!Instance.fuToAdd.Contains(listener))
            {
                Instance.fuToAdd.Add(listener);
            }
        }

        /// <summary>
        ///     Adds a listener to the on draw gizmos callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddOnGizmosListener(Action listener)
        {
#if UNITY_EDITOR
            if (!Instance.ogiToAdd.Contains(listener))
            {
                Instance.ogiToAdd.Add(listener);
            }

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
            if (!Instance.oguToAdd.Contains(listener))
            {
                Instance.oguToAdd.Add(listener);
            }
        }

        /// <summary>
        ///     Adds a listener to the on update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddUpdateListener(Action listener)
        {
            if (!Instance.uToAdd.Contains(listener))
            {
                Instance.uToAdd.Add(listener);
            }
        }

        /// <summary>
        ///     Adds a listener that gets periodically called. The period depends on the amount of objects subscribed.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void AddPeriodicUpdateListener(Action listener)
        {
            if (!Instance.puToAdd.Contains(listener))
            {
                Instance.puToAdd.Add(listener);
            }
        }

        /// <summary>
        ///     Removes a listener from the on editor only update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void RemoveEditorOnlyUpdateListener(Action listener)
        {
            if (!Instance.eouToRemove.Contains(listener))
            {
                Instance.eouToRemove.Add(listener);
            }
        }

        /// <summary>
        ///     Removes a listener from the on fixed update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void RemoveFixedUpdateListener(Action listener)
        {
            if (!Instance.fuToRemove.Contains(listener))
            {
                Instance.fuToRemove.Add(listener);
            }
        }

        /// <summary>
        ///     Removes a listener from the on draw gizmos callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void RemoveOnGizmosListener(Action listener)
        {
#if UNITY_EDITOR
            if (!Instance.ogiToRemove.Contains(listener))
            {
                Instance.ogiToRemove.Add(listener);
            }

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
            if (!Instance.oguToRemove.Contains(listener))
            {
                Instance.oguToRemove.Add(listener);
            }
        }

        /// <summary>
        ///     Removes a listener from the on update callback list.
        /// </summary>
        /// <param name="listener">
        ///     The listener.
        /// </param>
        public static void RemoveUpdateListener(Action listener)
        {
            if (!Instance.uToRemove.Contains(listener))
            {
                Instance.uToRemove.Add(listener);
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

            EditorOnlyUpdateListeners.Clear();
            FixedUpdateListeners.Clear();
            UpdateListeners.Clear();

            if (!Application.isPlaying)
            {
                EditorApplication.update += UpdateTick;
                EditorApplication.update += FixedUpdateTick;
            }
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
            foreach (var action in fuToRemove)
            {
                FixedUpdateListeners.Remove(action);
            }

            foreach (var action in fuToAdd)
            {
                if (!FixedUpdateListeners.Contains(action))
                {
                    FixedUpdateListeners.Add(action);
                }
            }

            fuToRemove.Clear();
            fuToAdd.Clear();

            foreach (var action in FixedUpdateListeners)
            {
                action();
            }
        }

#if UNITY_EDITOR

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

#endif

        /// <summary>
        ///     The on draw gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            foreach (var og in ogiToRemove)
            {
                OnGizmosListeners.Remove(og);
            }

            foreach (var og in ogiToAdd)
            {
                if (!OnGizmosListeners.Contains(og))
                {
                    OnGizmosListeners.Add(og);
                }
            }

            ogiToRemove.Clear();
            ogiToAdd.Clear();

            foreach (var og in OnGizmosListeners)
            {
                og();
            }

#endif
        }

        /// <summary>
        ///     The on gui.
        /// </summary>
        private void OnGui()
        {
            foreach (var og in oguToRemove)
            {
                OnGuiListeners.Remove(og);
            }

            foreach (var og in oguToAdd)
            {
                if (!OnGuiListeners.Contains(og))
                {
                    OnGuiListeners.Add(og);
                }
            }

            oguToRemove.Clear();
            oguToAdd.Clear();

            foreach (var og in OnGuiListeners)
            {
                og();
            }
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
            foreach (var action in uToRemove)
            {
                UpdateListeners.Remove(action);
            }

            foreach (var action in puToRemove)
            {
                PeriodicUpdateListener.Remove(action);
            }

            foreach (var action in uToAdd)
            {
                if (!UpdateListeners.Contains(action))
                {
                    UpdateListeners.Add(action);
                }
            }

            foreach (var action in puToAdd)
            {
                if (!PeriodicUpdateListener.Contains(action))
                {
                    PeriodicUpdateListener.Add(action);
                }
            }

            uToRemove.Clear();
            puToRemove.Clear();
            uToAdd.Clear();
            puToAdd.Clear();

            foreach (var action in UpdateListeners)
            {
                action();
            }

            var itemCount = PeriodicUpdateListener.Count;
            var periodicUpdateCalls = Mathf.Min(MaxPeriodicCallsPerUpdate, itemCount);

            for (var i = 0; i < periodicUpdateCalls; i++)
            {
                var index = (i + currentPeriodicOffset) % itemCount;
                PeriodicUpdateListener[index].Invoke();
            }

            currentPeriodicOffset = (currentPeriodicOffset + periodicUpdateCalls) % itemCount;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                foreach (var action in eouToRemove)
                {
                    EditorOnlyUpdateListeners.Remove(action);
                }

                foreach (var action in eouToAdd)
                {
                    if (!EditorOnlyUpdateListeners.Contains(action))
                    {
                        EditorOnlyUpdateListeners.Add(action);
                    }
                }

                eouToRemove.Clear();
                eouToAdd.Clear();

                foreach (var action in EditorOnlyUpdateListeners)
                {
                    action();
                }
            }
#endif
        }
    }
}