// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PP_GameObjectCollector.cs
// 
// Created:          12.08.2019  19:06
// Last modified:    20.08.2019  21:50
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

#endregion

using UnityEngine;

namespace Unity_Tools.Pipeline.Specialized
{
    #region Usings

    #endregion

    /// <summary>
    ///     The p p_ game object collector.
    /// </summary>
    public class PP_GameObjectCollector : PipelineStart<GameObject>
    {
        /// <summary>
        ///     The all game objects.
        /// </summary>
        private GameObject[] allGameObjects;

        /// <summary>
        ///     The index.
        /// </summary>
        private int index;

        /// <summary>
        ///     The amount left.
        /// </summary>
        public int AmountLeft => allGameObjects.Length - index;

        /// <summary>
        ///     The initialize.
        /// </summary>
        public override void Initialize()
        {
            allGameObjects = Object.FindObjectsOfType<GameObject>();
            index = 0;
        }

        /// <summary>
        ///     The get next item.
        /// </summary>
        /// <returns>
        ///     The <see cref="GameObject" />.
        /// </returns>
        protected override GameObject GetNextItem()
        {
            var item = allGameObjects[index];
            index++;
            return item;
        }

        /// <summary>
        ///     The has items.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected override bool HasItems()
        {
            if (allGameObjects == null)
            {
                return false;
            }

            return index < allGameObjects.Length - 1;
        }
    }
}