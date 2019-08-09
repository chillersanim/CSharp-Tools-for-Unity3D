using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity_Tools.Pipeline;
using Unity_Tools.Pipeline.Specialized;

namespace Assets.Unity_Tools
{
    public class ReplaceColliderExample : MonoBehaviour
    {
        public PipelineGraph graph;

        void OnEnable()
        {
            graph = BuildPipeline();
            graph.Initialize();
        }

        void Update()
        {
            graph.DoWork(5.0f);
        }

        public PipelineGraph BuildPipeline()
        {
            // Create the pipeline graph
            var graph = new PipelineGraph();

            // Create the pipeline nodes
            var getGameObjects = new PP_GameObjectCollector();
            var meshRendererFilter = new PP_FilterByComponent<Renderer>();
            var removeOldCollider = new PP_RemoveComponents<Collider>();
            var addCollider = new PP_AddCollider();

            // Add nodes to graph
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
