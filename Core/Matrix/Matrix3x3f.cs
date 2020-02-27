// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Matrix3x3f.cs
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
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityTools.Core
{
    #region

    #endregion

    /// <summary>
    /// Class for a 3x3 matrix of <see cref="float"/> values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int) + 9 * sizeof(float))]
    // ReSharper disable once InconsistentNaming
    public sealed class Matrix3x3f : MatrixF
    {
        #region Fields

        [FieldOffset(2 * sizeof(int) + 0 * sizeof(float))]
        private float x00;

        [FieldOffset(2 * sizeof(int) + 1 * sizeof(float))]
        private float x01;

        [FieldOffset(2 * sizeof(int) + 2 * sizeof(float))]
        private float x02;

        [FieldOffset(2 * sizeof(int) + 3 * sizeof(float))]
        private float x10;

        [FieldOffset(2 * sizeof(int) + 4 * sizeof(float))]
        private float x11;

        [FieldOffset(2 * sizeof(int) + 5 * sizeof(float))]
        private float x12;

        [FieldOffset(2 * sizeof(int) + 6 * sizeof(float))]
        private float x20;

        [FieldOffset(2 * sizeof(int) + 7 * sizeof(float))]
        private float x21;

        [FieldOffset(2 * sizeof(int) + 8 * sizeof(float))]
        private float x22;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix3x3f" /> class.
        /// </summary>
        [PublicAPI]
        public Matrix3x3f()
            : base(3, 3)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix3x3f" /> class.
        /// </summary>
        [PublicAPI]
        public Matrix3x3f(
            float x00,
            float x01,
            float x02,
            float x10,
            float x11,
            float x12,
            float x20,
            float x21,
            float x22)
            : base(3, 3)
        {
            this.x00 = x00;
            this.x01 = x01;
            this.x02 = x02;
            this.x10 = x10;
            this.x11 = x11;
            this.x12 = x12;
            this.x20 = x20;
            this.x21 = x21;
            this.x22 = x22;
        }

        private Matrix3x3f([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.x00 = info.GetSingle("x00");
            this.x01 = info.GetSingle("x01");
            this.x02 = info.GetSingle("x02");

            this.x10 = info.GetSingle("x10");
            this.x11 = info.GetSingle("x11");
            this.x12 = info.GetSingle("x12");

            this.x20 = info.GetSingle("x20");
            this.x21 = info.GetSingle("x21");
            this.x22 = info.GetSingle("x22");
        }

        #endregion

        #region Properties and Indexers

        /// <summary>
        ///     Gets or sets the first column as vector [x00, x10, x20]
        /// </summary>
        [PublicAPI]
        public Vector3 Column0
        {
            get
            {
                return new Vector3(this.x00, this.x10, this.x20);
            }
            set
            {
                this.x00 = value.x;
                this.x10 = value.y;
                this.x20 = value.z;
            }
        }

        /// <summary>
        ///     Gets or sets the second column as vector [x01, x11, x21]
        /// </summary>
        [PublicAPI]
        public Vector3 Column1
        {
            get
            {
                return new Vector3(this.x01, this.x11, this.x21);
            }
            set
            {
                this.x01 = value.x;
                this.x11 = value.y;
                this.x21 = value.z;
            }
        }

        /// <summary>
        ///     Gets or sets the third column as vector [x02, x12, x22]
        /// </summary>
        [PublicAPI]
        public Vector3 Column2
        {
            get
            {
                return new Vector3(this.x02, this.x12, this.x22);
            }
            set
            {
                this.x02 = value.x;
                this.x12 = value.y;
                this.x22 = value.z;
            }
        }

        /// <summary>
        ///     Gets or sets the first row as vector [x00, x01, x02]
        /// </summary>
        [PublicAPI]
        public Vector3 Row0
        {
            get
            {
                return new Vector3(this.x00, this.x01, this.x02);
            }
            set
            {
                this.x00 = value.x;
                this.x01 = value.y;
                this.X02 = value.z;
            }
        }

        /// <summary>
        ///     Gets or sets the second row as vector [x10, x11, x12]
        /// </summary>
        [PublicAPI]
        public Vector3 Row1
        {
            get
            {
                return new Vector3(this.x10, this.x11, this.x12);
            }
            set
            {
                this.x10 = value.x;
                this.x11 = value.y;
                this.X12 = value.z;
            }
        }

        /// <summary>
        ///     Gets or sets the third row as vector [x20, x21, x22]
        /// </summary>
        [PublicAPI]
        public Vector3 Row2
        {
            get
            {
                return new Vector3(this.x20, this.x21, this.x22);
            }
            set
            {
                this.x20 = value.x;
                this.x21 = value.y;
                this.X22 = value.z;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 0 and column 0
        /// </summary>
        [PublicAPI]
        public float X00
        {
            get
            {
                return this.x00;
            }

            set
            {
                this.x00 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 0 and column 1
        /// </summary>
        [PublicAPI]
        public float X01
        {
            get
            {
                return this.x01;
            }

            set
            {
                this.x01 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 0 and column 2
        /// </summary>
        [PublicAPI]
        public float X02
        {
            get
            {
                return this.x02;
            }

            set
            {
                this.x02 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 1 and column 0
        /// </summary>
        [PublicAPI]
        public float X10
        {
            get
            {
                return this.x10;
            }

            set
            {
                this.x10 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 1 and column 1
        /// </summary>
        [PublicAPI]
        public float X11
        {
            get
            {
                return this.x11;
            }

            set
            {
                this.x11 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 1 and column 2
        /// </summary>
        [PublicAPI]
        public float X12
        {
            get
            {
                return this.x12;
            }

            set
            {
                this.x12 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 2 and column 0
        /// </summary>
        [PublicAPI]
        public float X20
        {
            get
            {
                return this.x20;
            }

            set
            {
                this.x20 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 2 and column 1
        /// </summary>
        [PublicAPI]
        public float X21
        {
            get
            {
                return this.x21;
            }

            set
            {
                this.x21 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 2 and column 2
        /// </summary>
        [PublicAPI]
        public float X22
        {
            get
            {
                return this.x22;
            }

            set
            {
                this.x22 = value;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        [PublicAPI]
        public override object Clone()
        {
            return new Matrix3x3f(
                this.x00,
                this.x01,
                this.x02,
                this.x10,
                this.x11,
                this.x12,
                this.x20,
                this.x21,
                this.x22);
        }

        /// <inheritdoc />
        [PublicAPI]
        public override bool Equals(object obj)
        {
            var mat = obj as Matrix3x3f;
            if (mat == null)
            {
                return base.Equals(obj);
            }

            return this == mat;
        }

        /// <inheritdoc />
        [PublicAPI]
        public override int GetHashCode()
        {
            return CommonUtil.HashCode(this.Row0, this.Row1, this.Row2);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("x00", this.x00);
            info.AddValue("x01", this.x01);
            info.AddValue("x02", this.x02);

            info.AddValue("x10", this.x10);
            info.AddValue("x11", this.x11);
            info.AddValue("x12", this.x12);

            info.AddValue("x20", this.x20);
            info.AddValue("x21", this.x21);
            info.AddValue("x22", this.x22);
        }

        /// <inheritdoc />
        [PublicAPI]
        public override MatrixF Multiply(float scale)
        {
            var result = new Matrix3x3f();

            result.x00 = this.x00 * scale;
            result.x01 = this.x01 * scale;
            result.x02 = this.x02 * scale;

            result.x10 = this.x10 * scale;
            result.x11 = this.x11 * scale;
            result.x12 = this.x12 * scale;

            result.x20 = this.x20 * scale;
            result.x21 = this.x21 * scale;
            result.x22 = this.x22 * scale;

            return result;
        }

        public new Vector3 Row(int r)
        {
            if (r < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(r), "The row index must be positive.");
            }

            switch (r)
            {
                case 0:
                    return Row0;
                case 1:
                    return Row1;
                case 2:
                    return Row2;
                default:
                    return Vector3.zero;
            }
        }

        public new Vector3 Column(int c)
        {
            if (c < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(c), "The column index must be positive.");
            }

            switch (c)
            {
                case 0:
                    return Column0;
                case 1:
                    return Column1;
                case 2:
                    return Column2;
                default:
                    return Vector3.zero;
            }
        }

        /// <inheritdoc />
        [PublicAPI]
        public override void Scale(float scale)
        {
            this.x00 *= scale;
            this.x01 *= scale;
            this.x02 *= scale;

            this.x10 *= scale;
            this.x11 *= scale;
            this.x12 *= scale;

            this.x20 *= scale;
            this.x21 *= scale;
            this.x22 *= scale;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        ///     Constructs a <see cref="Matrix3x3f" /> from the columns provided.
        /// </summary>
        /// <param name="column0">The first column.</param>
        /// <param name="column1">The second column.</param>
        /// <param name="column2">The third column.</param>
        /// <returns>Returns the matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3f FromColumns(Vector3 column0, Vector3 column1, Vector3 column2)
        {
            return new Matrix3x3f { Column0 = column0, Column1 = column1, Column2 = column2 };
        }

        /// <summary>
        ///     Constructs a <see cref="Matrix3x3f" /> from the rows provided.
        /// </summary>
        /// <param name="row0">The first row.</param>
        /// <param name="row1">The second row.</param>
        /// <param name="row2">The third row.</param>
        /// <returns>Returns the matrx.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3f FromRows(Vector3 row0, Vector3 row1, Vector3 row2)
        {
            return new Matrix3x3f { Row0 = row0, Row1 = row1, Row2 = row2 };
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// Adds two <see cref="Matrix3x3f"/> instances.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3f operator +([NotNull] Matrix3x3f left, [NotNull] Matrix3x3f right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var result = new Matrix3x3f();

            result.x00 = left.x00 + right.x00;
            result.x01 = left.x01 + right.x01;
            result.x02 = left.x02 + right.x02;

            result.x10 = left.x10 + right.x10;
            result.x11 = left.x11 + right.x11;
            result.x12 = left.x12 + right.x12;

            result.x20 = left.x20 + right.x20;
            result.x21 = left.x21 + right.x21;
            result.x22 = left.x22 + right.x22;

            return result;
        }

        /// <summary>
        /// Tests two <see cref="Matrix3x3f"/> for numerical equality.
        /// </summary>
        [PublicAPI]
        public static bool operator ==([CanBeNull] Matrix3x3f left, [CanBeNull] Matrix3x3f right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return CommonUtil.Equals(left.x00, right.x00) && CommonUtil.Equals(left.x01, right.x01)
                   && CommonUtil.Equals(left.x02, right.x02) && CommonUtil.Equals(left.x10, right.x10)
                   && CommonUtil.Equals(left.x11, right.x11) && CommonUtil.Equals(left.x12, right.x12)
                   && CommonUtil.Equals(left.x20, right.x20) && CommonUtil.Equals(left.x21, right.x21)
                   && CommonUtil.Equals(left.x22, right.x22);
        }

        /// <summary>
        /// Converts the <see cref="Matrix3x3f"/> to a <see cref="Matrix3x3d"/>.
        /// </summary>
        /// <param name="matrix">The matrix to convert.</param>
        [CanBeNull]
        [PublicAPI]
        public static implicit operator MatrixD([CanBeNull] Matrix3x3f matrix)
        {
            return (Matrix3x3d)matrix;
        }

        /// <summary>
        /// Converts the <see cref="Matrix3x3f"/> to a <see cref="Matrix3x3d"/>.
        /// </summary>
        /// <param name="matrix">The matrix to convert.</param>
        [CanBeNull]
        [PublicAPI]
        public static implicit operator Matrix3x3d([CanBeNull] Matrix3x3f matrix)
        {
            if (matrix == null)
            {
                return null;
            }

            return new Matrix3x3d(
                matrix.x00,
                matrix.x01,
                matrix.x02,
                matrix.x10,
                matrix.x11,
                matrix.x12,
                matrix.x20,
                matrix.x21,
                matrix.x22);
        }

        /// <summary>
        /// Tests two <see cref="Matrix3x3d"/> for numerical inequality.
        /// </summary>
        [PublicAPI]
        public static bool operator !=([CanBeNull] Matrix3x3f left, [CanBeNull] Matrix3x3f right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return true;
            }

            return !CommonUtil.Equals(left.x00, right.x00) || !CommonUtil.Equals(left.x01, right.x01)
                   || !CommonUtil.Equals(left.x02, right.x02) || !CommonUtil.Equals(left.x10, right.x10)
                   || !CommonUtil.Equals(left.x11, right.x11) || !CommonUtil.Equals(left.x12, right.x12)
                   || !CommonUtil.Equals(left.x20, right.x20) || !CommonUtil.Equals(left.x21, right.x21)
                   || !CommonUtil.Equals(left.x22, right.x22);
        }

        /// <summary>
        /// Multiplies two <see cref="Matrix3x3f"/> instances.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3f operator *([NotNull] Matrix3x3f left, [NotNull] Matrix3x3f right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var result = new Matrix3x3f();

            result.x00 = left.x00 * right.x00 + left.x01 * right.x10 + left.x02 * right.x20;
            result.x01 = left.x00 * right.x01 + left.x01 * right.x11 + left.x02 * right.x21;
            result.x02 = left.x00 * right.x02 + left.x01 * right.x12 + left.x02 * right.x22;

            result.x10 = left.x10 * right.x00 + left.x11 * right.x10 + left.x12 * right.x20;
            result.x11 = left.x10 * right.x01 + left.x11 * right.x11 + left.x12 * right.x21;
            result.x12 = left.x10 * right.x02 + left.x11 * right.x12 + left.x12 * right.x22;

            result.x20 = left.x20 * right.x00 + left.x21 * right.x10 + left.x22 * right.x20;
            result.x21 = left.x20 * right.x01 + left.x21 * right.x11 + left.x22 * right.x21;
            result.x22 = left.x20 * right.x02 + left.x21 * right.x12 + left.x22 * right.x22;

            return result;
        }

        /// <summary>
        /// Multiplies a <see cref="Vector3f"/> with the <see cref="Matrix3x3f"/> and outputs the resulting column vector.
        /// </summary>
        [PublicAPI]
        public static Vector3 operator *(Vector3 left, [NotNull] Matrix3x3f right)
        {
            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return new Vector3(
                left.x * (right.x00 + right.x01 + right.x02),
                left.y * (right.x10 + right.x11 + right.x12),
                left.z * (right.x20 + right.x21 + right.x22));
        }

        /// <summary>
        /// Multiplies a <see cref="Matrix3x3f"/> with the <see cref="Vector3f"/> and outputs the resulting row vector.
        /// </summary>
        [PublicAPI]
        public static Vector3 operator *([NotNull] Matrix3x3f left, Vector3 right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            return new Vector3(
                (left.x00 + left.x10 + left.x20) * right.x,
                (left.x01 + left.x11 + left.x21) * right.y,
                (left.x02 + left.x12 + left.x22) * right.z);
        }

        /// <summary>
        /// Scales the <see cref="Matrix3x3f"/> with the given <see cref="float"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3f operator *([NotNull] Matrix3x3f left, float right)
        {
            return (Matrix3x3f)left.Multiply(right);
        }

        /// <summary>
        /// Scales the <see cref="Matrix3x3f"/> with the given <see cref="float"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3f operator *(float left, [NotNull] Matrix3x3f right)
        {
            return (Matrix3x3f)right.Multiply(left);
        }

        /// <summary>
        /// Subtracts the right <see cref="Matrix3x3f"/> from the left one.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3f operator -([NotNull] Matrix3x3f left, [NotNull] Matrix3x3f right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var result = new Matrix3x3f();

            result.x00 = left.x00 - right.x00;
            result.x01 = left.x01 - right.x01;
            result.x02 = left.x02 - right.x02;

            result.x10 = left.x10 - right.x10;
            result.x11 = left.x11 - right.x11;
            result.x12 = left.x12 - right.x12;

            result.x20 = left.x20 - right.x20;
            result.x21 = left.x21 - right.x21;
            result.x22 = left.x22 - right.x22;

            return result;
        }

        /// <summary>
        /// Negates the <see cref="Matrix3x3f"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3f operator -([NotNull] Matrix3x3f matrix)
        {
            return new Matrix3x3f(
                -matrix.x00,
                -matrix.x01,
                -matrix.x02,
                -matrix.x10,
                -matrix.x11,
                -matrix.x12,
                -matrix.x20,
                -matrix.x21,
                -matrix.x22);
        }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected internal override float GetAt(int row, int column)
        {
            switch (row)
            {
                case 0:
                    switch (column)
                    {
                        case 0:
                            return x00;
                        case 1:
                            return x01;
                        case 2:
                            return x02;
                        default:
                            return 0f;
                    }
                case 1:
                    switch (column)
                    {
                        case 0:
                            return x10;
                        case 1:
                            return x11;
                        case 2:
                            return x12;
                        default:
                            return 0f;
                    }
                case 2:
                    switch (column)
                    {
                        case 0:
                            return x20;
                        case 1:
                            return x21;
                        case 2:
                            return x22;
                        default:
                            return 0f;
                    }
                default:
                    return 0f;
            }
        }

        /// <inheritdoc />
        protected internal override void SetAt(int row, int column, float value)
        {
            switch (row)
            {
                case 0:
                    switch (column)
                    {
                        case 0:
                            x00 = value;
                            break;
                        case 1:
                            x01 = value;
                            break;
                        case 2:
                            x02 = value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(column));
                    }

                    break;
                case 1:
                    switch (column)
                    {
                        case 0:
                            x10 = value;
                            break;
                        case 1:
                            x11 = value;
                            break;
                        case 2:
                            x12 = value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(column));
                    }

                    break;
                case 2:
                    switch (column)
                    {
                        case 0:
                            x20 = value;
                            break;
                        case 1:
                            x21 = value;
                            break;
                        case 2:
                            x22 = value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(column));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(row));
            }
        }

        #endregion
    }
}