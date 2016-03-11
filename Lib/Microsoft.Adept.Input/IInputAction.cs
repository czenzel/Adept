// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Adept.Input
{
    /// <summary>
    /// An action that can be triggered by a form of input.
    /// </summary>
    public interface IInputAction
    {
        #region Public Methods
        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <returns>
        /// The result of the action.
        /// </returns>
        object Invoke();
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets a value that indicates if the trigger is enabled.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// The name of the action.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the action.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a list of the parameters associated with the action.
        /// </summary>
        IReadOnlyList<ParameterDefinition> Parameters { get; }

        /// <summary>
        /// Gets the type of value returned by the action.
        /// </summary>
        Type ReturnType { get; }

        /// <summary>
        /// Gets or sets the collection of values that will be used when executing the action.
        /// </summary>
        ParameterValueSet Values { get; set; }
        #endregion // Public Properties
    }
}
