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
    public class ControlBinding : DataBinding
    {
        #region Inspector Items
        [Tooltip("The target control that the will receive updates.")]
        public UIBehaviour control;
        #endregion // Inspector Items

        #region Overrides / Event Handlers
        protected override void Initialize()
        {
            Target = control;
            base.Initialize();
        }
        #endregion // Overrides / Event Handlers

        private void Start()
        {
            Initialize();
        }
    }
}
