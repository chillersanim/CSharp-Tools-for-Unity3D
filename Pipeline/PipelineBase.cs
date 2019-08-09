// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         PipelineBase.cs
// 
// Created:          09.08.2019  15:28
// Last modified:    09.08.2019  15:54
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

using System.Collections.Generic;

namespace Unity_Tools.Pipeline
{
    /// <summary>
    ///     The pipeline base.
    /// </summary>
    /// <typeparam name="TOut">
    /// </typeparam>
    public abstract class PipelineBase<TOut> : IPipelineNode
    {
        /// <summary>
        ///     The outputs.
        /// </summary>
        private readonly List<IItemReceiver<TOut>> outputs;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PipelineBase{TOut}" /> class.
        /// </summary>
        public PipelineBase()
        {
            outputs = new List<IItemReceiver<TOut>>();
            FollowupSteps = outputs.AsReadOnly();
        }

        /// <summary>
        ///     Gets the followup steps.
        /// </summary>
        public IReadOnlyList<IItemReceiver<TOut>> FollowupSteps { get; }

        /// <summary>
        ///     The initialize.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        ///     The process next item.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public abstract bool ProcessNextItem();

        /// <summary>
        ///     The add followup step.
        /// </summary>
        /// <param name="next">
        ///     The next.
        /// </param>
        public void AddFollowupStep(IItemReceiver<TOut> next)
        {
            if (next == null)
            {
                return;
            }

            if (outputs.Contains(next))
            {
                return;
            }

            outputs.Add(next);
        }

        /// <summary>
        ///     The remove followup step.
        /// </summary>
        /// <param name="next">
        ///     The next.
        /// </param>
        public void RemoveFollowupStep(IItemReceiver<TOut> next)
        {
            outputs.Remove(next);
        }
    }
}