// <copyright file="NullableValueConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2012 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Takes a target object and returns a null value if it is an empty string. No conversion is
    /// applied to the source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is designed to handle nullable types. Without the converter when the value is erased in
    /// the view an empty string is sent to the source. This converter makes it so a null is sent to
    /// the source.
    /// </para>
    /// <para>
    /// This is from http://blog.jeffhandley.com/archive/2008/07/09/binding-to-nullable-values-in-xaml.aspx.
    /// </para>
    /// <para>
    /// A ValueConversion attribute is not defined since all types are supported.
    /// </para>
    /// </remarks>
    public class NullableValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The original value unchanged.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// Converts a target value to a source value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The value unchanged unless its string representation is null or empty, then null is returned.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            return value;
        }
    }
}
