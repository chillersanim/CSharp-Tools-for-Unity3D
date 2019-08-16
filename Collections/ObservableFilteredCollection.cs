// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         ObservableFilteredCollection.cs
// 
// Created:          05.08.2019  15:19
<<<<<<< HEAD
// Last modified:    16.08.2019  16:31
=======
// Last modified:    15.08.2019  17:56
>>>>>>> refs/remotes/origin/master
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity_Tools.Core;

namespace Unity_Tools.Collections
{
    /// <summary>
    ///     The filtered collection.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class ObservableFilteredCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        ///     The data.
        /// </summary>
        [NotNull] private readonly List<T> data;

        /// <summary>
        ///     The filter.
        /// </summary>
        [NotNull] private readonly IFilter<T> filter;

        /// <summary>
        ///     The is item observable.
        /// </summary>
        private readonly bool isItemObservable;

        /// <summary>
        ///     The source.
        /// </summary>
        [NotNull] private readonly ICollection<T> source;

        /// <summary>
        ///     The used properties.
        /// </summary>
        [CanBeNull] private readonly string[] usedProperties;

        /// <summary>
        ///     The ignore change.
        /// </summary>
        private bool ignoreChange;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableObservableFilteredCollection{T}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <param name="usedProperties">
        ///     The used properties.
        /// </param>
        public ObservableFilteredCollection(ICollection<T> source, [NotNull] IFilter<T> filter, params string[] usedProperties)
        {
            data = new List<T>();
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.filter = filter ?? throw new ArgumentNullException(nameof(filter));
            this.usedProperties = usedProperties != null && usedProperties.Length == 0 ? null : usedProperties;

            isItemObservable = typeof(T).GetInterfaces().Any(i => i == typeof(INotifyPropertyChanged));

            Initialize();

            if (this.source is INotifyCollectionChanged observable)
            {
                observable.CollectionChanged += OnSourceChanged;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ObservableObservableFilteredCollection{T}" /> class.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="filter">
        ///     The filter.
        /// </param>
        /// <param name="usedProperties">
        ///     The used properties.
        /// </param>
        public ObservableFilteredCollection(
            [NotNull] ICollection<T> source,
            [NotNull] Func<T, bool> filter,
            params string[] usedProperties)
            : this(source, new FuncFilter(filter), usedProperties)
        {
        }

        /// <inheritdoc />
        public int Count => data.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(T item)
        {
            ignoreChange = true;

            source.Add(item);
            if (filter.Filter(item))
            {
                data.Add(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            OnPropertyChanged(nameof(Count));

            ignoreChange = false;
        }

        /// <inheritdoc />
        public void Clear()
        {
            ignoreChange = true;

            foreach (var item in data)
            {
                source.Remove(item);
            }

            var old = data.ToArray();
            data.Clear();

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, old));
            OnPropertyChanged(nameof(Count));

            ignoreChange = false;
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return data.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            var index = data.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            ignoreChange = true;

            data.RemoveAt(index);
            source.Remove(item);

            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            OnPropertyChanged(nameof(Count));

            ignoreChange = false;

            return true;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     The on collection changed.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            try
            {
                CollectionChanged?.Invoke(this, e);
            }
            catch
            {
                CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        ///     The on property changed.
        /// </summary>
        /// <param name="propertyName">
        ///     The property name.
        /// </param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        private void Initialize()
        {
            foreach (var item in source)
            {
                if (filter.Filter(item))
                {
                    data.Add(item);
                }

                if (isItemObservable)
                {
                    ((INotifyPropertyChanged) item).PropertyChanged += OnItemPropertyChanged;
                }
            }
        }

        /// <summary>
        ///     The on item property changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is T item))
            {
                return;
            }

            var matchesFilter = filter.Filter(item);

            if (matchesFilter)
            {
                if (!data.Contains(item))
                {
                    data.Add(item);
                    OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                }
            }
            else
            {
                if (data.Remove(item))
                {
                    OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                }
            }
        }

        /// <summary>
        ///     The on source changed.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ignoreChange)
            {
                return;
            }

            var hasChanges = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    var newItem = (T) e.NewItems[0];
                    if (filter.Filter(newItem))
                    {
                        data.Add(newItem);
                        hasChanges = true;
                    }

                    if (isItemObservable)
                    {
                        ((INotifyPropertyChanged) newItem).PropertyChanged += OnItemPropertyChanged;
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Remove:
                {
                    var oldItem = (T) e.OldItems[0];
                    if (filter.Filter(oldItem))
                    {
                        data.Remove(oldItem);
                        hasChanges = true;
                    }

                    if (isItemObservable)
                    {
                        ((INotifyPropertyChanged) oldItem).PropertyChanged -= OnItemPropertyChanged;
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Reset:
                {
                    if (isItemObservable)
                    {
                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                ((INotifyPropertyChanged) item).PropertyChanged -= OnItemPropertyChanged;
                            }
                        }
                        else
                        {
                            foreach (var item in data)
                            {
                                ((INotifyPropertyChanged) item).PropertyChanged -= OnItemPropertyChanged;
                            }
                        }
                    }

                    data.Clear();
                    Initialize();
                    hasChanges = true;
                    break;
                }
            }

            if (hasChanges)
            {
                OnCollectionChanged(e);
                OnPropertyChanged(nameof(Count));
            }
        }

        /// <summary>
        ///     The func filter.
        /// </summary>
        private class FuncFilter : IFilter<T>
        {
            /// <summary>
            ///     The function.
            /// </summary>
            [NotNull] private readonly Func<T, bool> function;

            /// <summary>
            ///     Initializes a new instance of the <see cref="FuncFilter" /> class.
            /// </summary>
            /// <param name="function">
            ///     The function.
            /// </param>
            public FuncFilter([NotNull] Func<T, bool> function)
            {
                this.function = function ?? throw new ArgumentNullException(nameof(function));
            }

            /// <inheritdoc />
            public bool Filter(T item)
            {
                return function(item);
            }
        }
    }
}