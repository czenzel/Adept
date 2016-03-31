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
    /// Base class for synchronization strategies that apply to properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class PropertyStrategyAttribute : Attribute
    {
        /// <summary>
        /// Adds custom actions to the message.
        /// </summary>
        /// <param name="message">
        /// The message where actions are added.
        /// </param>
        /// <param name="source">
        /// The source object where the strategy is applied.
        /// </param>
        /// <param name="property">
        /// The property that the strategy is applied to.
        /// </param>
        public abstract void AddActions(SyncMessage message, object source, PropertyInfo property);
    }
}
