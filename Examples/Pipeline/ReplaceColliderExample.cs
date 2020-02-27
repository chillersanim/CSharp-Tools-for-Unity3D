// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         ReplaceColliderExample.cs
// 
// Created:          27.01.2020  22:48
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

using UnityEngine;
using UnityTools.Pipeline;
using UnityTools.Pipeline.Specialized;

namespace UnityTools.Examples.Pipeline
{
    public class ReplaceColliderExample : MonoBehaviour
    {
        public PipelineGraph pipelineGraph;

        public PipelineGraph BuildPipeline()
        {
            // Create the pipeline pipelineGraph
            var graph = new PipelineGraph();

            // Create the pipeline nodes
            var getGameObjects = new GameObjectCollector();
            var meshRendererFilter = new FilterByComponent<Renderer>();
            var removeOldCollider = new RemoveComponents<Collider>();
            var addCollider = new AddCollider();

            // Add nodes to pipelineGraph
            graph.AddNode(getGameObjects);
            graph.AddNode(meshRendererFilter);
            graph.AddNode(removeOldCollider);
            graph.AddNode(addCollider);

            // Add node connections
            getGameObjects.AddFollowupStep(meshRendererFilter);
            meshRendererFilter.AddFollowupStep(removeOldCollider);
            removeOldCollider.AddFollowupStep(addCollider);
            // removeOldCollider doesn't need a followup step, as it is the last node

            return graph;
        }

        void OnEnable()
        {
            pipelineGraph = BuildPipeline();
            pipelineGraph.Initialize();
        }

        void Update()
        {
            pipelineGraph.DoWork(2.0f);
        }
    }
}
