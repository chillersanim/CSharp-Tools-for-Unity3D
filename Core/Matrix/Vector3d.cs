// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Vector3d.cs
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
    /// Container for 3 component vector data using doubles.
    /// </summary>
    [Serializable] 
    [StructLayout(LayoutKind.Explicit, Size = 24)]
    // ReSharper disable once InconsistentNaming
    public struct Vector3d : IVectorD, ISerializable
    {
        #region Constants

        /// <summary>
        /// The zero vector [0,0,0]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d Zero = new Vector3d(0, 0, 0);

        /// <summary>
        /// The one vector [1,1,1]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d One = new Vector3d(1, 1, 1);

        /// <summary> 
        /// The right vector [1,0,0]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d Right = new Vector3d(1, 0, 0);

        /// <summary>
        /// The left vector [-1,0,0]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d Left = new Vector3d(-1, 0, 0);        

        /// <summary>
        /// The up vector [0,1,0]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d Up = new Vector3d(0, 1, 0);

        /// <summary>
        /// The down vector [0,-1,0]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d Down = new Vector3d(0, -1, 0);

		/// <summary>
        /// The forward vector [0,0,1]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d Forward = new Vector3d(0, 0, 1);

        /// <summary>
        /// The back vector [0,0,-1]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d Back = new Vector3d(0, 0, -1);

		/// <summary> 
        /// The NaN vector [NaN,NaN,NaN]
        /// </summary>
        [PublicAPI]
		public static readonly Vector3d NaN = new Vector3d(double.NaN, double.NaN, double.NaN);
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

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3d"/> struct.
        /// </summary>
        /// <param name="x">
        /// The X
        /// </param>
		[PublicAPI]
        public Vector3d(double x)
        {
            this.X = x;
            this.Y = 0;
            this.Z = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3d"/> struct.
        /// </summary>
        /// <param name="x">
        /// The X
        /// </param>
        /// <param name="y">
        /// The Y
        /// </param>
		[PublicAPI]
        public Vector3d(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Z = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3d"/> struct.
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
        public Vector3d(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3d"/> struct, using the serialized data.
        /// </summary>
        private Vector3d(SerializationInfo info, StreamingContext context)
        {
            this.X = info.GetDouble("X");
            this.Y = info.GetDouble("Y");
            this.Z = info.GetDouble("Z");
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
				return 3;
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
                return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
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
                return new Vector3d(X + other[0], Y + other[1], Z + other[2]);
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
                return new Vector3d(X - other[0], Y - other[1], Z - other[2]);
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

            return X * other[0] + Y * other[1] + Z * other[2];
        }

        /// <inheritdoc/>
		[PublicAPI]
        public bool IsNaN
        {
            get
            {
                return double.IsNaN(this.X) || double.IsNaN(this.Y) || double.IsNaN(this.Z);
            }
        }

        /// <inheritdoc/>
		[PublicAPI]
        public bool IsInfinity
        {
            get
            {
                return double.IsInfinity(this.X) || double.IsInfinity(this.Y) || double.IsInfinity(this.Z);
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
        public Vector3d CompInv()
        {
            return new Vector3d(1d / X, 1d / Y, 1d / Z);
        }

        /// <summary>
        /// Inverts the vector.
        /// </summary>
        /// <returns>The resulting vector.</returns>
        [PublicAPI]
        public Vector3d Inv()
        {
            var length = this.Length;
            return new Vector3d(X / length, Y / length, Z / length);
        }

        /// <summary>
        /// Makes all components of the vector negative.
        /// </summary>
        /// <returns>Returns the negative only vector.</returns>
		[PublicAPI]
        public Vector3d NegAbs()
        {
            return new Vector3d(-Math.Abs(this.X), -Math.Abs(this.Y), -Math.Abs(this.Z));
        }    
        
        /// <summary>
        /// Makes all components of the vector positive.
        /// </summary>
        /// <returns>Returns the positive only vector.</returns>
		[PublicAPI]
        public Vector3d Abs()
        {
            return new Vector3d(Math.Abs(this.X), Math.Abs(this.Y), Math.Abs(this.Z));
        }

        /// <inheritdoc/>
		[PublicAPI]
        public override bool Equals(object obj)
        {
            var vo = obj as Vector3d?;
            if (!vo.HasValue)
            {
                return false;
            }
            
            var v = vo.Value;

            return Math.Abs(this.X - v.X) < double.Epsilon && Math.Abs(this.Y - v.Y) < double.Epsilon && Math.Abs(this.Z - v.Z) < double.Epsilon;
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
                return hash;
            }
        }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", this.X);
            info.AddValue("Y", this.Y);
            info.AddValue("Z", this.Z);
        }

        /// <summary>
        /// Inverts the vector.
        /// </summary>
        /// <returns>Returns the inverted vector.</returns>
		[PublicAPI]
        public Vector3d Inverse()
        {
            return new Vector3d(1d / this.X, 1d / this.Y, 1d / this.Z);
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <returns>Returns the normalized vector.</returns>
		[PublicAPI]
        public Vector3d Normalized()
        {
            return this / this.Length;
        }    

		/// <summary>
        /// Adds an offset to all components of this vector.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns the vector with applied offset.</returns>
		[PublicAPI]
		public Vector3d Offset(double offset)
		{
			return new Vector3d(this.X + offset, this.Y + offset, this.Z + offset);
		}

        /// <inheritdoc/>
		[PublicAPI]
        public override string ToString()
        {
            return $"Vector[{this.X}, {this.Y}, {this.Z}]";
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Multiplies two vectors component wise.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector3d ComponentProduct(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        /// <summary>
        /// Calculates the cross product.
        /// </summary>
        /// <returns>Returns the cross product.</returns>
		[PublicAPI]
        public static Vector3d Cross(Vector3d left, Vector3d right)
        {
            var x = left.Y * right.Z - left.Z * right.Y;
            var y = left.Z * right.X - left.X * right.Z;
            var z = left.X * right.Y - left.Y * right.X;

            return new Vector3d(x, y, z);
        }

		/// <summary>
        /// Calculates the bary centric coordinates for the given triangle and UV coordinates
        /// </summary>
		/// <param name="a">The first triangle edge.</param>
		/// <param name="b">The second triangle edge.</param>
		/// <param name="c">The third triangle edge.</param>
		/// <param name="u">The u coordinate.</param>
		/// <param name="v">The v coordinate.</param>
        /// <returns>Returns the bary centric prodct.</returns>
		[PublicAPI]
		public static Vector3d BaryCentric(Vector3d a, Vector3d b, Vector3d c, double u, double v)
		{
			return a + u * (b - a) + v * (c - a);
		}

		/// <summary>
        /// Calculates the angle between the two vectors.
        /// </summary>
		/// <param name="first">The first vector.</param>
		/// <param name="second">The second vector.</param>
        /// <returns>Returns the angle.</returns>
		[PublicAPI]
		public static double Angle(Vector3d first, Vector3d second)
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
        public static double Distance(Vector3d a, Vector3d b)
        {
			return Math.Sqrt(SqDistance(a, b));
        }

        /// <summary>
        /// Calculates the dot product
        /// </summary>
        /// <returns>Returns the dot product</returns>
		[PublicAPI]
        public static double Dot(Vector3d left, Vector3d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        /// <summary>
        /// Lerps the vector between <see cref="from"/> and <see cref="to"/> using the unclamped lerp value.
        /// </summary>
        /// <param name="from">The start vector.</param>
        /// <param name="to">The target vector.</param>
        /// <param name="lerp">The lerp value.</param>
        /// <returns>Returns the lerped vector.</returns>
		[PublicAPI]
        public static Vector3d Lerp(Vector3d from, Vector3d to, double lerp)
        {
            var x = to.X - from.X;
            var y = to.Y - from.Y;
            var z = to.Z - from.Z;
            return new Vector3d(from.X + (lerp * x), from.Y + (lerp * y), from.Z + (lerp * z));
        }

        /// <summary>
        /// Lerps the vector between <see cref="from"/> and <see cref="to"/> using the clamped lerp value.
        /// </summary>
        /// <param name="from">The start vector.</param>
        /// <param name="to">The target vector.</param>
        /// <param name="lerp">The lerp value.</param>
        /// <returns>Returns the lerped vector.</returns>
		[PublicAPI]
        public static Vector3d LerpC(Vector3d from, Vector3d to, double lerp)
        {
            lerp = Math.Max(Math.Min(lerp, 1f), 0f);
            var x = to.X - from.X;
            var y = to.Y - from.Y;
            var z = to.Z - from.Z;
            return new Vector3d(from.X + (lerp * x), from.Y + (lerp * y), from.Z + (lerp * z));
        }

		/// <summary>
        /// Clamps the vector to a value between min and max.
        /// </summary>
        /// <param name="vector">The vector to clamp.</param>
        /// <param name="min">The minimum values.</param>
		/// <param name="max">The maximum values.</param>
        /// <returns>Returns the clamped vector.</returns>
		[PublicAPI]
		public static Vector3d Clamp(Vector3d vector, Vector3d min, Vector3d max)
		{
			return Min(Max(vector, min), max);
		}

        /// <summary>
        /// Calculates the vector that contains the component wise maximum.
        /// </summary>
        /// <returns>Returns the maximum component vector.</returns>
		[PublicAPI]
        public static Vector3d Max (Vector3d left, Vector3d right)
        {
            return new Vector3d(Math.Max(left.X, right.X), Math.Max(left.Y, right.Y), Math.Max(left.Z, right.Z));
        }

		/// <summary>
        /// Finds the vector that is longer.
        /// </summary>
        /// <returns>Returns the longer vector.</returns>
		[PublicAPI]
		public static Vector3d LengthMax(Vector3d left, Vector3d right)
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
        public static Vector3d Min (Vector3d left, Vector3d right)
        {
            return new Vector3d(Math.Min(left.X, right.X), Math.Min(left.Y, right.Y), Math.Min(left.Z, right.Z));
        }

		/// <summary>
        /// Finds the vector that is shorter.
        /// </summary>
        /// <returns>Returns the shorter vector.</returns>
		[PublicAPI]
		public static Vector3d LengthMin(Vector3d left, Vector3d right)
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
        public static double SqDistance(Vector3d a, Vector3d b)
        {
            var deltaX = b.X - a.X;
            var deltaY = b.Y - a.Y;
            var deltaZ = b.Z - a.Z;

            return deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
        }

        #endregion

        #region Static Cast Methods

        /// <summary>
        /// Converts the vector explicitly to a Vector2f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector2(Vector3d v)
        {
            return new Vector2((float)v.X, (float)v.Y);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector2d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector2d(Vector3d v)
        {
            return new Vector2d(v.X, v.Y);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector3f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector3(Vector3d v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }

        /// <summary>
        /// Converts the vector explicitly to a Vector4f.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static explicit operator Vector4(Vector3d v)
        {
            return new Vector4((float)v.X, (float)v.Y, (float)v.Z);
        }

        /// <summary>
        /// Converts the vector implicitly to a Vector4d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator Vector4d(Vector3d v)
        {
            return new Vector4d(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Converts a vector2 implicitly to a Vector3d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static implicit operator Vector3d(Vector2 v)
        {
            return new Vector3d(v.x, v.y, 0d);
        }

        /// <summary>
        /// Converts a vector3 implicitly to a Vector3d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static implicit operator Vector3d(Vector3 v)
        {
            return new Vector3d(v.x, v.y, v.z);
        }

        /// <summary>
        /// Converts a vector4 explicitly to a Vector3d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector3d(Vector4 v)
        {
            return new Vector3d(v.x, v.y, v.z);
        }

        #endregion

        #region Static Operator Methods

        /// <summary>
        /// Evaluates whether two <see cref="Vector3d"/> are equal.
        /// </summary>
        /// <returns>Returns <c>true</c> when the vectors are component wise equal, <c>false</c> otherwise.</returns>
		[PublicAPI]
        public static bool operator == (Vector3d left, Vector3d right)
        {
            return CommonUtil.Equals(left.X, right.X) && CommonUtil.Equals(left.Y, right.Y) && CommonUtil.Equals(left.Z, right.Z);
        }

        /// <summary>
        /// Evaluates whether two <see cref="Vector3d"/> are inequal.
        /// </summary>
        /// <returns>Returns <c>true</c> when at least one component is not equal, <c>false</c> otherwise.</returns>
		[PublicAPI]
        public static bool operator != (Vector3d left, Vector3d right)
        {
            return !CommonUtil.Equals(left.X, right.X) || !CommonUtil.Equals(left.Y, right.Y) || !CommonUtil.Equals(left.Z, right.Z);
        }

        /// <summary>
        /// Sums two <see cref="Vector3d"/>.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector3d operator + (Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }		

        /// <summary>
        /// Subtracts two <see cref="Vector3d"/>.
        /// </summary>
        /// <returns>Returns the resulting vector.</returns>
		[PublicAPI]
        public static Vector3d operator - (Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

		/// <summary>
		/// Negates the vector.
		/// </summary>
		/// <returns>Returns the negated vector.</returns>
		[PublicAPI]
		public static Vector3d operator - (Vector3d vector)
		{
			return new Vector3d(-vector.X, -vector.Y, -vector.Z);
		}

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector3d"/>.
        /// </summary>
        /// <returns>Returns the dot product.</returns>
		[PublicAPI]
        public static double operator * (Vector3d left, Vector3d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        /// <summary>
        /// Scales a <see cref="Vector3d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector3d operator * (Vector3d left, double right)
        {
            return new Vector3d(left.X * right, left.Y * right, left.Z * right);
        }

        /// <summary>
        /// Scales a <see cref="Vector3d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector3d operator * (double left, Vector3d right)
        {
            return new Vector3d(left * right.X, left * right.Y, left * right.Z);
        }

        /// <summary>
        /// CompInv scales a <see cref="Vector3d"/>.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
        public static Vector3d operator / (Vector3d left, double right)
        {
            return new Vector3d(left.X / right, left.Y / right, left.Z / right);
        }

        #endregion
    }
}
