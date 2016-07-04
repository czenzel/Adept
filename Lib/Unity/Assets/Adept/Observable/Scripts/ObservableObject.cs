// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Adept
{
    /// <summary>
    /// The base class for an object that provides notification of property changes.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        #region Overridables / Event Triggers
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        static private string GetMemberName<T>(Expression<Func<T>> expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException("Value must be a lamda expression", "expression");
            }

            var mExpression = expression.Body as MemberExpression;
            if (mExpression == null)
            {
                throw new ArgumentException("The body of the expression must be a memberref", "expression");
            }

            return mExpression.Member.Name;
        }
        #endregion // Overridables / Event Triggers

        #region Internal Methods
        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool Set<T>(ref T storage, T value, String propertyName)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="property">An expression that represents the property used to notify listeners.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool Set<T>(ref T storage, T value, Expression<Func<T>> property)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(GetMemberName(property));
            return true;
        }
        #endregion // Internal Methods

        #region Public Events
        /// <summary>
        /// Raised when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion // Public Events
    }
}
