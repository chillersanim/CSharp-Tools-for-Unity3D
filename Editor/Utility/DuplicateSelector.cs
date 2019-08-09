// Solution:         Unity Tools
// Project:          Assembly-CSharp-Editor
// Filename:         DuplicateSelector.cs
// 
// Created:          09.08.2019  14:20
// Last modified:    09.08.2019  15:44
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

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity_Tools.Collections;
using Unity_Tools.Collections.SpatialTree;

namespace Unity_Tools.Editor.Utility
{
    public static class DuplicateSelector
    {
        private const float MaxDistForEquality = 0.01f;

        [MenuItem("Tools/Select/Visual duplicates")]
        public static void SelectVisualDuplicates()
        {
            var tree = new Spatial3DTree<GameObject>();

            var gos = Object.FindObjectsOfType<Transform>().Select(t => t.gameObject).ToArray();

            foreach (var go in gos)
            {
                tree.Add(go, go.transform.position);
            }

            var result = new List<Object>();
            var intermediateResult = new List<GameObject>();
            foreach (var go in gos)
            {
                intermediateResult.Clear();
                tree.SphereCast(go.transform.position, MaxDistForEquality, intermediateResult);

                if (intermediateResult.Count > 1)
                {
                    foreach (var other in intermediateResult)
                    {
                        if (other == go)
                        {
                            continue;
                        }

                        if (!go.transform.rotation.Equals(other.transform.rotation))
                        {
                            continue;
                        }

                        if (!go.transform.lossyScale.Equals(other.transform.lossyScale))
                        {
                            continue;
                        }

                        if (CompareMeshes(go, other))
                        {
                            result.Add(go);
                        }
                    }
                }
            }

            Selection.objects = result.ToArray();
        }

        private static bool CompareMeshes(GameObject go1, GameObject go2)
        {
            var mf1 = go1.GetComponent<MeshFilter>();
            var mf2 = go2.GetComponent<MeshFilter>();

            if (mf1 != null && mf2 != null && mf1.sharedMesh == mf2.sharedMesh)
            {
                return true;
            }

            return false;
        }
    }
}
