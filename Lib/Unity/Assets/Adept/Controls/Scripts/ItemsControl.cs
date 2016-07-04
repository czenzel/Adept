// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

namespace Adept.Unity
{
    /// <summary>
    /// Base class for controls that contain data items.
    /// </summary>
    public class ItemsControl : MonoBehaviour
    {
        #region Constants
        /// <summary>
        /// The name of the child GameObject that represents the instantiated item container.
        /// </summary>
        public const string ItemContainerName = "ItemContainer";

        /// <summary>
        /// The name of the child GameObject that represents the instantiated item template.
        /// </summary>
        public const string ItemTemplateName = "ItemTemplate";
        #endregion // Constants

        #region Member Variables
        private object itemsSource;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="ItemsControl"/>.
        /// </summary>
        public ItemsControl()
        {
            
        }
        #endregion // Constructors

        #region Internal Methods
        internal void ClearContainerForItem(GameObject container, object item)
        {
            // Create the template 
            var template = container.transform.Find(ItemTemplateName);

            // Next steps only done if template found
            if (template != null)
            {
                // Remove the template from the container
                template.transform.SetParent(null, false);

                // Find and unwire data context
                var context = template.GetComponent<DataContext>();
                if (context != null)
                {
                    context.Source = null;
                }
            }

            // Call override
            ClearContainerForItemOverride(container, item);

            // Destroy template
            if (template != null)
            {
                Destroy(template);
            }
        }

        private void CreateDefaultTemplates()
        {
            try
            {
                // Item Container
                if (itemContainerTemplate == null)
                {
                    itemContainerTemplate = new GameObject();
                    itemContainerTemplate.AddComponent<RectTransform>();
                    itemContainerTemplate.AddComponent<CanvasRenderer>();
                    
                    // Image
                    var img = itemContainerTemplate.AddComponent<Image>();
                    img.color = Color.gray;
                }

                // Item Template
                if (itemTemplate == null)
                {
                    itemTemplate = new GameObject();
                    itemTemplate.AddComponent<RectTransform>();
                    itemTemplate.AddComponent<CanvasRenderer>();
                    
                    // Text
                    var text = itemTemplate.AddComponent<Text>();
                    text.font = ResourceHelper.GetDefaultFont();
                    text.alignment = TextAnchor.MiddleLeft;
                    text.horizontalOverflow = HorizontalWrapMode.Overflow;

                    // Add a Data context
                    itemTemplate.AddComponent<DataContext>();

                    // Binding
                    var binding = itemTemplate.AddComponent<DefaultTextBinding>();
                    binding.text = text;
                }
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Error creating default templates. \"{0}\"", ex.Message);
            }
        }
        internal GameObject CreateItemContainer()
        {
            // Create
            var template = (GameObject)Instantiate(itemContainerTemplate);

            // Make sure the name is set
            template.name = ItemContainerName;

            // Return
            return template;
        }

        internal GameObject CreateItemTemplate()
        {
            // Create
            var template = (GameObject)Instantiate(itemTemplate);

            // Make sure the name is set
            template.name = ItemTemplateName;

            // Return
            return template;
        }

        internal void PrepareContainerForItem(GameObject container, object item)
        {
            // Create the template 
            var template = CreateItemTemplate();

            // Prepare the template
            PrepareTemplateForItem(template, item);

            // Layout
            var templateLayout = template.GetComponent<LayoutElement>();
            var containerLayout = container.AddComponent<LayoutElement>();
            if (templateLayout != null)
            {
                templateLayout.CopyTo(containerLayout);
            }
            else
            {
                containerLayout.minHeight = 30f;
            }

            // Add the template to the container
            template.transform.SetParent(container.transform, false);

            // Call override
            PrepareContainerForItemOverride(container, item);
        }

        internal void PrepareTemplateForItem(GameObject template, object item)
        {
            // Find and wire up data context
            var dataContext = template.GetComponent<DataContext>();
            if (dataContext == null)
            {
                dataContext = template.AddComponent<DataContext>();
            }
            dataContext.Source = item;
        }
        #endregion // Internal Methods

        #region Overridables / Event Triggers
        /// <summary>
        /// Undoes the effects of the <see cref="PrepareContainerForItemOverride"/> method.
        /// </summary>
        /// <param name="container">
        /// The container that will display the item template.
        /// </param>
        /// <param name="item">
        /// The item to represent.
        /// </param>
        /// <remarks>
        /// A class that inherits from <see cref="ItemsControl"/> can override this method to 
        /// undo one or more of the actions that it takes in the <see cref="PrepareContainerForItemOverride"/> 
        /// method. If you override ClearContainerForItemOverride, always call the base implementation 
        /// in your ClearContainerForItemOverride implementation.
        /// </remarks>
        protected virtual void ClearContainerForItemOverride(GameObject container, object item)
        {
        }

        /// <summary>
        /// Occurs when the contents of <see cref="ItemsSource"/> have changed, if the source implements <see cref="INotifyCollectionChanged"/>.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// A <see cref="NotifyCollectionChangedEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemContainerGenerator != null)
            {
                ItemContainerGenerator.HandleChange(e);
            }
        }

        /// <summary>
        /// Prepares the container to display the specified item.
        /// </summary>
        /// <param name="container">
        /// The container that will display the item template.
        /// </param>
        /// <param name="item">
        /// The item to represent.
        /// </param>
        /// <remarks>
        /// A class that inherits from <see cref="ItemsControl"/> can override this method 
        /// to apply styles, set bindings, and so on. If you override PrepareContainerForItemOverride, 
        /// always call the base implementation in your PrepareContainerForItemOverride implementation.
        /// </remarks>
        protected virtual void PrepareContainerForItemOverride(GameObject container, object item)
        {
        }

        protected virtual void Initialize()
        {
            this.ItemContainerGenerator = new ItemContainerGenerator(this);
            CreateDefaultTemplates();
            if (itemsSource != null)
            {
                ItemContainerGenerator.HandleChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        #endregion // Overridables / Event Triggers

        #region Public Properties
        /// <summary>
        /// Gets the <see cref="ItemContainerGenerator"/> that is associated with the control. 
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator { get; private set; }

        /// <summary>
        /// Gets or sets the collection of items in the list.
        /// </summary>
        public object ItemsSource
        {
            get
            {
                return itemsSource;
            }
            set
            {
                // Only continue if changing
                if (value != itemsSource)
                {
                    // If existing is observable, unsubscribe
                    INotifyCollectionChanged nc = itemsSource as INotifyCollectionChanged;
                    if (nc != null)
                    {
                        nc.CollectionChanged -= OnItemsChanged;
                    }

                    // Update
                    itemsSource = value;

                    // If new is observable, subscribe
                    nc = itemsSource as INotifyCollectionChanged;
                    if (nc != null)
                    {
                        nc.CollectionChanged += OnItemsChanged;
                    }

                    // Consider reset
                    OnItemsChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }
        #endregion // Public Properties

        #region Inspector Properties
        /// <summary>
        /// Gets or sets the GameObject that represents the template for each items container.
        /// </summary>
        [Tooltip("The GameObject that represents the template for each items container.")]
        public GameObject itemContainerTemplate;

        /// <summary>
        /// Gets or sets the Transform where child UI items should be added.
        /// </summary>
        [Tooltip("The Transform where child UI items should be added.")]
        public Transform itemsPanel;

        /// <summary>
        /// Gets or sets the GameObject that represents the template for each item.
        /// </summary>
        [Tooltip("The GameObject that represents the template for each item.")]
        public GameObject itemTemplate;
        #endregion // Inspector Properties
    }
}