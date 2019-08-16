// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PP_SetMaterial.cs
// 
// Created:          09.08.2019  15:48
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
#region usings

using System;
using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Unity_Tools.Pipeline.Specialized
{
    #region Usings

    #endregion

    /// <summary>
    ///     The p p_ set material.
    /// </summary>
    public sealed class PP_SetMaterial : PipelineItemWorker<GameObject>
    {
        /// <summary>
        ///     The material paths.
        /// </summary>
        private readonly string[] materialPaths;

        /// <summary>
        ///     The materials.
        /// </summary>
        [NotNull] private Material[] materials;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PP_SetMaterial" /> class.
        /// </summary>
        /// <param name="materials">
        ///     The materials.
        /// </param>
        public PP_SetMaterial(params Material[] materials)
        {
            this.materials = materials ?? Array.Empty<Material>();
            materialPaths = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PP_SetMaterial" /> class.
        /// </summary>
        /// <param name="materialPaths">
        ///     The material paths.
        /// </param>
        public PP_SetMaterial(params string[] materialPaths)
        {
            materials = Array.Empty<Material>();
            this.materialPaths = materialPaths ?? Array.Empty<string>();
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            if (materialPaths != null)
            {
                materials = new Material[materialPaths.Length];

                for (var i = 0; i < materialPaths.Length; i++)
                {
                    var path = materialPaths[i];
                    materials[i] = Resources.Load<Material>(path);
                }
            }
        }

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

            var renderer = item.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.sharedMaterials = materials;
            }
        }
    }
}