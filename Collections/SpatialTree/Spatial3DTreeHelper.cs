// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Spatial3DTreeHelper.cs
// 
// Created:          05.08.2019  11:27
// Last modified:    15.08.2019  17:56
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

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Unity_Tools.Collections.SpatialTree.Enumerators;

namespace Unity_Tools.Collections.SpatialTree
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