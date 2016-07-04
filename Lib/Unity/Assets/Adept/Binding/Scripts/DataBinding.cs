// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace Adept.Unity
{
    /// <summary>
    /// Represents a binding between a data item and another item item.
    /// </summary>
    [AddComponentMenu("Binding/Data Binding", 51)]
    public class DataBinding : BindingBase
    {
        #region Overrides / Event Handlers
        protected override void Initialize()
        {
            SourceMemberName = sourceMemberName;
            TargetMemberName = targetMemberName;
            base.Initialize();
        }
        #endregion // Overrides / Event Handlers

        private void Start()
        {
            Initialize();
        }

        #region Inspector Properties
        [Tooltip("The name of the field or property on the data item which will supply the value.")]
        public string sourceMemberName;

        [Tooltip("The name of the field or property on the target that will be updated.")]
        public string targetMemberName;
        #endregion // Inspector Properties
    }
}
