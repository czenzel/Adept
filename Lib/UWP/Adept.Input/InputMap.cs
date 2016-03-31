using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Adept.Input
{
    /// <summary>
    /// Maps one or more triggers to an action.
    /// </summary>
    public class InputMap
    {
        #region Member Variables
        private ObservableCollection<IInputTrigger> triggers;
        #endregion // Member Variables

        public InputMap()
        {
            triggers = new ObservableCollection<IInputTrigger>();
            triggers.CollectionChanged += Triggers_CollectionChanged;
        }

        private void Triggers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (IInputTrigger t in e.NewItems)
                    {
                        t.Fired += Trigger_Fired;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (IInputTrigger t in e.OldItems)
                    {
                        t.Fired -= Trigger_Fired;
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // TODO: Support Clear
                    throw new InvalidOperationException();
                    // break;
            }
        }

        private void Trigger_Fired(object sender, EventArgs e)
        {
            // Make sure we have an action
            if (Action == null)
            {
                throw new InvalidOperationException("Action is not specified.");
            }

            // Cast
            IInputTrigger trigger = (IInputTrigger)sender;

            // Invoke action
            Action.Invoke(trigger.Values);
        }

        #region Public Properties
        /// <summary>
        /// Gets or sets the action that will be invoked when any of the triggers fire.
        /// </summary>
        public IInputAction Action { get; set; }

        /// <summary>
        /// Gets the collection of triggers that can invoke the action.
        /// </summary>
        public Collection<IInputTrigger> Triggers => triggers;
        #endregion // Public Properties
    }
}
