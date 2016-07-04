// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using UnityEngine.UI;

namespace Adept.Unity
{
    public class DefaultTextBinding : BindingBase
    {
        #region Inspector Items
        [Tooltip("The text control that will represent the item.")]
        public Text text;
        #endregion // Inspector Items

        private void Start()
        {
            Converter = new DefaultTextConverter();
            TargetMemberName = "text";
            Target = text;
        }
    }
}
