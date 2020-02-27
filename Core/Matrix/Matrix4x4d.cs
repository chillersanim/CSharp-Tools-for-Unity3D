// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Matrix4x4d.cs
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

namespace UnityTools.Core
{
    #region

    #endregion

    /// <summary>
    /// Class for a 4x4 matrix of <see cref="double"/> values.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 2 * sizeof(int) + 16 * sizeof(double))]
    // ReSharper disable once InconsistentNaming
    public sealed class Matrix4x4d : MatrixD
    {
        #region Fields

        [FieldOffset(2 * sizeof(int) + 0 * sizeof(double))]
        private double x00;

        [FieldOffset(2 * sizeof(int) + 1 * sizeof(double))]
        private double x01;

        [FieldOffset(2 * sizeof(int) + 2 * sizeof(double))]
        private double x02;

        [FieldOffset(2 * sizeof(int) + 3 * sizeof(double))]
        private double x03;

        [FieldOffset(2 * sizeof(int) + 4 * sizeof(double))]
        private double x10;

        [FieldOffset(2 * sizeof(int) + 5 * sizeof(double))]
        private double x11;

        [FieldOffset(2 * sizeof(int) + 6 * sizeof(double))]
        private double x12;

        [FieldOffset(2 * sizeof(int) + 7 * sizeof(double))]
        private double x13;

        [FieldOffset(2 * sizeof(int) + 8 * sizeof(double))]
        private double x20;

        [FieldOffset(2 * sizeof(int) + 9 * sizeof(double))]
        private double x21;

        [FieldOffset(2 * sizeof(int) + 10 * sizeof(double))]
        private double x22;

        [FieldOffset(2 * sizeof(int) + 11 * sizeof(double))]
        private double x23;

        [FieldOffset(2 * sizeof(int) + 12 * sizeof(double))]
        private double x30;

        [FieldOffset(2 * sizeof(int) + 13 * sizeof(double))]
        private double x31;

        [FieldOffset(2 * sizeof(int) + 14 * sizeof(double))]
        private double x32;

        [FieldOffset(2 * sizeof(int) + 15 * sizeof(double))]
        private double x33;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix4x4f" /> class.
        /// </summary>
        [PublicAPI]
        public Matrix4x4d()
            : base(4, 4)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Matrix4x4f" /> class.
        /// </summary>
        [PublicAPI]
        public Matrix4x4d(
            double x00,
            double x01,
            double x02,
            double x03,
            double x10,
            double x11,
            double x12,
            double x13,
            double x20,
            double x21,
            double x22,
            double x23,
            double x30,
            double x31,
            double x32,
            double x33)
            : base(4, 4)
        {
            this.x00 = x00;
            this.x01 = x01;
            this.x02 = x02;
            this.x03 = x03;
            this.x10 = x10;
            this.x11 = x11;
            this.x12 = x12;
            this.x13 = x13;
            this.x20 = x20;
            this.x21 = x21;
            this.x22 = x22;
            this.x23 = x23;
            this.x30 = x30;
            this.x31 = x31;
            this.x32 = x32;
            this.x33 = x33;
        }

        private Matrix4x4d([NotNull] SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.x00 = info.GetDouble("x00");
            this.x01 = info.GetDouble("x01");
            this.x02 = info.GetDouble("x02");
            this.x03 = info.GetDouble("x03");

            this.x10 = info.GetDouble("x10");
            this.x11 = info.GetDouble("x11");
            this.x12 = info.GetDouble("x12");
            this.x13 = info.GetDouble("x13");

            this.x20 = info.GetDouble("x20");
            this.x21 = info.GetDouble("x21");
            this.x22 = info.GetDouble("x22");
            this.x23 = info.GetDouble("x23");

            this.x30 = info.GetDouble("x30");
            this.x31 = info.GetDouble("x31");
            this.x32 = info.GetDouble("x32");
            this.x33 = info.GetDouble("x33");
        }

        #endregion

        #region Properties and Indexers

        /// <summary>
        ///     Gets or sets the first column as vector [x00, x10, x20, x30]
        /// </summary>
        [PublicAPI]
        public Vector4d Column0
        {
            get
            {
                return new Vector4d(this.x00, this.x10, this.x20, this.x30);
            }
            set
            {
                this.x00 = value.X;
                this.x10 = value.Y;
                this.x20 = value.Z;
                this.x30 = value.W;
            }
        }

        /// <summary>
        ///     Gets or sets the second column as vector [x01, x11, x21, x31]
        /// </summary>
        [PublicAPI]
        public Vector4d Column1
        {
            get
            {
                return new Vector4d(this.x01, this.x11, this.x21, this.x31);
            }
            set
            {
                this.x01 = value.X;
                this.x11 = value.Y;
                this.x21 = value.Z;
                this.x31 = value.W;
            }
        }

        /// <summary>
        ///     Gets or sets the third column as vector [x02, x12, x22, x32]
        /// </summary>
        [PublicAPI]
        public Vector4d Column2
        {
            get
            {
                return new Vector4d(this.x02, this.x12, this.x22, this.x32);
            }
            set
            {
                this.x02 = value.X;
                this.x12 = value.Y;
                this.x22 = value.Z;
                this.x32 = value.W;
            }
        }

        /// <summary>
        ///     Gets or sets the fourth column as vector [x03, x13, x23, x33]
        /// </summary>
        [PublicAPI]
        public Vector4d Column3
        {
            get
            {
                return new Vector4d(this.x03, this.x13, this.x23, this.x33);
            }
            set
            {
                this.x03 = value.X;
                this.x13 = value.Y;
                this.x23 = value.Z;
                this.x33 = value.W;
            }
        }

        /// <summary>
        ///     Gets or sets the first row as vector [x00, x01, x02, x03]
        /// </summary>
        [PublicAPI]
        public Vector4d Row0
        {
            get
            {
                return new Vector4d(this.x00, this.x01, this.x02, this.x03);
            }
            set
            {
                this.x00 = value.X;
                this.x01 = value.Y;
                this.X02 = value.Z;
                this.x03 = value.W;
            }
        }

        /// <summary>
        ///     Gets or sets the second row as vector [x10, x11, x12, x13]
        /// </summary>
        [PublicAPI]
        public Vector4d Row1
        {
            get
            {
                return new Vector4d(this.x10, this.x11, this.x12, this.x13);
            }
            set
            {
                this.x10 = value.X;
                this.x11 = value.Y;
                this.X12 = value.Z;
                this.x13 = value.W;
            }
        }

        /// <summary>
        ///     Gets or sets the third row as vector [x20, x21, x22, x23]
        /// </summary>
        [PublicAPI]
        public Vector4d Row2
        {
            get
            {
                return new Vector4d(this.x20, this.x21, this.x22, this.x23);
            }
            set
            {
                this.x20 = value.X;
                this.x21 = value.Y;
                this.X22 = value.Z;
                this.x23 = value.W;
            }
        }

        /// <summary>
        ///     Gets or sets the fourth row as vector [x30, x31, x32, x33]
        /// </summary>
        [PublicAPI]
        public Vector4d Row3
        {
            get
            {
                return new Vector4d(this.x30, this.x31, this.x32, this.x33);
            }
            set
            {
                this.x30 = value.X;
                this.x31 = value.Y;
                this.X32 = value.Z;
                this.x33 = value.W;
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
        ///     Gets or sets the value from row 0 and column 3
        /// </summary>
        [PublicAPI]
        public double X03
        {
            get
            {
                return this.x03;
            }

            set
            {
                this.x03 = value;
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
        ///     Gets or sets the value from row 1 and column 3
        /// </summary>
        [PublicAPI]
        public double X13
        {
            get
            {
                return this.x13;
            }

            set
            {
                this.x13 = value;
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

        /// <summary>
        ///     Gets or sets the value from row 2 and column 3
        /// </summary>
        [PublicAPI]
        public double X23
        {
            get
            {
                return this.x23;
            }

            set
            {
                this.x23 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 3 and column 0
        /// </summary>
        [PublicAPI]
        public double X30
        {
            get
            {
                return this.x30;
            }

            set
            {
                this.x30 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 3 and column 1
        /// </summary>
        [PublicAPI]
        public double X31
        {
            get
            {
                return this.x31;
            }

            set
            {
                this.x31 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 3 and column 2
        /// </summary>
        [PublicAPI]
        public double X32
        {
            get
            {
                return this.x32;
            }

            set
            {
                this.x32 = value;
            }
        }

        /// <summary>
        ///     Gets or sets the value from row 3 and column 3
        /// </summary>
        [PublicAPI]
        public double X33
        {
            get
            {
                return this.x33;
            }

            set
            {
                this.x33 = value;
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        [PublicAPI]
        public override object Clone()
        {
            return new Matrix4x4d(
                this.x00,
                this.x01,
                this.x02,
                this.x03,
                this.x10,
                this.x11,
                this.x12,
                this.x13,
                this.x20,
                this.x21,
                this.x22,
                this.x23,
                this.x30,
                this.x31,
                this.x32,
                this.x33);
        }

        /// <inheritdoc />
        [PublicAPI]
        public override bool Equals(object obj)
        {
            var mat = obj as Matrix4x4d;
            if (mat == null) return base.Equals(obj);

            return this == mat;
        }

        /// <inheritdoc />
        [PublicAPI]
        public override int GetHashCode()
        {
            return CommonUtil.HashCode(this.Row0, this.Row1, this.Row2, this.Row3);
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("x00", this.x00);
            info.AddValue("x01", this.x01);
            info.AddValue("x02", this.x02);
            info.AddValue("x03", this.x03);

            info.AddValue("x10", this.x10);
            info.AddValue("x11", this.x11);
            info.AddValue("x12", this.x12);
            info.AddValue("x13", this.x13);

            info.AddValue("x20", this.x20);
            info.AddValue("x21", this.x21);
            info.AddValue("x22", this.x22);
            info.AddValue("x23", this.x23);

            info.AddValue("x30", this.x30);
            info.AddValue("x31", this.x31);
            info.AddValue("x32", this.x32);
            info.AddValue("x33", this.x33);
        }

        /// <summary>
        ///     Inverts the matrix and returns the result as a new matrix.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     An <see cref="InvalidOperationException" /> is thrown when the matrix
        ///     cannot be inverted.
        /// </exception>
        /// <returns>Returns the inverse of this matrix.</returns>
        [NotNull]
        [PublicAPI]
        public MatrixD Invert()
        {
            var result = (Matrix4x4d)this.Clone();
            var indices1 = new int[4];
            var indices2 = new int[4];
            var indices3 = new[] { -1, -1, -1, -1 };

            var a = 0;
            var b = 0;
            for (var i = 0; i < 4; ++i)
            {
                var value = 0.0;
                for (var j = 0; j < 4; ++j)
                {
                    if (indices3[j] == 0) continue;

                    for (var k = 0; k < 4; ++k)
                        if (indices3[k] == -1)
                        {
                            var num2 = Math.Abs(result.GetAt(j, k));
                            if (num2 <= value) continue;

                            value = num2;
                            b = j;
                            a = k;
                        }
                        else if (indices3[k] > 0)
                        {
                            return result;
                        }
                }

                ++indices3[a];
                if (b != a)
                    for (var j = 0; j < 4; ++j)
                    {
                        var num2 = result.GetAt(b, j);
                        result.SetAt(b, j, result.GetAt(a, j));
                        result.SetAt(a, j, num2);
                    }

                indices2[i] = b;
                indices1[i] = a;

                var temp = result.GetAt(a, a);
                if (CommonUtil.Equals(temp, 0.0))
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");

                var invTemp = 1.0 / temp;
                result.SetAt(a, a, 1.0);

                for (var j = 0; j < 4; ++j) result[a, j] *= invTemp;

                for (var j = 0; j < 4; ++j)
                {
                    if (a == j) continue;

                    var scalar = result.GetAt(j, a);
                    result.SetAt(j, a, 0.0);
                    for (var k = 0; k < 4; ++k) result[j, k] -= result.GetAt(a, k) * scalar;
                }
            }

            for (var i = 3; i >= 0; --i)
            {
                var switchInd = indices2[i];
                var otherInd = indices1[i];
                for (var j = 0; j < 4; ++j)
                {
                    var num = result.GetAt(j, switchInd);
                    result.SetAt(j, switchInd, result.GetAt(j, otherInd));
                    result.SetAt(j, otherInd, num);
                }
            }

            return result;
        }

        /// <inheritdoc />
        [PublicAPI]
        public override MatrixD Multiply(double scale)
        {
            var result = new Matrix4x4d
                         {
                             x00 = this.x00 * scale,
                             x01 = this.x01 * scale,
                             x02 = this.x02 * scale,
                             x03 = this.x03 * scale,
                             x10 = this.x10 * scale,
                             x11 = this.x11 * scale,
                             x12 = this.x12 * scale,
                             x13 = this.x13 * scale,
                             x20 = this.x20 * scale,
                             x21 = this.x21 * scale,
                             x22 = this.x22 * scale,
                             x23 = this.x23 * scale,
                             x30 = this.x30 * scale,
                             x31 = this.x31 * scale,
                             x32 = this.x32 * scale,
                             x33 = this.x33 * scale
                         };

            return result;
        }

        public new Vector4d Row(int r)
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
                case 3:
                    return Row3;
                default:
                    return Vector3d.Zero;
            }
        }

        public new Vector4d Column(int c)
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
                case 3:
                    return Column3;
                default:
                    return Vector3d.Zero;
            }
        }

        /// <inheritdoc />
        public override void Scale(double scale)
        {
            this.x00 *= scale;
            this.x01 *= scale;
            this.x02 *= scale;
            this.x03 *= scale;

            this.x10 *= scale;
            this.x11 *= scale;
            this.x12 *= scale;
            this.x13 *= scale;

            this.x20 *= scale;
            this.x21 *= scale;
            this.x22 *= scale;
            this.x23 *= scale;

            this.x30 *= scale;
            this.x31 *= scale;
            this.x32 *= scale;
            this.x33 *= scale;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        ///     Constructs a <see cref="Matrix4x4d" /> from the columns provided.
        /// </summary>
        /// <param name="column0">The first column.</param>
        /// <param name="column1">The second column.</param>
        /// <param name="column2">The third column.</param>
        /// <param name="column3">The fourth column.</param>
        /// <returns>Returns the matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d FromColumns(Vector4d column0, Vector4d column1, Vector4d column2, Vector4d column3)
        {
            var result = new Matrix4x4d { Column0 = column0, Column1 = column1, Column2 = column2, Column3 = column3 };
            return result;
        }

        /// <summary>
        ///     Constructs a <see cref="Matrix4x4d" /> from the rows provided.
        /// </summary>
        /// <param name="row0">The first row.</param>
        /// <param name="row1">The second row.</param>
        /// <param name="row2">The third row.</param>
        /// <param name="row3">The fourth row.</param>
        /// <returns>Returns the matrx.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d FromRows(Vector4d row0, Vector4d row1, Vector4d row2, Vector4d row3)
        {
            var result = new Matrix4x4d { Row0 = row0, Row1 = row1, Row2 = row2, Row3 = row3 };
            return result;
        }

        /// <summary>
        ///     Generates the frustum matrix.
        /// </summary>
        /// <param name="left">The left plane.</param>
        /// <param name="right">The right plane.</param>
        /// <param name="bottom">The bottom plane.</param>
        /// <param name="top">The far plane.</param>
        /// <param name="near">The near plane.</param>
        /// <param name="far">The far plane.</param>
        /// <returns>Returns the frustum matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Frustum(double left, double right, double bottom, double top, double near, double far)
        {
            var invWidth = 1.0 / (right - left);
            var invHeight = 1.0 / (top - bottom);
            var invLength = 1.0 / (far - near);
            var twoNear = 2.0 * near;
            return new Matrix4x4d
                   {
                       x00 = twoNear * invWidth,
                       x11 = twoNear * invHeight,
                       x20 = (right + left) * invWidth,
                       x21 = (top + bottom) * invHeight,
                       x22 = -(far + near) * invLength,
                       x23 = -1.0,
                       x32 = -(far * twoNear * invLength)
                   };
        }

        /// <summary>
        ///     Generates an identity matrix.
        /// </summary>
        /// <returns>Returns the identity matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Identity()
        {
            return new Matrix4x4d { x00 = 1, x11 = 1, x22 = 1, x33 = 1 };
        }

        /// <summary>
        ///     Generates a scaled identity matrix.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns>Returns the identity matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Identity(double scale)
        {
            return new Matrix4x4d { x00 = scale, x11 = scale, x22 = scale, x33 = scale };
        }

        /// <summary>
        ///     Generates the matrix that describes the rotation needed to point from the eye to the target while being orientated
        ///     by the up vector.
        /// </summary>
        /// <param name="eye">The start point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="up">The up vector.</param>
        /// <returns>Returns the lookat matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d LookAt(Vector3d eye, Vector3d target, Vector3d up)
        {
            var direction = (eye - target).Normalized();
            var left = Vector3d.Cross(up, direction).Normalized();
            // Coverage for special case where up and direction are parallel.
            if (!left.IsNormalized)
            {
                left = up == direction ? Vector3d.Left : Vector3d.Right;
            }

            var newUp = Vector3d.Cross(direction, left).Normalized();
            // Coverage for special case where direction and left are parallel.
            if (!newUp.IsNormalized)
            {
                newUp = direction == left ? Vector3d.Up : Vector3d.Down;
            }

            var result = new Matrix4x4d
                         {
                             x00 = left.X,
                             x01 = newUp.X,
                             x02 = direction.X,
                             x10 = left.Y,
                             x11 = newUp.Y,
                             x12 = direction.Y,
                             x20 = left.Z,
                             x21 = newUp.Z,
                             x22 = direction.Z,
                             x33 = 1
                         };

            return Translation(-eye) * result;
        }

        /// <summary>
        ///     Generates the orthographic matrix.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="near">The near plane.</param>
        /// <param name="far">The far plane.</param>
        /// <returns>Returns the orhographic matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Orthographic(double width, double height, double near, double far)
        {
            return Orthographic(-width / 2.0, width / 2.0, -height / 2.0, height / 2.0, near, far);
        }

        /// <summary>
        ///     Generates the orthographic matrix.
        /// </summary>
        /// <param name="left">The left plane.</param>
        /// <param name="right">The right plane.</param>
        /// <param name="bottom">The bottom plane.</param>
        /// <param name="top">The top plane.</param>
        /// <param name="near">The near plane.</param>
        /// <param name="far">The far plane.</param>
        /// <returns>Returns the orhographic matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Orthographic(
            double left,
            double right,
            double bottom,
            double top,
            double near,
            double far)
        {
            var invWidth = 1.0 / (right - left);
            var invHeight = 1.0 / (top - bottom);
            var invLength = 1.0 / (far - near);

            var result = new Matrix4x4d
                         {
                             x00 = 2.0 * invWidth,
                             x11 = 2.0 * invHeight,
                             x22 = -2.0 * invLength,
                             x30 = -(right + left) * invWidth,
                             x31 = -(top + bottom) * invHeight,
                             x32 = -(far + near) * invLength
                         };

            return result;
        }

        /// <summary>
        ///     Generates the perspective matrix.
        /// </summary>
        /// <param name="fov">The field of view.</param>
        /// <param name="aspect">The aspect ratio.</param>
        /// <param name="near">The near plane.</param>
        /// <param name="far">The far plane.</param>
        /// <returns>Returns the perspective matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Perspective(double fov, double aspect, double near, double far)
        {
            var top = near * Math.Tan(fov * 0.5);
            var bottom = -top;
            return Frustum(aspect * bottom, aspect * top, bottom, top, near, far);
        }

        /// <summary>
        ///     Generates the rotation matrix for the specified rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="angle">The angle to rotate.</param>
        /// <returns>Returns the rotation matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Rotation(Vector3d axis, double angle)
        {
            var negCos = Math.Cos(-angle);
            var negSin = Math.Sin(-angle);
            var invCos = 1.0 - negCos;
            axis = axis.Normalized();

            var invCosXy = invCos * axis.X * axis.Y;
            var invCosXz = invCos * axis.X * axis.Z;
            var invCosYz = invCos * axis.Y * axis.Z;
            var negSinX = negSin * axis.X;
            var negSinY = negSin * axis.Y;
            var negSinZ = negSin * axis.Z;

            return new Matrix4x4d
                   {
                       x00 = invCos * axis.X * axis.X + negCos,
                       x01 = invCosXy - negSinZ,
                       x02 = invCosXz + negSinY,
                       x10 = invCosXy + negSinZ,
                       x11 = invCos * axis.Y * axis.Y + negCos,
                       x12 = invCosYz - negSinX,
                       x20 = invCosXz - negSinY,
                       x21 = invCosYz + negSinX,
                       x22 = invCos * axis.Z * axis.Z + negCos,
                       X33 = 1.0
                   };
        }

        /// <summary>
        /// Generates the rotation matrix for the specified rotation around the x axis.
        /// </summary>
        /// <param name="theta">The rotation angle in radians.</param>
        /// <returns>Returns the rotation axis.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d RotationX(double theta)
        {
            var cosT = Math.Cos(theta);
            var sinT = Math.Sin(theta);

            return new Matrix4x4d
                   {
                       x00 = 1,
                       x11 = cosT,
                       x12 = sinT,
                       x21 = -sinT,
                       x22 = cosT,
                       x33 = 1
                   };
        }

        /// <summary>
        /// Generates the rotation matrix for the specified rotation around the y axis.
        /// </summary>
        /// <param name="theta">The rotation angle in radians.</param>
        /// <returns>Returns the rotation axis.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d RotationY(double theta)
        {
            var cosT = Math.Cos(theta);
            var sinT = Math.Sin(theta);

            return new Matrix4x4d
                   {
                       x00 = cosT,
                       x02 = -sinT,
                       x11 = 1,
                       x20 = sinT,
                       x22 = cosT,
                       x33 = 1
                   };
        }

        /// <summary>
        /// Generates the rotation matrix for the specified rotation around the z axis.
        /// </summary>
        /// <param name="theta">The rotation angle in radians.</param>
        /// <returns>Returns the rotation axis.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d RotationZ(double theta)
        {
            var cosT = Math.Cos(theta);
            var sinT = Math.Sin(theta);

            return new Matrix4x4d
                   {
                       x00 = cosT,
                       x01 = sinT,
                       x10 = -sinT,
                       x11 = cosT,
                       x22 = 1,
                       x33 = 1
                   };
        }

        /// <summary>
        ///     Generates the scale matrix for the specified scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns>Returns the scale matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Scale(Vector3d scale)
        {
            var result = new Matrix4x4d { x00 = scale.X, x11 = scale.Y, x22 = scale.Z, x33 = 1.0 };
            return result;
        }

        /// <summary>
        ///     Generates the translation matrix for the specified translation.
        /// </summary>
        /// <param name="translation">The translation.</param>
        /// <returns>Returns the translation matrix.</returns>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d Translation(Vector3d translation)
        {
            var result = Identity();
            result.x30 = translation.X;
            result.x31 = translation.Y;
            result.x32 = translation.Z;
            return result;
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// Adds two <see cref="Matrix4x4d"/> instances.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d operator +([NotNull] Matrix4x4d left, [NotNull] Matrix4x4d right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));

            if (right == null) throw new ArgumentNullException(nameof(right));

            var result = new Matrix4x4d
                         {
                             x00 = left.x00 + right.x00,
                             x01 = left.x01 + right.x01,
                             x02 = left.x02 + right.x02,
                             x03 = left.x03 + right.x03,
                             x10 = left.x10 + right.x10,
                             x11 = left.x11 + right.x11,
                             x12 = left.x12 + right.x12,
                             x13 = left.x13 + right.x13,
                             x20 = left.x20 + right.x20,
                             x21 = left.x21 + right.x21,
                             x22 = left.x22 + right.x22,
                             x23 = left.x23 + right.x23,
                             x30 = left.x30 + right.x30,
                             x31 = left.x31 + right.x31,
                             x32 = left.x32 + right.x32,
                             x33 = left.x33 + right.x33
                         };

            return result;
        }

        /// <summary>
        /// Tests two <see cref="Matrix4x4d"/> for numerical equality.
        /// </summary>
        [Pure]
        [PublicAPI]
        public static bool operator ==([CanBeNull] Matrix4x4d left, [CanBeNull] Matrix4x4d right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return false;
            }

            return CommonUtil.Equals(left.x00, right.x00) && CommonUtil.Equals(left.x01, right.x01)
                   && CommonUtil.Equals(left.x02, right.x02) && CommonUtil.Equals(left.x03, right.x03)
                   && CommonUtil.Equals(left.x10, right.x10) && CommonUtil.Equals(left.x11, right.x11)
                   && CommonUtil.Equals(left.x12, right.x12) && CommonUtil.Equals(left.x13, right.x13)
                   && CommonUtil.Equals(left.x20, right.x20) && CommonUtil.Equals(left.x21, right.x21)
                   && CommonUtil.Equals(left.x22, right.x22) && CommonUtil.Equals(left.x23, right.x23)
                   && CommonUtil.Equals(left.x30, right.x30) && CommonUtil.Equals(left.x31, right.x31)
                   && CommonUtil.Equals(left.x32, right.x32) && CommonUtil.Equals(left.x33, right.x33);
        }

        /// <summary>
        /// Converts the <see cref="Matrix4x4d"/> to a <see cref="Matrix4x4f"/>.
        /// </summary>
        /// <param name="matrix">The matrix to convert.</param>
        [Pure]
        [CanBeNull]
        [PublicAPI]
        public static explicit operator MatrixF([CanBeNull] Matrix4x4d matrix)
        {
            return (Matrix4x4f)matrix;
        }

        /// <summary>
        /// Converts the <see cref="Matrix4x4d"/> to a <see cref="Matrix4x4f"/>.
        /// </summary>
        /// <param name="matrix">The matrix to convert.</param>
        [Pure]
        [CanBeNull]
        [PublicAPI]
        public static explicit operator Matrix4x4f([CanBeNull] Matrix4x4d matrix)
        {
            if (matrix == null) return null;

            return new Matrix4x4f(
                (float)matrix.x00,
                (float)matrix.x01,
                (float)matrix.x02,
                (float)matrix.x03,
                (float)matrix.x10,
                (float)matrix.x11,
                (float)matrix.x12,
                (float)matrix.x13,
                (float)matrix.x20,
                (float)matrix.x21,
                (float)matrix.x22,
                (float)matrix.x23,
                (float)matrix.x30,
                (float)matrix.x31,
                (float)matrix.x32,
                (float)matrix.x33);
        }

        /// <summary>
        /// Tests two <see cref="Matrix4x4d"/> for numerical inequality.
        /// </summary>
        [Pure]
        [PublicAPI]
        public static bool operator !=([CanBeNull] Matrix4x4d left, [CanBeNull] Matrix4x4d right)
        {
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            {
                return true;
            }

            return !CommonUtil.Equals(left.x00, right.x00) && !CommonUtil.Equals(left.x01, right.x01)
                   && !CommonUtil.Equals(left.x02, right.x02) && !CommonUtil.Equals(left.x03, right.x03)
                   && !CommonUtil.Equals(left.x10, right.x10) && !CommonUtil.Equals(left.x11, right.x11)
                   && !CommonUtil.Equals(left.x12, right.x12) && !CommonUtil.Equals(left.x13, right.x13)
                   && !CommonUtil.Equals(left.x20, right.x20) && !CommonUtil.Equals(left.x21, right.x21)
                   && !CommonUtil.Equals(left.x22, right.x22) && !CommonUtil.Equals(left.x23, right.x23)
                   && !CommonUtil.Equals(left.x30, right.x30) && !CommonUtil.Equals(left.x31, right.x31)
                   && !CommonUtil.Equals(left.x32, right.x32) && !CommonUtil.Equals(left.x33, right.x33);
        }

        /// <summary>
        /// Multiplies two <see cref="Matrix4x4d"/> instances.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d operator *([NotNull] Matrix4x4d left, [NotNull] Matrix4x4d right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));

            if (right == null) throw new ArgumentNullException(nameof(right));

            var result = new Matrix4x4d();

            // TODO: Evaluate whether n^2.81 algorithm (Volker Strassen) is viable or not.
            result.x00 = left.x00 * right.x00 + left.x01 * right.x10 + left.x02 * right.x20 + left.x03 * right.x30;
            result.x01 = left.x00 * right.x01 + left.x01 * right.x11 + left.x02 * right.x21 + left.x03 * right.x31;
            result.x02 = left.x00 * right.x02 + left.x01 * right.x12 + left.x02 * right.x22 + left.x03 * right.x32;
            result.x03 = left.x00 * right.x03 + left.x01 * right.x13 + left.x02 * right.x23 + left.x03 * right.x33;

            result.x10 = left.x10 * right.x00 + left.x11 * right.x10 + left.x12 * right.x20 + left.x13 * right.x30;
            result.x11 = left.x10 * right.x01 + left.x11 * right.x11 + left.x12 * right.x21 + left.x13 * right.x31;
            result.x12 = left.x10 * right.x02 + left.x11 * right.x12 + left.x12 * right.x22 + left.x13 * right.x32;
            result.x13 = left.x10 * right.x03 + left.x11 * right.x13 + left.x12 * right.x23 + left.x13 * right.x33;

            result.x20 = left.x20 * right.x00 + left.x21 * right.x10 + left.x22 * right.x20 + left.x23 * right.x30;
            result.x21 = left.x20 * right.x01 + left.x21 * right.x11 + left.x22 * right.x21 + left.x23 * right.x31;
            result.x22 = left.x20 * right.x02 + left.x21 * right.x12 + left.x22 * right.x22 + left.x23 * right.x32;
            result.x23 = left.x20 * right.x03 + left.x21 * right.x13 + left.x22 * right.x23 + left.x23 * right.x33;

            result.x30 = left.x30 * right.x00 + left.x31 * right.x10 + left.x32 * right.x20 + left.x33 * right.x30;
            result.x31 = left.x30 * right.x01 + left.x31 * right.x11 + left.x32 * right.x21 + left.x33 * right.x31;
            result.x32 = left.x30 * right.x02 + left.x31 * right.x12 + left.x32 * right.x22 + left.x33 * right.x32;
            result.x33 = left.x30 * right.x03 + left.x31 * right.x13 + left.x32 * right.x23 + left.x33 * right.x33;

            return result;
        }

        /// <summary>
        /// Multiplies a <see cref="Vector4d"/> with the <see cref="Matrix4x4d"/> and outputs the resulting column vector.
        /// </summary>
        [Pure]
        [PublicAPI]
        public static Vector4d operator *(Vector4d left, [NotNull] Matrix4x4d right)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));

            return new Vector4d(
                left.X * (right.x00 + right.x01 + right.x02 + right.x03),
                left.Y * (right.x10 + right.x11 + right.x12 + right.x13),
                left.Z * (right.x20 + right.x21 + right.x22 + right.x23),
                left.W * (right.x30 + right.x31 + right.x32 + right.x33));
        }

        /// <summary>
        /// Multiplies a <see cref="Matrix4x4d"/> with the <see cref="Vector4d"/> and outputs the resulting row vector.
        /// </summary>
        [Pure]
        [PublicAPI]
        public static Vector4d operator *([NotNull] Matrix4x4d left, Vector4d right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));

            return new Vector4d(
                (left.x00 + left.x10 + left.x20 + left.x30) * right.X,
                (left.x01 + left.x11 + left.x21 + left.x31) * right.Y,
                (left.x02 + left.x12 + left.x22 + left.x32) * right.Z,
                (left.x03 + left.x13 + left.x23 + left.x33) * right.W);
        }

        /// <summary>
        /// Scales the <see cref="Matrix4x4d"/> with the given <see cref="double"/>.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d operator *([NotNull] Matrix4x4d left, double right)
        {
            return (Matrix4x4d)left.Multiply(right);
        }

        /// <summary>
        /// Scales the <see cref="Matrix4x4d"/> with the given <see cref="double"/>.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d operator *(double left, [NotNull] Matrix4x4d right)
        {
            return (Matrix4x4d)right.Multiply(left);
        }

        /// <summary>
        /// Subtracts the right <see cref="Matrix4x4d"/> from the left one.
        /// </summary>
        [Pure]
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d operator -([NotNull] Matrix4x4d left, [NotNull] Matrix4x4d right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));

            if (right == null) throw new ArgumentNullException(nameof(right));

            var result = new Matrix4x4d
                         {
                             x00 = left.x00 - right.x00,
                             x01 = left.x01 - right.x01,
                             x02 = left.x02 - right.x02,
                             x03 = left.x03 - right.x03,
                             x10 = left.x10 - right.x10,
                             x11 = left.x11 - right.x11,
                             x12 = left.x12 - right.x12,
                             x13 = left.x13 - right.x13,
                             x20 = left.x20 - right.x20,
                             x21 = left.x21 - right.x21,
                             x22 = left.x22 - right.x22,
                             x23 = left.x23 - right.x23,
                             x30 = left.x30 - right.x30,
                             x31 = left.x31 - right.x31,
                             x32 = left.x32 - right.x32,
                             x33 = left.x33 - right.x33
                         };

            return result;
        }

        /// <summary>
        /// Negates the <see cref="Matrix4x4d"/>.
        /// </summary>
        [NotNull]
        [PublicAPI]
        public static Matrix4x4d operator -([NotNull] Matrix4x4d matrix)
        {
            return new Matrix4x4d(
                -matrix.x00,
                -matrix.x01,
                -matrix.x02,
                -matrix.x03,
                -matrix.x10,
                -matrix.x11,
                -matrix.x12,
                -matrix.x13,
                -matrix.x20,
                -matrix.x21,
                -matrix.x22,
                -matrix.x23,
                -matrix.x30,
                -matrix.x31,
                -matrix.x32,
                -matrix.x33);
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
                        case 3:
                            return x03;
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
                        case 3:
                            return x13;
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
                        case 3:
                            return x23;
                        default:
                            return 0d;
                    }
                case 3:
                    switch (column)
                    {
                        case 0:
                            return x30;
                        case 1:
                            return x31;
                        case 2:
                            return x32;
                        case 3:
                            return x33;
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
                        case 3:
                            x03 = value;
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
                        case 3:
                            x13 = value;
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
                        case 3:
                            x23 = value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(column));
                    }

                    break;
                case 3:
                    switch (column)
                    {
                        case 0:
                            x30 = value;
                            break;
                        case 1:
                            x31 = value;
                            break;
                        case 2:
                            x32 = value;
                            break;
                        case 3:
                            x33 = value;
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