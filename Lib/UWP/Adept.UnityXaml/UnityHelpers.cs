// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Adept.UnityXaml
{
    /// <summary>
    /// Helper methods for working with Unity.
    /// </summary>
    static public class UnityHelpers
    {
        #region Member Variables
        static private UnityBridge bridge;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes <see cref="UnityHelpers"/>
        /// </summary>
        static UnityHelpers()
        {
            bridge = UnityBridge.Instance;
        }
        #endregion // Constructors

        #region Public Methods
        /// <summary>
        /// Loads the scene with the specified name.
        /// </summary>
        /// <param name="sceneName">
        /// The name of the scene to load.
        /// </param>
        /// <param name="progress">
        /// An optional progress handler to receive notifications during the load.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        static public Task LoadSceneAsync(string sceneName, IProgress<float> progress = null)
        {
            // Validate
            if (string.IsNullOrEmpty(sceneName)) throw new ArgumentException(nameof(sceneName));
            ValidateInitialized();

            // HACK: SceneManager.LoadSceneAsync never completes. Instead it gets to 90%. This is a known issue.
            // http://answers.unity3d.com/questions/1073557/additively-loaded-scene-gets-stuck-at-90-even-if-a.html#answer-1073667

            // Execute
            return bridge.InvokeAsync(() =>SceneManager.LoadSceneAsync(sceneName), progressCompleteOverride: 0.9f);
        }

        /// <summary>
        /// Validates that the <see cref="UnityBridge"/> has been initialized.
        /// </summary>
        static public void ValidateInitialized()
        {
            if (!bridge.IsInitialized)
            {
                throw new InvalidOperationException("This operation is not valid until Unity has completed initialization. Subscribe to the UnityBridge.Initialized event or await on UnityBridge.InitializedTask before calling this function.");
            }
        }
        #endregion // Public Methods
    }
}
