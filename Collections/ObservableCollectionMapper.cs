// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         ObservableCollectionMapper.cs
// 
// Created:          16.08.2019  16:33
// Last modified:    16.08.2019  16:56
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using JetBrains.Annotations;

namespace Unity_Tools.Collections
{
    /// <summary>
    ///     The observable collection mapper.
    /// </summary>
    /// <typeparam name="TSource">
    /// </typeparam>
    /// <typeparam name="TTarget">
    /// </typeparam>
    public class ObservableCollectionMapper<TSource, TTarget> : IReadOnlyCollection<TTarget>, INotifyCollectionChanged
    {
        /// <summary>
        ///     The factory.
        /// </summary>
        [NotNull] private readonly Func<TSource, TTarget> factory;

        /// <summary>
        ///     The remove action.
        /// </summary>
        [CanBeNull] private readonly Action<TTarget> RemoveAction;

        /// <summary>
        ///     The source.
        /// </summary>
        [NotNull] private readonly IList<TSource> source;

        /// <summary>
        ///     The target.
        /// </summary>
        [NotNull] private readonly ObservableCollection<TTarget> target;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableCollectionMapper{TSource,TTarget}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="newItemFactory">
        ///     The new item factory.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public ObservableCollectionMapper(
            [NotNull] IList<TSource> source,
            [NotNull] Func<TSource, TTarget> newItemFactory)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            factory = newItemFactory ?? throw new ArgumentNullException(nameof(newItemFactory));
            target = new ObservableCollection<TTarget>();

            if (!(source is INotifyCollectionChanged))
            {
                throw new ArgumentException("The source needs to implement the INotifyCollectionChanged interface.");
            }

            foreach (var item in this.source)
            {
                target.Add(factory(item));
            }

            ((INotifyCollectionChanged) this.source).CollectionChanged += OnSourceCollectionChanged;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableCollectionMapper{TSource,TTarget}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="newItemFactory">
        ///     The new item factory.
        /// </param>
        /// <param name="removeAction">
        ///     The remove action.
        /// </param>
        public ObservableCollectionMapper(
            [NotNull] IList<TSource> source,
            [NotNull] Func<TSource, TTarget> newItemFactory,
            [NotNull] Action<TTarget> removeAction)
            : this(source, newItemFactory)
        {
            RemoveAction = removeAction ?? throw new ArgumentNullException(nameof(removeAction));
        }

        public TTarget this[int index] => this.target[index];

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => target.CollectionChanged += value;
            remove => target.CollectionChanged -= value;
        }

        /// <inheritdoc />
        public int Count => target.Count;

        /// <inheritdoc />
        public IEnumerator<TTarget> GetEnumerator()
        {
            return target.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return target.GetEnumerator();
        }

        /// <summary>
        ///     The on add.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="index">
        ///     The index.
        /// </param>
        private void OnAdd(IList items, int index)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var sourceItem = (TSource) items[i];

                var targetItem = factory(sourceItem);
                target.Insert(index + i, targetItem);
            }
        }

        /// <summary>
        ///     The on move.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="oldIndex">
        ///     The old index.
        /// </param>
        /// <param name="newIndex">
        ///     The new index.
        /// </param>
        private void OnMove(IList items, int oldIndex, int newIndex)
        {
            if (oldIndex < newIndex)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    target.Move(oldIndex + i, newIndex + i);
                }
            }
            else
            {
                for (var i = 0; i < items.Count; i++)
                {
                    target.Move(oldIndex, newIndex + items.Count + i);
                }
            }
        }

        /// <summary>
        ///     The on remove.
        /// </summary>
        /// <param name="items">
        ///     The items.
        /// </param>
        /// <param name="startIndex">
        ///     The start index.
        /// </param>
        private void OnRemove(IList items, int startIndex)
        {
            for (var i = items.Count - 1; i >= 0; i--)
            {
                var item = target[startIndex + i];

                RemoveAction?.Invoke(item);

                target.RemoveAt(startIndex + i);
            }
        }

        /// <summary>
        ///     The on replace.
        /// </summary>
        /// <param name="oldItems">
        ///     The old items.
        /// </param>
        /// <param name="newItems">
        ///     The new items.
        /// </param>
        /// <param name="startIndex">
        ///     The start index.
        /// </param>
        private void OnReplace(IList oldItems, IList newItems, int startIndex)
        {
            for (var i = 0; i < oldItems.Count; i++)
            {
                var item = target[startIndex + i];

                RemoveAction?.Invoke(item);

                var sourceItem = (TSource) newItems[i];

                var targetItem = factory(sourceItem);
                target[startIndex + i] = targetItem;
            }
        }

        /// <summary>
        ///     The on reset.
        /// </summary>
        private void OnReset()
        {
            foreach (var item in target)
            {
                RemoveAction?.Invoke(item);
            }

            target.Clear();

            foreach (var sourceItem in source)
            {
                var targetItem = factory(sourceItem);
                target.Add(targetItem);
            }
        }

        /// <summary>
        ///     The on source collection changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnAdd(e.NewItems, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    OnMove(e.OldItems, e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    OnReplace(e.OldItems, e.NewItems, e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    OnRemove(e.OldItems, e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnReset();
                    break;
            }
        }
    }
}