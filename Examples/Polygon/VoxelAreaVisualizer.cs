using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTools.Core;
using UnityTools.Experimental;
using Rect = UnityTools.Core.Rect;

namespace UnityTools.Examples.Area
{
    [ExecuteAlways]
    public class VoxelAreaVisualizer : MonoBehaviour
    {
        public enum ShapeType
        {
            Circle,

            Rect
        }

        public enum Operation
        {
            Add,

            Remove
        }

        [Serializable]
        public class AreaOperation
        {
            public ShapeType Shape;

            public Operation Operation;

            public Vector2 Center;

            public Vector2 Size;

            public float Rotation;

            public bool Invert;
        }

        public bool FullInitialArea = false;

        [Header("Operations")]
        public AreaOperation[] Operations =
        {
            new AreaOperation
            {
                Shape = ShapeType.Circle, 
                Operation = Operation.Remove, 
                Center = new Vector2(-1, 1),
                Size = new Vector2(1, 0), 
                Rotation = 0,
                Invert = false
            }
        };

        public int Nodes;

        public int Leafs;

        public int ContentLeafs;

        [NonSerialized]
        private VoxelArea voxelArea;

        private void OnEnable()
        {
            this.GenerateData();
        }

        private void OnDrawGizmos()
        {
            if (this.voxelArea == null)
            {
                this.GenerateData();
            }  

            this.voxelArea.DrawGizmos();
        }

        private void GenerateData()
        {
            this.voxelArea = new VoxelArea();

            if (this.FullInitialArea)
            {
                this.voxelArea.Invert();
            }

            foreach (var operation in this.Operations)
            {
                IArea shape;

                switch (operation.Shape)
                {
                    case ShapeType.Rect:
                        shape = new Rect(operation.Center, operation.Size, operation.Rotation * Mathf.Deg2Rad);
                        break;
                    case ShapeType.Circle:
                        shape = new Circle(operation.Center, operation.Size.x);
                        break;
                    default:
                        continue;
                }

                if (operation.Invert)
                {
                    shape = new AreaInverse(shape);
                }

                switch (operation.Operation)
                {
                    case Operation.Add:
                        this.voxelArea.Add(shape);
                        break;
                    case Operation.Remove:
                        this.voxelArea.Subtract(shape);
                        break;
                }
            }

            this.Nodes = this.voxelArea.GetNodeCount();
            this.Leafs = this.voxelArea.GetLeafCount();
            this.ContentLeafs = this.voxelArea.GetContentLeafCount();
        }
    }
}
