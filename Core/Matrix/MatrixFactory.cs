// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         MatrixUtil.cs
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace UnityTools.Core
{
    #region

    #endregion

    /// <summary>
    ///     Factory class for matrices
    /// </summary>
    public static class MatrixFactory
    {
        #region Public Static Methods

        /// <summary>
        ///     Generates a matrix from the given data, interpreting the data as row after row
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <param name="data">The elements of the new matrix.</param>
        /// <exception cref="ArgumentNullException"><see cref="data" /> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The amount of values in <see cref="data" /> must be equal to
        ///     <see cref="rows" /> times <see cref="columns" />.
        /// </exception>
        /// <returns>Returns the initialized matrix with the given data.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF Build(int rows, int columns, [NotNull] params float[] data)
        {
            // NOTE: While this implementation is equal to the build with an IList<float>, for performance reasons it will not be replaced
            // so it can be optimized for arrays.
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length != rows * columns)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"Rows:{rows}, Columns:{columns}, Data amount:{data.Length}, Required amount:{rows * columns}",
                    "The data doesn't match the dimension of the matrix..");
            }

            var result = GetF(rows, columns);
            var index = 0;

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result.SetAt(i, j, data[index++]);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix from the given data, interpreting the data as row after row
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <param name="data">The elements of the new matrix.</param>
        /// <exception cref="ArgumentNullException"><see cref="data" /> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The amount of values in <see cref="data" /> must be equal to
        ///     <see cref="rows" /> times <see cref="columns" />.
        /// </exception>
        /// <returns>Returns the initialized matrix with the given data.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD Build(int rows, int columns, [NotNull] params double[] data)
        {
            // NOTE: While this implementation is equal to the build with an IList<float>, for performance reasons it will not be replaced
            // so it can be optimized for arrays.
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length != rows * columns)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"Rows:{rows}, Columns:{columns}, Data amount:{data.Length}, Required amount:{rows * columns}",
                    "The data doesn't match the dimension of the matrix..");
            }

            var result = GetD(rows, columns);
            var index = 0;

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result.SetAt(i, j, data[index++]);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix from the given data, interpreting the data as row after row
        /// </summary>
        /// <param name="rows">
        ///     The amount of rows.
        /// </param>
        /// <param name="columns">
        ///     The amount of columns.
        /// </param>
        /// <param name="data">
        ///     The elements of the new matrix, must have the length rows*columns. (Will be copied)
        /// </param>
        /// <exception cref="ArgumentNullException"><see cref="data" /> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The amount of values in <see cref="data" /> must be equal to
        ///     <see cref="rows" /> times <see cref="columns" />.
        /// </exception>
        /// <returns>
        ///     Returns the initialized matrix with the given data.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF Build(int rows, int columns, [NotNull] IList<float> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Count != rows * columns)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"Rows:{rows}, Columns:{columns}, Data amount:{data.Count}, Required amount:{rows * columns}",
                    "The data doesn't match the dimension of the matrix..");
            }

            var result = GetF(rows, columns);
            var index = 0;

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result.SetAt(i, j, data[index++]);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix from the given data, interpreting the data as row after row
        /// </summary>
        /// <param name="rows">
        ///     The amount of rows.
        /// </param>
        /// <param name="columns">
        ///     The amount of columns.
        /// </param>
        /// <param name="data">
        ///     The elements of the new matrix, must have the length rows*columns. (Will be copied)
        /// </param>
        /// <exception cref="ArgumentNullException"><see cref="data" /> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The amount of values in <see cref="data" /> must be equal to
        ///     <see cref="rows" /> times <see cref="columns" />.
        /// </exception>
        /// <returns>
        ///     Returns the initialized matrix with the given data.
        /// </returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD Build(int rows, int columns, [NotNull] IList<double> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Count != rows * columns)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(data),
                    $"Rows:{rows}, Columns:{columns}, Data amount:{data.Count}, Required amount:{rows * columns}",
                    "The data doesn't match the dimension of the matrix..");
            }

            var result = GetD(rows, columns);
            var index = 0;

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result.SetAt(i, j, data[index++]);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix from the given data, interpreting the data as [row, column]
        /// </summary>
        /// <param name="data">The elements of the matrix (will be copied)</param>
        /// <exception cref="ArgumentNullException">The data must not be null.</exception>
        /// <returns>Returns the initialized matrix with the given data.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF Build([NotNull] float[,] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var rowLength = data.GetLength(1);
            var columnLength = data.GetLength(0);
            var result = GetF(rowLength, columnLength);

            for (var j = 0; j < columnLength; j++)
            {
                for (var i = 0; i < rowLength; i++)
                {
                    result.SetAt(i, j, data[i, j]);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix from the given data, interpreting the data as [row, column]
        /// </summary>
        /// <param name="data">The elements of the matrix (will be copied)</param>
        /// <exception cref="ArgumentNullException"><see cref="data" /> must not be null.</exception>
        /// <returns>Returns the initialized matrix with the given data.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD Build([NotNull] double[,] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var rowLength = data.GetLength(1);
            var columnLength = data.GetLength(0);
            var result = GetD(rowLength, columnLength);

            for (var j = 0; j < columnLength; j++)
            {
                for (var i = 0; i < rowLength; i++)
                {
                    result.SetAt(i, j, data[i, j]);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix from the given data, interpreting the data as [row][column]
        /// </summary>
        /// <param name="columnAmount">The amount of columns</param>
        /// <param name="rows">Contains all rows (rows ending on zeroes can be truncated, empty rows can be replaced with null).</param>
        /// <exception cref="ArgumentNullException"><see cref="rows" /> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     All rows in <see cref="rows" /> must not contain more values than
        ///     specified in <see cref="columnAmount" />.
        /// </exception>
        /// <returns>Returns the initialized matrix with the given data.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF Build(int columnAmount, [NotNull] params float[][] rows)
        {
            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            var rowAmount = rows.Length;
            var result = GetF(rowAmount, columnAmount);

            for (var i = 0; i < rowAmount; i++)
            {
                var row = rows[i];
                if (row == null)
                {
                    continue;
                }

                if (row.Length > columnAmount)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(rows),
                        $"Columns:{columnAmount}, Row length:{row.Length}",
                        "No row can be longer than the provided column amount.");
                }

                for (var j = 0; j < row.Length; j++)
                {
                    result.SetAt(i, j, row[i]);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix from the given data, interpreting the data as [row][column]
        /// </summary>
        /// <param name="columnAmount">The amount of columns</param>
        /// <param name="rows">Contains all rows (rows ending on zeroes can be truncated, empty rows can be replaced with null).</param>
        /// <exception cref="ArgumentNullException"><see cref="rows" /> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     All rows in <see cref="rows" /> must not contain more values than
        ///     specified in <see cref="columnAmount" />.
        /// </exception>
        /// <returns>Returns the initialized matrix with the given data.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD Build(int columnAmount, [NotNull] params double[][] rows)
        {
            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            var rowAmount = rows.Length;
            var result = GetD(rowAmount, columnAmount);

            for (var i = 0; i < rowAmount; i++)
            {
                var row = rows[i];
                if (row == null)
                {
                    continue;
                }

                if (row.Length > columnAmount)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(rows),
                        $"Columns:{columnAmount}, Row length:{row.Length}",
                        "No row can be longer than the provided column amount.");
                }

                for (var j = 0; j < row.Length; j++)
                {
                    result.SetAt(i, j, row[i]);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a diagonale matrix from the given values.
        /// </summary>
        /// <param name="diagonalElements">The diagonale values.</param>
        /// <exception cref="ArgumentNullException"><see cref="diagonalElements" /> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="diagonalElements" /> must contain at least one value.</exception>
        /// <returns>Returns a matrix with the given values in the diagonale and all other values zero</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF Diagonal([NotNull] params float[] diagonalElements)
        {
            if (diagonalElements == null)
            {
                throw new ArgumentNullException(nameof(diagonalElements));
            }

            if (diagonalElements.Length < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(diagonalElements),
                    diagonalElements.Length,
                    "The diagonal elements need at least one value.");
            }

            var size = diagonalElements.Length;
            var result = GetF(size, size);

            for (var i = 0; i < size; i++)
            {
                result.SetAt(i, i, diagonalElements[i]);
            }

            return result;
        }

        /// <summary>
        ///     Generates a diagonale matrix from the given values.
        /// </summary>
        /// <param name="diagonalElements">The diagonale values.</param>
        /// <exception cref="ArgumentNullException"><see cref="diagonalElements" /> must not be null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="diagonalElements" /> must contain at least one value.</exception>
        /// <returns>Returns a matrix with the given values in the diagonale and all other values zero</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD Diagonal([NotNull] params double[] diagonalElements)
        {
            if (diagonalElements == null)
            {
                throw new ArgumentNullException(nameof(diagonalElements));
            }

            if (diagonalElements.Length < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(diagonalElements),
                    diagonalElements.Length,
                    "The diagonal elements need at least one value.");
            }

            var size = diagonalElements.Length;
            var result = GetD(size, size);

            for (var i = 0; i < size; i++)
            {
                result.SetAt(i, i, diagonalElements[i]);
            }

            return result;
        }

        /// <summary>
        ///     Generates a quadratic matrix with all diagonale elements set to one.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <exception cref="ArgumentOutOfRangeException">The size must be at least 1.</exception>
        /// <returns>Returns the identity matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD IdentityD(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "The size must be at least 1.");
            }

            var result = GetD(size, size);

            for (var i = 0; i < size; i++)
            {
                result.SetAt(i, i, 1.0);
            }

            return result;
        }

        /// <summary>
        ///     Generates a quadratic matrix with the <see cref="scale" /> value on the diagonale.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="scale">The scale for the diagonale.</param>
        /// <exception cref="ArgumentOutOfRangeException">The size must be at least 1.</exception>
        /// <returns>Returns the identity matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD IdentityD(int size, double scale)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "The size must be at least 1.");
            }

            var result = GetD(size, size);

            for (var i = 0; i < size; i++)
            {
                result.SetAt(i, i, scale);
            }

            return result;
        }

        /// <summary>
        ///     Generates a quadratic matrix with all diagonale elements set to one.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <exception cref="ArgumentOutOfRangeException">The size must be at least 1.</exception>
        /// <returns>Returns the identity matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF IdentityF(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "The size must be at least 1.");
            }

            var result = GetF(size, size);

            for (var i = 0; i < size; i++)
            {
                result.SetAt(i, i, 1.0f);
            }

            return result;
        }

        /// <summary>
        ///     Generates a quadratic matrix with the <see cref="scale" /> value on the diagonale.
        /// </summary>
        /// <param name="size">The size of the matrix.</param>
        /// <param name="scale">The scale for the diagonale.</param>
        /// <exception cref="ArgumentOutOfRangeException">The size must be at least 1.</exception>
        /// <returns>Returns the identity matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF IdentityF(int size, float scale)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "The size must be at least 1.");
            }

            var result = GetF(size, size);

            for (var i = 0; i < size; i++)
            {
                result.SetAt(i, i, scale);
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix where all components have the same initial value.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="rows" /> and <see cref="columns" /> must be at least 1.</exception>
        /// <returns>Returns the matrix.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MatrixD OneD(int rows, int columns)
        {
            return OneD(rows, columns, 1d);
        }

        /// <summary>
        ///     Generates a matrix where all components have the same initial value.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <param name="scale">Optional: The value for all components.</param>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="rows" /> and <see cref="columns" /> must be at least 1.</exception>
        /// <returns>Returns the matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD OneD(int rows, int columns, double scale)
        {
            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), rows, "At least one row is required.");
            }

            if (columns < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), columns, "At least one column is required.");
            }

            var result = GetD(rows, columns);

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result.SetAt(i, j, scale);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a matrix where all components have the same initial value.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="rows" /> and <see cref="columns" /> must be at least 1.</exception>
        /// <returns>Returns the matrix.</returns>
        [NotNull]
        [PublicAPI]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MatrixF OneF(int rows, int columns)
        {
            return OneF(rows, columns, 1f);
        }

        /// <summary>
        ///     Generates a matrix where all components have the same initial value.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <param name="scale">Optional: The value for all components.</param>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="rows" /> and <see cref="columns" /> must be at least 1.</exception>
        /// <returns>Returns the matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF OneF(int rows, int columns, float scale)
        {
            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), rows, "At least one row is required.");
            }

            if (columns < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), columns, "At least one column is required.");
            }

            var result = GetF(rows, columns);

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    result.SetAt(i, j, scale);
                }
            }

            return result;
        }

        /// <summary>
        ///     Generates a zero matrix.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="rows" /> and <see cref="columns" /> must be at least 1.</exception>
        /// <returns>Returns the zero matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixD ZeroD(int rows, int columns)
        {
            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), rows, "At least one row is required.");
            }

            if (columns < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), columns, "At least one column is required.");
            }

            return GetD(rows, columns);
        }

        /// <summary>
        ///     Generates a zero matrix.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="rows" /> and <see cref="columns" /> must be at least 1.</exception>
        /// <returns>Returns the zero matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static MatrixF ZeroF(int rows, int columns)
        {
            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), rows, "At least one row is required.");
            }

            if (columns < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), columns, "At least one column is required.");
            }

            return GetF(rows, columns);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Returns an instance of the best suited matrix for the given dimension.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <returns>Returns the empty matrix.</returns>
        [NotNull]
        private static MatrixD GetD(int rows, int columns)
        {
            if (rows == 3 && columns == 3)
            {
                return new Matrix3x3d();
            }

            if (rows == 4 && columns == 4)
            {
                return new Matrix4x4d();
            }

            return new MatrixMxNd(rows, columns);
        }

        /// <summary>
        ///     Returns an instance of the best suited matrix for the given dimension.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <returns>Returns the empty matrix.</returns>
        [NotNull]
        private static MatrixF GetF(int rows, int columns)
        {
            if (rows == 3 && columns == 3)
            {
                return new Matrix3x3f();
            }

            if (rows == 4 && columns == 4)
            {
                return new Matrix4x4f();
            }

            return new MatrixMxNf(rows, columns);
        }

        #endregion
    }
}