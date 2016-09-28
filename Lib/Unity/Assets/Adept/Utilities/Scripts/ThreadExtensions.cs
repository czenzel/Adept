// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;

namespace Adept
{
    static public class ThreadExtensions
    {
        /// <summary>
        /// Ensures that the action is performed on the Unity Application thread.
        /// </summary>
        /// <param name="action">
        /// The action to perform.
        /// </param>
        /// <remarks>
        /// If the current thread is already the App thread the action will be executed 
        /// inline. Otherwise, the action will be scheduled using InvokeOnAppThread.
        /// </remarks>
        static public void RunOnAppThread(Action action)
        {
            if (UnityEngine.WSA.Application.RunningOnAppThread())
            {
                action();
            }
            else
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(()=>action(), false);
            }
        }
    }
}
