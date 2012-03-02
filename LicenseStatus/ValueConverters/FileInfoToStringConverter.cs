// <copyright file="FileInfoToStringConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2012 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.IO;
    using System.Windows.Data;

    /// <summary>
    /// Translates between a FileInfo object and a string.
    /// </summary>
    [ValueConversion(typeof(FileInfo), typeof(string))]
    public class FileInfoToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The string value of the FileInfo.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = null;

            FileInfo file = value as FileInfo;

            if (file != null)
            {
                result = file.ToString();
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
        /// <returns>The FileInfo object if the string value is not empty or null, otherwise a null value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FileInfo result = null;

            string text = value as string;

            if (text != null)
            {
                if (!String.IsNullOrEmpty(text) && text.Trim().Length != 0)
                {
                    result = new FileInfo(text);
                }
            }

            return result;
        }
    }
}
