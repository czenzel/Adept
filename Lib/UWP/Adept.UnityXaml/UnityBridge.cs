// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityPlayer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Adept.UnityXaml
{
    /// <summary>
    /// Helper methods for working with Unity in Xaml.
    /// </summary>
    public class UnityBridge
    {
        #region Static Version
        #region Member Variables
        static private UnityBridge instance;
        #endregion // Member Variables

        #region Internal Methods
        /// <summary>
        /// A Unity "coroutine" that monitors the specified operation.
        /// </summary>
        /// <param name="func">
        /// The function that returns the Unity <see cref="AsyncOperation"/>.
        /// </param>
        /// <param name="taskSource">
        /// The <see cref="TaskCompletionSource{TResult}"/> that represents the operation.
        /// </param>
        /// <param name="cancellationToken">
        /// An optional <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <param name="progress">
        /// An optional <see cref="IProgress{T}"/> that receives the progress of the operation.
        /// </param>
        /// <param name="millisecondsYield">
        /// An <see cref="int"/> that indicates the number of milliseconds to yield back to Unity between 
        /// each progress report and check to see if the operation has completed.
        /// </param>
        /// <param name="progressCompleteOverride">
        /// Specifies a minimum progress value that will mark the Task complete instead of checking <see cref="AsyncOperation.isDone"/>. Zero (the default) means disabled.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerator"/> that represents the coroutine.
        /// </returns>
        static private IEnumerator MonitorOperationRoutine(Func<AsyncOperation> func, TaskCompletionSource<bool> taskSource, CancellationToken cancellationToken, IProgress<float> progress, int millisecondsYield = 50, float progressCompleteOverride = 0f)
        {
            // Validate
            if (millisecondsYield < 0) throw new ArgumentOutOfRangeException(nameof(millisecondsYield));
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (taskSource == null) throw new ArgumentNullException(nameof(taskSource));

            // Progress holders
            float currentProgress = -1;
            float lastProgress = -1;
            float secondsYield = ((float)millisecondsYield) / 1000f;

            // First, start the operation
            var operation = func();

            // Yield at least one frame
            yield return new WaitForEndOfFrame();

            // Loop until done
            while (!operation.isDone)
            {
                // Check for progress override and test
                if ((progressCompleteOverride > 0) && (operation.progress >= progressCompleteOverride)) { break; }

                try
                {
                    // Check for cancellation
                    if ((cancellationToken != null) && (cancellationToken.IsCancellationRequested))
                    {
                        taskSource.SetCanceled();
                    }

                    // Is there a progress handler, see if we need to update it
                    if (progress != null)
                    {
                        // There is. Get current progress.
                        currentProgress = operation.progress;

                        // Did it change since last time through the loop?
                        if (currentProgress != lastProgress)
                        {
                            // Yes, notify
                            progress.Report(currentProgress);

                            // And record for next time through the loop
                            lastProgress = currentProgress;
                        }
                    }
                }
                catch (Exception ex)
                {
                    taskSource.SetException(ex);
                }

                // Yield time back to Unity
                if (millisecondsYield > 0)
                {
                    // Yield specified time
                    yield return new WaitForSeconds(secondsYield);
                }
                else
                {
                    // Yield at least one frame
                    yield return new WaitForEndOfFrame();
                }
            }

            // Done!
            taskSource.SetResult(true);
        }

        /// <summary>
        /// A Unity hybrid "coroutine" that wraps another coroutine in an awaitable Task.
        /// </summary>
        /// <param name="func">
        /// The function that returns the coroutine to run.
        /// </param>
        /// <param name="taskSource">
        /// The <see cref="TaskCompletionSource{TResult}"/> that represents the operation.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerator"/> that represents the hybrid coroutine.
        /// </returns>
        static private IEnumerator MonitorRoutineRoutine(Func<IEnumerator> func, TaskCompletionSource<bool> taskSource)
        {
            // Validate
            if (func == null) throw new ArgumentNullException(nameof(func));
            if (taskSource == null) throw new ArgumentNullException(nameof(taskSource));

            // Kick off the child coroutine
            yield return func();

            // Mark the task complete
            taskSource.SetResult(true);
        }
        #endregion // Internal Methods

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="UnityBridge"/> singleton.
        /// </summary>
        static public UnityBridge Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UnityBridge();
                }
                return instance;
            }
        }
        #endregion // Public Properties
        #endregion // Static Version



        #region Instance Version
        #region Member Variables
        private AppCallbacks appCallbacks;
        private WinRTBridge.WinRTBridge bridge;
        private GameObject gameObject;
        private TaskCompletionSource<bool> initializedSource;
        private bool initStarted;
        private TaskCompletionSource<bool> renderingStartedSource;
        private MonoBehaviour monoBehaviour;
        #endregion // Member Variables

        /// <summary>
        /// Constructs the <see cref="UnityBridge"/>.
        /// </summary>
        private UnityBridge()
        {
            initializedSource = new TaskCompletionSource<bool>();
            renderingStartedSource = new TaskCompletionSource<bool>();
        }

        #region Internal Methods
        internal void Initialize(Control keyboardControl, SwapChainPanel swapPanel)
        {
            // Validate
            if (keyboardControl == null) throw new ArgumentNullException(nameof(keyboardControl));
            if (swapPanel == null) throw new ArgumentNullException(nameof(swapPanel));

            // If already initialized, just ignore
            if (initStarted) { throw new InvalidOperationException("Unity has already been initialized by another control. Be sure to set cache mode to required on the page that uses UnityView."); }
            initStarted = true;

            // Get callbacks singleton
            appCallbacks = AppCallbacks.Instance;

            // Setup scripting bridge
            bridge = new WinRTBridge.WinRTBridge();
            appCallbacks.SetBridge(bridge);

            // Subscribe to events in order to forward
            appCallbacks.Initialized += AppCallbacks_Initialized;
            appCallbacks.RenderingStarted += AppCallbacks_RenderingStarted;

            // Not sure if we should always do this, never do this or make it an option
            appCallbacks.SetKeyboardTriggerControl(keyboardControl);

            // Wire up to swap panel
            appCallbacks.SetSwapChainPanel(swapPanel);

            // Leaving this for now since it handles visibility, closing etc. 
            // Hoping it doesn't impact rendering by listening to size changed.
            appCallbacks.SetCoreWindowEvents(Window.Current.CoreWindow);

            // Initialize D3D
            appCallbacks.InitializeD3DXAML();
        }

        /// <summary>
        /// Starts the specified coroutine in Unity.
        /// </summary>
        /// <param name="func">
        /// The function that returns the coroutine to run
        /// </param>
        private void StartCoroutine(Func<IEnumerator> func)
        {
            // Validate
            if (func == null) throw new ArgumentNullException(nameof(func));

            // Run on Unity thread, don't wait
            appCallbacks.InvokeOnAppThread(() =>
            {
                // Make sure our GameObject singleton is created
                if (gameObject == null) { gameObject = new GameObject(); }

                // HACK: MonoBehavior cannot be added directly. Instead, something inheriting from MonoBehavior must be added.
                // However, just adding a class in this project doesn't seem to work. It causes memory protection issues.

                // Make sure our MonoBehavior singleton is created
                if (monoBehaviour == null) { monoBehaviour = gameObject.AddComponent<UserAuthorizationDialog>(); }

                // Start the monitoring coroutine
                monoBehaviour.StartCoroutine_Auto(func());

            }, false);
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        private void AppCallbacks_Initialized()
        {
            // Complete the task
            initializedSource.SetResult(true);

            // If event subscribers, notify
            if (Initialized != null)
            {
                Initialized(this, EventArgs.Empty);
            }
        }

        private void AppCallbacks_RenderingStarted()
        {
            // Complete the task
            renderingStartedSource.SetResult(true);

            // If event subscribers, notify
            if (RenderingStarted != null)
            {
                RenderingStarted(this, EventArgs.Empty);
            }
        }
        #endregion // Overrides / Event Handlers

        #region Public Methods
        /// <summary>
        /// Invokes the specified action on Unity's application thread.
        /// </summary>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <param name="waitUntilDone">
        /// <c>true</c> if the Task should continue until the operation has completed; otherwise the 
        /// Task will complete when the action has been scheduled for execution in Unity.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        /// <remarks>
        /// This method is required to interact with Unity scripts and the Unity engine in general.
        /// </remarks>
        public async Task InvokeAsync(Action action, bool waitUntilDone = true)
        {
            // Validate
            if (action == null) throw new ArgumentNullException(nameof(action));

            // Wait for initialization to complete if it hasn't already
            if (!initializedSource.Task.IsCompleted)
            {
                await initializedSource.Task;
            }

            // Which path is based on whether or not we're waiting for completion
            if (!waitUntilDone)
            {
                // Invoke on Unity's thread but don't wait
                appCallbacks.InvokeOnAppThread(() => action(), false);
            }
            else
            {
                // Same thing, but kick off as a Task
                await Task.Run(() =>
                {
                    // Invoke on Unity's thread
                    appCallbacks.InvokeOnAppThread(() => action(), true);
                });
            }
        }

        /// <summary>
        /// Invokes a Unity <see cref="AsyncOperation"/> as an awaitable <see cref="Task"/>.
        /// </summary>
        /// <param name="func">
        /// The function that returns the Unity <see cref="AsyncOperation"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the Task.
        /// </param>
        /// <param name="progress">
        /// A handler that can receive progress reports on the Unity operation. The default is <see langword = "null" />.
        /// </param>
        /// <param name="millisecondsYield">
        /// The number of milliseconds yielded to the context between each update of the operation. The default is 50ms.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the Unity asynchronous operation.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <b>IMPORTANT:</b> The underlying Unity <see cref="AsyncOperation">AsyncOperation</see> 
        /// cannot actually be canceled within Unity. Therefore if the <paramref name="cancellationToken"/> is set, 
        /// the <see cref="Task"/> returned by this operation will end with a <see cref="TaskCanceledException"/>.
        /// </para>
        /// <para>
        /// Currently the only way to wait for a Unity <see cref="AsyncOperation">AsyncOperation</see> 
        /// to complete is to monitor the value returned by the 
        /// <see cref="AsyncOperation.isDone">isDone</see> property in a loop. To make this loop 
        /// efficient, by default the loop will yield to the calling context for 50 ms on every iteration. The 
        /// amount of time that is yielded on each iteration can be adjusted by changing the 
        /// <paramref name="millisecondsYield"/> parameter.
        /// </para>
        /// <param name="progressCompleteOverride">
        /// Specifies a minimum progress value that will mark the Task complete instead of checking <see cref="AsyncOperation.isDone"/>. Zero (the default) means disabled.
        /// </param>
        public Task InvokeAsync(Func<AsyncOperation> func, CancellationToken cancellationToken, IProgress<float> progress = null, int millisecondsYield = 50, float progressCompleteOverride = 0f)
        {
            // Validate
            if (millisecondsYield < 0) throw new ArgumentOutOfRangeException(nameof(millisecondsYield));
            if (func == null) throw new ArgumentNullException(nameof(func));

            // Create a task completion source for this operation
            var taskSource = new TaskCompletionSource<bool>();

            // Schedule a coroutine to monitor the operation
            StartCoroutine(() => MonitorOperationRoutine(func: func, taskSource: taskSource, cancellationToken: cancellationToken, progress: progress, millisecondsYield: millisecondsYield, progressCompleteOverride: progressCompleteOverride));

            // Return the completion source for the task
            return taskSource.Task;
        }

        /// <summary>
        /// Invokes a Unity <see cref="AsyncOperation"/> as an awaitable <see cref="Task"/>.
        /// </summary>
        /// <param name="func">
        /// The function that returns the Unity <see cref="AsyncOperation"/>.
        /// </param>
        /// <param name="progress">
        /// A handler that can receive progress reports on the Unity operation. The default is <see langword = "null" />.
        /// </param>
        /// <param name="millisecondsYield">
        /// The number of milliseconds yielded to the context between each update of the operation. The default is 50ms.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the Unity asynchronous operation.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <b>IMPORTANT:</b> The underlying Unity <see cref="AsyncOperation">AsyncOperation</see> 
        /// cannot actually be canceled within Unity. Therefore if the <paramref name="cancellationToken"/> is set, 
        /// the <see cref="Task"/> returned by this operation will end with a <see cref="TaskCanceledException"/>.
        /// </para>
        /// <para>
        /// Currently the only way to wait for a Unity <see cref="AsyncOperation">AsyncOperation</see> 
        /// to complete is to monitor the value returned by the 
        /// <see cref="AsyncOperation.isDone">isDone</see> property in a loop. To make this loop 
        /// efficient, by default the loop will yield to the calling context for 50 ms on every iteration. The 
        /// amount of time that is yielded on each iteration can be adjusted by changing the 
        /// <paramref name="millisecondsYield"/> parameter.
        /// </para>
        /// <param name="progressCompleteOverride">
        /// Specifies a minimum progress value that will mark the Task complete instead of checking <see cref="AsyncOperation.isDone"/>. Zero (the default) means disabled.
        /// </param>
        public Task InvokeAsync(Func<AsyncOperation> func, IProgress<float> progress = null, int millisecondsYield = 50, float progressCompleteOverride = 0f)
        {
            return InvokeAsync(func: func, cancellationToken: CancellationToken.None, progress: progress, millisecondsYield: millisecondsYield, progressCompleteOverride: progressCompleteOverride);
        }

        /// <summary>
        /// Invokes a Unity coroutine as an awaitable <see cref="Task"/>.
        /// </summary>
        /// <param name="func">
        /// The function that returns the Unity coroutine.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the Unity coroutine.
        /// </returns>
        public Task InvokeAsync(Func<IEnumerator> func)
        {
            // Validate
            if (func == null) throw new ArgumentNullException(nameof(func));

            // Create a task completion source for this operation
            var taskSource = new TaskCompletionSource<bool>();

            // Schedule a coroutine to monitor the operation
            StartCoroutine(() => MonitorRoutineRoutine(func: func, taskSource: taskSource));

            // Return the completion source for the task
            return taskSource.Task;
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets a <see cref="Task"/> that will complete when Unity has initialized.
        /// </summary>
        public Task InitializedTask
        {
            get
            {
                return initializedSource.Task;
            }
        }

        /// <summary>
        /// Gets a <see cref="Task"/> that will complete when Unity rendering has started.
        /// </summary>
        public Task RenderingStartedTask
        {
            get
            {
                return renderingStartedSource.Task;
            }
        }

        /// <summary>
        /// Gets a value that indicates if Unity has initialized.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return initializedSource.Task.IsCompleted;
            }
        }

        /// <summary>
        /// Gets a value that indicates if Unity has started rendering frames.
        /// </summary>
        public bool IsRenderingStarted
        {
            get
            {
                return renderingStartedSource.Task.IsCompleted;
            }
        }
        #endregion // Public Properties

        #region Public Events
        /// <summary>
        /// Occurs when the Unity Engine has initialized.
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// Occurs when the Unity Engine has started rendering frames.
        /// </summary>
        public event EventHandler RenderingStarted;
        #endregion // Public Events
        #endregion // Instance Version
    }
}
