// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Matrix3x3d.cs
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

namespace Unity_Tools.Core
{
    #region

    #endregion

    /// <summary>
    /// Class for a 3x3 matrix of <see cref="double"/> values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int) + 9 * sizeof(double))]
    // ReSharper disable once InconsistentNaming
    public sealed class Matrix3x3d : MatrixD
    {
        #region Fields

        [FieldOffset(2 * sizeof(int) + 0 * sizeof(double))]
        private double x00;

        [FieldOffset(2 * sizeof(int) + 1 * sizeof(double))]
        private double x01;

        [FieldOffset(2 * sizeof(int) + 2 * sizeof(double))]
        private double x02;

        [FieldOffset(2 * sizeof(int) + 3 * sizeof(double))]
        private double x10;

        [FieldOffset(2 * sizeof(int) + 4 * sizeof(double))]
        private double x11;

        [FieldOffset(2 * sizeof(int) + 5 * sizeof(double))]
        private double x12;

        [FieldOffset(2 * sizeof(int) + 6 * sizeof(double))]
        private double x20;

        [FieldOffset(2 * sizeof(int) + 7 * sizeof(double))]
        private double x21;

        [FieldOffset(2 * sizeof(int) + 8 * sizeof(double))]
        private double x22;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix3x3d" /> class.
        /// </summary>
        [PublicAPI]
        public Matrix3x3d()
            : base(3, 3)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix3x3d" /> class.
        /// </summary>
        [PublicAPI]
        public Matrix3x3d(
            double x00,
            double x01,
            double x02,
            double x10,
            double x11,
            double x12,
            double x20,
            double x21,
            double x22)
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3x3d"/> class.
        /// </summary>
        private Matrix3x3d([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.x00 = info.GetDouble("x00");
            this.x01 = info.GetDouble("x01");
            this.x02 = info.GetDouble("x02");

            this.x10 = info.GetDouble("x10");
            this.x11 = info.GetDouble("x11");
            this.x12 = info.GetDouble("x12");

            this.x20 = info.GetDouble("x20");
            this.x21 = info.GetDouble("x21");
            this.x22 = info.GetDouble("x22");
        }

        #endregion

        #region Properties and Indexers

        /// <summary>
        ///     Gets or sets the first column as vector [x00, x10, x20]
        /// </summary>
        [PublicAPI]
        public Vector3d Column0
        {
            get
            {
                return new Vector3d(this.x00, this.x10, this.x20);
            }
            set
            {
                this.x00 = value.X;
                this.x10 = value.Y;
                this.x20 = value.Z;
            }
        }

        /// <summary>
        ///     Gets or sets the second column as vector [x01, x11, x21]
        /// </summary>
        [PublicAPI]
        public Vector3d Column1
        {
            get
            {
                return new Vector3d(this.x01, this.x11, this.x21);
            }
            set
            {
                this.x01 = value.X;
                this.x11 = value.Y;
                this.x21 = value.Z;
            }
        }

        /// <summary>
        ///     Gets or sets the third column as vector [x02, x12, x22]
        /// </summary>
        [PublicAPI]
        public Vector3d Column2
        {
            get
            {
                return new Vector3d(this.x02, this.x12, this.x22);
            }
            set
            {
                this.x02 = value.X;
                this.x12 = value.Y;
                this.x22 = value.Z;
            }
        }

        /// <summary>
        ///     Gets or sets the first row as vector [x00, x01, x02]
        /// </summary>
        [PublicAPI]
        public Vector3d Row0
        {
            get
            {
                return new Vector3d(this.x00, this.x01, this.x02);
            }
            set
            {
                this.x00 = value.X;
                this.x01 = value.Y;
                this.X02 = value.Z;
            }
        }

        /// <summary>
        ///     Gets or sets the second row as vector [x10, x11, x12]
        /// </summary>
        [PublicAPI]
        public Vector3d Row1
        {
            get
            {
                return new Vector3d(this.x10, this.x11, this.x12);
            }
            set
            {
                this.x10 = value.X;
                this.x11 = value.Y;
                this.X12 = value.Z;
            }
        }

        /// <summary>
        ///     Gets or sets the third row as vector [x20, x21, x22]
        /// </summary>
        [PublicAPI]
        public Vector3d Row2
        {
            get
            {
                return new Vector3d(this.x20, this.x21, this.x22);
            }
            set
            {
                this.x20 = value.X;
                this.x21 = value.Y;
                this.X22 = value.Z;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 0 and column 0
        /// </summary>
        [PublicAPI]
        public double X00
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
        public double X01
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
        public double X02
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
        public double X10
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
        public double X11
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
        public double X12
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
        public double X20
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
        public double X21
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
        public double X22
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
            return new Matrix3x3d(
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
            var mat = obj as Matrix3x3d;
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
        public override MatrixD Multiply(double scale)
        {
            var result = new Matrix3x3d();

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

        public new Vector3d Row(int r)
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
                    return Vector3d.Zero;
            }
        }

        public new Vector3d Column(int c)
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
                    return Vector3d.Zero;
            }
        }

        /// <inheritdoc />
        [PublicAPI]
        public override void Scale(double scale)
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
        ///     Constructs a <see cref="Matrix3x3d" /> from the columns provided.
        /// </summary>
        /// <param name="column0">The first column.</param>
        /// <param name="column1">The second column.</param>
        /// <param name="column2">The third column.</param>
        /// <returns>Returns the matrix.</returns>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static Matrix3x3d FromColumns(Vector3d column0, Vector3d column1, Vector3d column2)
        {
            return new Matrix3x3d { Column0 = column0, Column1 = column1, Column2 = column2 };
        }

        /// <summary>
        ///     Constructs a <see cref="Matrix3x3d" /> from the rows provided.
        /// </summary>
        /// <param name="row0">The first row.</param>
        /// <param name="row1">The second row.</param>
        /// <param name="row2">The third row.</param>
        /// <returns>Returns the matrx.</returns>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static Matrix3x3d FromRows(Vector3d row0, Vector3d row1, Vector3d row2)
        {
            return new Matrix3x3d { Row0 = row0, Row1 = row1, Row2 = row2 };
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// Adds two <see cref="Matrix3x3d"/> instances.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3d operator +([NotNull] Matrix3x3d left, [NotNull] Matrix3x3d right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var result = new Matrix3x3d
                         {
                             x00 = left.x00 + right.x00,
                             x01 = left.x01 + right.x01,
                             x02 = left.x02 + right.x02,
                             x10 = left.x10 + right.x10,
                             x11 = left.x11 + right.x11,
                             x12 = left.x12 + right.x12,
                             x20 = left.x20 + right.x20,
                             x21 = left.x21 + right.x21,
                             x22 = left.x22 + right.x22
                         };

            return result;
        }

        /// <summary>
        /// Tests two <see cref="Matrix3x3d"/> for numerical equality.
        /// </summary>
        [PublicAPI]
        public static bool operator ==([CanBeNull] Matrix3x3d left, [CanBeNull] Matrix3x3d right)
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
        /// Converts the <see cref="Matrix3x3d"/> to a <see cref="Matrix3x3f"/>.
        /// </summary>
        /// <param name="matrix">The matrix to convert.</param>
        [CanBeNull]
        [PublicAPI]
        public static explicit operator MatrixF([CanBeNull] Matrix3x3d matrix)
        {
            return (Matrix3x3f)matrix;
        }

        /// <summary>
        /// Converts the <see cref="Matrix3x3d"/> to a <see cref="Matrix3x3f"/>.
        /// </summary>
        /// <param name="matrix">The matrix to convert.</param>
        [CanBeNull]
        [PublicAPI]
        public static explicit operator Matrix3x3f([CanBeNull] Matrix3x3d matrix)
        {
            if (matrix == null)
            {
                return null;
            }

            return new Matrix3x3f(
                (float)matrix.x00,
                (float)matrix.x01,
                (float)matrix.x02,
                (float)matrix.x10,
                (float)matrix.x11,
                (float)matrix.x12,
                (float)matrix.x20,
                (float)matrix.x21,
                (float)matrix.x22);
        }

        /// <summary>
        /// Tests two <see cref="Matrix3x3d"/> for numerical inequality.
        /// </summary>
        [PublicAPI]
        public static bool operator !=([CanBeNull] Matrix3x3d left, [CanBeNull] Matrix3x3d right)
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
        /// Multiplies two <see cref="Matrix3x3d"/> instances.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3d operator *([NotNull] Matrix3x3d left, [NotNull] Matrix3x3d right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var result = new Matrix3x3d();

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
        /// Multiplies a <see cref="Vector3d"/> with the <see cref="Matrix3x3d"/> and outputs the resulting column vector.
        /// </summary>
        [PublicAPI]
        public static Vector3d operator *(Vector3d left, [NotNull] Matrix3x3d right)
        {
            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            return new Vector3d(
                left.X * (right.x00 + right.x01 + right.x02),
                left.Y * (right.x10 + right.x11 + right.x12),
                left.Z * (right.x20 + right.x21 + right.x22));
        }

        /// <summary>
        /// Multiplies a <see cref="Matrix3x3d"/> with the <see cref="Vector3d"/> and outputs the resulting row vector.
        /// </summary>
        [PublicAPI]
        public static Vector3d operator *([NotNull] Matrix3x3d left, Vector3d right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            return new Vector3d(
                (left.x00 + left.x10 + left.x20) * right.X,
                (left.x01 + left.x11 + left.x21) * right.Y,
                (left.x02 + left.x12 + left.x22) * right.Z);
        }

        /// <summary>
        /// Scales the <see cref="Matrix3x3d"/> with the given <see cref="double"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3d operator *([NotNull] Matrix3x3d left, double right)
        {
            return (Matrix3x3d)left.Multiply(right);
        }

        /// <summary>
        /// Scales the <see cref="Matrix3x3d"/> with the given <see cref="double"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3d operator *(double left, [NotNull] Matrix3x3d right)
        {
            return (Matrix3x3d)right.Multiply(left);
        }

        /// <summary>
        /// Subtracts the right <see cref="Matrix3x3d"/> from the left one.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3d operator -([NotNull] Matrix3x3d left, [NotNull] Matrix3x3d right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var result = new Matrix3x3d
                         {
                             x00 = left.x00 - right.x00,
                             x01 = left.x01 - right.x01,
                             x02 = left.x02 - right.x02,
                             x10 = left.x10 - right.x10,
                             x11 = left.x11 - right.x11,
                             x12 = left.x12 - right.x12,
                             x20 = left.x20 - right.x20,
                             x21 = left.x21 - right.x21,
                             x22 = left.x22 - right.x22
                         };

            return result;
        }

        /// <summary>
        /// Negates the <see cref="Matrix3x3d"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix3x3d operator -([NotNull] Matrix3x3d matrix)
        {
            return new Matrix3x3d(
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
        protected internal override double GetAt(int row, int column)
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
                            return 0d;
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
                            return 0d;
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
                            return 0d;
                    }
                default:
                    return 0d;
            }
        }

        /// <inheritdoc />
        protected internal override void SetAt(int row, int column, double value)
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