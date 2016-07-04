// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if !WINDOWS_UWP
namespace System.Collections.Specialized
{
    /// <summary>
    /// Describes the action that caused a <see cref="CollectionChanged"/> event.
    /// </summary>
    public enum NotifyCollectionChangedAction
    {
        /// <summary>
        /// One or more items were added to the collection.
        /// </summary>
        Add = 0,

        /// <summary>
        /// One or more items were moved within the collection.
        /// </summary>
        Move = 3,

        /// <summary>
        /// One or more items were removed from the collection.
        /// </summary>
        Remove = 1,

        /// <summary>
        /// One or more items were replaced in the collection.
        /// </summary>
        Replace = 2,

        /// <summary>
        /// The content of the collection changed dramatically.
        /// </summary>
        Reset = 4
    }

    /// <summary>
    /// Provides data for the <see cref="CollectionChanged"/> event.
    /// </summary>
    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        #region Nested Classes
        /// <summary>
        /// Replaces the localized resources from the .Net framework.
        /// </summary>
        static private class SR
        {
            public const string WrongActionForCtor = "The specified action is not supported by this constructor.";
            public const string MustBeResetAddOrRemoveActionForCtor = "Only Reset, Add or Remove actions are supported by this constructor.";
            public const string ResetActionRequiresNullItem = "Reset action requires a null item.";
            public const string ResetActionRequiresIndexMinus1 = "Reset action requires an index of -1.";
            public const string IndexCannotBeNegative = "Index cannot be negative";
        }
        #endregion // Nested Classes

        #region Member Variables
        private NotifyCollectionChangedAction _action;
        private IList _newItems;
        private int _newStartingIndex;
        private IList _oldItems;
        private int _oldStartingIndex;
        #endregion // Member Variables

        #region Constructors
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentException(SR.WrongActionForCtor, "action");
            }
            this.InitializeAdd(action, null, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(SR.MustBeResetAddOrRemoveActionForCtor, "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException(SR.ResetActionRequiresNullItem, "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                this.InitializeAddOrRemove(action, changedItems, -1);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(SR.MustBeResetAddOrRemoveActionForCtor, "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException(SR.ResetActionRequiresNullItem, "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                this.InitializeAddOrRemove(action, new object[] { changedItem }, -1);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(SR.WrongActionForCtor, "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            this.InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(SR.MustBeResetAddOrRemoveActionForCtor, "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException(SR.ResetActionRequiresNullItem, "action");
                }
                if (startingIndex != -1)
                {
                    throw new ArgumentException(SR.ResetActionRequiresIndexMinus1, "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                if (startingIndex < -1)
                {
                    throw new ArgumentException(SR.IndexCannotBeNegative, "startingIndex");
                }
                this.InitializeAddOrRemove(action, changedItems, startingIndex);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(SR.MustBeResetAddOrRemoveActionForCtor, "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException(SR.ResetActionRequiresNullItem, "action");
                }
                if (index != -1)
                {
                    throw new ArgumentException(SR.ResetActionRequiresIndexMinus1, "action");
                }
                this.InitializeAdd(action, null, -1);
            }
            else
            {
                this.InitializeAddOrRemove(action, new object[] { changedItem }, index);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(SR.WrongActionForCtor, "action");
            }
            this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, -1, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(SR.WrongActionForCtor, "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            this.InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList changedItems, int index, int oldIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException(SR.WrongActionForCtor, "action");
            }
            if (index < 0)
            {
                throw new ArgumentException(SR.IndexCannotBeNegative, "index");
            }
            this.InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object changedItem, int index, int oldIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException(SR.WrongActionForCtor, "action");
            }
            if (index < 0)
            {
                throw new ArgumentException(SR.IndexCannotBeNegative, "index");
            }
            object[] newItems = new object[] { changedItem };
            this.InitializeMoveOrReplace(action, newItems, newItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, object newItem, object oldItem, int index)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(SR.WrongActionForCtor, "action");
            }
            int oldStartingIndex = index;
            this.InitializeMoveOrReplace(action, new object[] { newItem }, new object[] { oldItem }, index, oldStartingIndex);
        }

        internal NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newIndex, int oldIndex)
        {
            this._newStartingIndex = -1;
            this._oldStartingIndex = -1;
            this._action = action;
            this._newItems = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
            this._oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
            this._newStartingIndex = newIndex;
            this._oldStartingIndex = oldIndex;
        }
        #endregion // Constructors

        #region Internal Methods
        private void InitializeAdd(NotifyCollectionChangedAction action, IList newItems, int newStartingIndex)
        {
            this._action = action;
            this._newItems = (newItems == null) ? null : ArrayList.ReadOnly(newItems);
            this._newStartingIndex = newStartingIndex;
        }

        private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList changedItems, int startingIndex)
        {
            if (action == NotifyCollectionChangedAction.Add)
            {
                this.InitializeAdd(action, changedItems, startingIndex);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                this.InitializeRemove(action, changedItems, startingIndex);
            }
        }

        private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int startingIndex, int oldStartingIndex)
        {
            this.InitializeAdd(action, newItems, startingIndex);
            this.InitializeRemove(action, oldItems, oldStartingIndex);
        }

        private void InitializeRemove(NotifyCollectionChangedAction action, IList oldItems, int oldStartingIndex)
        {
            this._action = action;
            this._oldItems = (oldItems == null) ? null : ArrayList.ReadOnly(oldItems);
            this._oldStartingIndex = oldStartingIndex;
        }
        #endregion // Internal Methods

        #region Public Properties
        /// <summary>
        /// Gets the action that caused the event.
        /// </summary>
        public NotifyCollectionChangedAction Action
        {
            get
            {
                return this._action;
            }
        }

        /// <summary>
        /// Gets the list of new items involved in the change.
        /// </summary>
        public IList NewItems
        {
            get
            {
                return this._newItems;
            }
        }

        /// <summary>
        /// Gets the index at which the change occurred.
        /// </summary>
        public int NewStartingIndex
        {
            get
            {
                return this._newStartingIndex;
            }
        }

        /// <summary>
        /// Gets the list of items affected by a Replace, Remove, or Move action.
        /// </summary>
        public IList OldItems
        {
            get
            {
                return this._oldItems;
            }
        }

        /// <summary>
        /// Gets the index at which a Move, Remove, or Replace action occurred.
        /// </summary>
        public int OldStartingIndex
        {
            get
            {
                return this._oldStartingIndex;
            }
        }
        #endregion // Public Properties
    }
}
#endif // !WINDOWS_UWP