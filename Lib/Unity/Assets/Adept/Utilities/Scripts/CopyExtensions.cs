// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine.UI;

namespace Adept.Unity
{
    static public class CopyExtensions
    {
        static public void CopyTo(this LayoutElement source, LayoutElement target)
        {
            if ((source == null) || (target == null)) { return; }

            target.enabled = source.enabled;
            target.flexibleHeight = source.flexibleHeight;
            target.flexibleWidth = source.flexibleWidth;
            target.ignoreLayout = source.ignoreLayout;
            target.minHeight = source.minHeight;
            target.minWidth = source.minWidth;
            target.preferredHeight = source.preferredHeight;
            target.preferredWidth = source.preferredWidth;
        }
    }
}
