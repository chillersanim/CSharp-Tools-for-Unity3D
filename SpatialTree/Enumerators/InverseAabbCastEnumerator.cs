// Copyright © 2019 Jasper Ermatinger

using UnityEngine;

namespace Unity_Collections.SpatialTree.Enumerators
{
    public sealed class InverseAabbCastEnumerator<T> : Spatial3DTreeExclusionEnumeratorBase<T> where T : class
    {
        private Vector3 min, max;

        public InverseAabbCastEnumerator(Spatial3DTree<T> tree, Vector3 min, Vector3 max) : base(tree)
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

        protected override bool IsAabbNotFullyInside(Vector3 start, Vector3 end)
        {
            return min.x < start.x && min.y < start.y && min.z < start.z ||
                   max.x > end.x && max.y > end.y && max.z > end.z;
        }

        protected override bool IsPointOutside(Vector3 point)
        {
            return point.x < min.x && point.y < min.y && point.y < min.z ||
                   point.x > max.x && point.y > max.y && point.z > max.z;
        }
    }
}