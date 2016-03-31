// Copyright (c) Microsoft. All rights reserved.
//
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adept.StateSync
{
    /// <summary>
    /// Represents a network synchronization message which can include one or more actions to apply.
    /// </summary>
    [JsonObject(Id = "Sync")]
    public class SyncMessage
    {
        #region Member Variables
        private List<ISyncAction> actions = new List<ISyncAction>();
        #endregion // Member Variables

        #region Public Properties
        /// <summary>
        /// Gets the list of actions to be applied as part of the synchronization.
        /// </summary>
        [JsonProperty("actions")]
        public List<ISyncAction> Actions => actions;

        /// <summary>
        /// Gets or sets the ID of the object being synchronized.
        /// </summary>
        [JsonProperty("syncId")]
        public string SyncId { get; set; }
        #endregion // Public Properties
    }
}
