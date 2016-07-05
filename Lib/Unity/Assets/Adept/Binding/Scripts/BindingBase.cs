// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using Windows.UI.Xaml.Data;

namespace Adept.Unity
{
    /// <summary>
    /// Specifies the direction of data flow in a binding operation.
    /// </summary>
    public enum BindingDirection
    {
        /// <summary>
        /// Data flows from the source to the target.
        /// </summary>
        SourceToTarget,

        /// <summary>
        /// Data flows from the target to the source.
        /// </summary>
        TargetToSource,
    }

    /// <summary>
    /// Describes how the data propagates in a binding.
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// Updates the target property when the binding is created. Changes to the source 
        /// object can also propagate to the target.
        /// </summary>
        OneWay,

        /// <summary>
        /// Updates either the target or the source object when either changes. When the 
        /// binding is created, the target property is updated from the source.
        /// </summary>
        TwoWay
    }

    /// <summary>
    /// Represents a binding between a data item and another item.
    /// </summary>
    public class BindingBase : MonoBehaviour
    {
        #region Member Variables
        private DataContext dataContext;
        private bool isInitialized;
        private object source;
        private MemberInfo sourceMember;
        private string sourceMemberName;
        private object target;
        private MemberInfo targetMember;
        private string targetMemberName;
        private IValueConverter converter;
        private object converterParameter;
        #endregion // Member Variables

        #region Serialized Variables
        [SerializeField]
        [Tooltip("The language to pass to the converter.")]
        private string converterLanguage = string.Empty;

        [SerializeField]
        [Tooltip("A parameter that can be used in the converter logic.")]
        private string _converterParameter; // Inspector only. Stored as Object in converterParameter field during Initialize

        [SerializeField]
        [Tooltip("The converter that will be used to convert values during binding.")]
        private ConverterBehaviour _converter; // Inspector only. Stored as IValueConverter in converter field during Initialize

        [SerializeField]
        [Tooltip("The direction of the data flow in the binding.")]
        private BindingMode mode;
        #endregion // Serialized Variables

        #region Internal Methods
        private void LogBindingException(object value, Type targetType, Exception ex)
        {
            string valueInfo = (value == null ? "(null)" : string.Format("{0} ({1})", value, value.GetType().Name));
            string targetInfo = targetType.Name;
            string converterInfo = (converter == null ? "(none)" : converter.GetType().Name);

            Debug.LogError(string.Format("Binding failed. Value: {0}, Target Type: {1}, Converter: {2}, Error: {3}", valueInfo, targetInfo, converterInfo, ex.Message));
        }

        private void PropSourceChanging(object oldValue, object newValue, PropertyChangedEventHandler handler)
        {
            // If the old value supports property change notifications, unsubscribe
            INotifyPropertyChanged pOld = oldValue as INotifyPropertyChanged;
            if (pOld != null)
            {
                pOld.PropertyChanged -= handler;
            }

            // If the new value supports property change notifications, subscribe
            INotifyPropertyChanged pNew = newValue as INotifyPropertyChanged;
            if (pNew != null)
            {
                pNew.PropertyChanged += handler;
            }
        }

        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Is the property changing the member we're watching?
            if (e.PropertyName == SourceMemberName)
            {
                // Apply the binding
                ApplyBinding(BindingDirection.SourceToTarget);
            }
        }

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Is the property changing the member we're watching?
            if (e.PropertyName == TargetMemberName)
            {
                // Is it two-way?
                if (Mode == BindingMode.TwoWay)
                {
                    // Apply the binding
                    ApplyBinding(BindingDirection.TargetToSource);
                }
            }
        }

        /// <summary>
        /// Tests to see if a converter is needed to convert the value and adds one if necessary.
        /// </summary>
        /// <param name="value">
        /// The value being assigned.
        /// </param>
        /// <param name="targetType">
        /// The target type the value needs to be stored in.
        /// </param>
        private void TestAddConverter(object value, Type targetType)
        {
            // If a converter exists, use it. If no converter exists we can't test without an actual value.
            if ((converter == null) && (value != null))
            {
                // Is it directly assignable?
                if (!targetType.IsAssignableFrom(value.GetType()))
                {
                    converter = new ConvertableValueConverter();
                }
            }
        }

        private void TryGetMembers()
        {
            // Get source or target members if not already obtained
            if ((sourceMember == null) && (source != null) && (!string.IsNullOrEmpty(sourceMemberName)))
            {
                sourceMember = source.GetMemeber(sourceMemberName);
                if (sourceMember == null)
                {
                    Debug.LogWarningFormat("Source Member {0} could not be found on type {1}", sourceMemberName, source.GetType().Name);
                }
            }

            if ((targetMember == null) && (target != null) && (!string.IsNullOrEmpty(targetMemberName)))
            {
                targetMember = target.GetMemeber(targetMemberName);
                if (targetMember == null)
                {
                    Debug.LogWarningFormat("Target Member {0} could not be found on type {1}", targetMemberName, target.GetType().Name);
                }
            }
        }
        #endregion // Internal Methods

        #region Overridables / Event Triggers
        protected virtual void Awake()
        {
            // Migrate inspector values to class values
            if ((_converterParameter != null) && (converterParameter == null))
            {
                converterParameter = _converterParameter;
            }

            if ((_converter != null) && (converter == null))
            {
                converter = _converter;
            }

            // We are now initialized
            isInitialized = true;
        }

        protected virtual void OnEnable()
        {
            // Try to get DataContext
            dataContext = GetComponentInParent<DataContext>();

            // If no data context found, warn and bail
            if (dataContext == null)
            {
                Debug.LogWarningFormat("No DataContext could be found for {0}", GetType().Name);
                return;
            }

            // Register with DataContext
            dataContext.RegisterBinding(this);

            // Treat as a source change
            HandleSourceChanged(dataContext.Source);
        }

        protected virtual void OnDisable()
        {
            // If we have a DataContext, unregister
            if (dataContext != null)
            {
                dataContext.UnregisterBinding(this);
            }
        }
        #endregion // Overridables / Event Triggers

        #region Public Methods
        /// <summary>
        /// Applies the binding.
        /// </summary>
        /// <param name="direction">
        /// The direction of the data flow.
        /// </param>
        public virtual void ApplyBinding(BindingDirection direction)
        {
            // If not initialized, ignore
            if (!IsInitialized) { return; }

            // Get the value
            var value = GetValue(direction);

            // Set the value
            SetValue(direction, value);
        }

        /// <summary>
        /// Gets the current value of the binding.
        /// </summary>
        /// <param name="direction">
        /// The direction in which the data is flowing.
        /// </param>
        /// <returns>
        /// The current value.
        /// </returns>
        public virtual object GetValue(BindingDirection direction)
        {
            // Try to get members if not already obtained
            TryGetMembers();
            
            // Placeholder
            object value = null;

            // Which direction?
            switch (direction)
            {
                case BindingDirection.SourceToTarget:
                    // If member found, use member. Otherwise use whole object.
                    if (sourceMember != null)
                    {
                        value = sourceMember.GetValue(source);
                    }
                    else
                    {
                        value = source;
                    }
                    break;

                case BindingDirection.TargetToSource:
                    // If member found, use member. Otherwise use whole object.
                    if (targetMember != null)
                    {
                        value = targetMember.GetValue(target);
                    }
                    else
                    {
                        value = target;
                    }
                    break;
            }

            // Done
            return value;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void HandleSourceChanged(object newSource)
        {
            PropSourceChanging(source, newSource, SourcePropertyChanged);
            source = newSource;
            ApplyBinding(BindingDirection.SourceToTarget);
        }

        public virtual void SetValue(BindingDirection direction, object value)
        {
            // Try to get members if not already obtained
            TryGetMembers();

            // Which member and object
            MemberInfo setMember = null;
            object setObject = null;
            switch (direction)
            {
                case BindingDirection.SourceToTarget:
                    setMember = targetMember;
                    setObject = target;
                    break;

                case BindingDirection.TargetToSource:
                    setMember = sourceMember;
                    setObject = source;
                    break;
            }

            // Is member available
            if (setMember != null)
            {
                // What is the target type?
                var targetType = setMember.GetValueType();

                // Check to see if we need to add a converter
                TestAddConverter(value, targetType);

                // If we have a converter, convert the value
                if (converter != null)
                {
                    try
                    {
                        switch (direction)
                        {
                            case BindingDirection.SourceToTarget:
                                value = converter.Convert(value, targetType, converterParameter, converterLanguage);
                                break;
                            case BindingDirection.TargetToSource:
                                value = converter.ConvertBack(value, targetType, converterParameter, converterLanguage);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogBindingException(value, targetType, ex);
                        
                        // No need to continue
                        return;
                    }
                }

                // Get the current value
                var currentValue = setMember.GetValue(setObject);

                // Only update if actually changing
                if (!object.Equals(currentValue, value))
                {
                    try
                    {
                        setMember.SetValue(setObject, value);
                    }
                    catch (Exception ex)
                    {
                        LogBindingException(value, targetType, ex);
                    }
                }
            }
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Get or sets the converter used to convert values during binding.
        /// </summary>
        public IValueConverter Converter
        {
            get
            {
                return converter;
            }
            set
            {
                if (converter != value)
                {
                    // Update
                    converter = value;

                    // Apply the binding
                    ApplyBinding(BindingDirection.SourceToTarget);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that names the language to pass to any converter specified 
        /// by the Converter property.
        /// </summary>
        /// <remarks>
        /// If a value for ConverterLanguage is specified, this value is used for the language 
        /// value when invoking the converter logic. Specifically, this provides the value of 
        /// the language parameter of the Convert or ConvertBack methods of the specific 
        /// converter that is requested with the Converter property. By default and in the 
        /// absence of ConverterLanguage being set, the value passed for language is an 
        /// empty string.
        /// </remarks>
        public string ConverterLanguage
        {
            get
            {
                return converterLanguage;
            }
            set
            {
                if (converterLanguage != value)
                {
                    converterLanguage = value;
                    ApplyBinding(BindingDirection.SourceToTarget);
                }
            }
        }

        /// <summary>
        /// Gets or sets a parameter that can be used in the converter logic.
        /// </summary>
        public object ConverterParameter
        {
            get
            {
                return converterParameter;
            }
            set
            {
                if (converterParameter != value)
                {
                    converterParameter = value;
                    ApplyBinding(BindingDirection.SourceToTarget);
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates if the binding has been initialized.
        /// </summary>
        public bool IsInitialized { get { return isInitialized; } }

        /// <summary>
        /// Gets or sets a value that indicates the direction of the data flow in the binding.
        /// </summary>
        public BindingMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the source member (property or field) that will participate in the binding.
        /// </summary>
        public string SourceMemberName
        {
            get
            {
                return sourceMemberName;
            }
            set
            {
                // Ensure changing
                if (sourceMemberName != value)
                {
                    // Store
                    sourceMemberName = value;

                    // Clear current source member
                    sourceMember = null;

                    // Apply the binding (which will attempt to recalculate source)
                    ApplyBinding(BindingDirection.SourceToTarget);
                }
            }
        }

        /// <summary>
        /// Gets or sets the item that is the target of the binding.
        /// </summary>
        public object Target
        {
            get
            {
                return target;
            }
            set
            {
                if (value != target)
                {
                    PropSourceChanging(source, value, TargetPropertyChanged);
                    target = value;
                    ApplyBinding(BindingDirection.SourceToTarget);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the target member (property or field) that will participate in the binding.
        /// </summary>
        public string TargetMemberName
        {
            get
            {
                return targetMemberName;
            }
            set
            {
                // Ensure changing
                if (targetMemberName != value)
                {
                    // Store
                    targetMemberName = value;

                    // Clear current target member
                    targetMember = null;

                    // Apply the binding (which will attempt to recalculate target)
                    ApplyBinding(BindingDirection.SourceToTarget);
                }
            }
        }
        #endregion // Public Properties
    }
}
