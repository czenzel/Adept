// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace Adept.Unity
{
    /// <summary>
    /// Represents the source of data for multiple bindings.
    /// </summary>
    [AddComponentMenu("Binding/Data Context", 50)]
    public class DataContext : MonoBehaviour
    {
        #region Member Variables
        private object source;
        #endregion // Member Variables

        #region Internal Methods
        private void InnerSourceChanged(object source)
        {
            // Get all bindings in the current context
            var bindings = GetComponentsInParent<BindingBase>(true);
            
            // Update the target item
            foreach (var binding in bindings)
            {
                binding.Source = source;
            }
        }
        #endregion // Internal Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the source of data for the data context.
        /// </summary>
        public object Source
        {
            get
            {
                return source;
            }
            set
            {
                if (value != source)
                {
                    InnerSourceChanged(value);
                    source = value;
                }
            }
        }
        #endregion // Public Properties
    }
}
