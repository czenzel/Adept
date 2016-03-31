// Copyright (c) Microsoft. All rights reserved.
//
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Adept.UnityXaml
{
    /// <summary>
    /// Extension methods for working with Unity classes.
    /// </summary>
    static public class UnityExtensions
    {
        /// <summary>
        /// Converts a <see cref="UnityEngine.AsyncOperation"/> to a <see cref="Task"/>.
        /// </summary>
        /// <param name="operation">
        /// The Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> to convert to a <see cref="Task"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the Task.
        /// </param>
        /// <param name="progress">
        /// A handler that can receive progress reports on the Unity operation.
        /// </param>
        /// <param name="millisecondsYield">
        /// The number of milliseconds yielded to the context between each update of the operation. The default is 50ms.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the Unity asynchronous operation.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <b>IMPORTANT:</b> The underlying Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> 
        /// cannot actually be canceled within Unity. Therefore if the <paramref name="cancellationToken"/> is set, 
        /// the <see cref="Task"/> returned by this operation will end with a <see cref="TaskCanceledException"/>.
        /// </para>
        /// <para>
        /// Currently the only way to wait for a Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> 
        /// to complete is to monitor the value returned by the 
        /// <see cref="UnityEngine.AsyncOperation.isDone">isDone</see> property in a loop. To make this loop 
        /// efficient, by default the loop will yield to the calling context for 50 ms on every iteration. The 
        /// amount of time that is yielded on each iteration can be adjusted by changing the 
        /// <paramref name="millisecondsYield"/> parameter.
        /// </para>
        /// </remarks>
        static public async Task AsTask(this UnityEngine.AsyncOperation operation, CancellationToken cancellationToken, IProgress<float> progress, int millisecondsYield = 50)
        {
            // Validate
            if (millisecondsYield < 0) throw new ArgumentOutOfRangeException(nameof(millisecondsYield));
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            
            // Can't wait less than 1 ms
            if (millisecondsYield < 1) { millisecondsYield = 1; }

            // Progress holders
            float currentProgress = -1;
            float lastProgress = -1;

            // Loop until done
            while (!operation.isDone)
            {
                // Check for cancellation
                if (cancellationToken != null)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                // Is there a progress handler, see if we need to update it
                if (progress != null)
                {
                    // There is. Get current progress.
                    currentProgress = operation.progress;

                    // Did it change since last time through the loop?
                    if (currentProgress != lastProgress)
                    {
                        // Yes, notify
                        progress.Report(currentProgress);

                        // And record for next time through the loop
                        lastProgress = currentProgress;
                    }
                }

                // Delay to yield time back to the context
                await Task.Delay(millisecondsYield);
            }
        }

        /// <summary>
        /// Converts a <see cref="UnityEngine.AsyncOperation"/> to a <see cref="Task"/>.
        /// </summary>
        /// <param name="operation">
        /// The Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> to convert to a <see cref="Task"/>.
        /// </param>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> that can be used to cancel the Task.
        /// </param>
        /// <param name="millisecondsYield">
        /// The number of milliseconds yielded to the context between each update of the operation. The default is 50ms.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the Unity asynchronous operation.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <b>IMPORTANT:</b> The underlying Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> 
        /// cannot actually be canceled within Unity. Therefore if the <paramref name="cancellationToken"/> is set, 
        /// the <see cref="Task"/> returned by this operation will end with a <see cref="TaskCanceledException"/>.
        /// </para>
        /// <para>
        /// Currently the only way to wait for a Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> 
        /// to complete is to monitor the value returned by the 
        /// <see cref="UnityEngine.AsyncOperation.isDone">isDone</see> property in a loop. To make this loop 
        /// efficient, by default the loop will yield to the calling context for 50 ms on every iteration. The 
        /// amount of time that is yielded on each iteration can be adjusted by changing the 
        /// <paramref name="millisecondsYield"/> parameter.
        /// </para>
        /// </remarks>
        static public Task AsTask(this UnityEngine.AsyncOperation operation, CancellationToken cancellationToken, int millisecondsYield = 50)
        {
            return UnityExtensions.AsTask(operation: operation, cancellationToken: cancellationToken, progress: null, millisecondsYield: millisecondsYield);
        }

        /// <summary>
        /// Converts a <see cref="UnityEngine.AsyncOperation"/> to a <see cref="Task"/>.
        /// </summary>
        /// <param name="operation">
        /// The Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> to convert to a <see cref="Task"/>.
        /// </param>
        /// <param name="progress">
        /// A handler that can receive progress reports on the Unity operation.
        /// </param>
        /// <param name="millisecondsYield">
        /// The number of milliseconds yielded to the context between each update of the operation. The default is 50ms.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the Unity asynchronous operation.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <b>IMPORTANT:</b> The underlying Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> 
        /// cannot actually be canceled within Unity. Therefore if the <paramref name="cancellationToken"/> is set, 
        /// the <see cref="Task"/> returned by this operation will end with a <see cref="TaskCanceledException"/>.
        /// </para>
        /// <para>
        /// Currently the only way to wait for a Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> 
        /// to complete is to monitor the value returned by the 
        /// <see cref="UnityEngine.AsyncOperation.isDone">isDone</see> property in a loop. To make this loop 
        /// efficient, by default the loop will yield to the calling context for 50 ms on every iteration. The 
        /// amount of time that is yielded on each iteration can be adjusted by changing the 
        /// <paramref name="millisecondsYield"/> parameter.
        /// </para>
        /// </remarks>
        static public Task AsTask(this UnityEngine.AsyncOperation operation, IProgress<float> progress, int millisecondsYield = 50)
        {
            return UnityExtensions.AsTask(operation: operation, cancellationToken: CancellationToken.None, progress: progress, millisecondsYield: millisecondsYield);
        }

        /// <summary>
        /// Converts a <see cref="UnityEngine.AsyncOperation"/> to a <see cref="Task"/>.
        /// </summary>
        /// <param name="operation">
        /// The Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> to convert to a <see cref="Task"/>.
        /// </param>
        /// <param name="millisecondsYield">
        /// The number of milliseconds yielded to the context between each update of the operation. The default is 50ms.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that represents the Unity asynchronous operation.
        /// </returns>
        /// <remarks>
        /// <para>
        /// <b>IMPORTANT:</b> The underlying Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> 
        /// cannot actually be canceled within Unity. Therefore if the <paramref name="cancellationToken"/> is set, 
        /// the <see cref="Task"/> returned by this operation will end with a <see cref="TaskCanceledException"/>.
        /// </para>
        /// <para>
        /// Currently the only way to wait for a Unity <see cref="UnityEngine.AsyncOperation">AsyncOperation</see> 
        /// to complete is to monitor the value returned by the 
        /// <see cref="UnityEngine.AsyncOperation.isDone">isDone</see> property in a loop. To make this loop 
        /// efficient, by default the loop will yield to the calling context for 50 ms on every iteration. The 
        /// amount of time that is yielded on each iteration can be adjusted by changing the 
        /// <paramref name="millisecondsYield"/> parameter.
        /// </para>
        /// </remarks>
        static public Task AsTask(this UnityEngine.AsyncOperation operation, int millisecondsYield = 50)
        {
            return UnityExtensions.AsTask(operation: operation, cancellationToken: CancellationToken.None, progress: null, millisecondsYield: millisecondsYield);
        }
    }
}
