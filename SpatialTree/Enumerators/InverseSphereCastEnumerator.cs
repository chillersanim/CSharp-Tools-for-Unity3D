// Copyright © 2019 Jasper Ermatinger

using UnityEngine;

namespace Unity_Collections.SpatialTree.Enumerators
{
    public sealed class InverseSphereCastEnumerator<T> : Spatial3DTreeExclusionEnumeratorBase<T> where T : class
    {
        private Vector3 center;

        private float sqrRadius;

        public InverseSphereCastEnumerator(Spatial3DTree<T> tree, Vector3 center, float radius) : base(tree)
        {
            this.center = center;
            sqrRadius = radius * radius;
        }

        public void Restart(Vector3 center, float radius)
        {
            this.center = center;
            sqrRadius = radius * radius;
            Reset();
        }

        /// <inheritdoc />
        protected override bool IsAabbNotFullyInside(Vector3 start, Vector3 end)
        {
            var a = start - center;
            var b = end - center;

            var xx = Mathf.Max(a.x * a.x, b.x * b.x);
            var yy = Mathf.Max(a.y * a.y, b.y * b.y);
            var zz = Mathf.Max(a.z * a.z, b.z * b.z);

            return xx + yy + zz <= sqrRadius;
        }

        /// <inheritdoc />
        protected override bool IsPointOutside(Vector3 point)
        {
            var x = point.x - center.x;
            var y = point.y - center.y;
            var z = point.z - center.z;

            return x * x + y * y + z * z > sqrRadius;
        }
    }
}