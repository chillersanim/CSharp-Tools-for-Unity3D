// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         IVectorF.cs
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

using JetBrains.Annotations;

namespace Unity_Tools.Core
{
    /// <summary>
    /// Interface for single-precision floating-point vectors.
    /// </summary>
    public interface IVectorF : IVector
    {
        /// <summary>
        /// Gets a value indicating whether at least one of the components is either <see cref="float.PositiveInfinity"/> or <see cref="float.NegativeInfinity"/>.
        /// </summary>
        [PublicAPI]
        bool IsInfinity { get; }

        /// <summary>
        /// Gets a value indicating whether at least one of the components is <see cref="float.NaN"/>.
        /// </summary>
        [PublicAPI]
        bool IsNaN { get; }

        /// <summary>
        /// Gets the value at the given index.
        /// </summary>
        /// <param name="index">The index/dimension to take the value from.</param>
        /// <returns>Returns the value at the given index, or 0 if the index is over the bounds.</returns>
        [PublicAPI]
        float this[int index] { get; }

        /// <summary>
        /// Gets the vector length.
        /// </summary>
        [PublicAPI]
        float Length { get; }

        /// <summary>
        /// Gets the square vector length.
        /// </summary>
        [PublicAPI]
        float SqLength { get; }

        IVectorF Add(IVectorF other);

        float Dot(IVectorF other);

        IVectorF Subtract(IVectorF other);
    }
}
