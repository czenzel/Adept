// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using UnityEngine.EventSystems;

namespace Adept.Unity
{
    /// <summary>
    /// Represents a binding between a data item and a UI Control.
    /// </summary>
    [AddComponentMenu("Binding/ItemsSource Binding", 54)]
    public class ItemsSourceBinding : TargetedBinding
    {
        #region Inspector Items
        [SerializeField]
        [Tooltip("The ItemsControl that will participate in the binding. (Required)")]
        private ItemsControl itemsControl;
        #endregion // Inspector Items

        #region Overrides / Event Handlers
        protected override void Awake()
        {
            // Set known member names
            TargetMemberName = "ItemsSource";

            // Convert inspector values to property values
            Target = itemsControl;

            // Pass to base
            base.Awake();
        }
        #endregion // Overrides / Event Handlers

        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="Adept.Unity.ItemsControl"/> that will participate in the binding.
        /// </summary>
        public ItemsControl ItemsControl
        {
            get
            {
                return itemsControl;
            }
            set
            {
                itemsControl = value;
            }
        }
        #endregion // Public Properties
    }
}
