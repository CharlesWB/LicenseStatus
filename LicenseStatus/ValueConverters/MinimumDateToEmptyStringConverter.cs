// <copyright file="MinimumDateToEmptyStringConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2010 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Takes a DateTime and converts it to an empty string if the DateTime is MinValue. Otherwise it returns the value.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class MinimumDateToEmptyStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>An empty string if the DateTime is MinValue, otherwise the original value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime)
            {
                if ((DateTime)value == DateTime.MinValue)
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
