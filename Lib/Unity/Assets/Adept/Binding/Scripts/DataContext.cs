// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private List<BindingBase> bindings = new List<BindingBase>();
        private object source;
        #endregion // Member Variables

        #region Internal Methods
        private void InnerSourceChanged(object source)
        {
            // Update all registered bindings
            lock (bindings)
            {
                foreach (var binding in bindings)
                {
                    binding.HandleSourceChanged(source);
                }
            }
        }
        #endregion // Internal Methods

        #region Public Methods
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RegisterBinding(BindingBase binding)
        {
            if (binding == null) throw new ArgumentNullException("binding");
            lock (bindings)
            {
                if (!bindings.Contains(binding))
                {
                    bindings.Add(binding);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnregisterBinding(BindingBase binding)
        {
            if (binding == null) throw new ArgumentNullException("binding");
            lock (bindings)
            {
                bindings.Remove(binding);
            }
        }
        #endregion // Public Methods

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
