// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         VectorNd.cs
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
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace UnityTools.Core
{
    /// <summary>
    /// Container for N component vector data using doubles.
    /// </summary>
    [Serializable]
    public struct VectorNd : IVectorD, ISerializable
    {
        #region Constants

        /// <summary>
        /// The zero vector.
        /// </summary>
		[PublicAPI]
        public static readonly VectorNd Zero = new VectorNd(0);

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the values of this vector.
        /// </summary>
        private readonly double[] values;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VectorNd"/> class.
        /// </summary>
        /// <param name="values">
        /// The values of the vector, will be copied.
        /// </param>
        [PublicAPI]
		public VectorNd(params double[] values)
        {
            if(values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if(values.Length == 0)
            {
                throw new ArgumentException("A vector needs at least one dimension.", nameof(values));
            }

            // Copy the values
            this.values = new double[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                this.values[i] = values[i];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VectorNd"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The streaming context.
        /// </param>
        private VectorNd(SerializationInfo info, StreamingContext context)
        {
            this.values = (double[])info.GetValue("values", typeof(double[]));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VectorNd"/> class.
        /// </summary>
        /// <param name="values">
        /// The values of the vector, will be used directly.
        /// </param>
        internal VectorNd(ref double[] values)
        {
            this.values = values;
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

                if (index >= this.values.Length)
                {
                    return 0;
                }

                return this.values[index];
            }
            set
            {
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("index", "The index must be positive.");
                }

                if (index >= this.values.Length)
                {
                    throw new ArgumentOutOfRangeException("index", "The index must be less than the dimension.");
                }

                this.values[index] = value;
            }
        }

        /// <inheritdoc/>
		[PublicAPI]
        public int Dimension
        {
            get
            {
                return this.values.Length;
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
                double result = 0;
                for (var i = 0; i < this.values.Length; i++)
                {
                    var value = this.values[i];
                    result += value * value;
                }

                return result;
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
                var dim = this.Dimension;
                var result = new double[dim];
                for (var i = 0; i < dim; i++)
                {
                    result[i] = this.values[i] + other[i];
                }

                return new VectorNd(ref result);
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
                var dim = this.Dimension;
                var result = new double[dim];
                for (var i = 0; i < dim; i++)
                {
                    result[i] = this.values[i] - other[i];
                }

                return new VectorNd(ref result);
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

            if (other.Dimension < this.Dimension)
            {
                return other.Dot(this);
            }

            var dim = this.Dimension;
            var result = 0.0;
            for (var i = 0; i < dim; i++)
            {
                result += this.values[i] * other[i];
            }

            return result;
        }

        /// <inheritdoc/>
		[PublicAPI]
		public bool IsNaN
        {
            get
            {
                for (var i = 0; i < this.values.Length; i++)
                {
                    if (double.IsNaN(this.values[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

		/// <inheritdoc/>
		[PublicAPI]
        public bool IsInfinity
        {
            get
            {
                for (var i = 0; i < this.values.Length; i++)
                {
                    if (double.IsInfinity(this.values[i]))
                    {
                        return true;
                    }
                }

                return false;
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
        public VectorNd CompInv()
        {
            var newValues = new double[this.values.Length];
            for (var i = 0; i < this.values.Length; i++)
            {
                newValues[i] = 1d / this.values[i];
            }

            return new VectorNd(ref newValues);
        }

        /// <summary>
        /// Inverts the vector.
        /// </summary>
        /// <returns>The resulting vector.</returns>
        [PublicAPI]
        public VectorNd Inv()
        {
            var newValues = new double[this.values.Length];
            var length = this.Length;
            for (var i = 0; i < this.values.Length; i++)
            {
                newValues[i] = this.values[i] / length;
            }

            return new VectorNd(ref newValues);
        }

        /// <summary>
        /// Makes all components positive
        /// </summary>
        /// <returns></returns>
		[PublicAPI]
        public VectorNd Abs()
        {
            var newValues = new double[this.values.Length];
            for (var i = 0; i < this.values.Length; i++)
            {
                newValues[i] = Math.Abs(this.values[i]);
            }

            return new VectorNd(ref newValues);
        }

        /// <summary>
        /// Makes all components negative
        /// </summary>
        /// <returns></returns>
		[PublicAPI]
        public VectorNd NegAbs()
        {
            var newValues = new double[this.values.Length];
            for (var i = 0; i < this.values.Length; i++)
            {
                newValues[i] = -Math.Abs(this.values[i]);
            }

            return new VectorNd(ref newValues);
        }

        /// <inheritdoc/>
		[PublicAPI]
        public override int GetHashCode()
        {
            return CommonUtil.HashCode(this.values);            
        }

        /// <inheritdoc/>
		[PublicAPI]
        public override string ToString()
        {
            var builder = new StringBuilder("VectorNd[");
            for (var i = 0; i < this.values.Length; i++)
            {
                builder.Append(this.values[i]);
                if (i < this.values.Length - 1)
                {
                    builder.Append(',');
                    builder.Append(' ');
                }
            }

            return builder.ToString();
        }

        /// <inheritdoc/>
		[PublicAPI]
        public override bool Equals(object obj)
        {
            var vo = obj as VectorNd?;

            if (!vo.HasValue)
            {
                return false;
            }

            var v = vo.Value;
            var dim = Math.Max(this.Dimension, v.Dimension);

            for (var i = 0; i < dim; i++)
            {
                if (CommonUtil.Equals(this[i], v[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("values", this.values);
        }        

        #endregion

        #region Static Methods

        /// <summary>
        /// Calculates the dot product or two vectors.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <returns>Returns the dot product.</returns>
		[PublicAPI]
        public static double DotP(VectorNd left, VectorNd right)
        {
            double result = 0;
            var dim = Math.Min(left.Dimension, right.Dimension);

            for (var i = 0; i < dim; i++)
            {
                result += left[i] * right[i];
            }

            return result;
        }

        /// <summary>
        /// Calculates the minimum for all components of both vectors.
        /// </summary>
        /// <param name="left"> The left vector. </param>
        /// <param name="right"> The right vector. </param>
        /// <returns>Returns the minimum component vector.</returns>
		[PublicAPI]
        public static VectorNd Max(VectorNd left, VectorNd right)
        {
            var dim = Math.Max(left.Dimension, right.Dimension);
            var result = new double[dim];

            for (var i = 0; i < dim; i++)
            {
                result[i] = Math.Max(left[i], right[i]);
            }

            return new VectorNd(ref result);
        }

        /// <summary>
        /// Calculates the maximum for all components of both vectors.
        /// </summary>
        /// <param name="left"> The left vector. </param>
        /// <param name="right"> The right vector. </param>
        /// <returns>Returns the maximum component vector.</returns>
		[PublicAPI]
        public static VectorNd Min(VectorNd left, VectorNd right)
        {
            var dim = Math.Max(left.Dimension, right.Dimension);
            var result = new double[dim];

            for (var i = 0; i < dim; i++)
            {
                result[i] = Math.Min(left[i], right[i]);
            }

            return new VectorNd(ref result);
        }

        #endregion

        #region Static Casts

        /// <summary>
        /// Converts the <see cref="Vector2"/> to a <see cref="VectorNd"/> implicitly.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator VectorNd (Vector2 v)
        {
            var values = new double[2];
            values[0] = v.x;
            values[1] = v.y;
            return new VectorNd(ref values);
        }

        /// <summary>
        /// Converts the <see cref="Vector2d"/> to a <see cref="VectorNd"/> implicitly.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator VectorNd (Vector2d v)
        {
            var values = new double[2];
            values[0] = v.X;
            values[1] = v.Y;
            return new VectorNd(ref values);
        }

        /// <summary>
        /// Converts the <see cref="Vector3"/> to a <see cref="VectorNd"/> implicitly.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator VectorNd (Vector3 v)
        {
            var values = new double[3];
            values[0] = v.x;
            values[1] = v.y;
            values[2] = v.z;
            return new VectorNd(ref values);
        }

        /// <summary>
        /// Converts the <see cref="Vector3d"/> to a <see cref="VectorNd"/> implicitly.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator VectorNd (Vector3d v)
        {
            var values = new double[3];
            values[0] = v.X;
            values[1] = v.Y;
            values[2] = v.Z;
            return new VectorNd(ref values);
        }

        /// <summary>
        /// Converts the <see cref="Vector4"/> to a <see cref="VectorNd"/> implicitly.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator VectorNd (Vector4 v)
        {
            var values = new double[4];
            values[0] = v.x;
            values[1] = v.y;
            values[2] = v.z;
            values[3] = v.w;
            return new VectorNd(ref values);
        }

        /// <summary>
        /// Converts the <see cref="Vector4d"/> to a <see cref="VectorNd"/> implicitly.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator VectorNd (Vector4d v)
        {
            var values = new double[4];
            values[0] = v.X;
            values[1] = v.Y;
            values[2] = v.Z;
            values[3] = v.W;
            return new VectorNd(ref values);
        }

        /// <summary>
        /// Converts the <see cref="VectorNf"/> to a <see cref="VectorNd"/> implicitly.
        /// </summary>
        /// <param name="v">The vector to convert.</param>
        /// <returns>Returns the converted vector.</returns>
		[PublicAPI]
        public static implicit operator VectorNd (VectorNf v)
        {
            var values = new double[v.Dimension];
            for (var i = 0; i < v.Dimension; i++)
            {
                values[i] = v[i];            
            }

            return new VectorNd(ref values);
        }

        /// <summary>
        /// Converts a vectorNd implicitly to a Vector2d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector2d(VectorNd v)
        {
            return new Vector2d(v[0], v[1]);
        }

        /// <summary>
        /// Converts a vectorNd implicitly to a Vector3d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector3d(VectorNd v)
        {
            return new Vector3d(v[0], v[1], v[2]);
        }

        /// <summary>
        /// Converts a vectorNd implicitly to a Vector4d.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector4d(VectorNd v)
        {
            return new Vector4d(v[0], v[1], v[2], v[3]);
        }

        /// <summary>
        /// Converts a vectorNd implicitly to a Vector2.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector2(VectorNd v)
        {
            return new Vector2((float)v[0], (float)v[1]);
        }

        /// <summary>
        /// Converts a vectorNd implicitly to a Vector3.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector3(VectorNd v)
        {
            return new Vector3((float)v[0], (float)v[1], (float)v[2]);
        }

        /// <summary>
        /// Converts a vectorNd implicitly to a Vector4.
        /// </summary>
        /// <returns>Returns the converted vector.</returns>
        [PublicAPI]
        public static explicit operator Vector4(VectorNd v)
        {
            return new Vector4((float)v[0], (float)v[1], (float)v[2], (float)v[3]);
        }

        #endregion

        #region Static Operators

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <returns>Returns the sum of both vectors.</returns>
        [PublicAPI]
		public static VectorNd operator +(VectorNd left, VectorNd right)
		{
			var dim = Math.Max(left.Dimension, right.Dimension);
			var values = new double[dim];
            for (var i = 0; i < dim; i++)
            {
                values[i] = left.values[i] + right.values[i];
            }

            return new VectorNd(ref values);
		}

		/// <summary>
        /// Substracts two vectors.
        /// </summary>
        /// <returns>Returns the difference of both vectors.</returns>
		[PublicAPI]
		public static VectorNd operator -(VectorNd left, VectorNd right)
		{
			var dim = Math.Max(left.Dimension, right.Dimension);
			var values = new double[dim];
            for (var i = 0; i < dim; i++)
            {
                values[i] = left.values[i] - right.values[i];
            }

            return new VectorNd(ref values);
		}

		/// <summary>
        /// Negates the vector.
        /// </summary>
        /// <returns>Returns the negative vector.</returns>
		[PublicAPI]
		public static VectorNd operator -(VectorNd v)
		{
			var values = new double[v.Dimension];
            for (var i = 0; i < v.Dimension; i++)
            {
                values[i] = -v.values[i];
            }

            return new VectorNd(ref values);
		}

		/// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <returns>Returns the dot product.</returns>
		[PublicAPI]
		public static double operator *(VectorNd left, VectorNd right)
		{
			return DotP(left, right);
		}

		/// <summary>
        /// Scales a vector.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		public static VectorNd operator *(VectorNd left, double right)
		{		
			var values = new double[left.Dimension];
            for (var i = 0; i < left.Dimension; i++)
            {
                values[i] = left.values[i] * right;
            }

            return new VectorNd(ref values);
		}

		/// <summary>
        /// Scales a vector.
        /// </summary>
        /// <returns>Returns the scaled vector.</returns>
		[PublicAPI]
		public static VectorNd operator *(double left, VectorNd right)
		{
			return right * left;
		}

		/// <summary>
        /// CompInv scales a vector.
        /// </summary>
        /// <returns>Returns the inverse scaled vector.</returns>
		[PublicAPI]
		public static VectorNd operator /(VectorNd left, double right)
		{
			return left * (1 / right);
		}

		/// <summary>
        /// Tests two vectors for equality.
        /// </summary>
		/// <remarks>Only the components are checked, if the dimensions don't match, then the vectors are still considered equals when all extending dimension values of the larger vector are zero.</remarks>
        /// <returns>Returns <c>true</c> if both vectors are equal, <c>false</c> otherwise..</returns>
		[PublicAPI]
        public static bool operator == (VectorNd left, VectorNd right)
        {
            var dim = Math.Max(left.Dimension, right.Dimension);

            for (var i = 0; i < dim; i++)
            {
                if (!CommonUtil.Equals(left[i], right[i]))
                {
                    return false;
                }
            }

            return true;
        }

		/// <summary>
        /// Tests two vectors for inequality.
        /// </summary>
		/// <remarks>Only the components are checked, if the dimensions don't match, then the vectors are still considered equals when all extending dimension values of the larger vector are zero.</remarks>
        /// <returns>Returns <c>true</c> if both vectors are inequal, <c>false</c> otherwise..</returns>
		[PublicAPI]
        public static bool operator != (VectorNd left, VectorNd right)
        {
            var dim = Math.Max(left.Dimension, right.Dimension);

            for (var i = 0; i < dim; i++)
            {
                if (!CommonUtil.Equals(left[i], right[i]))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
