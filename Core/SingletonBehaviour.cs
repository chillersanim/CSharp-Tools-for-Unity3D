// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         SingletonBehaviour.cs
// 
// Created:          09.08.2019  15:25
// Last modified:    15.08.2019  17:57
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
// 

using JetBrains.Annotations;
using UnityEngine;

namespace Unity_Tools.Core
{
    /// <summary>
    /// Base class for all MonoBehaviours that need to follow the singleton pattern. 
    /// </summary>
    /// <typeparam name="T">The type of the implementing class. Needed to provide the instance.</typeparam>
    [ExecuteInEditMode]
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// The instance reference.
        /// </summary>
        [CanBeNull] private static T instance;

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
                    instance = SingletonHelper.Container.AddComponent<T>();
                }

                Debug.Assert(instance != null, "instance != null");
                return instance;
            }
        }

        /// <summary>
        /// Called when the instance is being disabled.
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
                    container.hideFlags = HideFlags.NotEditable;
                }

                return container;
            }
        }
    }
}