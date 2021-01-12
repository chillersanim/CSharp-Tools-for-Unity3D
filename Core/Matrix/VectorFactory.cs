// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         VectorFactory.cs
// 
// Created:          27.01.2020  22:49
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

using System;
using JetBrains.Annotations;

namespace Unity_Tools.Core
{
    /// <summary>
    /// Factory class for vector generation.
    /// </summary>
    public static class VectorFactory
    {
        /// <summary>
        /// Builds a <see cref="IVectorD"/> from the provided values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>Returns an <see cref="IVectorD"/> with the same dimension as the <see cref="T:double[]"/> length.</returns>
        [NotNull]
        [PublicAPI]
        public static IVectorD Build([NotNull] params double[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            switch (values.Length)
            {
                case 2:
                    return new Vector2d(values[0], values[1]);
                case 3:
                    return new Vector3d(values[0], values[1], values[2]);
                case 4:
                    return new Vector4d(values[0], values[1], values[2], values[3]);
                default: return new VectorNd(values);
            }
        }

        /// <summary>
        /// Builds a <see cref="IVectorD"/> from the provided values, using the array directly in the vector if possible.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>Returns an <see cref="IVectorD"/> with the same dimension as the <see cref="T:double[]"/> length.</returns>
        [NotNull]
        internal static IVectorD Build([NotNull] ref double[] values)
        {
            switch (values.Length)
            {
                case 2:
                    return new Vector2d(values[0], values[1]);
                case 3:
                    return new Vector3d(values[0], values[1], values[2]);
                case 4:
                    return new Vector4d(values[0], values[1], values[2], values[3]);
                default: return new VectorNd(ref values);
            }
        }
    }
}
