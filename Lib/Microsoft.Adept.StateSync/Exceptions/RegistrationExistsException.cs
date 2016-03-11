// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Adept.StateSync
{
    /// <summary>
    /// Thrown when a synchronization registration already exists.
    /// </summary>
    public class RegistrationExistsException : Exception
    {
        /// <summary>
        /// Initializes a new <see cref="RegistrationExistsException"/>.
        /// </summary>
        /// <param name="type">
        /// The registration type.
        /// </param>
        /// <param name="instanceId">
        /// The registration key.
        /// </param>
        public RegistrationExistsException(Type type, string instanceId)
        {
            // Validate
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (instanceId == null) throw new ArgumentNullException(nameof(instanceId));

            // Store
            Type = type;
            InstanceId = instanceId;
        }

        /// <summary>
        /// Gets the unique id of the registration or <see langword = "null" /> for the default instance.
        /// </summary>
        public string InstanceId { get; private set; }

        /// <inheritdoc/>
        public override string Message
        {
            get
            {
                if (InstanceId == null)
                {
                    return $"A registration for the default instance of '{Type.FullName}' already exists.";
                }
                else
                {
                    return $"A registration for the instance '{InstanceId}' of '{Type.FullName}' already exists.";
                }
            }
        }

        /// <summary>
        /// Gets the type of the registration.
        /// </summary>
        public Type Type { get; private set; }
    }
}
