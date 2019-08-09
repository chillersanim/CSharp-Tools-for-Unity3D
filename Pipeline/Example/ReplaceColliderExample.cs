// Solution:         Unity Tools
// Project:          Assembly-CSharp
// Filename:         ReplaceColliderExample.cs
// 
// Created:          09.08.2019  16:53
// Last modified:    09.08.2019  16:54
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

using UnityEngine;
using Unity_Tools.Pipeline.Specialized;

namespace Unity_Tools.Pipeline.Example
{
    public class ReplaceColliderExample : MonoBehaviour
    {
        public PipelineGraph pipelineGraph;

        void OnEnable()
        {
            pipelineGraph = BuildPipeline();
            pipelineGraph.Initialize();
        }

        void Update()
        {
            pipelineGraph.DoWork(2.0f);
        }

        public PipelineGraph BuildPipeline()
        {
            // Create the pipeline pipelineGraph
            var graph = new PipelineGraph();

            // Create the pipeline nodes
            var getGameObjects = new PP_GameObjectCollector();
            var meshRendererFilter = new PP_FilterByComponent<Renderer>();
            var removeOldCollider = new PP_RemoveComponents<Collider>();
            var addCollider = new PP_AddCollider();

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
    }
}
