// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         UnityObjectUtil.cs
// 
// Created:          29.01.2020  19:31
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
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace UnityTools.Core
{
    /// <summary>
    ///     The unity object util.
    /// </summary>
    public static class UnityObjectUtil
    {
        /// <summary>
        ///     The get or add component.
        /// </summary>
        /// <param name="go">
        ///     The go.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public static T GetOrAddComponent<T>(this GameObject go)
            where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }

        /// <summary>
        ///     The get or create child.
        /// </summary>
        /// <param name="go">
        ///     The go.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="ignoreCase">
        ///     The ignore case.
        /// </param>
        /// <returns>
        ///     The <see cref="GameObject" />.
        /// </returns>
        public static GameObject GetOrCreateChild(this GameObject go, string name, bool ignoreCase = false)
        {
            if (ignoreCase)
            {
                name = name.ToLowerInvariant();
            }

            for (var i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i).gameObject;
                var childName = ignoreCase ? child.name.ToLowerInvariant() : child.name;

                if (childName == name)
                {
                    return child;
                }
            }

            var newChild = new GameObject(name);
            newChild.transform.parent = go.transform;
            newChild.transform.position = Vector3.zero;
            newChild.transform.rotation = Quaternion.identity;
            newChild.transform.localScale = Vector3.one;

            return newChild;
        }

        /// <summary>
        ///     The get or create hierarchy.
        /// </summary>
        /// <param name="hierarchyPath">
        ///     The hierarchy path.
        /// </param>
        /// <returns>
        ///     The <see cref="GameObject" />.
        /// </returns>
        public static GameObject GetOrCreateHierarchy(string hierarchyPath)
        {
            var parts = hierarchyPath.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);

            var go = GameObject.Find('/' + parts[0]);

            if (go == null)
            {
                go = new GameObject(parts[0]);
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
#if UNITY_EDITOR
                go.isStatic = true;
#endif
            }

            for (var i = 1; i < parts.Length; i++)
            {
                var found = false;

                for (var j = 0; j < go.transform.childCount; j++)
                {
                    var child = go.transform.GetChild(j).gameObject;
                    if (child.name == parts[i])
                    {
                        go = child;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    // If it is the last child, create the hierarchy child instead.
                    var newGo = new GameObject(parts[i]);
                    newGo.transform.parent = go.transform;
                    newGo.transform.localPosition = Vector3.zero;
                    newGo.transform.localRotation = Quaternion.identity;
                    newGo.transform.localScale = Vector3.one;
                    go = newGo;

#if UNITY_EDITOR
                    go.isStatic = true;
#endif
                }
            }

            return go;
        }
        
        /// <summary>
        ///     The make object name unique.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string MakeObjectNameUnique(string name)
        {
            if (!GameObject.Find(name))
            {
                return name;
            }

            var index = 1;

            while (GameObject.Find($"{name} ({index})"))
            {
                index++;
            }

            return $"{name} ({index})";
        }

        /// <summary>
        ///     Removes an <see cref="object" /> in the editor or in game
        /// </summary>
        /// <param name="behaviour">
        /// </param>
        public static void Remove(this Object behaviour)
        {
            if (behaviour == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(behaviour);
            }
            else
#endif
            {
                Object.Destroy(behaviour);
            }
        }

        /// <summary>
        ///     The remove component.
        /// </summary>
        /// <param name="go">
        ///     The go.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void RemoveComponent<T>(this GameObject go)
            where T : Component
        {
            if (go == null)
            {
                return;
            }

            var component = go.GetComponent<T>();

            if (component != null)
            {
                component.Remove();
            }
        }

        /// <summary>
        ///     The remove components.
        /// </summary>
        /// <param name="go">
        ///     The go.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        public static void RemoveComponents<T>(this GameObject go)
            where T : Component
        {
            if (go == null)
            {
                return;
            }

            var components = go.GetComponents<T>();

            if (components != null)
            {
                foreach (var component in components)
                {
                    component.Remove();
                }
            }
        }
    }
}