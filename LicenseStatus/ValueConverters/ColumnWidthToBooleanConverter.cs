// <copyright file="ColumnWidthToBooleanConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Translates a column width (double) to a boolean.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="GridViewColumnMenu"/>.
    /// </remarks>
    [ValueConversion(typeof(double), typeof(bool))]
    public class ColumnWidthToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Returns true if the value is non-zero, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = true;

            if (value is double)
            {
                if ((double)value == 0)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts a target value to a source value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Returns double.NaN if value is true, otherwise zero. If the value is not a boolean then Binding.DoNothing is returned.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? double.NaN : 0;
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}
