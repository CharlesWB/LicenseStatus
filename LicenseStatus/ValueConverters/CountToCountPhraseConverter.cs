// <copyright file="CountToCountPhraseConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2010 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Takes a count value and converts it to a phrase with the value and the description with correct plurality.
    /// </summary>
    /// <remarks>
    /// ConverterParameter is used to specify which word to use so that only one converter is needed. If the
    /// parameter is unknown then the parameter itself will be used as the word.
    /// </remarks>
    [ValueConversion(typeof(int), typeof(string), ParameterType = typeof(string))]
    public class CountToCountPhraseConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The integer followed by the descriptive phrase.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = String.Empty;

            if (value is int)
            {
                int count = (int)value;
                string parameterText = parameter as string;
                string description;

                switch (parameterText)
                {
                    case "License":
                        description = count == 1 ? "license" : "licenses";
                        break;
                    case "Feature":
                        description = count == 1 ? "feature" : "features";
                        break;
                    case "User":
                        description = count == 1 ? "user" : "users";
                        break;
                    case "Item":
                        description = count == 1 ? "item" : "items";
                        break;
                    default:
                        description = parameterText;
                        break;
                }

                result = String.Format(CultureInfo.InvariantCulture, "{0} {1}", count, description);
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
        /// <returns>Not implemented.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
