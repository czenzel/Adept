// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if !WINDOWS_UWP
namespace System.Collections.Specialized
{
    /// <summary>
    /// Represents the method that handles the <see cref="CollectionChanged"/> event. 
    /// </summary>
    /// <param name="sender">
    /// The object that raised the event.
    /// </param>
    /// <param name="e">
    /// Information about the event.
    /// </param>
    public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);

    /// <summary>
    /// Notifies listeners of dynamic changes, such as when items get added and removed or the whole list is refreshed.
    /// </summary>
    public interface INotifyCollectionChanged
    {
#region Public Events
        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;
#endregion // Public Events
    }
}
#endif // !WINDOWS_UWP