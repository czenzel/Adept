// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if !WINDOWS_UWP
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A collection that implements the <see cref="INotifyCollectionChanged"/> and 
    /// <see cref="INotifyPropertyChanged"/> interfaces.
    /// </summary>
    /// <typeparam name="T">
    /// The type of item stored in the collection.
    /// </typeparam>
    public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Nested Classes
        private class SimpleMonitor : IDisposable
        {
            #region Member Variables
            private int _busyCount;
            #endregion // Member Variables

            #region Public Methods
            public void Dispose()
            {
                this._busyCount--;
            }

            public void Enter()
            {
                this._busyCount++;
            }
            #endregion // Public Methods

            #region Public Properties
            public bool Busy
            {
                get
                {
                    return (this._busyCount > 0);
                }
            }
            #endregion // Public Properties
        }

        /// <summary>
        /// Replaces the localized resources from the .Net framework.
        /// </summary>
        static private class SR
        {
            public const string ObservableCollectionReentrancyNotAllowed = "ObservableCollection reentrancy is not allowed.";
        }
        #endregion // Nested Classes

        #region Member Variables
        private SimpleMonitor _monitor;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        #endregion // Member Variables

        #region Constructors
        public ObservableCollection()
        {
            this._monitor = new SimpleMonitor();
        }

        public ObservableCollection(IEnumerable<T> collection)
        {
            this._monitor = new SimpleMonitor();
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            this.CopyFrom(collection);
        }

        public ObservableCollection(List<T> list)
            : base((list != null) ? new List<T>(list.Count) : list)
        {
            this._monitor = new SimpleMonitor();
            this.CopyFrom(list);
        }
        #endregion // Constructors

        #region Internal Methods
        protected IDisposable BlockReentrancy()
        {
            this._monitor.Enter();
            return this._monitor;
        }

        protected void CheckReentrancy()
        {
            if ((this._monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1))
            {
                throw new InvalidOperationException(SR.ObservableCollectionReentrancyNotAllowed);
            }
        }

        private void CopyFrom(IEnumerable<T> collection)
        {
            IList<T> items = base.Items;
            if ((collection != null) && (items != null))
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        items.Add(enumerator.Current);
                    }
                }
            }
        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            this.CheckReentrancy();
            T item = base[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        private void OnCollectionReset()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
        #endregion // Internal Methods

        #region Overrides / Event Handlers
        protected override void ClearItems()
        {
            this.CheckReentrancy();
            base.ClearItems();
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionReset();
        }

        protected override void InsertItem(int index, T item)
        {
            this.CheckReentrancy();
            base.InsertItem(index, item);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        protected override void RemoveItem(int index)
        {
            this.CheckReentrancy();
            T item = base[index];
            base.RemoveItem(index);
            this.OnPropertyChanged("Count");
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
        }

        protected override void SetItem(int index, T item)
        {
            this.CheckReentrancy();
            T oldItem = base[index];
            base.SetItem(index, item);
            this.OnPropertyChanged("Item[]");
            this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, oldItem, item, index);
        }
        #endregion // Overrides / Event Handlers

        #region Overridables / Event Triggers
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                using (this.BlockReentrancy())
                {
                    this.CollectionChanged(this, e);
                }
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }
        #endregion // Overridables / Event Triggers

        #region Public Methods
        public void Move(int oldIndex, int newIndex)
        {
            this.MoveItem(oldIndex, newIndex);
        }
        #endregion // Public Methods

        #region Internal Events
        #pragma warning disable 0067
        protected event PropertyChangedEventHandler PropertyChanged;
        #pragma warning restore 0067
        #endregion // Public Events

        #region Public Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion // Public Events

        #region INotifyPropertyChanged Implementation
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                this.PropertyChanged += value;
            }
            remove
            {
                this.PropertyChanged -= value;
            }
        }
        #endregion // INotifyPropertyChanged Implementation
    }
}
#endif // !WINDOWS_UWP