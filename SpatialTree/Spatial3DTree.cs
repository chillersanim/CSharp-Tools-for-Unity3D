// Copyright © 2019 Jasper Ermatinger

#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Unity_Collections.SpatialTree.Enumerators;

#endregion

namespace Unity_Collections.SpatialTree
{
    #region Usings

    #endregion

    /// <summary>
    ///     The spatial 3 d tree.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class Spatial3DTree<T> : IEnumerable<T>
        where T : class
    {
        /// <summary>
        ///     The enumerator.
        /// </summary>
        [NotNull] private readonly SphereCastEnumerator<T> enumerator;

        /// <summary>
        ///     The initial offset.
        /// </summary>
        public Vector3 InitialOffset;

        /// <summary>
        ///     The initial size.
        /// </summary>
        public Vector3 InitialSize;

        /// <summary>
        ///     The root.
        /// </summary>
        [NotNull] public Spatial3DCell<T> Root;

        /// <summary>
        ///     The shape enumerator.
        /// </summary>
        [CanBeNull] private ShapeCastEnumerator<T> shapeEnumerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Spatial3DTree{T}" /> class.
        /// </summary>
        public Spatial3DTree()
        {
            InitialSize = new Vector3(3, 3, 3);
            InitialOffset = Vector3.zero;
            Root = Spatial3DCell<T>.GetCell(InitialOffset - InitialSize / 2f, InitialSize);
            enumerator = new SphereCastEnumerator<T>(this, Vector3.zero, 1f);
        }

        /// <summary>
        ///     The count.
        /// </summary>
        public int Count => Root.TotalItemAmount;

        /// <summary>
        ///     The depth.
        /// </summary>
        public int Depth => Root.GetDepth();

        /// <summary>
        ///     The total cell count.
        /// </summary>
        public int TotalCellCount => Root.GetCellCount();

        /// <summary>
        ///     The get enumerator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Spatial3DTreeEnumerator<T>(this);
        }

        /// <summary>
        ///     The get enumerator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        public void Add(T item, Vector3 position)
        {
            while (!FitsInRoot(position)) Grow();

            Root.AddItem(item, position, 1);
        }

        /// <summary>
        ///     The clear.
        /// </summary>
        public void Clear()
        {
            Spatial3DCell<T>.Pool(Root);
            Root = Spatial3DCell<T>.GetCell(InitialOffset - InitialSize / 2, InitialSize);
        }

        /// <summary>
        ///     The contains.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Contains(T item, Vector3 position)
        {
            return Root.Contains(item, position);
        }

        /// <summary>
        ///     The move item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="from">
        ///     The from.
        /// </param>
        /// <param name="to">
        ///     The to.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool MoveItem(T item, Vector3 from, Vector3 to)
        {
            while (!FitsInRoot(to)) Grow();

            var result = Root.MoveItem(item, from, to, 1);

            while (CanShrink()) Shrink();

            return result;
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Remove(T item, Vector3 position)
        {
            var result = Root.RemoveItem(item, position);

            while (CanShrink()) Shrink();

            return result;
        }

        /// <summary>
        ///     The shape cast enumerator.
        /// </summary>
        /// <param name="shape">
        ///     The shape.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator<T> ShapeCastEnumerator(IShape shape)
        {
            return new ShapeCastEnumerator<T>(this, shape);
        }

        /// <summary>
        ///     The sphere cast.
        /// </summary>
        /// <param name="center">
        ///     The center.
        /// </param>
        /// <param name="radius">
        ///     The radius.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IList<T> SphereCast(Vector3 center, float radius)
        {
            var result = new List<T>();
            enumerator.Restart(center, radius);

            while (enumerator.MoveNext()) result.Add(enumerator.Current);

            return result;
        }

        /// <summary>
        ///     The sphere cast.
        /// </summary>
        /// <param name="center">
        ///     The center.
        /// </param>
        /// <param name="radius">
        ///     The radius.
        /// </param>
        /// <param name="result">
        ///     The result.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public int SphereCast(Vector3 center, float radius, T[] result)
        {
            if (result == null) throw new ArgumentNullException();

            if (result.Length == 0) return 0;

            var length = 0;
            enumerator.Restart(center, radius);

            while (length < result.Length && enumerator.MoveNext())
            {
                result[length] = enumerator.Current;
                length++;
            }

            return length;
        }

        /// <summary>
        ///     The sphere cast.
        /// </summary>
        /// <param name="center">
        ///     The center.
        /// </param>
        /// <param name="radius">
        ///     The radius.
        /// </param>
        /// <param name="result">
        ///     The result.
        /// </param>
        public void SphereCast(Vector3 center, float radius, IList<T> result)
        {
            result.Clear();
            enumerator.Restart(center, radius);

            while (enumerator.MoveNext()) result.Add(enumerator.Current);
        }

        /// <summary>
        ///     The sphere cast enumerator.
        /// </summary>
        /// <param name="center">
        ///     The center.
        /// </param>
        /// <param name="radius">
        ///     The radius.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator<T> SphereCastEnumerator(Vector3 center, float radius)
        {
            return new SphereCastEnumerator<T>(this, center, radius);
        }

        /// <summary>
        ///     The can shrink.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool CanShrink()
        {
            if (Root.Size.x <= InitialSize.x) return false;

            if (Root.Children == null)
            {
                var sc = Spatial3DCell<T>.SubdivisionAmount;
                var centerSize = Root.Size / sc;
                var centerStart = Root.Start + centerSize * (sc / 2);

                for (var i = 0; i < Root.Items.Count; i++)
                    if (!FitsInAabb(Root.Items[i].Position, centerStart, centerSize))
                        return false;

                return true;
            }

            var middle = Root.Children.Length / 2;

            if (Root.Children[middle] == null) return false;

            for (var i = 0; i < Root.Children.Length; i++)
                if (i != middle && Root.Children[i] != null && Root.Children[i].TotalItemAmount != 0)
                    return false;

            return true;
        }

        /// <summary>
        ///     The fits in aabb.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <param name="start">
        ///     The start.
        /// </param>
        /// <param name="end">
        ///     The end.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool FitsInAabb(Vector3 position, Vector3 start, Vector3 end)
        {
            return start.x <= position.x && start.y <= position.y && start.z <= position.z && end.x > position.x
                   && end.y > position.y && end.z > position.z;
        }

        /// <summary>
        ///     The fits in root.
        /// </summary>
        /// <param name="position">
        ///     The position.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool FitsInRoot(Vector3 position)
        {
            return FitsInAabb(position, Root.Start, Root.End);
        }

        /// <summary>
        ///     The grow.
        /// </summary>
        private void Grow()
        {
            var sc = Spatial3DCell<T>.SubdivisionAmount;
            var start = Root.Start - sc / 2 * Root.Size;
            var size = Root.Size * sc;

            if (Root.Children == null)
            {
                Root.Start = start;
                Root.Size = size;
            }
            else
            {
                var newRoot = Spatial3DCell<T>.GetCell(start, size);
                newRoot.TotalItemAmount = Root.TotalItemAmount;
                newRoot.Children = Spatial3DCell<T>.GetChildArray();
                newRoot.Children[newRoot.Children.Length / 2] = Root;
                Root = newRoot;
            }
        }

        /// <summary>
        ///     The shrink.
        /// </summary>
        private void Shrink()
        {
            Debug.Assert(CanShrink(), "The tree cannot be shrunk.");

            if (Root.Children == null)
            {
                var sc = Spatial3DCell<T>.SubdivisionAmount;
                Root.Size = Root.Size / sc;
                Root.Start = Root.Start + Root.Size * (sc / 2);
            }
            else
            {
                var middle = Root.Children.Length / 2;
                var center = Root.Children[middle];
                Root.Children[middle] = null;
                Spatial3DCell<T>.Pool(Root);
                Root = center;

                Debug.Assert(center != null);
            }
        }
    }
}