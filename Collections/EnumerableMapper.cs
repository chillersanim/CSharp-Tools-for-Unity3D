// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         EnumerableMapper.cs
// 
// Created:          07.01.2020  16:47
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

namespace UnityTools.Collections
{
    public class EnumerableMapper<TIn, TOut> : IEnumerable<TOut>
    {
        private readonly IEnumerable<TIn> source;

        private Func<TIn, TOut> mapping;

        public EnumerableMapper(IEnumerable<TIn> source, Func<TIn, TOut> mapping)
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
        }

        public Func<TIn, TOut> Mapping
        {
            get => mapping;
            set => mapping = value ?? throw new ArgumentNullException();
        }

        public IEnumerator<TOut> GetEnumerator()
        {
            foreach (var item in source)
            {
                yield return mapping(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
