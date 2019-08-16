// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PP_FilterByAnyComponents.cs
// 
// Created:          09.08.2019  15:48
// Last modified:    16.08.2019  16:31
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
// 
#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

#endregion

namespace Unity_Tools.Pipeline.Specialized
{
    #region Usings

    #endregion

    /// <summary>
    ///     The p p_ filter by any components.
    /// </summary>
    public sealed class PP_FilterByAnyComponents : PipelineFilter<GameObject>
    {
        /// <summary>
        ///     The types.
        /// </summary>
        [NotNull] private readonly Type[] types;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PP_FilterByAnyComponents" /> class.
        /// </summary>
        /// <param name="types">
        ///     The types.
        /// </param>
        public PP_FilterByAnyComponents(params Type[] types)
        {
            this.types = types ?? Array.Empty<Type>();
        }

        /// <summary>
        ///     The join.
        /// </summary>
        /// <param name="filters">
        ///     The filters.
        /// </param>
        /// <returns>
        ///     The <see cref="PP_FilterByAnyComponents" />.
        /// </returns>
        public static PP_FilterByAnyComponents Join(params PP_FilterByAnyComponents[] filters)
        {
            var types = new HashSet<Type>();

            foreach (var filter in filters)
            {
                foreach (var type in filter.types)
                {
                    if (!types.Contains(type))
                    {
                        types.Add(type);
                    }
                }
            }

            return new PP_FilterByAnyComponents(types.ToArray());
        }

        /// <summary>
        ///     The test.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        protected override bool Test(GameObject item)
        {
            if (item == null)
            {
                return false;
            }

            foreach (var type in types)
            {
                if (item.GetComponent(type) != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}