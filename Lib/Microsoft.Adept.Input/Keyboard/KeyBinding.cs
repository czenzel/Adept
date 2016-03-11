// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Microsoft.Adept.Input
{
    /// <summary>
    /// Represents a binding between a keyboard key and an action.
    /// </summary>
    public class KeyBinding
    {
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="KeyBinding"/> instance.
        /// </summary>
        public KeyBinding()
        {
            IsEnabled = true;
        }
        #endregion // Constructors

        #region Public Properties
        /// <summary>
        /// Gets or sets the action that will be invoked when any of the triggers fire.
        /// </summary>
        public IInputAction Action { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the binding is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if the binding is enabled; otherwise <c>false</c>. The default is <c>true</c>.
        /// </value>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="VirtualKey"/> involved in the binding.
        /// </summary>
        /// <value>
        /// The <see cref="VirtualKey"/> involved in the binding.
        /// </value>
        public VirtualKey Key { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="VirtualKey"/> that must be pressed in combination with the key to satisfy the binding.
        /// </summary>
        /// <value>
        /// The <see cref="VirtualKey"/> modifier.
        /// </value>
        public VirtualKey Modifier { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the action should be triggered when the key is released.
        /// </summary>
        /// <value>
        /// <c>true</c> if the action should be triggered when the key is released; otherwise <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool WhenReleased { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the action should be triggered when the key is repeated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the action should be triggered when the key is repeated; otherwise <c>false</c>. The default is <c>false</c>.
        /// </value>
        public bool WhenRepeated { get; set; }
        #endregion // Public Properties
    }
}
