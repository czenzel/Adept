// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using System;
using System.Globalization;
using System.Windows.Data;

namespace Adept.Unity
{
    /// <summary>
    /// A value converter that attempts to convert values using the <see cref="IConvertible"/> interface.
    /// </summary>
    public class ConvertableValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, targetType, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(value, targetType, culture);
        }
    }
}
