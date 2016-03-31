// Copyright (c) Microsoft. All rights reserved.
//
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Adept.StateSync
{
    /// <summary>
    /// A synchronization action that sets a property.
    /// </summary>
    [JsonObject(Id = "SetProperty")]
    public class SetPropertyAction : SyncAction
    {
        #region Public Methods
        /// <inheritdoc/>
        public override Task ApplyAsync(object target)
        {
            // Validate
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(PropertyName)) throw new ArgumentException(nameof(PropertyName));

            // Get the property
            var property = target.GetType().GetProperty(PropertyName);

            // Set the property
            property.SetValue(target, Value);

            // Done
            return Task.CompletedTask;
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the name of the property to set.
        /// </summary>
        [JsonProperty("name")]
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the value to set for the property.
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }
        #endregion // Public Properties
    }
}
