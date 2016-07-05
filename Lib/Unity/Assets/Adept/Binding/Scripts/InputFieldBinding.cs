// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using UnityEngine.UI;

namespace Adept.Unity
{
    /// <summary>
    /// A binding between a data item and an <see cref="InputField"/> UI control.
    /// </summary>
    [AddComponentMenu("Binding/Input Field Binding", 53)]
    public class InputFieldBinding : TargetedBinding
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("The input field that will participate in the binding.")]
        private InputField inputField;
        #endregion // Serialized Items

        #region Overrides / Event Handlers
        protected override void Awake()
        {
            // Set mode to bi-directional
            Mode = BindingMode.TwoWay;

            // Set the target member to text
            TargetMemberName = "text";

            // Set the target
            Target = inputField;

            if (inputField != null)
            {
                // Subscribe to character change
                inputField.onValueChanged.AddListener(OnInputChanged);
            }

            // Pass to base
            base.Awake();
        }

        private void OnInputChanged(string text)
        {
            if (Mode == BindingMode.TwoWay)
            {
                ApplyBinding(BindingDirection.TargetToSource);
            }
        }
        #endregion // Overrides / Event Handlers

        #region Public Properties
        /// <summary>
        /// Gets or sets the <see cref="InputField"/> that will participate in the binding.
        /// </summary>
        public InputField InputField
        {
            get
            {
                return inputField;
            }
            set
            {
                inputField = value;
            }
        }
        #endregion // Public Properties
    }
}
