// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PipelineFilter.cs
// 
// Created:          12.08.2019  19:04
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

namespace Unity_Tools.Pipeline
{
    /// <summary>
    ///     The pipeline filter.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public abstract class PipelineFilter<T> : PipelineWorker<T, T>
    {
        /// <summary>
        ///     The process next item.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public override bool ProcessNextItem()
        {
            if (!HasWaitingItems)
            {
                return false;
            }

            var item = GetNextItem();
            var result = Test(item);

            if (result)
            {
                foreach (var output in FollowupSteps)
                {
                    output.AddItem(item);
                }
            }

            return true;
        }

        /// <summary>
        ///     The test.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected abstract bool Test(T item);
    }
}