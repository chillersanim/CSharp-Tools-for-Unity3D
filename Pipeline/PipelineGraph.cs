// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PipelineGraph.cs
// 
// Created:          16.08.2019  16:33
// Last modified:    25.10.2019  11:38
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
using System.Diagnostics;

namespace Unity_Tools.Pipeline
{
    /// <summary>
    ///     The pipeline pipelineGraph.
    /// </summary>
    public class PipelineGraph
    {
        /// <summary>
        ///     The nodes.
        /// </summary>
        private readonly List<IPipelineNode> nodes = new List<IPipelineNode>();

        /// <summary>
        ///     The stopwatch.
        /// </summary>
        private readonly Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        ///     The add node.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        public void AddNode(IPipelineNode node)
        {
            if (node == null || nodes.Contains(node))
            {
                return;
            }

            nodes.Add(node);
        }

        /// <summary>
        ///     The do work.
        /// </summary>
        /// <param name="maxMilliseconds">
        ///     The max time.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool DoWork(float maxMilliseconds)
        {
            stopwatch.Reset();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalMilliseconds < maxMilliseconds)
            {
                var hadWork = false;

                foreach (var node in nodes)
                {
                    hadWork |= node.ProcessNextItem();
                }

                if (!hadWork)
                {
                    stopwatch.Stop();
                    return false;
                }
            }

            stopwatch.Stop();
            return true;
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        public void Initialize()
        {
            foreach (var node in nodes)
            {
                node.Initialize();
            }
        }

        /// <summary>
        ///     The remove node.
        /// </summary>
        /// <param name="node">
        ///     The node.
        /// </param>
        public void RemoveNode(IPipelineNode node)
        {
            if (node == null)
            {
                return;
            }

            var index = nodes.IndexOf(node);

            if (index < 0 || index >= nodes.Count)
            {
                return;
            }

            if (index < nodes.Count - 1)
            {
                nodes[index] = nodes[nodes.Count - 1];
                nodes.RemoveAt(nodes.Count - 1);
            }
            else
            {
                nodes.RemoveAt(index);
            }
        }
    }
}