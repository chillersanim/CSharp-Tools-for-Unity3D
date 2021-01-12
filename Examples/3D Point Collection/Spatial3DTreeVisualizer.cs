// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         Spatial3DTreeVisualizer.cs
// 
// Created:          27.01.2020  22:47
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

using Unity_Tools.Collections;
using Unity_Tools.Collections.Internals;
using UnityEngine;

namespace Unity_Tools.Examples
{
    public static class Spatial3DTreeVisualizer
    {
        private static readonly Color[] LevelColors;

        static Spatial3DTreeVisualizer()
        {
            LevelColors = new Color[16];

            for (var i = 0; i < 3; i++)
            {
                LevelColors[i] = Color.Lerp(Color.blue, Color.green, i / 3f);
            }

            for (var i = 0; i < 3; i++)
            {
                LevelColors[i + 3] = Color.Lerp(Color.green, Color.yellow, i / 3f);
            }

            for (var i = 0; i < 3; i++)
            {
                LevelColors[i + 6] = Color.Lerp(Color.yellow, Color.red, i / 3f);
            }

            for (var i = 0; i < 3; i++)
            {
                LevelColors[i+9] = Color.Lerp(Color.red, Color.magenta, i / 3f);
            }

            for (var i = 0; i < 3; i++)
            {
                LevelColors[i + 12] = Color.Lerp(Color.magenta, Color.grey, i / 3f);
            }

            LevelColors[15] = Color.black;
        }

        public static void DrawTreeGizmos<T>(Spatial3DTree<T> tree)
        {
            if (tree == null)
            {
                return;
            }

            DrawTreeCellGizmos(tree.Root, 0);
        }

        private static void DrawTreeCellGizmos<T>(Spatial3DCell<T> cell, int currentDepth)
        {
            Gizmos.color = LevelColors[Mathf.Clamp(currentDepth, 0, 15)];
            Gizmos.DrawWireCube(cell.Center, cell.Size);

            if (cell.Children != null)
            {
                foreach (var child in cell.Children)
                {
                    if (child != null)
                    {
                        DrawTreeCellGizmos(child, currentDepth + 1);
                    }
                }
            }
        }
    }
}
