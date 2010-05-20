// <copyright file="ZeroToEmptyStringConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2010 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Takes an integer and converts it to an empty string if the value is zero. Otherwise it returns the value.
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class ZeroToEmptyStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>An empty string if the value is zero, otherwise the original value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                if ((int)value == 0)
                {
                    return String.Empty;
                }
            }

            return value;
        }

        /// <summary>
        /// Converts a target value to a source value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Not implemented.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
