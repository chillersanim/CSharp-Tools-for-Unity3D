// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IFilter.cs
// 
// Created:          29.01.2020  19:27
// Last modified:    05.02.2020  19:39
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

using JetBrains.Annotations;

namespace Unity_Tools.Core
{
    /// <summary>
    ///     The Filter interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IFilter<in T>
    {
        /// <summary>
        ///     Evaluates whether the item matches the filter criterias.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     Returns <c>true</c> if the item matches the filter criterias, <c>false</c> otherwise.
        /// </returns>
        bool Filter([CanBeNull] T item);
    }
}