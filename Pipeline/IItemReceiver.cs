// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IItemReceiver.cs
// 
// Created:          09.08.2019  15:28
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
namespace Unity_Tools.Pipeline
{
    /// <summary>
    ///     The ItemReceiver interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public interface IItemReceiver<in T> : IPipelineNode
    {
        /// <summary>
        ///     The add item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        void AddItem(T item);
    }
}