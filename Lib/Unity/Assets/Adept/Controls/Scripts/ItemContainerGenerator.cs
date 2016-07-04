// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

namespace Adept.Unity
{
    public class ItemContainerGenerator
    {
        #region Member Variables
        private Dictionary<object, GameObject> containersByItem;
        private Dictionary<GameObject, object> itemsByContainer;
        private ItemsControl parent;
        #endregion // Member Variables

        public ItemContainerGenerator(ItemsControl parent)
        {
            // Validate
            if (parent == null) throw new ArgumentNullException("parent");

            // Store
            this.parent = parent;

            // Create
            containersByItem = new Dictionary<object, GameObject>();
            itemsByContainer = new Dictionary<GameObject, object>();
        }

        private void AddItem(object item)
        {
            // If already created, ignore
            if (containersByItem.ContainsKey(item)) { return; }

            // Create the container
            var container = parent.CreateItemContainer();

            // Prepare the container
            parent.PrepareContainerForItem(container, item);

            // Add the container to the panel
            container.transform.SetParent(parent.itemsPanel.transform, false);

            // Add to lookup
            containersByItem[item] = container;
            itemsByContainer[container] = item;
        }

        private void ClearItems()
        {
            var items = containersByItem.Keys.ToList();
            foreach (var item in items)
            {
                RemoveItem(item);
            }
        }

        private void RemoveItem(object item)
        {
            // If not created, ignore
            if (!containersByItem.ContainsKey(item)) { return; }

            // Get the container
            var container = containersByItem[item];

            // Clear the container (which removes bindings and frees the template)
            parent.ClearContainerForItem(container, item);

            // Remove the container from the panel
            container.transform.SetParent(null, false);

            // Remove from lookup
            containersByItem.Remove(item);
            itemsByContainer.Remove(container);

            // Destroy the container
            GameObject.Destroy(container);
        }

        private void Reset()
        {
            ClearItems();
            var enumerable = parent.ItemsSource as IEnumerable;
            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    AddItem(item);
                }
            }
        }

        internal void HandleChange(NotifyCollectionChangedEventArgs e)
        {
            // Verify that the parent has a panel
            if (parent.itemsPanel == null)
            {
                throw new InvalidOperationException("The items control does not have an Items Panel set.");
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        AddItem(item);
                    }
                    break;
                
                case NotifyCollectionChangedAction.Move:
                    // TODO: Not implemented
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        RemoveItem(item);
                    }
                    break;
                
                case NotifyCollectionChangedAction.Replace:
                    // TODO: Not implemented
                    break;
                
                case NotifyCollectionChangedAction.Reset:
                    Reset();
                    break;
            }
        }

        /// <summary>
        /// Returns the <see cref="GameObject"/> corresponding to the given item.
        /// </summary>
        /// <param name="item">
        /// The item to find the <see cref="GameObject"/> for.
        /// </param>
        /// <returns>
        /// A <see cref="GameObject"/> that corresponds to the given item. 
        /// Returns null if the item does not belong to the item collection, 
        /// or if a <see cref="GameObject"/> has not been generated for it.
        /// </returns>
        public GameObject ContainerFromItem(object item)
        {
            if (containersByItem.ContainsKey(item))
            {
                return containersByItem[item];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the item that corresponds to the specified, generated <see cref="GameObject"/>.
        /// </summary>
        /// <param name="container"></param>
        /// <returns>
        /// An object that is the item which corresponds to the specified, generated <see cref="GameObject"/>. 
        /// If the <see cref="GameObject"/> has not been generated, null is returned. 
        /// </returns>
        public object ItemFromContainer(GameObject container)
        {
            if (itemsByContainer.ContainsKey(container))
            {
                return itemsByContainer[container];
            }
            else
            {
                return null;
            }
        }
    }
}
