using Unity_Tools.Collections;
using Unity_Tools.Collections.SpatialTree;
using UnityEngine;

namespace Unity_Tools
{
    public static class Spatial3DTreeVisualizer
    {
        private static Color[] LevelColors;

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
            if (cell.Children != null)
            {
                foreach (var child in cell.Children)
                {
                    if (child != null)
                    {
                        DrawTreeCellGizmos<T>(child, currentDepth + 1);
                    }
                }
            }

            Gizmos.color = LevelColors[Mathf.Clamp(currentDepth, 0, 15)];
            Gizmos.DrawWireCube(cell.Center, cell.Size);
        }
    }
}
