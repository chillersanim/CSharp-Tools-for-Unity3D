// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PP_SetLayer.cs
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

#endregion

namespace Unity_Tools.Pipeline.Specialized
{
    #region Usings

    #endregion

    /// <summary>
    ///     The p p_ set layer.
    /// </summary>
    public sealed class PP_SetLayer : PipelineItemWorker<GameObject>
    {
        /// <summary>
        ///     The layer name.
        /// </summary>
        private readonly string layerName;

        /// <summary>
        ///     The layer.
        /// </summary>
        private int layer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PP_SetLayer" /> class.
        /// </summary>
        /// <param name="layer">
        ///     The layer.
        /// </param>
        public PP_SetLayer(int layer)
        {
            layerName = LayerMask.LayerToName(layer);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PP_SetLayer" /> class.
        /// </summary>
        /// <param name="layerName">
        ///     The layer name.
        /// </param>
        public PP_SetLayer(string layerName)
        {
            this.layerName = layerName;
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// </exception>
        public override void Initialize()
        {
            base.Initialize();

            if (string.IsNullOrEmpty(layerName))
            {
                throw new ArgumentException("The layer name cannot be null or empty.");
            }

            var foundLayer = LayerMask.NameToLayer(layerName);

            if (foundLayer == -1)
            {
                throw new ArgumentException("There is no layer with the given layer name.");
            }

            layer = foundLayer;
        }

        /// <summary>
        ///     The work on item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        protected override void WorkOnItem(GameObject item)
        {
            item.layer = layer;
        }
    }
}