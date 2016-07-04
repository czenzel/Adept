// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#if !WINDOWS_UWP
using System.Globalization;

namespace System.Windows.Data
{
    /// <summary>
    /// Exposes methods that allow modifying the data as it passes through the binding engine.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">
        /// The source data being passed to the target.
        /// </param>
        /// <param name="targetType">
        /// The <see cref="Type"/> of data expected by the target property.
        /// </param>
        /// <param name="parameter">
        /// An optional parameter to be used in the converter logic.
        /// </param>
        /// <param name="culture">
        /// The culture of the conversion.
        /// </param>
        /// <returns>
        /// The value to be passed to the target property.
        /// </returns>
        Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">
        /// The value that is produced by the binding target.
        /// </param>
        /// <param name="targetType">
        /// The type to convert to.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
        /// <returns>
        /// A converted value.
        /// </returns>
        Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture);
    }
}
#endif // !WINDOWS_UWP