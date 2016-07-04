// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Adept.Unity
{
    /// <summary>
    /// Provides data for the <see cref="ItemsControl.SelectionChanged">SelectionChanged</see> event.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// A <see cref="SelectionChangedEventArgs"/> containing the event data.
    /// </param>
    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

    /// <summary>
    /// Represents a control that allows a user to select items from among its child elements.
    /// </summary>
    public class Selector : ItemsControl
    {
        #region Member Variables
        private object selectedItem;
        #endregion // Member Variables

        #region Internal Methods
        private void SelectContainer(GameObject container)
        {
            var selectable = container.GetComponent<Selectable>();
            if (selectable != null)
            {
                selectable.Select();
            }
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        protected override void ClearContainerForItemOverride(GameObject container, object item)
        {
            // Remove selectable
            var selectable = container.GetComponent<Selectable>();
            if (selectable != null)
            {
                Destroy(selectable);
            }

            // Remove selection trigger
            var trigger = container.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                // TODO: Do we need to unsubscribe to avoid a memory leak?
                Destroy(trigger);
            }

            // Pass on to base
            base.ClearContainerForItemOverride(container, item);
        }

        private void ContainerSelected(BaseEventData e)
        {
            // Selected object is the container
            var so = e.selectedObject;
            if (so == null) { return; }

            // Get the item from the container
            var item = ItemContainerGenerator.ItemFromContainer(so);

            // If the item was obtained, specify it as selected
            SelectedItem = item;
        }

        private void InnerSelectionChanged(SelectionChangedEventArgs e)
        {
            // If a new item is selected, make the container selected
            if (e.AddedItems.Count > 0)
            {
                // Try to get the container for the item
                var container = ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]);

                // If obtained, select
                if (container != null) { SelectContainer(container); }
            }

            // Pass on to overrides
            OnSelectionChanged(e);
        }

        protected override void PrepareContainerForItemOverride(GameObject container, object item)
        {
            // Let base process first
            base.PrepareContainerForItemOverride(container, item);

            // Find existing or create selectable
            var selectable = container.GetComponent<Selectable>();
            if (selectable == null)
            {
                selectable = container.AddComponent<Selectable>();
            }

            // Find existing or create event trigger
            var trigger = container.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = container.AddComponent<EventTrigger>();
            }

            // Create the new entry
            EventTrigger.Entry entry = new EventTrigger.Entry();

            // Set event ID to select
            entry.eventID = EventTriggerType.Select;

            // Link Select event to ContainerSelected handler
            entry.callback.AddListener(ContainerSelected);

            // Add entry to the trigger
            trigger.triggers.Add(entry);

            /*
            // Make sure there is a delegates collection
            if (trigger.triggers == null) { trigger.triggers = new List<EventTrigger.Entry>(); }

            // Create selection callback
            EventTrigger.TriggerEvent callback = new EventTrigger.TriggerEvent();
            callback.AddListener(ContainerSelected);

            // Subscribe to selection
            trigger.triggers.Add(new EventTrigger.Entry()
                {
                    eventID = EventTriggerType.Select,
                    callback = callback,
                });
                */
        }
        #endregion // Overrides / Event Handlers

        #region Overridables / Event Triggers
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }
        #endregion // Overridables / Event Triggers

        #region Public Properties
        /// <summary>
        /// Gets or sets the first item in the current selection or returns null if the selection is empty.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (value != selectedItem)
                {
                    var unsel = new ArrayList();
                    var sel = new ArrayList();

                    if (selectedItem != null)
                    {
                        unsel.Add(selectedItem);
                    }

                    selectedItem = value;

                    if (selectedItem != null)
                    {
                        sel.Add(selectedItem);
                    }

                    InnerSelectionChanged(new SelectionChangedEventArgs(unsel, sel));
                }
            }
        }
        #endregion // Public Properties

        #region Public Events
        /// <summary>
        /// Occurs when the selection of a <see cref="Selector"/> changes. 
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;
        #endregion // Public Events
    }
}
