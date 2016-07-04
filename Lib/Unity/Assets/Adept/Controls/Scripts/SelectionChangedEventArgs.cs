// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Collections;

namespace Adept.Unity
{
    /// <summary>
    /// Provides data for the <see cref="SelectionChanged"/> event. 
    /// </summary>
    public class SelectionChangedEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionChangedEventArgs"/> class. 
        /// </summary>
        /// <param name="removedItems">
        /// The items that were unselected during this event.
        /// </param>
        /// <param name="addedItems">
        /// The items that were selected during this event.
        /// </param>
        public SelectionChangedEventArgs(IList removedItems, IList addedItems)
        {
            RemovedItems = (removedItems ?? new ArrayList());
            AddedItems = (addedItems ?? new ArrayList());
        }
        #endregion // Constructors

        #region Public Properties
        /// <summary>
        /// Gets a list of items that were selected during this event.
        /// </summary>
        public IList AddedItems { get; private set; }

        /// <summary>
        /// Gets a list of items that were unselected during this event.
        /// </summary>
        public IList RemovedItems { get; private set; }
        #endregion // Public Properties
    }
}
