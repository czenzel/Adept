// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using Windows.UI.Xaml.Data;

namespace Adept.Unity
{
    /// <summary>
    /// A value converter that attempts to convert values using the <see cref="IConvertible"/> interface.
    /// </summary>
    public class ConvertableValueConverter : IValueConverter
    {
        static private IFormatProvider TryGetFormatProvider(string language)
        {
            // Placeholder
            IFormatProvider provider = null;

            // Must have a language
            if (!string.IsNullOrEmpty(language))
            {
                // Try to load a culture
                try
                {
                    provider = new System.Globalization.CultureInfo(language);
                }
                catch (Exception) { }
            }

            // Done
            return provider;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var provider = TryGetFormatProvider(language);
            if (provider != null)
            {
                return System.Convert.ChangeType(value, targetType, provider);
            }
            else
            {
                return System.Convert.ChangeType(value, targetType);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var provider = TryGetFormatProvider(language);
            if (provider != null)
            {
                return System.Convert.ChangeType(value, targetType, provider);
            }
            else
            {
                return System.Convert.ChangeType(value, targetType);
            }
        }
    }
}
