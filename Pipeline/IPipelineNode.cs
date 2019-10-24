// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IPipelineNode.cs
// 
// Created:          12.08.2019  19:04
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

namespace Unity_Tools.Pipeline
{
    /// <summary>
    ///     The PipelineNode interface.
    /// </summary>
    public interface IPipelineNode
    {
        /// <summary>
        ///     The initialize.
        /// </summary>
        void Initialize();

        /// <summary>
        ///     The process next item.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool ProcessNextItem();
    }
}