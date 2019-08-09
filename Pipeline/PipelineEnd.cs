// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         PipelineEnd.cs
// 
// Created:          09.08.2019  15:28
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

namespace Unity_Tools.Pipeline
{
    /// <summary>
    ///     The pipeline end.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class PipelineEnd<T> : IItemReceiver<T>
    {
        /// <summary>
        ///     The collected.
        /// </summary>
        private readonly List<T> collected;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PipelineEnd{T}" /> class.
        /// </summary>
        public PipelineEnd()
        {
            collected = new List<T>();
            Collected = collected.AsReadOnly();
        }

        /// <summary>
        ///     Gets the collected.
        /// </summary>
        public IReadOnlyList<T> Collected { get; }

        /// <summary>
        ///     The add item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        public void AddItem(T item)
        {
            collected.Add(item);
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        public void Initialize()
        {
            collected.Clear();
        }

        /// <summary>
        ///     The process next item.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool ProcessNextItem()
        {
            return false;
        }
    }
}