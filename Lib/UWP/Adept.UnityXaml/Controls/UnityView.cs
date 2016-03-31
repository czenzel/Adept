// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityPlayer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Adept.UnityXaml
{
    /// <summary>
    /// A Xaml control that can display Unity 3D content.
    /// </summary>
    [TemplatePart(Name = SWAP_PANEL_NAME, Type = typeof(SwapChainPanel))]
    [TemplatePart(Name = PLACEHOLDER_ELEMENT_NAME, Type = typeof(UIElement))]
    public sealed class UnityView : Control
    {
        #region Constants
        private const string SWAP_PANEL_NAME = "SwapPanel";
        private const string PLACEHOLDER_ELEMENT_NAME = "PlaceholderElement";
        #endregion // Constants

        #region Dependency Property Definitions
        /// <summary>
        /// Identifies the <see cref="PlaceholderSource"/> dependency property.
        /// </summary>
        static public readonly DependencyProperty PlaceholderSourceProperty = DependencyProperty.Register("PlaceholderSource", typeof(ImageSource), typeof(UnityView), new PropertyMetadata(null));
        #endregion // Dependency Property Definitions

        #region Member Variables
        private AppCallbacks appCallbacks;
        private WinRTBridge.WinRTBridge bridge;
        private TaskCompletionSource<bool> initializedSource;
        private UIElement placeholderElement;
        private TaskCompletionSource<bool> renderingSource;
        private SwapChainPanel swapPanel;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="UnityView"/> instance.
        /// </summary>
        public UnityView()
        {
            this.DefaultStyleKey = typeof(UnityView);
            initializedSource = new TaskCompletionSource<bool>();
            renderingSource = new TaskCompletionSource<bool>();
        }
        #endregion // Constructors

        #region Internal Methods
        private void LoadUnity()
        {
            // Get callbacks singleton
            appCallbacks = AppCallbacks.Instance;

            // Setup scripting bridge
            bridge = new WinRTBridge.WinRTBridge();
            appCallbacks.SetBridge(bridge);

            // Subscribe to events in order to forward
            appCallbacks.Initialized += AppCallbacks_Initialized;
            appCallbacks.RenderingStarted += AppCallbacks_RenderingStarted;

            // Not sure if we should always do this, never do this or make it an option
            appCallbacks.SetKeyboardTriggerControl(this);

            // Wire up to swap panel
            appCallbacks.SetSwapChainPanel(swapPanel);

            // Leaving this for now since it handles visibility, closing etc. 
            // Hoping it doesn't impact rendering by listening to size changed.
            appCallbacks.SetCoreWindowEvents(Window.Current.CoreWindow);

            // Might optionally move this to another Init method or something
            appCallbacks.InitializeD3DXAML();
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
            // Hide the placeholder element(s)
            if (placeholderElement != null)
            {
                placeholderElement.Visibility = Visibility.Collapsed;
            }

            // Complete the task
            renderingSource.SetResult(true);

            // If event subscribers, notify
            if (RenderingStarted != null)
            {
                RenderingStarted(this, EventArgs.Empty);
            }
        }

        protected override void OnApplyTemplate()
        {
            // Pass to base first
            base.OnApplyTemplate();

            // Get the swap chain panel
            swapPanel = GetTemplateChild(SWAP_PANEL_NAME) as SwapChainPanel;

            // Ensure panel was found
            if (swapPanel == null) { throw new InvalidOperationException($"A {nameof(SwapChainPanel)} named {SWAP_PANEL_NAME} is required in the control template."); }

            // Attempt to get the preview element
            placeholderElement = GetTemplateChild(PLACEHOLDER_ELEMENT_NAME) as UIElement;

            // Panel found. If not in design mode, load Unity.
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LoadUnity();
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
        /// Invokes the specified function on Unity's application thread.
        /// </summary>
        /// <typeparam name="T">
        /// The type of value returned by the function.
        /// </typeparam>
        /// <param name="func">
        /// The function to perform.
        /// </param>
        /// <param name="waitUntilDone">
        /// <c>true</c> if the Task should continue until the operation has completed; otherwise the 
        /// Task will complete when the action has been scheduled for execution in Unity.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that yields the result of the operation.
        /// </returns>
        /// <remarks>
        /// This method is required to interact with Unity scripts and the Unity engine in general.
        /// </remarks>
        public async Task<T> InvokeAsync<T>(Func<T> func, bool waitUntilDone = true)
        {
            T result = default(T);
            await InvokeAsync(() => { result = func(); }, waitUntilDone);
            return result;

        }

        /// <summary>
        /// Loads the scene with the specified name.
        /// </summary>
        /// <param name="sceneName">
        /// The name of the scene to load.
        /// </param>
        /// <param name="progress">
        /// An optional progress handler to receive notifications during the load.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        public Task LoadSceneAsync(string sceneName, IProgress<float> progress = null)
        {
            // Invoke on Unity thread. Don't wait for completion as we'll use the task created during scheduling.
            return InvokeAsync(() =>
            {
                return SceneManager.LoadSceneAsync(sceneName).AsTask(progress);
            }, false);
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the source of the placeholder image. This is a dependency property.
        /// </summary>
        /// <value>
        /// The source of the placeholder image.
        /// </value>
        public ImageSource PlaceholderSource
        {
            get
            {
                return (ImageSource)GetValue(PlaceholderSourceProperty);
            }
            set
            {
                SetValue(PlaceholderSourceProperty, value);
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
    }
}
