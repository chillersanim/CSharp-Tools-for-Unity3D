// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         PointPlacement.cs
// 
// Created:          23.08.2019  13:36
// Last modified:    25.08.2019  15:59
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

using Unity_Tools.Core;
using UnityEngine;

namespace Unity_Tools.Examples
{
    public class PointPlacement : MonoBehaviour
    {
        public int Amount = 1000;

        public Vector3 max = Vector3.one * 10;
        public Vector3 min = Vector3.one * -10;

        public GameObject Prefab;

        // Start is called before the first frame update
        void OnEnable()
        {
            if (Prefab == null)
            {
                return;
            }

            for (var i = 0; i < Amount; i++)
            {
                var position = new Vector3(Random.value, Random.value, Random.value);
                position = min + position.ScaleComponents((max - min));

                Instantiate(Prefab, position, Random.rotation, this.transform);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(min + (max - min) / 2f, max - min);
        }
    }
}
