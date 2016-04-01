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
        private UnityBridge bridge;
        private UIElement placeholderElement;
        private SwapChainPanel swapPanel;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="UnityView"/> instance.
        /// </summary>
        public UnityView()
        {
            this.DefaultStyleKey = typeof(UnityView);
        }
        #endregion // Constructors


        #region Overrides / Event Handlers
        private void Bridge_RenderingStarted(object sender, EventArgs e)
        {
            // Hide the placeholder element(s)
            if (placeholderElement != null)
            {
                placeholderElement.Visibility = Visibility.Collapsed;
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
                // Get the bridge
                bridge = UnityBridge.Instance;

                // Subscribe to bridge events
                bridge.RenderingStarted += Bridge_RenderingStarted;

                // Initialize Unity
                bridge.Initialize(this, swapPanel);
            }
        }
        #endregion // Overrides / Event Handlers

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
    }
}
