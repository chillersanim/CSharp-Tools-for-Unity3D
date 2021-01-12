// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         MatrixD.cs
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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace Unity_Tools.Core
{
    #region

    #endregion

    /// <summary>
    /// Basis class for all matrices using <see cref="double"/> values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int))]
    public abstract class MatrixD : ICloneable, ISerializable
    {
        #region ICloneable Members

        /// <inheritdoc />
        [Pure]
        public virtual object Clone()
        {
            var result = MatrixFactory.ZeroD(this.Rows, this.Columns);

            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    result.SetAt(i, j, this.GetAt(i, j));
                }
            }

            return result;
        }

        #endregion

        #region ISerializable Members

        /// <inheritdoc />
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("rows", this.Rows);
            info.AddValue("columns", this.Columns);
        }

        #endregion

        #region Fields

        /// <summary>
        /// Stores the amount of rows in this matrix.
        /// </summary>
        [FieldOffset(0)]
        [PublicAPI]
        private readonly int rows;

        /// <summary>
        /// Stores the amount of columns in this matrix.
        /// </summary>
        [FieldOffset(sizeof(int))]
        [PublicAPI]
        private readonly int columns;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixD"/> class.
        /// </summary>
        protected MatrixD(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixD"/> class.
        /// </summary>
        protected MatrixD([NotNull] SerializationInfo info, StreamingContext context)
        {
            this.rows = info.GetInt32("rows");
            this.columns = info.GetInt32("columns");
        }

        #endregion

        #region Properties and Indexers

        /// <summary>
        /// Gets or sets the value in the given position.
        /// </summary>
        /// <param name="index">The index, defined as: index = row * (this.columns) + column.</param>
        /// <returns></returns>
        [PublicAPI]
        public double this[int index]
        {
            [Pure]
            get
            {
                if (index < 0 || index >= this.rows * this.columns)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),
                        $"The index must be between 0 (inclusive) and rows*columns = {this.rows * this.columns} (exclusive).");
                }

                var row = index / this.columns;
                var column = index % this.columns;

                return this.GetAt(row, column);
            }

            set
            {
                if (index < 0 || index >= this.rows * this.columns)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),
                        $"The index must be between 0 (inclusive) and rows*columns = {this.rows * this.columns} (exclusive).");
                }

                var row = index / this.columns;
                var column = index % this.columns;

                this.SetAt(row, column, value);
            }
        }

        /// <summary>
        /// Gets or sets a value in the given row and column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>Returns the value stored in the given row and column.</returns>
        [PublicAPI]
        public double this[int row, int column]
        {
            [Pure]
            get
            {
                if (row < 0 || row >= this.Rows)
                {
                    throw new ArgumentException(
                        $"The row must be between 0 (inclusive) and {this.Rows} (exclusive).",
                        nameof(row));
                }

                if (column < 0 || column >= this.Columns)
                {
                    throw new ArgumentException(
                        $"The column must be between 0 (inclusive) and {this.Columns} (exclusive).",
                        nameof(column));
                }

                return this.GetAt(row, column);
            }

            set
            {
                if (row < 0 || row >= this.Rows)
                {
                    throw new ArgumentException(
                        $"The row must be between 0 (inclusive) and {this.Rows} (exclusive).",
                        nameof(row));
                }

                if (column < 0 || column >= this.Columns)
                {
                    throw new ArgumentException(
                        $"The column must be between 0 (inclusive) and {this.Columns} (exclusive).",
                        nameof(column));
                }

                this.SetAt(row, column, value);
            }
        }

        /// <summary>
        /// Gets the amount of columns in this matrix.
        /// </summary>
        [PublicAPI]
        public int Columns
        {
            [Pure]
            get
            {
                return this.columns;
            }
        }

        /// <summary>
        /// Gets the amount of rows in this matrix.
        /// </summary>
        [PublicAPI]
        public int Rows
        {
            [Pure]
            get
            {
                return this.rows;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Adds two matrices together and returns the result as new matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this one (this + other).</param>
        /// <returns>Returns the resulting matrix.</returns>
        [Pure]
        [NotNull]
        [PublicAPI]
        public MatrixD Add([NotNull] MatrixD other)
        {
            var result = MatrixFactory.ZeroD(this.Rows, this.Columns);
            this.Add(other, result);
            return result;
        }

        /// <summary>
        ///     Adds two matrices together and stores the result in the output matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this one (this + other).</param>
        /// <param name="output">The matrix in which the result is stored.</param>
        [PublicAPI]
        public void Add([NotNull] MatrixD other, [NotNull] MatrixD output)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (this.Rows != other.Rows || this.Columns != other.Columns)
            {
                throw new ArgumentException(
                    $"The other matrix has the wrong size (Size: {other.Rows}x{other.Columns}, Required: {this.Rows}x{this.Columns}",
                    nameof(other));
            }

            if (this.Rows != output.Rows || this.Columns != output.Columns)
            {
                throw new ArgumentException(
                    $"The output matrix has the wrong size (Size: {output.Rows}x{output.Columns}, Required: {this.Rows}x{this.Columns}",
                    nameof(output));
            }

            this.AddInternal(other, output);
        }

        /// <summary>
        ///     Clamps all values near zero to zero
        /// </summary>
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CoerceZero()
        {
            this.CoerceZero(double.Epsilon);
        }

        /// <summary>
        ///     Clamps all values near zero to zero
        /// </summary>
        /// <param name="epsilon">The maximum distance to zero for which an element is considered zero.</param>
        [PublicAPI]
        public virtual void CoerceZero(double epsilon)
        {
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    if (CommonUtil.Equals(this.GetAt(i, j), 0, epsilon))
                    {
                        this.SetAt(i, j, 0f);
                    }
                }
            }
        }

        /// <inheritdoc />
        [Pure]
        [PublicAPI]
        public override bool Equals(object obj)
        {
            return this == obj as MatrixD;
        }

        /// <inheritdoc />
        [Pure]
        [PublicAPI]
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            {
                var hash = (int)2166136261;
                for (var i = 0; i < this.Rows; i++)
                {
                    for (var j = 0; j < this.Columns; j++)
                    {
                        hash = (hash * 16777619) ^ this.GetAt(i, j).GetHashCode();
                    }
                }

                return hash;
            }
        }

        /// <summary>
        ///     Multiplies two matrices and returns the result as new matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with this one (this * other)</param>
        /// <returns>Returns the resulting matrix.</returns>
        [Pure]
        [NotNull]
        [PublicAPI]
        public MatrixD Multiply([NotNull] MatrixD other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this.Columns != other.Rows)
            {
                throw new ArgumentException(
                    $"The amount of columns of the left matrix mismatch the amount of rows of the right matrix. (left: {this.Columns}, right: {other.Rows})");
            }

            var result = MatrixFactory.ZeroD(this.Rows, other.Columns);
            this.MultiplyInternal(other, result);
            return result;
        }

        /// <summary>
        ///     Multiplies two matrices and stores the result in the output matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with this one (this * other)</param>
        /// <param name="output">The matrix to store the result in</param>
        [PublicAPI]
        public void Multiply([NotNull] MatrixD other, [NotNull] MatrixD output)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (this.Columns != other.Rows)
            {
                throw new ArgumentException(
                    $"The amount of columns of the left matrix mismatch the amount of rows of the right matrix. (left: {this.Columns}, right: {other.Rows})");
            }

            if (output.Rows != this.Rows || output.Columns != other.Columns)
            {
                throw new ArgumentException(
                    $"The output matrix has the wrong size (Size: {output.Rows}x{output.Columns}, Required: {this.Rows}x{other.Columns}",
                    nameof(output));
            }

            this.MultiplyInternal(other, output);
        }

        /// <summary>
        ///     Multiplies this matrix with a scalar and returns the result as new matrix.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns>Returns the resulting matrix.</returns>
        [Pure]
        [NotNull]
        [PublicAPI]
        public virtual MatrixD Multiply(double scale)
        {
            var result = MatrixFactory.ZeroD(this.Rows, this.Columns);

            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    result.SetAt(i, j, this.GetAt(i, j) * scale);
                }
            }

            return result;
        }

        /// <summary>
        ///     Negates all components of this matrix.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public MatrixD Negate()
        {
            var result = MatrixFactory.ZeroD(this.Rows, this.Columns);
            this.Negate(result);
            return result;
        }

        /// <summary>
        ///     Negates all components of this matrix and stores the result in the output matrix.
        /// </summary>
        /// <param name="output">The matrix in which the output will be stored.</param>
        [PublicAPI]
        public void Negate([NotNull] MatrixD output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (output.Rows != this.Rows || output.Columns != this.Columns)
            {
                throw new ArgumentException(
                    $"The output matrix has the wrong size (Size: {output.Rows}x{output.Columns}, Required: {this.Rows}x{this.Columns}",
                    nameof(output));
            }

            this.NegateInternal(output);
        }

        /// <summary>
        /// Gets the row at the given index.
        /// </summary>
        /// <param name="r">The row index.</param>
        /// <returns>The row.</returns>
        public IVectorD Row(int r)
        {
            var result = new double[columns];
            for (var c = 0; c < columns; c++)
            {
                result[c] = GetAt(r, c);
            }

            return VectorFactory.Build(ref result);
        }

        /// <summary>
        /// Gets the column at the given index.
        /// </summary>
        /// <param name="c">The column index.</param>
        /// <returns>The column.</returns>
        public IVectorD Column(int c)
        {
            var result = new double[rows];
            for (var r = 0; r < rows; r++)
            {
                result[r] = GetAt(r, c);
            }

            return VectorFactory.Build(ref result);
        }

        /// <summary>
        ///     Scales this matrix.
        /// </summary>
        /// <param name="scale">The scale.</param>
        [PublicAPI]
        public virtual void Scale(double scale)
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    this.SetAt(i, j, this.GetAt(i, j) * scale);
                }
            }
        }

        /// <summary>
        ///     Subtracts the other matrix from this one and returns the result as a new matrix.
        /// </summary>
        /// <param name="other">The other matrix.</param>
        /// <returns>Returns the resulting matrix.</returns>
        [Pure]
        [NotNull]
        [PublicAPI]
        public MatrixD Subtract([NotNull] MatrixD other)
        {
            var result = MatrixFactory.ZeroD(this.Rows, this.Columns);
            this.Subtract(other, result);
            return result;
        }

        /// <summary>
        ///     Subtracts the other matrix from this one and stores the result in the output matrix.
        /// </summary>
        /// <param name="other">The other matrix.</param>
        /// <param name="output"></param>
        [PublicAPI]
        public void Subtract([NotNull] MatrixD other, [NotNull] MatrixD output)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (this.Rows != other.Rows || this.Columns != other.Columns)
            {
                throw new ArgumentException(
                    $"The other matrix has the wrong size (Size: {other.Rows}x{other.Columns}, Required: {this.Rows}x{this.Columns}",
                    nameof(other));
            }

            if (this.Rows != output.Rows || this.Columns != output.Columns)
            {
                throw new ArgumentException(
                    $"The output matrix has the wrong size (Size: {output.Rows}x{output.Columns}, Required: {this.Rows}x{this.Columns}",
                    nameof(output));
            }

            this.SubtractInternal(other, output);
        }

        /// <inheritdoc/>
        [Pure]
        [PublicAPI]
        public override string ToString()
        {
            var texts = new string[this.Rows, this.Columns];
            var widths = new int[this.Columns];

            for (var r = 0; r < this.Rows; r++)
            {
                for (var c = 0; c < this.Columns; c++)
                {
                    var value = this.GetAt(r, c);
                    var text = CommonUtil.NumberString(value);
                    texts[r, c] = text;

                    if (widths[c] < text.Length)
                    {
                        widths[c] = text.Length;
                    }
                }
            }

            var sb = new StringBuilder(8 + this.Rows * this.Columns * 8);
            sb.Append("MatrixUtil ");
            sb.Append(this.Rows);
            sb.Append('x');
            sb.Append(this.Columns);
            sb.AppendLine(" (Double)");
            sb.Append(' ');

            for (var r = 0; r < this.Rows; r++)
            {
                for (var c = 0; c < this.Columns; c++)
                {
                    var text = texts[r, c];

                    Debug.Assert(text != null, "text != null");
                    for (var i = text.Length; i < widths[c]; i++)
                    {
                        sb.Append(' ');
                    }

                    sb.Append(text);

                    if (c < this.Columns - 1)
                    {
                        sb.Append("  ");
                    }
                }

                if (r < this.Rows - 1)
                {
                    sb.AppendLine();
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Transposes the matrix and returns the result as new matrix
        /// </summary>
        /// <returns>The transposed matrix</returns>
        [Pure]
        [NotNull]
        [PublicAPI]
        public MatrixD Transpose()
        {
            var result = MatrixFactory.ZeroD(this.Columns, this.Rows);
            this.Transpose(result);
            return result;
        }

        /// <summary>
        ///     Transposes the matrix and stores the result in the output matrix
        /// </summary>
        /// <param name="output">The matrix in which the result will be stored.</param>
        [PublicAPI]
        public void Transpose([NotNull] MatrixD output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (this.Rows != output.Columns || this.Columns != output.Rows)
            {
                throw new ArgumentException(
                    $"The output matrix has the wrong size (Size: {output.Rows}x{output.Columns}, Required: {this.Columns}x{this.Rows}");
            }

            this.TransposeInternal(output);
        }

        /// <summary>
        ///     Does the addition process, can assume that the preconditions are met.
        /// </summary>
        /// <param name="other">The other matrix.</param>
        /// <param name="output">The output matrix (Preinitialized).</param>
        protected virtual void AddInternal([NotNull] MatrixD other, [NotNull] MatrixD output)
        {
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    output.SetAt(i, j, this.GetAt(i, j) + other.GetAt(i, j));
                }
            }
        }

        /// <summary>
        ///     Does the multiplication process, can assume that the preconditions are met.
        /// </summary>
        /// <param name="other">The other matrix.</param>
        /// <param name="output">The output matrix (Preinitialized).</param>
        protected virtual void MultiplyInternal([NotNull] MatrixD other, [NotNull] MatrixD output)
        {
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < other.Columns; j++)
                {
                    double entry = 0;

                    for (var lr = 0; lr < this.Columns; lr++)
                    {
                        entry += this.GetAt(i, lr) * other.GetAt(lr, j);
                    }

                    output.SetAt(i, j, entry);
                }
            }
        }

        /// <summary>
        ///     Does the negation process, can assume that the preconditions are met.
        /// </summary>
        /// <param name="output">The other matrix.</param>
        protected virtual void NegateInternal([NotNull] MatrixD output)
        {
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    output.SetAt(i, j, -this.GetAt(i, j));
                }
            }
        }

        /// <summary>
        ///     Does the substraction process, can assume that the preconditions are met.
        /// </summary>
        /// <param name="other">The other matrix.</param>
        /// <param name="output">The output matrix (Preinitialized).</param>
        protected virtual void SubtractInternal([NotNull] MatrixD other, [NotNull] MatrixD output)
        {
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    output.SetAt(i, j, this.GetAt(i, j) - other.GetAt(i, j));
                }
            }
        }

        /// <summary>
        ///     Does the transposition process, can assume that the preconditions are met.
        /// </summary>
        /// <param name="output">The output matrix (Preinitialized).</param>
        protected virtual void TransposeInternal([NotNull] MatrixD output)
        {
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    output.SetAt(j, i, this.GetAt(i, j));
                }
            }
        }

        #endregion

        #region Public Operators

        /// <summary>
        ///     Adds two matrices together and returns the result as a new matrix.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static MatrixD operator +([NotNull] MatrixD left, [NotNull] MatrixD right)
        {
            return left.Add(right);
        }

        /// <summary>
        /// Tests two <see cref="MatrixD"/> for numerical equality.
        /// </summary>
        [Pure]
        [PublicAPI]
        public static bool operator ==([CanBeNull] MatrixD left, [CanBeNull] MatrixD right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            if (left.Rows != right.Rows || left.Columns != right.Columns)
            {
                return false;
            }

            for (var i = 0; i < left.Rows; i++)
            {
                for (var j = 0; j < left.Columns; j++)
                {
                    if (!CommonUtil.Equals(left.GetAt(i, j), right.GetAt(i, j)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Converts the <see cref="MatrixD"/> to a <see cref="MatrixF"/>.
        /// </summary>
        /// <param name="matrix">The matrix to convert.</param>
        [Pure]
        [CanBeNull]
        [PublicAPI]
        public static explicit operator MatrixF([CanBeNull] MatrixD matrix)
        {
            if (matrix == null)
            {
                return null;
            }

            var result = MatrixFactory.ZeroF(matrix.Rows, matrix.Columns);

            for (var i = 0; i < matrix.Rows; i++)
            {
                for (var j = 0; j < matrix.Columns; j++)
                {
                    result.SetAt(i, j, (float)matrix.GetAt(i, j));
                }
            }

            return result;
        }

        /// <summary>
        /// Tests two <see cref="MatrixD"/> for numerical inequality.
        /// </summary>
        [Pure]
        [PublicAPI]
        public static bool operator !=([CanBeNull] MatrixD left, [CanBeNull] MatrixD right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return true;
            }

            if (left.Rows != right.Rows || left.Columns != right.Columns)
            {
                return true;
            }

            for (var i = 0; i < left.Rows; i++)
            {
                for (var j = 0; j < left.Columns; j++)
                {
                    if (!CommonUtil.Equals(left.GetAt(i, j), right.GetAt(i, j)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Multiplies two <see cref="MatrixD"/> instances.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static MatrixD operator *([NotNull] MatrixD left, [NotNull] MatrixD right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        /// Multiplies a <see cref="IVectorD"/> with the <see cref="MatrixD"/> and outputs the resulting column vector.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static IVectorD operator *([NotNull] IVectorD left, [NotNull] MatrixD right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (right.Rows != left.Dimension)
            {
                throw new ArgumentException(
                    "The dimension of the vector must match the amount of rows in the matrix.",
                    nameof(right));
            }

            var values = new double[right.Columns];

            for (var i = 0; i < right.Rows; i++)
            {
                var value = left[i];
                for (var j = 0; j < right.Columns; j++)
                {
                    value *= right.GetAt(i, j);
                }

                values[i] = value;
            }

            return VectorFactory.Build(ref values);
        }

        /// <summary>
        /// Multiplies a <see cref="MatrixD"/> with the <see cref="IVectorD"/> and outputs the resulting row vector.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static IVectorD operator *([NotNull] MatrixD left, [NotNull] IVectorD right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (left.Columns != right.Dimension)
            {
                throw new ArgumentException(
                    "The dimension of the vector must match the amount of columns in the matrix.",
                    nameof(right));
            }

            var values = new double[left.Rows];

            for (var j = 0; j < left.Columns; j++)
            {
                var value = right[j];
                for (var i = 0; i < left.Rows; i++)
                {
                    value *= left.GetAt(i, j);
                }

                values[j] = value;
            }

            return VectorFactory.Build(ref values);
        }

        /// <summary>
        /// Scales the <see cref="MatrixD"/> with the given <see cref="double"/>.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static MatrixD operator *([NotNull] MatrixD left, double right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        /// Scales the <see cref="MatrixD"/> with the given <see cref="double"/>.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static MatrixD operator *(double left, [NotNull] MatrixD right)
        {
            return right.Multiply(left);
        }

        /// <summary>
        /// Subtracts the right <see cref="MatrixD"/> from the left one.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static MatrixD operator -([NotNull] MatrixD left, [NotNull] MatrixD right)
        {
            return left.Subtract(right);
        }

        /// <summary>
        /// Negates the <see cref="MatrixD"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static MatrixD operator -([NotNull] MatrixD matrix)
        {
            return matrix.Negate();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Unchecked method to get the value at the given row and column. <br />
        ///     Doesn't do a boundary check.
        /// </summary>
        /// <param name="row">The row to access.</param>
        /// <param name="column">The column to access.</param>
        /// <returns>Returns the requested value.</returns>
        [Pure]
        protected internal abstract double GetAt(int row, int column);

        /// <summary>
        ///     Unchecked method to set the value at the given row and column. <br />
        ///     Doesn't do a boundary check.
        /// </summary>
        /// <param name="row">The row to access.</param>
        /// <param name="column">The column to access.</param>
        /// <param name="value">The new value.</param>
        protected internal abstract void SetAt(int row, int column, double value);

        #endregion
    }
}