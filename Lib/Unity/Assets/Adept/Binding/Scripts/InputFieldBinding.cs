// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using UnityEngine.UI;

namespace Adept.Unity
{
    /// <summary>
    /// Represents a binding between a data item and an Input Field Control.
    /// </summary>
    [AddComponentMenu("Binding/Input Field Binding", 53)]
    public class InputFieldBinding : BindingBase
    {
        #region Inspector Items
        [Tooltip("The input field that will represent the item.")]
        public InputField inputField;

        [Tooltip("The name of the field or property on the data item which will supply the value.")]
        public string sourceMemberName;
        #endregion // Inspector Items

        #region Overrides / Event Handlers
        protected override void Initialize()
        {
            SourceMemberName = sourceMemberName;

            if (inputField != null)
            {
                // Set the target member to text
                TargetMemberName = "text";

                // Set the target
                Target = inputField;

                // Set mode to bi-directional
                Mode = BindingMode.TwoWay;

                // Subscribe to character change
                inputField.onValueChanged.AddListener(OnInputChanged);
            }

            base.Initialize();
        }

        private void OnInputChanged(string text)
        {
            if (Mode == BindingMode.TwoWay)
            {
                ApplyBinding(BindingDirection.TargetToSource);
            }
        }
        #endregion // Overrides / Event Handlers

        private void Start()
        {
            Initialize();
        }
    }
}
