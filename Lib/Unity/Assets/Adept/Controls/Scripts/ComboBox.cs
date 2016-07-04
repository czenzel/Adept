// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace Adept.Unity
{
    /// <summary>
    /// A selection control that combines a non-editable selection box and a drop-down 
    /// containing a list box that allows users to select an item from a list.
    /// </summary>
    [AddComponentMenu("UI/Combo Box", 41)]
    public class ComboBox : Selector
    {
        #region Constants
        /// <summary>
        /// The name of the child GameObject that represents the instantiated selected item.
        /// </summary>
        public const string SelectedItemTemplateName = "SelectedItemTemplate";
        #endregion // Constants

        #region Member Variables
        private bool dropdownOpen;
        #endregion // Member Variables

        // Use this for initialization
        private void Start()
        {
            base.Initialize();
            if (selectedItemContainer == null)
            {
                selectedItemContainer = transform.Find("SelectedItemContainer").gameObject;
            }
            if (itemsContainer == null)
            {
                itemsContainer = transform.Find("ItemsContainer").gameObject;
            }
            this.itemsContainer.SetActive(false);
            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            // Pass to base first
            base.OnSelectionChanged(e);

            // Hide the dropdown
            HideDrowpdown();

            // If there is no selected item container, nothing else to do
            if (selectedItemContainer == null) { return; }

            // If there was a previously selected item template instance, destroy it
            var selectedItemTrans = selectedItemContainer.transform.Find(SelectedItemTemplateName);
            if (selectedItemTrans != null)
            {
                Destroy(selectedItemTrans.gameObject);
            }

            // If there is no selected item, bail
            if (SelectedItem == null) { return; }

            // Create a new selected item template instance
            var selectedItemTemplate = CreateItemTemplate();

            // Prepare the template which does binding
            PrepareTemplateForItem(selectedItemTemplate, SelectedItem);

            // Give it the correct name
            selectedItemTemplate.name = SelectedItemTemplateName;

            // Add it to the container
            selectedItemTemplate.transform.SetParent(selectedItemContainer.transform, false);
        }

        public void HideDrowpdown()
        {
            dropdownOpen = false;
            if (itemsContainer != null)
            {
                itemsContainer.SetActive(dropdownOpen);
            }
        }

        public void ShowDropdown()
        {
            dropdownOpen = true;
            if (itemsContainer != null)
            {
                itemsContainer.SetActive(dropdownOpen);
            }
        }

        public void ToggleDropdown()
        {
            dropdownOpen = !dropdownOpen;
            if (itemsContainer != null)
            {
                itemsContainer.SetActive(dropdownOpen);
            }
        }
 
        #region Inspector Properties
        /// <summary>
        /// Gets or sets the contianer that will hold the template instance for the selected item.
        /// </summary>
        [Tooltip("The contianer that will hold the template instance for the selected item.")]
        public GameObject selectedItemContainer;

        /// <summary>
        /// Gets or sets the container that will hold template instances for all items in the list.
        /// </summary>
        [Tooltip("The container that will hold template instances for all items in the list.")]
        public GameObject itemsContainer;
        #endregion // Inspector  Properties
    }
}