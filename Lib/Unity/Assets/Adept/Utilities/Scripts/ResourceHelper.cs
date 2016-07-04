// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;

namespace Adept.Unity
{
    static public class ResourceHelper
    {
        static private Font defaultFont;

        static public Font GetDefaultFont()
        {
            // Get default
            if (defaultFont == null)
            {
                defaultFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            }

            //// Return clone
            //return (Font)UnityEngine.Object.Instantiate(defaultFont);
            return defaultFont;
        }
    }
}
