// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Vector4d.cs
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
    /// <summary>
    /// Container for 4 component vector data using doubles.
    /// </summary>
    [Serializable] 
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    // ReSharper disable once InconsistentNaming
    public struct Vector4d : IVectorD, ISerializable
    {
        #region Constants

        /// <summary>
        /// The zero vector [0,0,0,0]
        /// </summary>
        [PublicAPI]
		public static readonly Vector4d Zero = new Vector4d(0, 0, 0, 0);

        /// <summary>
        /// The one vector [1,1,1,1]
        /// </summary>
        [PublicAPI]
		public static readonly Vector4d One = new Vector4d(1, 1, 1, 1);

		/// <summary> 
        /// The NaN vector [NaN,NaN,NaN,NaN]
        /// </summary>
        [PublicAPI]
		public static readonly Vector4d NaN = new Vector4d(double.NaN, double.NaN, double.NaN, double.NaN);
        #endregion

        #region Fields

        /// <summary>
        /// The X component
        /// </summary>
        [FieldOffset(0)] 
		[PublicAPI]
        public readonly double X;

        /// <summary>
        /// The Y component
        /// </summary>
        [FieldOffset(8)] 
		[PublicAPI]
        public readonly double Y;

        /// <summary>
        /// The Z component
        /// </summary>
        [FieldOffset(16)] 
		[PublicAPI]
        public readonly double Z;

        /// <summary>
        /// The W component
        /// </summary>
        [FieldOffset(24)] 
		[PublicAPI]
        public readonly double W;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4d"/> struct.
        /// </summary>
        /// <param name="x">
        /// The X
        /// </param>
		[PublicAPI]
        public Vector4d(double x)
        {
            this.X = x;
            this.Y = 0;
            this.Z = 0;
            this.W = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4d"/> struct.
        /// </summary>
        /// <param name="x">
        /// The X
        /// </param>
        /// <param name="y">
        /// The Y
        /// </param>
		[PublicAPI]
        public Vector4d(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Z = 0;
            this.W = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4d"/> struct.
        /// </summary>
        /// <param name="x">
        /// The X
        /// </param>
        /// <param name="y">
        /// The Y
        /// </param>
        /// <param name="z">
        /// The Z
        /// </param>
		[PublicAPI]
        public Vector4d(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4d"/> struct.
        /// </summary>
        /// <param name="x">
        /// The X
        /// </param>
        /// <param name="y">
        /// The Y
        /// </param>
        /// <param name="z">
        /// The Z
        /// </param>
        /// <param name="w">
        /// The W
        /// </param>
		[PublicAPI]
        public Vector4d(double x, double y, double z, double w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector4d"/> struct, using the serialized data.
        /// </summary>
        private Vector4d(SerializationInfo info, StreamingContext context)
        {
            this.X = info.GetDouble("X");
            this.Y = info.GetDouble("Y");
            this.Z = info.GetDouble("Z");
            this.W = info.GetDouble("W");
        }

        #endregion

        #region Properties and Indexers

        /// <inheritdoc/>
		[PublicAPI]
        public double this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("index", "The index must be positive.");
                }

                switch(index)
                {
                    case 0:
                        return this.X;
                    case 1:
                        return this.Y;
                    case 2:
                        return this.Z;
                    case 3:
                        return this.W;
                    default:
                        return 0;
                }
            }
        }  

		/// <inheritdoc/>
		[PublicAPI]
		public int Dimension
		{
			get
			{
				return 4;
			}
		}

        /// <inheritdoc/>
		[PublicAPI]
        public double Length
        {
            get
            {
                return Math.Sqrt(this.SqLength); 
            }
        }

        /// <inheritdoc/>
		[PublicAPI]
        public double SqLength
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z) + (this.W * this.W);
            }
        }

        IVectorD IVectorD.Add(IVectorD other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this.Dimension >= other.Dimension)
            {
                return new Vector4d(X + other[0], Y + other[1], Z + other[2], W + other[3]);
            }
            else
            {
                return other.Add(this);
            }
        }

        IVectorD IVectorD.Subtract(IVectorD other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (this.Dimension >= other.Dimension)
            {
                return new Vector4d(X - other[0], Y - other[1], Z - other[2], W - other[3]);
            }
            else
            {
                return other.Subtract(this);
            }
        }

        double IVectorD.Dot(IVectorD other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return X * other[0] + Y * other[1] + Z * other[2] + W * other[3];
        }

        /// <inheritdoc/>
		[PublicAPI]
        public bool IsNaN
        {
            get
            {
                return double.IsNaN(this.X) || double.IsNaN(this.Y) || double.IsNaN(this.Z) || double.IsNaN(this.W);
            }
        }

        /// <inheritdoc/>
		[PublicAPI]
        public bool IsInfinity
        {
            get
            {
                return double.IsInfinity(this.X) || double.IsInfinity(this.Y) || double.IsInfinity(this.Z) || double.IsInfinity(this.W);
            }
        }

		/// <inheritdoc/>
		[PublicAPI]
		public bool IsZero
		{
			get
			{
				return  CommonUtil.Equals(this.SqLength, 0); 			}
		}

		/// <inheritdoc/>
		[PublicAPI]
		public bool IsNormalized
		{
			get
			{
				return CommonUtil.Equals(this.SqLength, 1);
			}
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Inverts the vector component wise.
        /// </summary>
        /// <returns>The resulting vector.</returns>
        [PublicAPI]
        public Vector4d CompInv()
        {
            return new Vector4d(1d / X, 1d / Y, 1d / Z, 1d / W);
        }

        /// <summary>
        /// Inverts the vector. (v * v.Inv() = 1)
        /// </summary>
        /// <returns>The resulting vector.</returns>
        [PublicAPI]
        public Vector4d Inv()
        {
            var length = this.Length;
            return new Vector4d(X / length, Y / length, Z / length, W / length);
        }

        /// <summary>
        /// Makes all components of the vector negative.
        /// </summary>
        /// <returns>Returns the negative only vector.</returns>
		[PublicAPI]
        public Vector4d NegAbs()
        {
            return new Vector4d(-Math.Abs(this.X), -Math.Abs(this.Y), -Math.Abs(this.Z), -Math.Abs(this.W));
        }    
        
        /// <summary>
        /// Makes all components of the vector positive.
        /// </summary>
        /// <returns>Returns the positive only vector.</returns>
		[PublicAPI]
        public Vector4d Abs()
        {
            return new Vector4d(Math.Abs(this.X), Math.Abs(this.Y), Math.Abs(this.Z), Math.Abs(this.W));
        }

        /// <inheritdoc/>
		[PublicAPI]
        public override bool Equals(object obj)
        {
            var vo = obj as Vector4d?;
            if (!vo.HasValue)
            {
                return false;
            }
            
            var v = vo.Value;

            return Math.Abs(this.X - v.X) < double.Epsilon && Math.Abs(this.Y - v.Y) < double.Epsilon && Math.Abs(this.Z - v.Z) < double.Epsilon && Math.Abs(this.W - v.W) < double.Epsilon;
        }

        /// <inheritdoc/>
		[PublicAPI]
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = (hash * 16777619) ^ this.X.GetHashCode();
                hash = (hash * 16777619) ^ this.Y.GetHashCode();
                hash = (hash * 16777619) ^ this.Z.GetHashCode();
                hash = (hash * 16777619) ^ this.W.GetHashCode();  
                return hash;
            }
        }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", this.X);
            info.AddValue("Y", this.Y);
            info.AddValue("Z", this.Z);
            info.AddValue("W", this.W);
        }

        /// <summary>
        /// Inverts the vector.
        /// </summary>
        /// <returns>Returns the inverted vector.</returns>
		[PublicAPI]
        public Vector4d Inverse()
        {
            return new Vector4d(1d / this.X, 1d / this.Y, 1d / this.Z, 1d / this.W);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <returns>Returns the normalized vector.</returns>
		[PublicAPI]
        public Vector4d Normalized()
        {
            return this / this.Length;
        }    

		/// <summary>
        /// Adds an offset to all components of this vector.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns the vector with applied offset.</returns>
		[PublicAPI]
		public Vector4d Offset(double offset)
		{
			return new Vector4d(this.X + offset, this.Y + offset, this.Z + offset, this.W + offset);
		}

        /// <inheritdoc/>
		[PublicAPI]
        public override string ToString()
        {
            return $"Vector[{this.X}, {this.Y}, {this.Z}, {this.W}]";
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Multiplies two vectors component wise.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector4d ComponentProduct(Vector4d left, Vector4d right)
        {
            return new Vector4d(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
        }


		/// <summary>
        /// Calculates the angle between the two vectors.
        /// </summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
        /// <returns>Returns the angle.</returns>
		[PublicAPI]
		public static double Angle(Vector4d first, Vector4d second)
		{
			return (double)Math.Acos(Dot(first, second) / (first.Length * second.Length));
		}

        /// <summary>
        /// Calculates the distance between the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Returns the distance.</returns>
		[PublicAPI]
        public static double Distance(Vector4d a, Vector4d b)
        {
			return Math.Sqrt(SqDistance(a, b));
        }

        /// <summary>
        /// Calculates the dot product
        /// </summary>
        /// <returns>Returns the dot product</returns>
		[PublicAPI]
        public static double Dot(Vector4d left, Vector4d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }

        /// <summary>
        /// Lerps the vector between <see cref="from"/> and <see cref="to"/> using the unclamped lerp value.
        /// </summary>
        /// <param name="from">The start vector.</param>
        /// <param name="to">The target vector.</param>
        /// <param name="lerp">The lerp value.</param>
        /// <returns>Returns the lerped vector.</returns>
		[PublicAPI]
        public static Vector4d Lerp(Vector4d from, Vector4d to, double lerp)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;
            var z = to.Z - from.Z;
            var w = to.W - from.W;
            return new Vector4d(from.X + (lerp * x), from.Y + (lerp * y), from.Z + (lerp * z), from.W + (lerp * w));
        }

        /// <summary>
        /// Lerps the vector between <see cref="from"/> and <see cref="to"/> using the clamped lerp value.
        /// </summary>
        /// <param name="from">The start vector.</param>
        /// <param name="to">The target vector.</param>
        /// <param name="lerp">The lerp value.</param>
        /// <returns>Returns the lerped vector.</returns>
		[PublicAPI]
        public static Vector4d LerpC(Vector4d from, Vector4d to, double lerp)
        {
            lerp = Math.Max(Math.Min(lerp, 1f), 0f);
            var x = to.X - from.X;
            var y = to.Y - from.Y;
            var z = to.Z - from.Z;
            var w = to.W - from.W;
            return new Vector4d(from.X + (lerp * x), from.Y + (lerp * y), from.Z + (lerp * z), from.W + (lerp * w));
        }

		/// <summary>
        /// Clamps the vector to a value between min and max.
        /// </summary>
        /// <param name="vector">The vector to clamp.</param>
        /// <param name="min">The minimum values.</param>
		/// <param name="max">The maximum values.</param>
        /// <returns>Returns the clamped vector.</returns>
		[PublicAPI]
		public static Vector4d Clamp(Vector4d vector, Vector4d min, Vector4d max)
		{
			return Min(Max(vector, min), max);
		}

        /// <summary>
        /// Calculates the vector that contains the component wise maximum.
        /// </summary>
        /// <returns>Returns the maximum component vector.</returns>
		[PublicAPI]
        public static Vector4d Max (Vector4d left, Vector4d right)
        {
            return new Vector4d(Math.Max(left.X, right.X), Math.Max(left.Y, right.Y), Math.Max(left.Z, right.Z), Math.Max(left.W, right.W));
        }

		/// <summary>
        /// Finds the vector that is longer.
        /// </summary>
        /// <returns>Returns the longer vector.</returns>
		[PublicAPI]
		public static Vector4d LengthMax(Vector4d left, Vector4d right)
		{
			if(left.SqLength >= right.SqLength)
			{
				return left;
			}

			return right;
		}

        /// <summary>
        /// Calculates the vector that contains the component wise minimum.
        /// </summary>
        /// <returns>Returns the minimum component vector.</returns>
		[PublicAPI]
        public static Vector4d Min (Vector4d left, Vector4d right)
        {
            return new Vector4d(Math.Min(left.X, right.X), Math.Min(left.Y, right.Y), Math.Min(left.Z, right.Z), Math.Min(left.W, right.W));
        }

		/// <summary>
        /// Finds the vector that is shorter.
        /// </summary>
        /// <returns>Returns the shorter vector.</returns>
		[PublicAPI]
		public static Vector4d LengthMin(Vector4d left, Vector4d right)
		{
			if(left.SqLength <= right.SqLength)
			{
				return left;
			}

			return right;
		}

        /// <summary>
        /// Calculates the square distance between the two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>Returns the square distance.</returns>
		[PublicAPI]
        public static double SqDistance(Vector4d a, Vector4d b)
        {
            var deltaX = b.X - a.X;
            var deltaY = b.Y - a.Y;
            var deltaZ = b.Z - a.Z;
            var deltaW = b.W - a.W;

            return deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ + deltaW * deltaW;
        }

        #endregion

        #region Static Cast Methods

        /// <summary>
        /// Converts the vector explicitly to a Vector2f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector2(Vector4d v)
        {
            return new Vector2((float)v.X, (float)v.Y);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector2d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector2d(Vector4d v)
        {
            return new Vector2d(v.X, v.Y);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector3f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector3(Vector4d v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector3d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector3d(Vector4d v)
        {
            return new Vector3d(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector4f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector4(Vector4d v)
        {
            return new Vector4((float)v.X, (float)v.Y, (float)v.Z, (float)v.W);
        }

        /// <summary>
        /// Converts a vector2 implicitly to a Vector4d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static implicit operator Vector4d(Vector2 v)
        {
            return new Vector4d(v.x, v.y, 0d, 0d);
        }

        /// <summary>
        /// Converts a vector3 implicitly to a Vector4d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static implicit operator Vector4d(Vector3 v)
        {
            return new Vector4d(v.x, v.y, v.z, 0d);
        }

        /// <summary>
        /// Converts a vector4 explicitly to a Vector4d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static implicit operator Vector4d(Vector4 v)
        {
            return new Vector4d(v.x, v.y, v.z, v.w);
        }

        #endregion

        #region Static Operator Methods

        /// <summary>
        /// Evaluates whether two <see cref="Vector4d"/> are equal.
        /// </summary>
        /// <returns>Returns <c>true</c> when the vectors are component wise equal, <c>false</c> otherwise.</returns>
		[PublicAPI]
        public static bool operator == (Vector4d left, Vector4d right)
        {
            return CommonUtil.Equals(left.X, right.X) && CommonUtil.Equals(left.Y, right.Y) && CommonUtil.Equals(left.Z, right.Z) && CommonUtil.Equals(left.W, right.W);
        }

        /// <summary>
        /// Evaluates whether two <see cref="Vector4d"/> are inequal.
        /// </summary>
        /// <returns>Returns <c>true</c> when at least one component is not equal, <c>false</c> otherwise.</returns>
		[PublicAPI]
        public static bool operator != (Vector4d left, Vector4d right)
        {
            return !CommonUtil.Equals(left.X, right.X) || !CommonUtil.Equals(left.Y, right.Y) || !CommonUtil.Equals(left.Z, right.Z) || !CommonUtil.Equals(left.W, right.W);
        }

        /// <summary>
        /// Sums two <see cref="Vector4d"/>.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector4d operator + (Vector4d left, Vector4d right)
        {
            return new Vector4d(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }		

        /// <summary>
        /// Subtracts two <see cref="Vector4d"/>.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector4d operator - (Vector4d left, Vector4d right)
        {
            return new Vector4d(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

		/// <summary>
		/// Negates the vector.
		/// </summary>
		/// <returns>Returns the negated vector.</returns>
		[PublicAPI]
		public static Vector4d operator - (Vector4d vector)
		{
			return new Vector4d(-vector.X, -vector.Y, -vector.Z, -vector.W);
		}

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector4d"/>.
        /// </summary>
        /// <returns>Returns the dot product.</returns>
		[PublicAPI]
        public static double operator * (Vector4d left, Vector4d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }

        /// <summary>
        /// Scales a <see cref="Vector4d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector4d operator * (Vector4d left, double right)
        {
            return new Vector4d(left.X * right, left.Y * right, left.Z * right, left.W * right);
        }

        /// <summary>
        /// Scales a <see cref="Vector4d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector4d operator * (double left, Vector4d right)
        {
            return new Vector4d(left * right.X, left * right.Y, left * right.Z, left * right.W);
        }

        /// <summary>
        /// CompInv scales a <see cref="Vector4d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector4d operator / (Vector4d left, double right)
        {
            return new Vector4d(left.X / right, left.Y / right, left.Z / right, left.W / right);
        }

        #endregion
    }
}
