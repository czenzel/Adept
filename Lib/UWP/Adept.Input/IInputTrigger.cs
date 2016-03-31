using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Microsoft.Adept.Input
{
    /*
    /// <summary>
    /// The base interface for an input trigger.
    /// </summary>
    public interface IInputTriggerBase
    {
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
        #endregion // Public Properties
    }*/

    /// <summary>
    /// The interface for a class that can trigger an input action.
    /// </summary>
    public interface IInputTrigger
    {
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
        /// Gets the list of values provided by the trigger.
        /// </summary>
        ParameterValueSet Values { get; }
        #endregion // Public Properties

        #region Public Events
        /// <summary>
        /// Raised at the moment the trigger fires.
        /// </summary>
        event EventHandler Fired;
        #endregion // Public Events
    }

    /*
    /// <summary>
    /// An input trigger that provides custom data.
    /// </summary>
    /// <typeparam name="T">
    /// The data provided by the trigger.
    /// </typeparam>
    public interface IInputTrigger<T> : IInputTriggerBase
    {
        #region Public Events
        /// <summary>
        /// Raised at the moment the trigger starts.
        /// </summary>
        event TypedEventHandler<IInputTrigger<T>, T> Started;

        /// <summary>
        /// Raised at the moment the trigger ends.
        /// </summary>
        event TypedEventHandler<IInputTrigger<T>, T> Ended;
        #endregion // Public Events
    }
    */
}
