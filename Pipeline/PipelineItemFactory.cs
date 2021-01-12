// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PipelineItemFactory.cs
// 
// Created:          12.08.2019  19:04
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

namespace Unity_Tools.Pipeline
{
    /// <summary>
    ///     Base class for all pipeline nodes that create and forward an item based on an input.
    ///     Implements the <see cref="PipelineWorker{Tin,Tout}" />
    /// </summary>
    /// <typeparam name="Tin">
    ///     The type of the tin.
    /// </typeparam>
    /// <typeparam name="Tout">
    ///     The type of the tout.
    /// </typeparam>
    /// <seealso cref="PipelineWorker{Tin,Tout}" />
    public abstract class PipelineItemFactory<Tin, Tout> : PipelineWorker<Tin, Tout>
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
            var result = GetItem(item);

            foreach (var output in FollowupSteps)
            {
                output.AddItem(result);
            }

            return true;
        }

        /// <summary>
        ///     The get item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="Tout" />.
        /// </returns>
        protected abstract Tout GetItem(Tin item);
    }
}