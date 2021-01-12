// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         CallController.cs
// 
// Created:          01.02.2020  12:26
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
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityTools.Core
{
    /// <summary>
    /// A helper class that works similar to events but gives more control and is optimized for many listeners
    /// </summary>
    public sealed class CallController
    {
        private const int InitialActiveSize = 64;

        private readonly List<Action> itemsToAdd;
        private readonly List<Action> itemsToRemove;

        private int activeCount;
        private Action[] activeItems;

        private bool keepCallOrder;

        public CallController(bool keepCallOrder = false)
        {
            this.itemsToAdd = new List<Action>();
            this.itemsToRemove = new List<Action>();
            this.activeItems = new Action[InitialActiveSize];
            this.activeCount = 0;
            this.keepCallOrder = keepCallOrder;
        }

        /// <summary>
        /// The amount of registered listeners.
        /// </summary>
        public int Count => activeCount - itemsToRemove.Count + itemsToAdd.Count;

        /// <summary>
        /// Value indicating whether the call order must stay the same (true) or can be shuffled (false).
        /// </summary>
        /// <remarks>
        /// If the calls need to be issued in the same order as components got registered, or if a reordering of the call order would cause problems, this property should be set to <c>true</c>.<br/>
        /// However, if the call order isn't important, you should set it to <c>false</c>, to improve performance when removing call listeners.
        /// </remarks>
        public bool KeepCallOrder
        {
            get => keepCallOrder;
            set => keepCallOrder = value;
        }

        public void AddListener(Action listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            if (int.MaxValue - itemsToAdd.Count < activeCount)
            {
                // In case of overflow (More than 2^31 call listeners wanted to register, there is probably something wrong.)
                throw new Exception("Can't store anymore call listeners, the internal buffer has reached it's limits (2^31 registered listeners).");
            }

            if (itemsToRemove.Contains(listener))
            {
                itemsToRemove.Remove(listener);
            }

            if (itemsToAdd.Contains(listener))
            {
                return;
            }

            itemsToAdd.Add(listener);
        }

        public void Clear()
        {
            this.itemsToAdd.Clear();
            this.itemsToRemove.Clear();

            for (var i = 0; i < activeCount; i++)
            {
                activeItems[i] = null;
            }

            activeCount = 0;
        }

        public bool ContainsListener(Action listener)
        {
            if (listener == null)
            {
                return false;
            }

            if (itemsToAdd.Contains(listener))
            {
                return true;
            }

            if (itemsToRemove.Contains(listener))
            {
                return false;
            }

            for (var i = 0; i < activeCount; i++)
            {
                if (activeItems[i] == listener)
                {
                    return true;
                }
            }

            return false;
        }

        public void Invoke()
        {
            if (itemsToRemove.Count > 0)
            {
                RemoveOld();
            }

            if (itemsToAdd.Count > 0)
            {
                AddNew();
            }

            itemsToRemove.Clear();
            itemsToAdd.Clear();

            for (var i = 0; i < activeCount; i++)
            {
                activeItems[i].Invoke();
            }
        }
        
        public void RemoveListener(Action listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            if (itemsToAdd.Contains(listener))
            {
                itemsToAdd.Remove(listener);
            }

            if (itemsToRemove.Contains(listener))
            {
                return;
            }

            itemsToRemove.Add(listener);
        }

        private void AddNew() 
        {
            // No need to check for duplicates within the new items, as they are duplicate free
            var oldSize = activeCount;

            // Grow the array if needed.
            ReserveSpace(activeCount + itemsToAdd.Count);

            foreach (var itemToAdd in itemsToAdd)
            {
                var hasDuplicate = false;
                for (var i = 0; i < oldSize; i++)
                {
                    if (activeItems[i] == itemToAdd)
                    {
                        hasDuplicate = true;
                        break;
                    }
                }

                if (!hasDuplicate)
                {
                    activeItems[activeCount] = itemToAdd;
                    activeCount++;
                }
            }

            itemsToAdd.Clear();
        }

        private void RemoveOld()
        {
            foreach (var itemToRemove in itemsToRemove)
            {
                for (var i = 0; i < activeCount; i++)
                {
                    if (activeItems[i] == itemToRemove)
                    {
                        if (keepCallOrder)
                        {
                            // Move later items forward and keep the order
                            for (var j = i; j < activeCount - 1; j++)
                            {
                                activeItems[j] = activeItems[j + 1];
                            }

                            activeItems[activeCount - 1] = null;
                        }
                        else
                        {
                            // Just replace the item to remove with the last item
                            if (i < activeCount - 1)
                            {
                                activeItems[i] = activeItems[activeCount - 1];
                                activeItems[activeCount - 1] = null;
                            }
                            else
                            {
                                activeItems[i] = null;
                            }
                        }

                        activeCount--;
                        break;
                    }
                }
            }

            itemsToRemove.Clear();
        }

        private void ReserveSpace(int count)
        {
            if (count < activeItems.Length)
            {
                return;
            }

            Debug.Assert(count < int.MaxValue);

            var newCount = activeItems.Length;
            while (newCount < count)
            {
                newCount *= 2;

                if (newCount < 0)
                {
                    // Overflow
                    newCount = int.MaxValue;
                }
            }

            var old = activeItems;
            activeItems = new Action[newCount];

            for (var i = 0; i < activeCount; i++)
            {
                activeItems[i] = old[i];
            }
        }
    }

    /// <summary>
    /// A helper class that works similar to events but gives more control and is optimized for many listeners
    /// </summary>
    public sealed class CallController<T>
    {
        private const int InitialActiveSize = 64;

        private readonly List<Action<T>> itemsToAdd;
        private readonly List<Action<T>> itemsToRemove;

        private int activeCount;
        private Action<T>[] activeItems;

        private bool keepCallOrder;

        public CallController(bool keepCallOrder = false)
        {
            this.itemsToAdd = new List<Action<T>>();
            this.itemsToRemove = new List<Action<T>>();
            this.activeItems = new Action<T>[InitialActiveSize];
            this.activeCount = 0;
            this.keepCallOrder = keepCallOrder;
        }

        /// <summary>
        /// The amount of registered listeners.
        /// </summary>
        public int Count => activeCount - itemsToRemove.Count + itemsToAdd.Count;

        /// <summary>
        /// Value indicating whether the call order must stay the same (true) or can be shuffled (false).
        /// </summary>
        /// <remarks>
        /// If the calls need to be issued in the same order as components got registered, or if a reordering of the call order would cause problems, this property should be set to <c>true</c>.<br/>
        /// However, if the call order isn't important, you should set it to <c>false</c>, to improve performance when removing call listeners.
        /// </remarks>
        public bool KeepCallOrder
        {
            get => keepCallOrder;
            set => keepCallOrder = value;
        }

        public void AddListener(Action<T> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            if (int.MaxValue - itemsToAdd.Count < activeCount)
            {
                // In case of overflow (More than 2^31 call listeners wanted to register, there is probably something wrong.)
                throw new Exception("Can't store anymore call listeners, the internal buffer has reached it's limits (2^31 registered listeners).");
            }

            if (itemsToRemove.Contains(listener))
            {
                itemsToRemove.Remove(listener);
            }

            if (itemsToAdd.Contains(listener))
            {
                return;
            }

            itemsToAdd.Add(listener);
        }

        public void Clear()
        {
            this.itemsToAdd.Clear();
            this.itemsToRemove.Clear();

            for (var i = 0; i < activeCount; i++)
            {
                activeItems[i] = null;
            }

            activeCount = 0;
        }

        public bool ContainsListener(Action<T> listener)
        {
            if (listener == null)
            {
                return false;
            }

            if (itemsToAdd.Contains(listener))
            {
                return true;
            }

            if (itemsToRemove.Contains(listener))
            {
                return false;
            }

            for (var i = 0; i < activeCount; i++)
            {
                if (activeItems[i] == listener)
                {
                    return true;
                }
            }

            return false;
        }

        public void Invoke(T value)
        {
            if (itemsToRemove.Count > 0)
            {
                RemoveOld();
            }

            if (itemsToAdd.Count > 0)
            { 
                AddNew();
            }

            itemsToRemove.Clear();
            itemsToAdd.Clear();

            for (var i = 0; i < activeCount; i++)
            {
                activeItems[i].Invoke(value);
            }
        }

        public static CallController<T> operator +(CallController<T> controller, Action<T> listener)
        {
            controller.AddListener(listener);
            return controller;
        }

        public static CallController<T> operator -(CallController<T> controller, Action<T> listener)
        {
            controller.RemoveListener(listener);
            return controller;
        }

        public void RemoveListener(Action<T> listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            if (itemsToAdd.Contains(listener))
            {
                itemsToAdd.Remove(listener);
            }

            if (itemsToRemove.Contains(listener))
            {
                return;
            }

            itemsToRemove.Add(listener);
        }

        private void AddNew()
        {
            // No need to check for duplicates within the new items, as they are duplicate free
            var oldSize = activeCount;

            // Grow the array if needed.
            ReserveSpace(activeCount + itemsToAdd.Count);

            foreach (var itemToAdd in itemsToAdd)
            {
                var hasDuplicate = false;
                for (var i = 0; i < oldSize; i++)
                {
                    if (activeItems[i] == itemToAdd)
                    {
                        hasDuplicate = true;
                        break;
                    }
                }

                if (!hasDuplicate)
                {
                    activeItems[activeCount] = itemToAdd;
                    activeCount++;
                }
            }

            itemsToAdd.Clear();
        }

        private void RemoveOld()
        {
            foreach (var itemToRemove in itemsToRemove)
            {
                for (var i = 0; i < activeCount; i++)
                {
                    if (activeItems[i] == itemToRemove)
                    {
                        if (keepCallOrder)
                        {
                            // Move later items forward and keep the order
                            for (var j = i; j < activeCount - 1; j++)
                            {
                                activeItems[j] = activeItems[j + 1];
                            }

                            activeItems[activeCount - 1] = null;
                        }
                        else
                        {
                            // Just replace the item to remove with the last item
                            if (i < activeCount - 1)
                            {
                                activeItems[i] = activeItems[activeCount - 1];
                                activeItems[activeCount - 1] = null;
                            }
                            else
                            {
                                activeItems[i] = null;
                            }
                        }

                        activeCount--;
                        break;
                    }
                }
            }

            itemsToRemove.Clear();
        }

        private void ReserveSpace(int count)
        {
            if (count < activeItems.Length)
            {
                return;
            }

            Debug.Assert(count < int.MaxValue);

            var newCount = activeItems.Length;
            while (newCount < count)
            {
                newCount *= 2;

                if (newCount < 0)
                {
                    // Overflow
                    newCount = int.MaxValue;
                }
            }

            var old = activeItems;
            activeItems = new Action<T>[newCount];

            for (var i = 0; i < activeCount; i++)
            {
                activeItems[i] = old[i];
            }
        }
    }
}
