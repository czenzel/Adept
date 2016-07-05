// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using UnityEngine.EventSystems;

namespace Adept.Unity
{
    /// <summary>
    /// Represents a binding between a data item and a UI Control.
    /// </summary>
    [AddComponentMenu("Binding/Control Binding", 52)]
    public class ControlBinding : TargetedBinding
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("The target control that will participate in the binding.")]
        private UIBehaviour control;

        [SerializeField]
        [Tooltip("The name of the target member (property or field) that will participate in the binding.")]
        private string _targetMemberName; // Inspector only.
        #endregion // Serialized Variables

        #region Overrides / Event Handlers
        protected override void Awake()
        {
            // Convert inspector values to property values
            TargetMemberName = _targetMemberName;
            Target = control;

            // Pass to base
            base.Awake();
        }
        #endregion // Overrides / Event Handlers

        #region Public Properties
        /// <summary>
        /// Gets or sets the target control that will participate in the binding.
        /// </summary>
        public UIBehaviour Control
        {
            get
            {
                return control;
            }
            set
            {
                if (control != value)
                {
                    control = value;
                    Target = value;
                }
            }
        }
        #endregion // Public Properties
    }
}
