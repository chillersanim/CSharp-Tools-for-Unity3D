// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PP_AddCollider.cs
// 
// Created:          12.08.2019  19:05
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

using System.Collections.Generic;
using JetBrains.Annotations;
using Unity_Tools.Core;
using UnityEngine;

#endregion

namespace Unity_Tools.Pipeline.Specialized
{
    #region Usings

    #endregion

    /// <summary>
    ///     The p p_ add collider.
    /// </summary>
    public class PP_AddCollider : PipelineItemWorker<GameObject>
    {
        /// <summary>
        ///     The triangles cache.
        /// </summary>
        [NotNull] private readonly List<int> trianglesCache = new List<int>();

        /// <summary>
        ///     The vertice cache.
        /// </summary>
        [NotNull] private readonly List<Vector3> verticeCache = new List<Vector3>();

        /// <summary>
        ///     The work on item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void WorkOnItem(GameObject item)
        {
            if (item == null)
            {
                return;
            }

            // Remove old collider's
            Collider col;
            while ((col = item.GetComponent<Collider>()) != null)
            {
                col.Remove();
            }

            var meshFilter = item.GetComponent<MeshFilter>();

            if (meshFilter == null)
            {
                Debug.LogWarning("Item has no mesh filter, can't generate a collider without mesh.");
                return;
            }

            var mesh = meshFilter.sharedMesh;

            if (mesh == null)
            {
                Debug.LogWarning("There is no mesh defined in the item, can't generate a collider without mesh.");
                return;
            }

            if (mesh.subMeshCount > 1)
            {
                // Don't make it complicated, just assign a mesh collider if the mesh is already partitioned...
                var mc = item.AddComponent<MeshCollider>();
                mc.sharedMesh = mesh;
                return;
            }

            // Extract the vertices and triangle information from the mesh
            mesh.GetVertices(verticeCache);
            mesh.GetTriangles(trianglesCache, 0);

            // Figure out which collider is adequate for the given mesh
            if (MeshUtil.IsCuboid(verticeCache, trianglesCache))
            {
                var cc = item.AddComponent<BoxCollider>();
                cc.center = mesh.bounds.center;
                cc.size = mesh.bounds.size;
            }
            else
            {
                var mc = item.AddComponent<MeshCollider>();
                mc.sharedMesh = mesh;

                if (MeshUtil.IsConvex(verticeCache, trianglesCache))
                {
                    mc.convex = true;
                }
            }
        }
    }
}