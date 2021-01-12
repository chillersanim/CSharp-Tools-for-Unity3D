using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DataPostProcessor.External.UnityTools.Collections
{
    /// <summary>
    /// A data structure that stores the content of a fixed size grid in 3 dimensions.<br/>
    /// Data is stored in x major, then y major then z order
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class Grid3<TValue> : IEnumerable<(TValue value, int x, int y, int z)>
    {
        private readonly TValue[] data;

        private readonly int sizeX, sizeY, sizeZ;

        public int SizeX => this.sizeX;

        public int SizeY => this.sizeY;

        public int SizeZ => this.sizeZ;

        public TValue this[int x, int y, int z]
        {
            get
            {
                if (x < 0 || x >= this.sizeX)
                {
                    throw new ArgumentOutOfRangeException(nameof(x), "The x index must be smaller than SizeX and zero or positive.");
                }

                if (y < 0 || y >= this.sizeY)
                {
                    throw new ArgumentOutOfRangeException(nameof(y), "The y index must be smaller than SizeY and zero or positive.");
                }

                if (z < 0 || z >= this.sizeZ)
                {
                    throw new ArgumentOutOfRangeException(nameof(x), "The z index must be smaller than SizeZ and zero or positive.");
                }

                return this.data[x * (this.sizeY * this.sizeZ) + y * this.sizeZ + z];
            }
            set
            {
                if (x < 0 || x >= this.sizeX)
                {
                    throw new ArgumentOutOfRangeException(nameof(x), "The x index must be smaller than SizeX and zero or positive.");
                }

                if (y < 0 || y >= this.sizeY)
                {
                    throw new ArgumentOutOfRangeException(nameof(y), "The y index must be smaller than SizeY and zero or positive.");
                }

                if (z < 0 || z >= this.sizeZ)
                {
                    throw new ArgumentOutOfRangeException(nameof(x), "The z index must be smaller than SizeZ and zero or positive.");
                }

                this.data[x * (this.sizeY * this.sizeZ) + y * this.sizeZ + z] = value;
            }
        }

        public Grid3(int cubeSize) : this(cubeSize, cubeSize, cubeSize)
        { }

        public Grid3(int sizeX, int sizeY, int sizeZ)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.sizeZ = sizeZ;

            this.data = new TValue[sizeX * sizeY * sizeZ];
        }

        public Grid3(Vector3Int size) : this(size.x, size.y, size.z)
        { }

        public Grid3((int x, int y, int z) size) : this(size.x, size.y, size.z)
        { }

        /// <inheritdoc />
        public IEnumerator<(TValue value, int x, int y, int z)> GetEnumerator()
        {
            var i = 0;

            for (var x = 0; x < this.sizeX; x++)
            {
                for (var y = 0; y < this.sizeY; y++)
                {
                    for (var z = 0; z < this.sizeZ; z++)
                    {
                        yield return (this.data[i], x, y, z);
                        i++;
                    }
                }
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
