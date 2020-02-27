// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         SingletonBehaviour.cs
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

using System;
using JetBrains.Annotations;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityTools.Components
{
    /// <summary>
    /// Base class for all MonoBehaviours that need to follow the singleton pattern. 
    /// </summary>
    /// <typeparam name="T">The type of the implementing class. Needed to provide the instance.</typeparam>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [ExecuteInEditMode]
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        /// <summary>
        /// The instance reference.
        /// </summary>
        [CanBeNull] private static T instance;

        /// <summary>
        /// Gets a value indicating whether accessing the singleton instance is allowed or not (disallowed when the instance has been destroyed on application quit)
        /// </summary>
        public static bool CanAccessInstance => instance != null || !SingletonHelper.IsQuitting;

        /// <summary>
        /// Gets the single class instance, if no such instance exists, a new instance will be created.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    if (SingletonHelper.IsQuitting)
                    {
                        throw new InvalidOperationException("Can't access the singleton instance after it has been destroyed and the application is quitting.");
                    }

                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        instance = SingletonHelper.Container.AddComponent<T>();
                    }
                }

                Debug.Assert(instance != null, "instance != null");
                return instance;
            }
        }

        /// <summary>
        /// Called when Unity begins to shut down.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            SingletonHelper.OnApplicationExit();
        }

        /// <summary>
        /// Called when the instance is being destroyed.
        /// </summary>
        [UsedImplicitly]
        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Called when the instance is being enabled.
        /// </summary>
        [UsedImplicitly]
        protected virtual void OnEnable()
        {
            if (instance != null && instance != this)
            {
                Debug.Log("Singleton instance of " + this.GetType().FullName + " already exists.");
                if (Application.isPlaying)
                {
                    Destroy(this);
                }
                else
                {
                    DestroyImmediate(this);
                }

                return;
            }

            instance = this as T;
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    internal static class SingletonHelper
    {
        /// <summary>
        /// The gameobject that stores all automaticaly instantiated singletons.
        /// </summary>
        [CanBeNull] private static GameObject container;

        /// <summary>
        /// Gets the container object for all automaticaly instantiated singletons.
        /// </summary>
        [NotNull]
        public static GameObject Container
        {
            get
            {
                if (container == null)
                {
                    container = new GameObject("Singleton Container");
                    container.hideFlags = HideFlags.NotEditable | HideFlags.DontUnloadUnusedAsset;
                }

                return container;
            }
        }

        public static bool IsQuitting { get; private set; }

        public static void OnApplicationExit()
        {
            IsQuitting = true;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitializeInEditor()
        {
            // Required to make sure old references and values are removed, even if the editor doesn't reload the assembly
            IsQuitting = false;
        }

        [MenuItem("GameObject/Cleanup/Remove empty singleton containers")]
        private static void RemoveEmptyContainers()
        {
            var gos = GameObject.FindObjectsOfType<GameObject>();
            foreach (var go in gos)
            {
                if(go.name == "Singleton Container")
                {
                    if (go.GetComponents(typeof(MonoBehaviour)).Length < 1)
                    {
                        GameObject.DestroyImmediate(go);
                    }
                }
            }
        }
#endif
    }
}