// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Microsoft.Adept.Input
{
    /// <summary>
    /// A class that maps keyboard input to actions.
    /// </summary>
    public class KeyboardMapper
    {
        #region Member Variables
        private List<KeyBinding> bindings = new List<KeyBinding>();
        private List<VirtualKey> pressedKeys = new List<VirtualKey>();
        #endregion // Member Variables

        private void ExecuteBindings(VirtualKey key, bool repeated, bool released)
        {
            // Find all matching bindings
            var query = (from b in bindings
                         where b.IsEnabled &&                   // Enabled
                         b.Key == key &&                        // Matches key
                         b.WhenReleased == released &&          // Matches released or pressed passed in above

                         (b.Modifier == VirtualKey.None         // No modifier
                         || pressedKeys.Contains(b.Modifier))   // - OR - modifier is currently pressed

                         select b);

            // Repeated?
            if (repeated)
            {
                query = query.Where(b => b.WhenRepeated);
            }

            // To list
            var active = query.ToList();

            // Fire all
            foreach (var b in active)
            {
                try
                {
                    b.Action.Invoke();
                }
                catch (Exception ex)
                {
                    // TODO: Log exception.
                    Debug.WriteLine(string.Format("Error executing binding: {0}", ex.Message));
                }
            }
        }

        /// <summary>
        /// Processes a KeyDown event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// A <see cref="KeyRoutedEventArgs"/> which contains the event data.
        /// </param>
        private void ProcessKeyDown(CoreWindow sender, KeyEventArgs e)
        {
            // Repeated?
            bool repeated = e.KeyStatus.WasKeyDown;

            if (!repeated)
            {
                // Add to pressed
                pressedKeys.Add(e.VirtualKey);
            }

            // Execute matching bindings
            ExecuteBindings(e.VirtualKey, repeated, false);
        }

        /// <summary>
        /// Processes a KeyUp event.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// A <see cref="KeyRoutedEventArgs"/> which contains the event data.
        /// </param>
        private void ProcessKeyUp(CoreWindow sender, KeyEventArgs e)
        {
            // Repeated?
            bool repeated = (!e.KeyStatus.IsKeyReleased);

            if (!repeated)
            {
                // Remove from pressed
                pressedKeys.Remove(e.VirtualKey);
            }

            // Execute matching bindings
            ExecuteBindings(e.VirtualKey, repeated, true);
        }

        /// <summary>
        /// Subscribes to keyboard events on the specified control.
        /// </summary>
        public void SubscribeEvents()
        {
            Window.Current.CoreWindow.KeyDown += ProcessKeyDown;
            Window.Current.CoreWindow.KeyUp += ProcessKeyUp;
        }

        /// <summary>
        /// Unsubscribes from keyboard events on the specified control.
        /// </summary>
        public void UnsubscribeEvents()
        {
            Window.Current.CoreWindow.KeyDown -= ProcessKeyDown;
            Window.Current.CoreWindow.KeyUp -= ProcessKeyUp;
        }

        /// <summary>
        /// Gets the list of keyboard bindings.
        /// </summary>
        public List<KeyBinding> Bindings
        {
            get
            {
                return bindings;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                bindings = value;
            }
        }
    }
}
