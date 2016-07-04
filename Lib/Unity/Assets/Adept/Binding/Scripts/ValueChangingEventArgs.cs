// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adept.Unity
{
    /// <summary>
    /// Provides the data for methods that handle values changing.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of value that is changing.
    /// </typeparam>
    public class ValueChangingEventArgs<TValue> : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="ValueChangingEventArgs"/> instance.
        /// </summary>
        /// <param name="oldValue">
        /// The old value.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        public ValueChangingEventArgs(TValue oldValue, TValue newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        #endregion // Constructors

        #region Public Properties
        /// <summary>
        /// Gets the old value.
        /// </summary>
        public TValue OldValue { get; private set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public TValue NewValue { get; private set; }
        #endregion // Public Properties
    }
}
