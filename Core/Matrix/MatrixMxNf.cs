// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         MatrixMxNf.cs
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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace Unity_Tools.Core
{
    #region

    #endregion

    /// <summary>
    /// Class for custom sized matrices using <see cref="float"/> values.
    /// </summary>
    [Serializable]
    public sealed class MatrixMxNf : MatrixF
    {
        #region Fields

        [NotNull]
        private readonly float[] data;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var builder = new StringBuilder();

            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    builder.Append(this.GetAt(i, j).ToString(CultureInfo.InvariantCulture));
                    if (j < this.Columns - 1)
                    {
                        builder.Append(' ');
                    }
                }

                if (i < this.Rows - 1)
                {
                    builder.Append(';');
                }
            }

            info.AddValue("data", builder.ToString());
        }

        #endregion

        #region Constructors

        /// <inheritdoc />
        [PublicAPI]
        public MatrixMxNf(int rows, int columns)
            : base(rows, columns)
        {
            this.ValidateRowAmount(rows);
            this.ValidateColumnAmount(columns);

            this.data = new float[rows * columns];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MatrixMxNf" /> class.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <param name="data">The data, it will be copied.</param>
        [PublicAPI]
        public MatrixMxNf(int rows, int columns, [NotNull] IList<float> data)
            : base(rows, columns)
        {
            this.ValidateRowAmount(rows);
            this.ValidateColumnAmount(columns);

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "The matrix data cannot be null.");
            }

            if (data.Count != rows * columns)
            {
                throw new ArgumentException("The data length must be equal to rows*columns.");
            }

            this.data = new float[rows * columns];

            for (var i = 0; i < rows * columns; i++)
            {
                this.data[i] = data[i];
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MatrixMxNf" /> class.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <param name="data">The data, will not be copied.</param>
        [PublicAPI]
        public MatrixMxNf(int rows, int columns, [NotNull] params float[] data)
            : base(rows, columns)
        {
            this.ValidateRowAmount(rows);
            this.ValidateColumnAmount(columns);

            if (data.Length != rows * columns)
            {
                throw new ArgumentException("The data length must be equal to rows*columns.");
            }

            this.data = data;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MatrixMxNf" /> class.
        /// </summary>
        /// <param name="rows">The amount of rows.</param>
        /// <param name="columns">The amount of columns.</param>
        /// <param name="data">The data, will not be copied.</param>
        [PublicAPI]
        public MatrixMxNf(int rows, int columns, [NotNull] ref float[] data)
            : base(rows, columns)
        {
            this.ValidateRowAmount(rows);
            this.ValidateColumnAmount(columns);

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "The matrix data cannot be null.");
            }

            if (data.Length != rows * columns)
            {
                throw new ArgumentException("The data length must be equal to rows*columns.");
            }

            this.data = data;
        }

        private MatrixMxNf([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.data = new float[this.Rows * this.Columns];

            var newData = (string)info.GetValue("data", typeof(string));
            if (newData == null)
            {
                return;
            }

            var columns = newData.Split(';');

            if (columns == null || columns.Length != this.Columns)
            {
                throw new SerializationException("The amount of columns mismatch the stored amount of columns.");
            }

            for (var j = 0; j < this.Columns; j++)
            {
                var column = columns[j];
                if (column == null)
                {
                    throw new SerializationException("One of the columns was empty.");
                }

                var rows = column.Split(' ');

                if (rows.Length != this.Rows)
                {
                    throw new SerializationException(
                        $"The amount of rows mismatch the stored amount of rows on column {j}.");
                }

                for (var i = 0; i < this.Rows; i++)
                {
                    if (float.TryParse(rows[i], NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                    {
                        this.SetAt(i, j, value);
                    }
                    else
                    {
                        throw new SerializationException(
                            "The value at row {i} and column {j} could not be converted to a double.");
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        protected internal override float GetAt(int row, int column)
        {
            return this.data[row * this.Columns + column];
        }

        protected internal override void SetAt(int row, int column, float value)
        {
            this.data[row * this.Columns + column] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateColumnAmount(int columns)
        {
            if (columns <= 0)
            {
                throw new ArgumentException("A matrix needs at least one column.", nameof(columns));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateRowAmount(int rows)
        {
            if (rows <= 0)
            {
                throw new ArgumentException("A matrix needs at least one row.", nameof(rows));
            }
        }

        #endregion
    }
}