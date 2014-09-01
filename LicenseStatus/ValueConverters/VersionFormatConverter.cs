// <copyright file="VersionFormatConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Takes a Version and converts it to its major.minor format.
    /// </summary>
    [ValueConversion(typeof(Version), typeof(string))]
    public class VersionFormatConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The version number in major.minor format.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((Version)value).ToString(2);
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
