// Copyright © 2019 Jasper Ermatinger

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Unity_Collections.SpatialTree.Enumerators;

namespace Unity_Collections.SpatialTree
{
    public static class Spatial3DTreeHelper
    {
        public static void ShapeCast<T>([NotNull] this Spatial3DTree<T> tree, [NotNull] IShape shape,
            [NotNull] IList<T> result) where T : class
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));

            if (shape == null) throw new ArgumentNullException(nameof(shape));

            if (result == null) throw new ArgumentNullException(nameof(result));

            var shapeEnumerator = new ShapeCastEnumerator<T>(tree, shape);

            while (shapeEnumerator.MoveNext()) result.Add(shapeEnumerator.Current);
        }

        public static IList<T> ShapeCast<T>([NotNull] this Spatial3DTree<T> tree, [NotNull] IShape shape)
            where T : class
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));

            if (shape == null) throw new ArgumentNullException(nameof(shape));

            var shapeEnumerator = new ShapeCastEnumerator<T>(tree, shape);
            var result = new List<T>();

            while (shapeEnumerator.MoveNext()) result.Add(shapeEnumerator.Current);

            return result;
        }

        public static IList<T> SphereCast<T>([NotNull] this Spatial3DTree<T> tree, Vector3 center, float radius,
            [NotNull] IList<T> result) where T : class
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));

            if (result == null) throw new ArgumentNullException(nameof(result));

            var enumerator = new SphereCastEnumerator<T>(tree, center, radius);

            while (enumerator.MoveNext()) result.Add(enumerator.Current);

            return result;
        }

        public static IList<T> SphereCast<T>([NotNull] this Spatial3DTree<T> tree, Vector3 center, float radius)
            where T : class
        {
            if (tree == null) throw new ArgumentNullException(nameof(tree));

            var result = new List<T>();
            var enumerator = new SphereCastEnumerator<T>(tree, center, radius);

            while (enumerator.MoveNext()) result.Add(enumerator.Current);

            return result;
        }
    }
}