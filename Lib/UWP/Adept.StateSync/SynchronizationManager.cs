// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adept.StateSync
{
    /// <summary>
    /// Synchronizes object state across devices and application instances.
    /// </summary>
    public class SynchronizationManager
    {
        #region Nested Classes
        private class SyncRegistration
        {
            public string Id;
            public INotifyPropertyChanged Instance;
            public bool IsHost;
            public PropertyChangedEventHandler PropertyChanged;
        }
        #endregion // Nested Classes

        #region Member Variables
        private Dictionary<string, SyncRegistration> registrations = new Dictionary<string,SyncRegistration>();
        #endregion // Member Variables

        #region Overrides / Event Handlers
        private void Registration_PropertyChanged(SyncRegistration sender, PropertyChangedEventArgs e)
        {
            // Get the actual registered object
            var source = sender.Instance;

            // Get the property from the sender
            var property = source.GetType().GetProperty(e.PropertyName);

            // Look for a strategy
            var strategy = property.GetCustomAttribute<PropertyStrategyAttribute>(true);

            // If strategy was found, execute it
            if (strategy != null)
            {
                // Create the message
                SyncMessage message = new SyncMessage()
                {
                    SyncId = sender.Id
                };

                // Ask the strategy to add the actions
                strategy.AddActions(message, source, property);

                // TODO: Send the message
            }
        }
        #endregion // Overrides / Event Handlers

        #region Public Methods
        /// <summary>
        /// Starts synchronization of the object.
        /// </summary>
        /// <param name="instance">
        /// The instance of the object to synchronize.
        /// </param>
        /// <param name="id">
        /// The unique ID of the instance.
        /// </param>
        /// <param name="isHost">
        /// <c>true</c> if the instance should act as the host instance; otherwise <c>false</c>.
        /// </param>
        /// <remarks>
        /// <para>
        /// If <paramref name="isHost"/> is <c>true</c>, <see cref="SynchronizationManager"/> will 
        /// automatically send a <c>FullState</c> message with the current state of the object 
        /// provided at least one other client is currently connected. In addition, this instance 
        /// will be used to respond to <c>GetFullState</c> messages in the future.
        /// </para>
        /// <para>
        /// If<paramref name="isHost"/> is <c>false</c>, <see cref = "SynchronizationManager" /> will
        /// automatically send a <c>GetFullState</c> message provided at least one other client is 
        /// currently connected.
        /// </para>
        /// </remarks>
        public void StartSync(INotifyPropertyChanged instance, string id, bool isHost = false)
        {
            // Validate
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));

            // Thread safe
            lock (registrations)
            {
                // Look for an existing registration
                if (registrations.ContainsKey(id)) { throw new InvalidOperationException($"The ID '{id}' is already registered."); }

                // Create Registration
                var reg = new SyncRegistration()
                {
                    Id = id,
                    Instance = instance,
                    IsHost = isHost
                };

                // Register
                registrations[id] = reg;

                // Setup property changed to registration changed delegate
                reg.PropertyChanged = (o, e) => Registration_PropertyChanged(reg, e); 

                // Subscribe to changes
                instance.PropertyChanged += reg.PropertyChanged;
            }

            if (isHost)
            {
                // TODO: Send full state
            }
        }

        /// <summary>
        /// Stops synchronization of the specified object.
        /// </summary>
        /// <param name="id">
        /// The unique ID of the object.
        /// </param>
        public void StopSync(string id)
        {
            // Validate
            if (id == null) throw new ArgumentNullException(nameof(id));
            
            // Thread safe
            lock (registrations)
            {
                // Try to get the instance
                if (!registrations.ContainsKey(id)) { return; }
                var reg = registrations[id];
                var instance = reg.Instance;

                // Unsubscribe from changes
                instance.PropertyChanged -= reg.PropertyChanged;

                // Unregister
                registrations.Remove(id);
            }
        }
        #endregion // Public Methods
    }
}
