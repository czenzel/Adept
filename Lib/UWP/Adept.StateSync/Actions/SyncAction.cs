// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adept.StateSync
{
    /// <summary>
    /// The interface for a single action to apply as part of a synchronization message.
    /// </summary>
    public interface ISyncAction
    {
        /// <summary>
        /// Applies the action to the specified target.
        /// </summary>
        /// <param name="target">
        /// The target where the action will be applied.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        Task ApplyAsync(object target);
    }

    /// <summary>
    /// Represents a single action to apply as part of a synchronization message.
    /// </summary>
    public abstract class SyncAction : ISyncAction
    {
        /// <inheritdoc/>
        public abstract Task ApplyAsync(object target);
    }
}
