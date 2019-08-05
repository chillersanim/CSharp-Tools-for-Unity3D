// Copyright © 2019 Jasper Ermatinger

using Assets.Scripts.Common.SpatialTree;
using UnityEngine;

namespace ScriptTools.Collections.SpatialGrid3D
{
    public sealed class AabbCastEnumerator<T> : Spatial3DTreeInclusionEnumeratorBase<T> where T : class
    {
        private Vector3 min, max;

        public AabbCastEnumerator(Spatial3DTree<T> tree, Vector3 min, Vector3 max) : base(tree)
        {
            this.min = min;
            this.max = max;
        }

        public void Restart(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
            Reset();
        }

        /// <inheritdoc />
        protected override bool IsAabbInside(Vector3 start, Vector3 end)
        {
            return min.x <= end.x && min.y <= end.y && min.z <= end.z &&
                   max.x >= start.x && max.y >= start.y && max.z >= start.z;
        }

        /// <inheritdoc />
        protected override bool IsPointInside(Vector3 point)
        {
            return point.x >= min.x && point.y >= min.y && point.z >= min.z &&
                   point.x <= max.x && point.y <= max.y && point.z <= max.z;
        }
    }
}