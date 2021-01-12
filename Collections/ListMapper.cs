// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         ListMapper.cs
// 
// Created:          07.01.2020  16:25
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
using System.Collections;
using System.Collections.Generic;

namespace Unity_Tools.Collections
{
    /// <summary>
    /// A list mapper that changes the interface using a mapping function
    /// </summary>
    /// <typeparam name="TIn">The item type of the source collection.</typeparam>
    /// <typeparam name="TOut">The item type of the mapped list.</typeparam>
    public class ListMapper<TIn, TOut> : IList<TOut>
    {
        private readonly IList<TIn> source;

        private Func<TIn, TOut> mapping;

        private Func<TOut, TIn> remapping;

        /// <summary>
        /// Initialized a new ListMapper
        /// </summary>
        /// <param name="source">The source collection. [Not null]</param>
        /// <param name="mapping">The item mapping. [Not null]</param>
        public ListMapper(IList<TIn> source, Func<TIn, TOut> mapping)
        {
            if (this.source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (this.mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            this.source = source;
            this.mapping = mapping;
            this.remapping = null;
        }

        /// <summary>
        /// Initialized a new ListMapper
        /// </summary>
        /// <param name="source">The source collection. [Not null]</param>
        /// <param name="mapping">The item mapping. [Not null]</param>
        /// <param name="reverseMapping">The reverse item mapping that allows modifications of the source using this list mapper. [Can be null]</param>
        public ListMapper(IList<TIn> source, Func<TIn, TOut> mapping, Func<TOut, TIn> reverseMapping)
        {
            if (this.source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (this.mapping == null)
            {
                throw new ArgumentNullException(nameof(mapping));
            }

            this.source = source;
            this.mapping = mapping;
            this.remapping = reverseMapping;
        }

        public Func<TIn, TOut> Mapping
        {
            get => mapping;
            set => mapping = value ?? throw new ArgumentNullException();
        }

        public Func<TOut, TIn> ReverseMapping
        {
            get => remapping;
            set => remapping = value ?? throw new ArgumentNullException();
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            foreach(var item in source)
            {
                yield return mapping(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TOut item)
        {
            if(remapping == null)
            {
                throw new NotSupportedException("Adding or replacing items in the mapped collection is not supported unless a reverse mapping is provided.");
            }

            source.Add(remapping(item));
        }

        public void Clear()
        {
            this.source.Clear();
        }

        public bool Contains(TOut item)
        {
            return IndexOf(item) < 0;
        }

        public void CopyTo(TOut[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Array is null.");
            }

            if(arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "ArrayIndex is less than 0.");
            }

            if (array.Length < arrayIndex + this.Count)
            {
                throw new ArgumentException(
                    "The number of elements in the source ICollection<T> is greater than the available space from arrayIndex to the end of the destination array.");
            }

            foreach(var item in source)
            {
                array[arrayIndex++] = mapping(item);
            }
        }

        public bool Remove(TOut item)
        {
            var index = IndexOf(item);
            if(index < 0)
            {
                return false;
            }

            source.RemoveAt(index);
            return true;
        }

        public int Count => source.Count;
        public bool IsReadOnly => source.IsReadOnly || remapping == null;

        public int IndexOf(TOut item)
        {
            if (remapping != null)
            {
                var sourceItem = remapping(item);
                return source.IndexOf(sourceItem);
            }

            for (var i = 0; i < source.Count; i++)
            {
                if (Equals(mapping(source[i]), item))
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, TOut item)
        {
            if (remapping == null)
            {
                throw new NotSupportedException("Adding or replacing items in the mapped collection is not supported unless a reverse mapping is provided.");
            }

            source.Insert(index, remapping(item));
        }

        public void RemoveAt(int index)
        {
            source.RemoveAt(index);
        }

        public TOut this[int index]
        {
            get => mapping(source[index]);
            set
            {
                if (remapping == null)
                {
                    throw new NotSupportedException("Adding or replacing items in the mapped collection is not supported unless a reverse mapping is provided.");
                }

                source[index] = remapping(value);
            }
        }
    }
}
