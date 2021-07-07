// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CameraUtil.cs
// 
// Created:          23.06.2021  23.39
// Last modified:    23.06.2021  23.39
// 
// --------------------------------------------------------------------------------------
// 
// MIT License
// 
// Copyright (c) 2021 chillersanim
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

using UnityEngine;

namespace UnityTools.Core
{
    public static class ColorUtil
    {
        /// <summary>
        /// Changes the red channel of the given color, the other channels are taken from the input color.
        /// </summary>
        /// <param name="color">The input color.</param>
        /// <param name="red">The new value for the red channel.</param>
        public static Color SetR(in this Color color, in float red)
        {
            return new Color(red, color.g, color.b, color.a);
        }

        /// <summary>
        /// Changes the green channel of the given color, the other channels are taken from the input color.
        /// </summary>
        /// <param name="color">The input color.</param>
        /// <param name="green">The new value for the green channel.</param>
        public static Color SetG(in this Color color, in float green)
        {
            return new Color(color.r, green, color.b, color.a);
        }

        /// <summary>
        /// Changes the blue channel of the given color, the other channels are taken from the input color.
        /// </summary>
        /// <param name="color">The input color.</param>
        /// <param name="blue">The new value for the blue channel.</param>
        public static Color SetB(in this Color color, in float blue)
        {
            return new Color(color.r, color.g, blue, color.a);
        }

        /// <summary>
        /// Changes the alpha channel of the given color, the other channels are taken from the input color.
        /// </summary>
        /// <param name="color">The input color.</param>
        /// <param name="alpha">The new value for the alpha channel.</param>
        public static Color SetA(in this Color color, in float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
