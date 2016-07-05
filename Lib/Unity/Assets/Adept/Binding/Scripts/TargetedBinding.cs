// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace Adept.Unity
{
    /// <summary>
    /// Represents a binding where the target is known and shouldn't be displayed in the inspector.
    /// </summary>
    public abstract class TargetedBinding : BindingBase
    {
        #region Serialized Variables
        [SerializeField]
        [Tooltip("The name of the source member (property or field) that will participate in the binding.")]
        private string _sourceMemberName; // Inspector only.
        #endregion // Serialized Variables

        #region Overrides / Event Handlers
        protected override void Awake()
        {
            // Convert inspector values to property values
            SourceMemberName = _sourceMemberName;

            // Pass to base
            base.Awake();
        }
        #endregion // Overrides / Event Handlers
    }
}
