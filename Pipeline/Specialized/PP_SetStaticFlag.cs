// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PP_SetStaticFlag.cs
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

#endregion

namespace Unity_Tools.Pipeline.Specialized
{
    #region Usings

    #endregion

    /// <summary>
    ///     The p p_ set static flag.
    /// </summary>
    public class PP_SetStaticFlag : PipelineItemWorker<GameObject>
    {
        /// <summary>
        ///     The flag.
        /// </summary>
        private readonly bool flag;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PP_SetStaticFlag" /> class.
        /// </summary>
        /// <param name="flag">
        ///     The flag.
        /// </param>
        public PP_SetStaticFlag(bool flag)
        {
            this.flag = flag;
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

            // The isStatic property is only available in the editor
#if UNITY_EDITOR
            item.isStatic = flag;
#endif
        }
    }
}