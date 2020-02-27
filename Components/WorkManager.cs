using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityTools.Core;
using Debug = UnityEngine.Debug;

namespace UnityTools.Components
{
    /// <summary>
    /// Provides a mechanism to enqueue work at set priorities, that gets worked on over multiple frames with a given time limit per frame.
    /// </summary>
    public sealed class WorkManager : SingletonBehaviour<WorkManager>
    {
        // The work items are assumed to be sorted descending by priority§
        private readonly List<WorkItem> work = new List<WorkItem>();
        
        private readonly Stopwatch stopwatch = new Stopwatch();

        [SerializeField]
        private double maxWorkTime = 0.002;

        /// <summary>
        /// The maximum amount of time allocated per frame for working on the work queue.
        /// </summary>
        public static double MaxWorkTime
        {
            get
            {
                if (CanAccessInstance)
                {
                    return Instance.maxWorkTime;
                }

                return 0f;
            }
            set
            {
                if (double.IsInfinity(value) || double.IsNaN(value))
                {
                    throw new ArgumentException("The value cannot be NaN or infinity.");
                }

                if (CanAccessInstance)
                {
                    Instance.maxWorkTime = Math.Max(value, 0.0001);
                }
            }
        }

        /// <summary>
        /// The total amount of queued work items.
        /// </summary>
        public static int QueuedCount
        {
            get
            {
                if (CanAccessInstance)
                {
                    return Instance.work.Count;
                }

                return 0;
            }
        }

        /// <summary>
        /// Enqueues an action with an optional priority and optional callback to be invoked when the action was executed.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <param name="onFinished">An optional callback that gets called when the action has been successfully invoked.</param>
        /// <param name="priority">The priority of the action (Lower values mean higher priority).</param>
        public void Enqueue(Action action, Action onFinished = null, int priority = 0)
        {
            // Find the first item with a lower priority than the provided priority.
            var index = work.BinarySearchLocation(priority, (w, p) => w.Priority.CompareTo(p));
            work.Insert(index, new WorkItem(action, action, onFinished, WorkType.Action, priority));
        }

        /// <summary>
        /// Enqueues a function with an optional priority and optional callback to be invoked when the function was executed returning the result.
        /// </summary>
        /// <param name="function">The function to invoke.</param>
        /// <param name="onFinished">An optional callback that gets called when the function has been successfully invoked, returning the result of the function.</param>
        /// <param name="priority">The priority of the function (Lower values mean higher priority).</param>
        public void Enqueue<T>(Func<T> function, Action<T> onFinished = null, int priority = 0)
        {
            // Find the first item with a lower priority than the provided priority.
            var index = work.BinarySearchLocation(priority, (w, p) => w.Priority.CompareTo(p));
            work.Insert(index,
                new WorkItem(
                    function,
                    (Func<object>) (() => function),
                    onFinished != null ? (Action<object>) (o => onFinished.Invoke((T) o)) : null,
                    WorkType.Function,
                    priority));
        }

        /// <summary>
        /// Cancels the first occurrences of the action in the work queue.
        /// </summary>
        /// <param name="action">The action to cancel.</param>
        /// <returns>Returns <c>true</c> if an action was cancelled, <c>false</c> otherwise.</returns>
        public bool Cancel(Action action)
        {
            for (var i = work.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(work[i].Source, action))
                {
                    work.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Cancels the first occurrences of the function in the work queue.
        /// </summary>
        /// <param name="function">The function to cancel.</param>
        /// <returns>Returns <c>true</c> if an function was cancelled, <c>false</c> otherwise.</returns>
        public bool Cancel<T>(Func<T> function)
        {
            for (var i = work.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(work[i].Source, function))
                {
                    work.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Cancels all occurrences of the action in the work queue.
        /// </summary>
        /// <param name="action">The action to cancel.</param>
        /// <returns>Returns <c>true</c> if any action was cancelled, <c>false</c> otherwise.</returns>
        public bool CancelAll(Action action)
        {
            var found = false;

            for (var i = work.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(work[i].Source, action))
                {
                    work.RemoveAt(i);
                    found = true;
                }
            }

            return found;
        }

        /// <summary>
        /// Cancels all occurrences of the function in the work queue.
        /// </summary>
        /// <param name="function">The function to cancel.</param>
        /// <returns>Returns <c>true</c> if any function was cancelled, <c>false</c> otherwise.</returns>
        public bool CancelAll<T>(Func<T> function)
        {
            var found = false;

            for (var i = work.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(work[i].Source, function))
                {
                    work.RemoveAt(i);
                    found = true;
                }
            }

            return found;
        }

        private void OnUpdate()
        {
            stopwatch.Restart();

            while (work.Count > 0 && stopwatch.Elapsed.TotalMilliseconds < maxWorkTime)
            {
                var index = work.Count - 1;
                var workItem = work[index];
                work.RemoveAt(index);

                try
                {
                    switch (workItem.Type)
                    {
                        case WorkType.Action:
                        {
                            ((Action) workItem.Method).Invoke();
                            ((Action) workItem.OnFinished)?.Invoke();
                            break;
                        }
                        case WorkType.Function:
                        {
                            var result = ((Func<object>)workItem.Method).Invoke();
                            ((Action<object>)workItem.OnFinished)?.Invoke(result);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            stopwatch.Stop();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            CallProvider.AddUpdateListener(OnUpdate);
        }

        private void OnDisable()
        {
            CallProvider.RemoveUpdateListener(OnUpdate);
        }

        private struct WorkItem
        {
            public readonly int Priority;

            public readonly WorkType Type;

            public readonly object Source;

            public readonly object Method;

            public readonly object OnFinished;

            public WorkItem(object source, object method, object onFinished, WorkType type, int priority)
            {
                this.Source = source;
                this.Method = method;
                this.OnFinished = onFinished;
                this.Type = type;
                this.Priority = priority;
            }
        }

        private enum WorkType
        {
            Action,

            Function
        }
    }
}
