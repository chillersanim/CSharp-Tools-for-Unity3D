// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PP_AddComponent.cs
// 
// Created:          09.08.2019  15:48
<<<<<<< HEAD
// Last modified:    16.08.2019  16:31
=======
// Last modified:    15.08.2019  17:57
>>>>>>> refs/remotes/origin/master
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

using UnityEngine;

namespace Unity_Tools.Pipeline.Specialized
{
    #region Usings

    #endregion

    /// <summary>
    ///     The p p_ add component.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public sealed class PP_AddComponent<T> : PipelineItemWorker<GameObject>
        where T : Component
    {
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

            item.AddComponent<T>();
        }
    }
}