// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Adept.StateSync
{
    /// <summary>
    /// A strategy for synchronizing the raw property value.
    /// </summary>
    public class SynchronizeValueAttribute : PropertyStrategyAttribute
    {
        /// <inheritdoc/>
        public override void AddActions(SyncMessage message, object source, PropertyInfo property)
        {
            // Validate
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (property == null) throw new ArgumentNullException(nameof(property));

            // Create the action
            var action = new SetPropertyAction()
            {
                PropertyName = property.Name,
                Value = property.GetValue(source),
            };

            // Add it to the message
            message.Actions.Add(action);
        }
    }
}
