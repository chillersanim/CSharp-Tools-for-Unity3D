// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PipelineWorker.cs
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

using System.Collections.Generic;

namespace Unity_Tools.Pipeline
{
    /// <summary>
    ///     The pipeline worker.
    /// </summary>
    /// <typeparam name="Tin">
    /// </typeparam>
    /// <typeparam name="Tout">
    /// </typeparam>
    public abstract class PipelineWorker<Tin, Tout> : PipelineBase<Tout>, IItemReceiver<Tin>
    {
        /// <summary>
        ///     The queue.
        /// </summary>
        private readonly Queue<Tin> queue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PipelineWorker{Tin,Tout}" /> class.
        /// </summary>
        public PipelineWorker()
        {
            queue = new Queue<Tin>();
        }

        /// <summary>
        ///     The has waiting items.
        /// </summary>
        public bool HasWaitingItems => queue.Count > 0;

        /// <summary>
        ///     The waiting item count.
        /// </summary>
        public int WaitingItemCount => queue.Count;

        /// <summary>
        ///     The add item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        public void AddItem(Tin item)
        {
            queue.Enqueue(item);
        }

        /// <summary>
        ///     The get next item.
        /// </summary>
        /// <returns>
        ///     The <see cref="Tin" />.
        /// </returns>
        protected Tin GetNextItem()
        {
            return queue.Dequeue();
        }
    }
}