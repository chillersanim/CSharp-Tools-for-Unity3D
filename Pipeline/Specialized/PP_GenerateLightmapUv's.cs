// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PP_GenerateLightmapUv's.cs
// 
// Created:          16.08.2019  16:33
// Last modified:    16.08.2019  16:56
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
#region usings

#endregion

using UnityEditor;
using UnityEngine;

namespace Unity_Tools.Pipeline.Specialized
{
    #region Usings

    #endregion

    /// <summary>
    ///     The p p_ generate lightmap uvs.
    /// </summary>
    public class PP_GenerateLightmapUvs : PipelineItemWorker<GameObject>
    {
        /// <summary>
        ///     The work on item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void WorkOnItem(GameObject item)
        {
#if UNITY_EDITOR
            if (item == null)
            {
                return;
            }

            var mf = item.GetComponent<MeshFilter>();

            if (mf == null)
            {
                return;
            }

            var sm = mf.sharedMesh;

            if (sm == null)
            {
                return;
            }

            Unwrapping.GenerateSecondaryUVSet(sm);
#endif
        }
    }
}