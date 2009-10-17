// <copyright file="WrapPanelToMinMaxHeightConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2009 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Takes a WrapPanel and converts it to a double that represents the desired height of a single child
    /// or of all children.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is used by the License properties WrapPanel to set the MinHeight and MaxHeight of the Grid. This
    /// makes it so the GridSplitter will not oversize or undersize the contents.
    /// </para>
    /// <para>
    /// This assumes that the all children have the same desired height.
    /// </para>
    /// <para>
    /// The ConverterParameter is MinHeight or MaxHeight. This determines which value is returned.
    /// </para>
    /// </remarks>
    [ValueConversion(typeof(WrapPanel), typeof(double), ParameterType = typeof(string))]
    public class WrapPanelToMinMaxHeightConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The requested height.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result = 0;

            WrapPanel panel = value as WrapPanel;

            if (panel != null)
            {
                if (panel.Children.Count != 0)
                {
                    if (parameter.ToString() == "MinHeight")
                    {
                        result = panel.Children[0].DesiredSize.Height;
                    }
                    else
                    {
                        result = panel.Children.Count * panel.Children[0].DesiredSize.Height;
                    }

                    // This is specific to having a parent of the WrapPanel define the margin instead of the WrapPanel
                    // defining the margin.
                    FrameworkElement parent = panel.Parent as FrameworkElement;
                    result = result + parent.Margin.Top + parent.Margin.Bottom;
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
        /// <returns>Not implemented.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
