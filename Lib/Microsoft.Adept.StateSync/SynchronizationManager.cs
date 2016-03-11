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

namespace Microsoft.Adept.StateSync
{
    /// <summary>
    /// Synchronizes object state across devices and application instances.
    /// </summary>
    public class SynchronizationManager
    {
        #region Nested Classes
        private class SyncRegistration
        {
            public INotifyPropertyChanged Instance;
            public string InstanceId;
            public bool IsHost;
            public Type Type;
        }
        #endregion // Nested Classes

        #region Member Variables
        private Dictionary<Type, Dictionary<string, SyncRegistration>> registrations = new Dictionary<Type, Dictionary<string,SyncRegistration>>();
        #endregion // Member Variables

        #region Internal Methods
        /// <summary>
        /// Registers the specified instance for the type and instance ID.
        /// </summary>
        /// <param name="instance">
        /// The instance to register.
        /// </param>
        /// <param name="isHost">
        /// <c>true</c> if the instance is acting as the host; otherwise <c>false</c>.
        /// </param>
        /// <param name="type">
        /// The type used for the registration or <see langword = "null" /> to use the type of the instance.
        /// </param>
        /// <param name="instanceId">
        /// The ID of the instance to register or <see langword = "null" /> for the default instance.
        /// </param>
        private void Register(INotifyPropertyChanged instance, bool isHost, Type type, string instanceId)
        {
            // Validate
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            // If no type specified, use instance type. Otherwise validate.
            if (type == null)
            {
                type = instance.GetType();
            }
            else
            {
                // Instance type
                var instanceType = instance.GetType();

                // Make sure compatible
                if (!type.IsAssignableFrom(instanceType))
                {
                    throw new InvalidOperationException($"The types '{type.Name}' and '{instanceType.Name}' are not compatible.");
                }
            }

            // Look for an existing registration
            var existing = TryGetRegistration(type, instanceId);
            if (existing != null) { throw new RegistrationExistsException(type, instanceId); }

            // Get registration table for type
            if (!registrations.ContainsKey(type)) { registrations[type] = new Dictionary<string, SyncRegistration>(); }
            var typeTable = registrations[type];

            // Register
            typeTable[instanceId] = new SyncRegistration()
            {
                Type = type,
                InstanceId = instanceId,
                Instance = instance,
                IsHost = isHost
            };
        }

        /// <summary>
        /// Tries to get the registered instance.
        /// </summary>
        /// <param name="type">
        /// The type of instance to locate.
        /// </param>
        /// <param name="instanceId">
        /// The ID of the instance to locate or <see langword = "null" /> for the default instance.
        /// </param>
        /// <returns>
        /// The instance or <see langword = "null" /> if the instance is not found.
        /// </returns>
        private INotifyPropertyChanged TryGetRegistration(Type type, string instanceId)
        {
            // Get registration table for type
            if (!registrations.ContainsKey(type)) { return null; }
            var typeTable = registrations[type];

            // Lookup by Id
            if (!typeTable.ContainsKey(instanceId)) { return null; }

            // Return instance
            return typeTable[instanceId].Instance;
        }
        #endregion // Internal Methods

        #region Public Methods

        /// <summary>
        /// Starts synchronization of the specified object instance.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to synchronize.
        /// </typeparam>
        /// <param name="instance">
        /// The instance of the object to synchronize.
        /// </param>
        /// <param name="instanceId">
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
        public void StartSync(INotifyPropertyChanged instance, bool isHost = false, Type type = null, string instanceId = null)
        {
            // Register (this includes validation)
            Register(instance)

            TryGetRegistration<T>(instanceId);

            // Register
            registrations.Add(new SyncRegistration()
            {
                Type = typeof(T),
                InstanceId = instanceId,
                Instance = instance,
                IsHost = isHost
            });

            // TODO: Subscribe to changes

            if (isHost)
            {
                // TODO: Send full state
            }

        }

        /// <summary>
        /// Starts synchronization of the specified object instance.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to synchronize.
        /// </typeparam>
        /// <param name="instance">
        /// The instance of the object to synchronize.
        /// </param>
        /// <param name="instanceId">
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
        public void StartSync<T>(T instance, string instanceId, bool isHost = false) where T : INotifyPropertyChanged
        {

        }


        /// <summary>
        /// Starts synchronization of the default instance for the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to synchronize.
        /// </typeparam>
        /// <param name="instance">
        /// The instance of the object to synchronize.
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
        public void StartSync<T>(T instance, bool isHost = false) where T : INotifyPropertyChanged
        {

        }

        /// <summary>
        /// Stops synchronization of the specified object instance.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to stop synchronizing.
        /// </typeparam>
        /// <param name="instanceId">
        /// The unique ID of the instance.
        /// </param>
        public void StopSync<T>(string instanceId) where T : INotifyPropertyChanged
        {

        }

        /// <summary>
        /// Stops synchronization of the default instance for the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to stop synchronizing.
        /// </typeparam>
        public void StopSync<T>() where T : INotifyPropertyChanged
        {

        }
        #endregion // Public Methods
    }
}
