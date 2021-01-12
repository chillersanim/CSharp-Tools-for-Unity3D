// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         MainThreadDispatch.cs
// 
// Created:          29.01.2020  19:28
// Last modified:    05.02.2020  19:55
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Unity_Tools.Components
{
    public static class MainThreadDispatch
    {
        private static readonly ConcurrentQueue<Action> AsynchronousWorkload = new ConcurrentQueue<Action>();

        private static Thread mainThread;
        private static int maxWorkTimeMs = 30;

        /// <summary>
        /// Used to make sure that the main thread dispatcher doesn't close while an invocation is being setup.
        /// </summary>
        private static readonly ReaderWriterLockSlim RwLock = new ReaderWriterLockSlim();

        private static readonly List<SynchronousAwait> syncAwaitCache = new List<SynchronousAwait>();

        private static readonly ConcurrentBag<SynchronousAwait> SynchronousAwaits =
            new ConcurrentBag<SynchronousAwait>();

        private static readonly ConcurrentQueue<SynchronousTask> SynchronousWorkload = new ConcurrentQueue<SynchronousTask>();
        private static readonly ThreadLocal<SynchronousAction> ThreadLocalSynchronousAction = new ThreadLocal<SynchronousAction>(() => new SynchronousAction());

        private static readonly ThreadLocal<SynchronousAwait> ThreadLocalSynchronousAwait = new ThreadLocal<SynchronousAwait>(() => new SynchronousAwait());

        private static readonly ThreadLocal<SynchronousDelegate> ThreadLocalSynchronousDelegate = new ThreadLocal<SynchronousDelegate>(() => new SynchronousDelegate());

        private static readonly ThreadLocal<SynchronousFunction> ThreadLocalSynchronousFunction = new ThreadLocal<SynchronousFunction>(() => new SynchronousFunction());

        private static readonly Stopwatch Timer = new Stopwatch();

        /// <summary>
        /// The maximum amount of work time the main thread can invest in working on the workload in milliseconds
        /// </summary>
        public static int MaxWorkTimeMs
        {
            get => maxWorkTimeMs;
            set => maxWorkTimeMs = value > 0
                ? value
                : throw new ArgumentOutOfRangeException(nameof(value), "The max work time must be larger than zero.");
        }

        /// <summary>
        /// Synchronously awaits until the tester returns <c>true</c> and then runs the awaiter (also synchronously). 
        /// </summary>
        /// <param name="tester">The tester, should return <c>true</c> only when the awaiter can be called, and <c>false</c> if the awaiter is not yet ready.</param>
        /// <param name="awaiter">The action to invoke when the tester returns <c>true</c>.</param>
        public static void Await(Func<bool> tester, Action awaiter)
        {
            SynchronousAwait syncAwait;

            try
            {
                // Enter read lock to make sure that the main thread dispatcher is not going to shut down while preparing the invocation
                if (!RwLock.TryEnterReadLock(5))
                {
                    throw new TimeoutException();
                }

                if (mainThread == null)
                {
                    throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
                }

                if (Thread.CurrentThread == mainThread)
                {
                    awaiter.Invoke();
                    return;
                }

                // Don't need to lock, the value only belongs to this thread and can't be in the pipeline (Value is replaced on timeout)
                syncAwait = ThreadLocalSynchronousAwait.Value;
                syncAwait.Set(tester, awaiter);
                SynchronousAwaits.Add(syncAwait);
            }
            finally
            {
                RwLock.ExitReadLock();
            }

            syncAwait.WaitHandle.Wait();

            if (syncAwait.Exception != null)
            {
                // Forward exception to calling thread
                var ex = syncAwait.Exception;
                syncAwait.Reset();
                throw ex;
            }

            syncAwait.Reset();
        }

        /// <summary>
        /// Determines whether the calling thread is the main thread.
        /// </summary>
        /// <returns><c>true</c> if the calling thread is the main thread, <c>false</c> otherwise.</returns>
        public static bool CheckAccess()
        {
            if (mainThread == null)
            {
                throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
            }

            return mainThread == Thread.CurrentThread;
        }

        /// <summary>
        /// Call this from the main thread at least once before calling any other method.
        /// </summary>
        public static void Initialize()
        {
            if (mainThread != null && mainThread != Thread.CurrentThread)
            {
                throw new InvalidOperationException("The main thread dispatch has already been initialized from another thread, are you sure you only call initialize from the main thread?");
            }

            if (mainThread == null)
            {
                mainThread = Thread.CurrentThread;
                CallProvider.AddUpdateListener(OnUpdate);
            }
        }

        /// <summary>
        /// Executes the specified Action synchronously on the main thread.
        /// </summary>
        /// <param name="action">The work to perform.</param>
        public static async Task Invoke(Action action)
        {
            SynchronousAction syncAction;

            try
            {
                // Enter read lock to make sure that the main thread dispatcher is not going to shut down while preparing the invocation
                if (!RwLock.TryEnterReadLock(5))
                {
                    throw new TimeoutException();
                }

                if (mainThread == null)
                {
                    throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
                }

                if (Thread.CurrentThread == mainThread)
                {
                    action.Invoke();
                    return;
                }

                // Don't need to lock, the value only belongs to this thread and can't be in the pipeline (Value is replaced on timeout)
                syncAction = ThreadLocalSynchronousAction.Value;
                syncAction.Set(action);
                SynchronousWorkload.Enqueue(syncAction);
            }
            finally
            {
                RwLock.ExitReadLock();
            }

            await syncAction.WaitHandle.WaitAsync();

            if (syncAction.Exception != null)
            {
                // Forward exception to calling thread
                var ex = syncAction.Exception;
                syncAction.Reset();
                throw ex;
            }

            syncAction.Reset();
        }

        /// <summary>
        /// Executes the specified Action synchronously on the main thread or cancels when exceeding the given timeout.
        /// </summary>
        /// <param name="action">The work to perform.</param>
        /// <param name="timeout">The maximum time to wait for the action to finish.</param>
        public static async Task Invoke(Action action, TimeSpan timeout)
        {
            SynchronousAction syncAction;

            try
            {
                // Enter read lock to make sure that the main thread dispatcher is not going to shut down while preparing the invocation
                if (!RwLock.TryEnterReadLock(5))
                {
                    throw new TimeoutException();
                }

                if (mainThread == null)
                {
                    throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
                }

                if (Thread.CurrentThread == mainThread)
                {
                    action.Invoke();
                    return;
                }

                syncAction = ThreadLocalSynchronousAction.Value;
                syncAction.Set(action);
                SynchronousWorkload.Enqueue(syncAction);
            }
            finally
            {
                RwLock.ExitReadLock();
            }

            if (!(await syncAction.WaitHandle.WaitAsync(timeout)))
            {
                // The task timed out, make sure that the task doesn't get executed after throwing a timeout exception
                lock (syncAction)
                {
                    // Make sure the task is not done
                    if (syncAction.State != SynchronousTaskState.Finished)
                    {
                        // Reset old entry so it doesn't get executed anymore and replace it with a new one
                        syncAction.Reset();
                        ThreadLocalSynchronousAction.Value = new SynchronousAction();
                        throw new TimeoutException();
                    }
                }
            }

            if (syncAction.Exception != null)
            {
                // Forward exception to calling thread
                var ex = syncAction.Exception;
                syncAction.Reset();
                throw ex;
            }

            syncAction.Reset();
        }

        /// <summary>
        /// Executes the specified function synchronously on the main thread and returns the result.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        public static async Task<T> Invoke<T>(Func<T> function)
        {
            SynchronousFunction syncFunction;

            try
            {
                // Enter read lock to make sure that the main thread dispatcher is not going to shut down while preparing the invocation
                if (!RwLock.TryEnterReadLock(5))
                {
                    throw new TimeoutException();
                }

                if (mainThread == null)
                {
                    throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
                }

                if (Thread.CurrentThread == mainThread)
                {
                    return function.Invoke();
                }

                // Don't need to lock, the value only belongs to this thread and can't be in the pipeline (Value is replaced on timeout)
                syncFunction = ThreadLocalSynchronousFunction.Value;
                syncFunction.Set(() => function());
                SynchronousWorkload.Enqueue(syncFunction);
            }
            finally
            {
                RwLock.ExitReadLock();
            }

            await syncFunction.WaitHandle.WaitAsync();

            if (syncFunction.Exception != null)
            {
                // Forward exception to calling thread
                var ex = syncFunction.Exception;
                syncFunction.Reset();
                throw ex;
            }

            var result = (T)syncFunction.Result;
            syncFunction.Reset();
            return result;
        }

        /// <summary>
        /// Executes the specified function synchronously on the main thread and returns the result or cancels when exceeding the given timeout.
        /// </summary>
        /// <param name="function">The function to evaluate.</param>
        /// <param name="timeout">The maximum time to wait for the evaluation to finish.</param>
        public static async Task<T> Invoke<T>(Func<T> function, TimeSpan timeout)
        {
            SynchronousFunction syncFunction;

            try
            {
                // Enter read lock to make sure that the main thread dispatcher is not going to shut down while preparing the invocation
                if (!RwLock.TryEnterReadLock(5))
                {
                    throw new TimeoutException();
                }

                if (mainThread == null)
                {
                    throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
                }

                if (Thread.CurrentThread == mainThread)
                {
                    return function.Invoke();
                }

                syncFunction = ThreadLocalSynchronousFunction.Value;
                syncFunction.Set(() => function());
                SynchronousWorkload.Enqueue(syncFunction);
            }
            finally
            {
                RwLock.ExitReadLock();
            }

            if (!(await syncFunction.WaitHandle.WaitAsync(timeout)))
            {
                // The task timed out, make sure that the task doesn't get executed after throwing a timeout exception
                lock (syncFunction)
                {
                    // Make sure the task is not done
                    if(syncFunction.State != SynchronousTaskState.Finished)
                    {
                        // Reset old entry so it doesn't get executed anymore and replace it with a new one
                        syncFunction.Reset();
                        ThreadLocalSynchronousFunction.Value = new SynchronousFunction();
                        throw new TimeoutException();
                    }
                }
            }

            if (syncFunction.Exception != null)
            {
                // Forward exception to calling thread
                var ex = syncFunction.Exception;
                syncFunction.Reset();
                throw ex;
            }

            var result = (T)syncFunction.Result;
            syncFunction.Reset();
            return result;
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the main thread and returns the result.
        /// </summary>
        /// <param name="delegate">The delegate to evaluate.</param>
        /// <param name="arguments">The arguments that need to be passed to the delegate</param>
        public static async Task<object> Invoke(Delegate @delegate, params object[] arguments)
        {
            SynchronousDelegate syncDelegate;

            try
            {
                // Enter read lock to make sure that the main thread dispatcher is not going to shut down while preparing the invocation
                if (!RwLock.TryEnterReadLock(5))
                {
                    throw new TimeoutException();
                }

                if (mainThread == null)
                {
                    throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
                }

                if (Thread.CurrentThread == mainThread)
                {
                    return @delegate.DynamicInvoke(arguments);
                }

                // Don't need to lock, the value only belongs to this thread and can't be in the pipeline (Value is replaced on timeout)
                syncDelegate = ThreadLocalSynchronousDelegate.Value;
                syncDelegate.Set(@delegate, arguments);
                SynchronousWorkload.Enqueue(syncDelegate);
            }
            finally
            {
                RwLock.ExitReadLock();
            }

            await syncDelegate.WaitHandle.WaitAsync();

            if (syncDelegate.Exception != null)
            {
                // Forward exception to calling thread
                var ex = syncDelegate.Exception;
                syncDelegate.Reset();
                throw ex;
            }

            var result = syncDelegate.Result;
            syncDelegate.Reset();
            return result;
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the main thread and returns the result or cancels when exceeding the given timeout.
        /// </summary>
        /// <param name="delegate">The delegate to evaluate.</param>
        /// <param name="timeout">The maximum time to wait for the evaluation to finish.</param>
        /// <param name="arguments">The arguments that need to be passed to the delegate</param>
        public static async Task<object> Invoke(Delegate @delegate, TimeSpan timeout, params object[] arguments)
        {
            SynchronousDelegate syncDelegate;

            try
            {
                // Enter read lock to make sure that the main thread dispatcher is not going to shut down while preparing the invocation
                if (!RwLock.TryEnterReadLock(5))
                {
                    throw new TimeoutException();
                }

                if (mainThread == null)
                {
                    throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
                }

                if (Thread.CurrentThread == mainThread)
                {
                    return @delegate.DynamicInvoke(arguments);
                }

                syncDelegate = ThreadLocalSynchronousDelegate.Value;
                syncDelegate.Set(@delegate, arguments);
                SynchronousWorkload.Enqueue(syncDelegate);
            }
            finally
            {
                RwLock.ExitReadLock();
            }

            if (!(await syncDelegate.WaitHandle.WaitAsync(timeout)))
            {
                // The task timed out, make sure that the task doesn't get executed after throwing a timeout exception
                lock (syncDelegate)
                {
                    // Make sure the task is not done
                    if (syncDelegate.State != SynchronousTaskState.Finished)
                    {
                        // Reset old entry so it doesn't get executed anymore and replace it with a new one
                        syncDelegate.Reset();
                        ThreadLocalSynchronousFunction.Value = new SynchronousFunction();
                        throw new TimeoutException();
                    }
                }
            }

            if (syncDelegate.Exception != null)
            {
                // Forward exception to calling thread
                var ex = syncDelegate.Exception;
                syncDelegate.Reset();
                throw ex;
            }

            var result = syncDelegate.Result;
            syncDelegate.Reset();
            return result;
        }

        /// <summary>
        /// Executes the specified Action asynchronously on the main thread.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        public static void InvokeAsync(Action action)
        {
            try
            {
                // Enter read lock to make sure that the main thread dispatcher is not going to shut down while preparing the invocation
                if (!RwLock.TryEnterReadLock(5))
                {
                    throw new TimeoutException();
                }

                if (mainThread == null)
                {
                    throw new InvalidOperationException("The main thread dispatcher has not been initialized. Make sure to call Initialize() at least once from the main thread before using the dispatcher.");
                }

                AsynchronousWorkload.Enqueue(action);
            }
            finally
            {
                RwLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Executes the specified delegate asynchronously on the main thread.
        /// </summary>
        /// <param name="delegate">The delegate to evaluate.</param>
        /// <param name="arguments">The arguments that need to be passed to the delegate</param>
        public static void InvokeAsync(Delegate @delegate, params object[] arguments)
        {
            InvokeAsync(() => @delegate.DynamicInvoke(arguments));
        }

        private static void OnUpdate()
        {
            Timer.Restart();

#if UNITY_EDITOR
            Profiler.BeginSample("MTD: update");
            Profiler.BeginSample("MTD: Sync workload");
#endif

            // Always do all synchronous workload at the start to unblock waiting threads as fast as possible
            while (Timer.ElapsedMilliseconds <= maxWorkTimeMs && SynchronousWorkload.TryDequeue(out var task))
            {
#if UNITY_EDITOR
                Profiler.BeginSample("MTD: Sync task");
#endif

                try
                {
                    lock (task)
                    {
                        // If the task has timed out, ignore it.
                        if (task.State != SynchronousTaskState.Waiting)
                        {
                            continue;
                        }

                        if (task is SynchronousAction sa)
                        {
                            sa.Action.Invoke();
                        }
                        else if (task is SynchronousFunction sf)
                        {
                            sf.Result = sf.Function.Invoke();
                        }
                        else if (task is SynchronousDelegate sd)
                        {
                            sd.Result = sd.Delegate.DynamicInvoke(sd.Arguments);
                        }

                        task.Finished();
                    }
                }
                catch (Exception e)
                {
                    task.RaiseException(e);
                }

#if UNITY_EDITOR
                Profiler.EndSample();
#endif
            }

#if UNITY_EDITOR
            Profiler.EndSample();
            Profiler.BeginSample("MTD: Async workload");
#endif

            // If there is time budget left, work of some async workload
            while (Timer.ElapsedMilliseconds <= maxWorkTimeMs && AsynchronousWorkload.TryDequeue(out var action))
            {
#if UNITY_EDITOR
                Profiler.BeginSample("MTD: Async task");
#endif

                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError("An asynchronous call on the main thread caused an exception:\n" + e);
                }

#if UNITY_EDITOR
                Profiler.EndSample();
#endif
            }

#if UNITY_EDITOR
            Profiler.EndSample();
            Profiler.BeginSample("MTD: Sync await workload");
#endif

            // In case jobs were scheduled, start them now.
            JobHandle.ScheduleBatchedJobs();


            // Test all awaits whether they have finished, there's no time limit here, but should take very little time (mostly single bool checks) 
            while (SynchronousAwaits.TryTake(out var syncAwait))
            {
#if UNITY_EDITOR
                Profiler.BeginSample("MTD: Sync await task");
#endif

                try
                {
                    if (syncAwait.Tester.Invoke())
                    {
                        // If the tester returns a success, call the awaiter to make sure it is finished
                        syncAwait.Awaiter.Invoke();
                        syncAwait.Finished();
                    }
                    else
                    {
                        // Otherwise, put the awaiter in the cache so it gets back into the synchronous awaits collection
                        syncAwaitCache.Add(syncAwait);
                    }
                }
                catch (Exception e)
                {
                    // In case of exception, log it and don't put back
                    syncAwait.RaiseException(e);
                }

#if UNITY_EDITOR
                Profiler.EndSample();
#endif
            }

#if UNITY_EDITOR
            Profiler.BeginSample("MTD: Sync await putback");
#endif

            // Put back all sync await to test in the next frame.
            foreach (var syncAwait in syncAwaitCache)
            {
                SynchronousAwaits.Add(syncAwait);
            }

            syncAwaitCache.Clear();

#if UNITY_EDITOR
            Profiler.EndSample();
            Profiler.EndSample();
            Profiler.EndSample();
#endif

            Timer.Stop();
        }

        private sealed class SynchronousAction : SynchronousTask
        {
            public SynchronousAction()
            {
                this.Action = null;
            }

            public Action Action { get; private set; }

            /// <summary>
            /// Always call reset before returning control to the caller, as otherwise references may stay that keep objects alive
            /// </summary>
            public override void Reset()
            {
                this.Action = null;
                base.Reset();
            }

            public void Set(Action action)
            {
                this.Action = action;
            }
        }

        private sealed class SynchronousAwait : SynchronousTask
        {
            public SynchronousAwait()
            {
                this.Awaiter = null;
            }

            public Action Awaiter { get; private set; }
            public Func<bool> Tester { get; private set; }

            /// <summary>
            /// Always call reset before returning control to the caller, as otherwise references may stay that keep objects alive
            /// </summary>
            public override void Reset()
            {
                this.Tester = null;
                this.Awaiter = null;
                base.Reset();
            }

            public void Set(Func<bool> tester, Action awaiter)
            {
                this.Tester = tester;
                this.Awaiter = awaiter;
            }
        }

        private sealed class SynchronousDelegate : SynchronousTask
        {
            public SynchronousDelegate()
            {
                this.Delegate = null;
            }

            public object[] Arguments { get; private set; }
            public Delegate Delegate { get; private set; }

            public object Result { get; set; }

            /// <summary>
            /// Always call reset before returning control to the caller, as otherwise references may stay that keep objects alive
            /// </summary>
            public override void Reset()
            {
                this.Delegate = null;
                this.Arguments = null;
                Result = null;
                base.Reset();
            }

            public void Set(Delegate dlg, object[] arguments)
            {
                this.Delegate = dlg;
                this.Arguments = arguments;
                base.Set();
            }
        }

        private sealed class SynchronousFunction : SynchronousTask
        {
            public SynchronousFunction()
            {
                this.Function = null;
            }

            public Func<object> Function { get; private set; }

            public object Result { get; set; }

            /// <summary>
            /// Always call reset before returning control to the caller, as otherwise references may stay that keep objects alive
            /// </summary>
            public override void Reset()
            {
                this.Function = null;
                Result = null;
                base.Reset();
            }

            public void Set(Func<object> dlg)
            {
                this.Function = dlg;
                base.Set();
            }
        }

        private abstract class SynchronousTask : IDisposable
        {
            public readonly SemaphoreSlim WaitHandle;

            protected SynchronousTask()
            {
                this.State = SynchronousTaskState.Idle;
                this.WaitHandle = new SemaphoreSlim(0, 1);
                this.Exception = null;
            }

            public Exception Exception { get; private set; }

            public SynchronousTaskState State { get; private set; }

            public void Dispose()
            {
                WaitHandle.Dispose();
            }

            public void Finished()
            {
                State = SynchronousTaskState.Finished;
                WaitHandle.Release(1);
            }

            public void RaiseException(Exception ex)
            {
                Exception = ex;
                this.State = SynchronousTaskState.Finished;
                WaitHandle.Release();
            }

            public virtual void Reset()
            {
                this.Exception = null;
                this.State = SynchronousTaskState.Idle;
            }

            protected void Set()
            {
                State = SynchronousTaskState.Waiting;
            }
        }

        private enum SynchronousTaskState
        {
            Waiting,

            Finished,

            Idle
        }
    }
}
