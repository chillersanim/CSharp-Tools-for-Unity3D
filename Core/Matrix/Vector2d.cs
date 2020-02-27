// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Vector2d.cs
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
    /// Container for 2 component vector data using doubles.
    /// </summary>
    [Serializable] 
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    // ReSharper disable once InconsistentNaming
    public struct Vector2d : IVectorD, ISerializable
    {
        #region Constants

        /// <summary>
        /// The zero vector [0,0]
        /// </summary>
        [PublicAPI]
		public static readonly Vector2d Zero = new Vector2d(0, 0);

        /// <summary>
        /// The one vector [1,1]
        /// </summary>
        [PublicAPI]
		public static readonly Vector2d One = new Vector2d(1, 1);

		/// <summary> 
        /// The NaN vector [NaN,NaN]
        /// </summary>
        [PublicAPI]
		public static readonly Vector2d NaN = new Vector2d(double.NaN, double.NaN);
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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2d"/> struct.
        /// </summary>
        /// <param name="x">
        /// The X
        /// </param>
		[PublicAPI]
        public Vector2d(double x)
        {
            this.X = x;
            this.Y = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2d"/> struct.
        /// </summary>
        /// <param name="x">
        /// The X
        /// </param>
        /// <param name="y">
        /// The Y
        /// </param>
		[PublicAPI]
        public Vector2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2d"/> struct, using the serialized data.
        /// </summary>
        private Vector2d(SerializationInfo info, StreamingContext context)
        {
            this.X = info.GetDouble("X");
            this.Y = info.GetDouble("Y");
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
				return 2;
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
                return (this.X * this.X) + (this.Y * this.Y);
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
                return new Vector2d(X + other[0], Y + other[1]);
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
                return new Vector2d(X - other[0], Y - other[1]);
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

            return X * other[0] + Y * other[1];
        }

        /// <inheritdoc/>
		[PublicAPI]
        public bool IsNaN
        {
            get
            {
                return double.IsNaN(this.X) || double.IsNaN(this.Y);
            }
        }

        /// <inheritdoc/>
		[PublicAPI]
        public bool IsInfinity
        {
            get
            {
                return double.IsInfinity(this.X) || double.IsInfinity(this.Y);
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
        public Vector2d CompInv()
        {
            return new Vector2d(1d / X, 1d / Y);
        }

        /// <summary>
        /// Inverts the vector.
        /// </summary>
        /// <returns>The resulting vector.</returns>
        [PublicAPI]
        public Vector2d Inv()
        {
            var length = this.Length;
            return new Vector2d(X / length, Y / length);
        }

        /// <summary>
        /// Makes all components of the vector negative.
        /// </summary>
        /// <returns>Returns the negative only vector.</returns>
		[PublicAPI]
        public Vector2d NegAbs()
        {
            return new Vector2d(-Math.Abs(this.X), -Math.Abs(this.Y));
        }    
        
        /// <summary>
        /// Makes all components of the vector positive.
        /// </summary>
        /// <returns>Returns the positive only vector.</returns>
		[PublicAPI]
        public Vector2d Abs()
        {
            return new Vector2d(Math.Abs(this.X), Math.Abs(this.Y));
        }

        /// <inheritdoc/>
		[PublicAPI]
        public override bool Equals(object obj)
        {
            var vo = obj as Vector2d?;
            if (!vo.HasValue)
            {
                return false;
            }
            
            var v = vo.Value;

            return Math.Abs(this.X - v.X) < double.Epsilon && Math.Abs(this.Y - v.Y) < double.Epsilon;
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
                return hash;
            }
        }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", this.X);
            info.AddValue("Y", this.Y);
        }

        /// <summary>
        /// Inverts the vector.
        /// </summary>
        /// <returns>Returns the inverted vector.</returns>
		[PublicAPI]
        public Vector2d Inverse()
        {
            return new Vector2d(1d / this.X, 1d / this.Y);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <returns>Returns the normalized vector.</returns>
		[PublicAPI]
        public Vector2d Normalized()
        {
            return this / this.Length;
        }    

		/// <summary>
        /// Adds an offset to all components of this vector.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns the vector with applied offset.</returns>
		[PublicAPI]
		public Vector2d Offset(double offset)
		{
			return new Vector2d(this.X + offset, this.Y + offset);
		}

        /// <inheritdoc/>
		[PublicAPI]
        public override string ToString()
        {
            return $"Vector[{this.X}, {this.Y}]";
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Multiplies two vectors component wise.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector2d ComponentProduct(Vector2d left, Vector2d right)
        {
            return new Vector2d(left.X * right.X, left.Y * right.Y);
        }


		/// <summary>
        /// Calculates the angle between the two vectors.
        /// </summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
        /// <returns>Returns the angle.</returns>
		[PublicAPI]
		public static double Angle(Vector2d first, Vector2d second)
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
        public static double Distance(Vector2d a, Vector2d b)
        {
			return Math.Sqrt(SqDistance(a, b));
        }

        /// <summary>
        /// Calculates the dot product
        /// </summary>
        /// <returns>Returns the dot product</returns>
		[PublicAPI]
        public static double Dot(Vector2d left, Vector2d right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        /// <summary>
        /// Lerps the vector between <see cref="from"/> and <see cref="to"/> using the unclamped lerp value.
        /// </summary>
        /// <param name="from">The start vector.</param>
        /// <param name="to">The target vector.</param>
        /// <param name="lerp">The lerp value.</param>
        /// <returns>Returns the lerped vector.</returns>
		[PublicAPI]
        public static Vector2d Lerp(Vector2d from, Vector2d to, double lerp)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;
            return new Vector2d(from.X + (lerp * x), from.Y + (lerp * y));
        }

        /// <summary>
        /// Lerps the vector between <see cref="from"/> and <see cref="to"/> using the clamped lerp value.
        /// </summary>
        /// <param name="from">The start vector.</param>
        /// <param name="to">The target vector.</param>
        /// <param name="lerp">The lerp value.</param>
        /// <returns>Returns the lerped vector.</returns>
		[PublicAPI]
        public static Vector2d LerpC(Vector2d from, Vector2d to, double lerp)
        {
            lerp = Math.Max(Math.Min(lerp, 1f), 0f);
            var x = to.X - from.X;
            var y = to.Y - from.Y;
            return new Vector2d(from.X + (lerp * x), from.Y + (lerp * y));
        }

		/// <summary>
        /// Clamps the vector to a value between min and max.
        /// </summary>
        /// <param name="vector">The vector to clamp.</param>
        /// <param name="min">The minimum values.</param>
		/// <param name="max">The maximum values.</param>
        /// <returns>Returns the clamped vector.</returns>
		[PublicAPI]
		public static Vector2d Clamp(Vector2d vector, Vector2d min, Vector2d max)
		{
			return Min(Max(vector, min), max);
		}

        /// <summary>
        /// Calculates the vector that contains the component wise maximum.
        /// </summary>
        /// <returns>Returns the maximum component vector.</returns>
		[PublicAPI]
        public static Vector2d Max (Vector2d left, Vector2d right)
        {
            return new Vector2d(Math.Max(left.X, right.X), Math.Max(left.Y, right.Y));
        }

		/// <summary>
        /// Finds the vector that is longer.
        /// </summary>
        /// <returns>Returns the longer vector.</returns>
		[PublicAPI]
		public static Vector2d LengthMax(Vector2d left, Vector2d right)
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
        public static Vector2d Min (Vector2d left, Vector2d right)
        {
            return new Vector2d(Math.Min(left.X, right.X), Math.Min(left.Y, right.Y));
        }

		/// <summary>
        /// Finds the vector that is shorter.
        /// </summary>
        /// <returns>Returns the shorter vector.</returns>
		[PublicAPI]
		public static Vector2d LengthMin(Vector2d left, Vector2d right)
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
        public static double SqDistance(Vector2d a, Vector2d b)
        {
            var deltaX = b.X - a.X;
            var deltaY = b.Y - a.Y;

            return deltaX * deltaX + deltaY * deltaY;
        }

        #endregion

        #region Static Cast Methods

        /// <summary>
        /// Converts the vector explicitly to a Vector2f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector2(Vector2d v)
        {
            return new Vector2((float)v.X, (float)v.Y);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector3f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector3(Vector2d v)
        {
            return new Vector3((float)v.X, (float)v.Y);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector4f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector4(Vector2d v)
        {
            return new Vector4((float)v.X, (float)v.Y);
        }

        /// <summary>
        /// Converts a vector2 implicitly to a Vector2d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static implicit operator Vector2d(Vector2 v)
        {
            return new Vector2d(v.x, v.y);
        }

        /// <summary>
        /// Converts a vector3 explicitly to a Vector2d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector2d(Vector3 v)
        {
            return new Vector2d(v.x, v.y);
        }

        /// <summary>
        /// Converts a vector4 explicitly to a Vector2d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector2d(Vector4 v)
        {
            return new Vector2d(v.x, v.y);
        }

        /// <summary>
        /// Converts the vector implicitly to a Vector3d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator Vector3d(Vector2d v)
        {
            return new Vector3d(v.X, v.Y);
        }

        /// <summary>
        /// Converts the vector implicitly to a Vector4d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator Vector4d(Vector2d v)
        {
            return new Vector4d(v.X, v.Y);
        }

        #endregion

        #region Static Operator Methods

        /// <summary>
        /// Evaluates whether two <see cref="Vector2d"/> are equal.
        /// </summary>
        /// <returns>Returns <c>true</c> when the vectors are component wise equal, <c>false</c> otherwise.</returns>
		[PublicAPI]
        public static bool operator == (Vector2d left, Vector2d right)
        {
            return CommonUtil.Equals(left.X, right.X) && CommonUtil.Equals(left.Y, right.Y);
        }

        /// <summary>
        /// Evaluates whether two <see cref="Vector2d"/> are inequal.
        /// </summary>
        /// <returns>Returns <c>true</c> when at least one component is not equal, <c>false</c> otherwise.</returns>
		[PublicAPI]
        public static bool operator != (Vector2d left, Vector2d right)
        {
            return !CommonUtil.Equals(left.X, right.X) || !CommonUtil.Equals(left.Y, right.Y);
        }

        /// <summary>
        /// Sums two <see cref="Vector2d"/>.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector2d operator + (Vector2d left, Vector2d right)
        {
            return new Vector2d(left.X + right.X, left.Y + right.Y);
        }		

        /// <summary>
        /// Subtracts two <see cref="Vector2d"/>.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector2d operator - (Vector2d left, Vector2d right)
        {
            return new Vector2d(left.X - right.X, left.Y - right.Y);
        }

		/// <summary>
		/// Negates the vector.
		/// </summary>
		/// <returns>Returns the negated vector.</returns>
		[PublicAPI]
		public static Vector2d operator - (Vector2d vector)
		{
			return new Vector2d(-vector.X, -vector.Y);
		}

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector2d"/>.
        /// </summary>
        /// <returns>Returns the dot product.</returns>
		[PublicAPI]
        public static double operator * (Vector2d left, Vector2d right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        /// <summary>
        /// Scales a <see cref="Vector2d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector2d operator * (Vector2d left, double right)
        {
            return new Vector2d(left.X * right, left.Y * right);
        }

        /// <summary>
        /// Scales a <see cref="Vector2d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector2d operator * (double left, Vector2d right)
        {
            return new Vector2d(left * right.X, left * right.Y);
        }

        /// <summary>
        /// CompInv scales a <see cref="Vector2d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector2d operator / (Vector2d left, double right)
        {
            return new Vector2d(left.X / right, left.Y / right);
        }

        #endregion
    }
}
