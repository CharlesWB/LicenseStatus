// <copyright file="TimeToTimeDayConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2009 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Takes a DateTime and formats it to the time value. If the date is not today then the day will be appended.
    /// If the date is yesterday or tomorrow then the descriptive date is appended.
    /// </summary>
    /// <remarks>
    /// <para>The time is shown with ShortTime since lmstat does not report seconds in its time.</para>
    /// <para>Since most times will be applicable to today the date is not shown unless it is a different day.</para>
    /// </remarks>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class TimeToTimeDayConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The time and a descriptive day if not today.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = String.Empty;

            if (value is DateTime)
            {
                DateTime time;
                if (DateTime.TryParse(value.ToString(), out time))
                {
                    if (time != DateTime.MinValue)
                    {
                        if (time.Date == DateTime.Today.Date)
                        {
                            result = time.ToShortTimeString();
                        }
                        else if (time.Date == DateTime.Today.Date.AddDays(1))
                        {
                            result = String.Format(CultureInfo.InvariantCulture, "{0} Tomorrow", time.ToShortTimeString());
                        }
                        else if (time.Date == DateTime.Today.Date.AddDays(-1))
                        {
                            result = String.Format(CultureInfo.InvariantCulture, "{0} Yesterday", time.ToShortTimeString());
                        }
                        else
                        {
                            result = String.Format(CultureInfo.InvariantCulture, "{0} on {1}", time.ToShortTimeString(), time.Date.ToShortDateString());
                        }
                    }
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
