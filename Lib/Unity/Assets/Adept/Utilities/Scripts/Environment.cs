// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if WINDOWS_UWP
using Windows.System.Profile;
#endif

namespace Adept
{
    /// <summary>
    /// Provides information about the environment in which the application is running.
    /// </summary>
    static public class Environment
    {
        /// <summary>
        /// Gets a value that indicates if the application is running on a holographic 
        /// computing device such as the HoloLens.
        /// </summary>
        static public bool IsHolographic
        {
            get
            {
                #if WINDOWS_UWP
                return AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Holographic";
                #else
                return false;
                #endif
            }
        }
    }
}