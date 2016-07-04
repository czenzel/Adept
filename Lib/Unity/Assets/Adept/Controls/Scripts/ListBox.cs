// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace Adept.Unity
{
    /// <summary>
    /// Contains a list of selectable items.
    /// </summary>
    [AddComponentMenu("UI/List Box", 40)]
    public class ListBox : Selector
    {
        // Use this for initialization
        private void Start()
        {
            base.Initialize();
            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
        }
    }
}