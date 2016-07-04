using System;
using System.Windows.Data;
using UnityEngine;

namespace Adept
{
    public abstract class ValueConverter : MonoBehaviour, IValueConverter
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
        public abstract object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);

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
        public abstract object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);
    }
}
