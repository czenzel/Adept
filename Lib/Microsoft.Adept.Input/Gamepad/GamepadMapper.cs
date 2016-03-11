// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Microsoft.Adept.Input
{
    /// <summary>
    /// A class that maps <see cref="Gamepad"/> input to application actions.
    /// </summary>
    public class GamepadMapper
    {


        /// <summary>
        /// Processes a KeyDown event which may include a GamePad virtual key.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// A <see cref="KeyRoutedEventArgs"/> which contains the event data.
        /// </param>
        public void ProcessKeyDown(object sender, KeyRoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Processes a KeyUp event which may include a GamePad virtual key.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// A <see cref="KeyRoutedEventArgs"/> which contains the event data.
        /// </param>
        public void ProcessKeyUp(object sender, KeyRoutedEventArgs e)
        {

        }

        /// <summary>
        /// Subscribes to <see cref="GamePad"/> related events on the specified control.
        /// </summary>
        /// <param name="control">
        /// The control supplying the events.
        /// </param>
        public void SubscribeEvents(Control control)
        {
            control.KeyDown += ProcessKeyDown;
            control.KeyUp += ProcessKeyUp;
        }

        /// <summary>
        /// Unsubscribes from <see cref="GamePad"/> related events on the specified control.
        /// </summary>
        /// <param name="control">
        /// The control supplying the events.
        /// </param>
        public void UnsubscribeEvents(Control control)
        {
            control.KeyDown -= ProcessKeyDown;
            control.KeyUp -= ProcessKeyUp;
        }
    }
}
