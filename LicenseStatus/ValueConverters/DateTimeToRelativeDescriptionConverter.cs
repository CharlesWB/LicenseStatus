// <copyright file="DateTimeToRelativeDescriptionConverter.cs" company="Charles W. Bozarth">
// Copyright (C) 2012 Charles W. Bozarth
// Refer to MainWindow.xaml.cs for the full copyright notice.
// </copyright>

namespace LicenseStatus
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Takes a DateTime value and converts it to a string that describes the value relative to today.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToRelativeDescriptionConverter : IValueConverter
    {
        /// <summary>
        /// Converts a source value to a target value.
        /// </summary>
        /// <param name="value">The value produced by the binding source. </param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The description of the DateTime relative to today.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = "None";

            if (value is DateTime)
            {
                DateTime time;
                if (DateTime.TryParse(value.ToString(), out time))
                {
                    if (time != DateTime.MinValue)
                    {
                        DateTime currentDate = DateTime.Today;

                        //// Used for testing relative dates using the test status file.
                        //// currentDate = new DateTime(2009, 03, 17, 11, 15, 0);

                        if (time < currentDate)
                        {
                            result = "A long time ago";
                        }

                        if (time > currentDate)
                        {
                            result = "Sometime in the future";
                        }

                        if (time.Year == currentDate.Year && time.Date > currentDate)
                        {
                            result = "Later this year";
                        }

                        if (time.Year == currentDate.Year && time.Date < currentDate)
                        {
                            result = "Earlier this year";
                        }

                        if (DateTimeToRelativeDescriptionConverter.BeginningOfMonth(time) == DateTimeToRelativeDescriptionConverter.BeginningOfMonth(currentDate) && time.Date > currentDate)
                        {
                            result = "Later this month";
                        }

                        if (DateTimeToRelativeDescriptionConverter.BeginningOfMonth(time) == DateTimeToRelativeDescriptionConverter.BeginningOfMonth(currentDate) && time.Date < currentDate)
                        {
                            result = "Earlier this month";
                        }

                        if (DateTimeToRelativeDescriptionConverter.BeginningOfWeek(time) == DateTimeToRelativeDescriptionConverter.BeginningOfWeek(currentDate).AddDays(7))
                        {
                            result = "Next week";
                        }

                        if (DateTimeToRelativeDescriptionConverter.BeginningOfWeek(time) == DateTimeToRelativeDescriptionConverter.BeginningOfWeek(currentDate).AddDays(-7))
                        {
                            result = "Last week";
                        }

                        if (DateTimeToRelativeDescriptionConverter.BeginningOfWeek(time) == DateTimeToRelativeDescriptionConverter.BeginningOfWeek(currentDate) && time.Date > currentDate)
                        {
                            result = "Later this week";
                        }

                        if (DateTimeToRelativeDescriptionConverter.BeginningOfWeek(time) == DateTimeToRelativeDescriptionConverter.BeginningOfWeek(currentDate) && time.Date < currentDate)
                        {
                            result = "Earlier this week";
                        }

                        if (time.Date == currentDate.Date.AddDays(1))
                        {
                            result = "Tomorrow";
                        }

                        if (time.Date == currentDate.Date.AddDays(-1))
                        {
                            result = "Yesterday";
                        }

                        if (time.Date == currentDate.Date)
                        {
                            result = "Today";
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

        /// <summary>
        /// Determines the first day of the week containing the specified date.
        /// </summary>
        /// <param name="date">The date to determine the first day of the week of.</param>
        /// <returns>The first day of the week containing the specified date.</returns>
        private static DateTime BeginningOfWeek(DateTime date)
        {
            return date.Date.AddDays(-1 * (int)date.DayOfWeek);
        }

        /// <summary>
        /// Determines the first day of the month containing the specified date.
        /// </summary>
        /// <param name="date">The date to determine the first day of the month of.</param>
        /// <returns>The first day of the month containing the specified date.</returns>
        private static DateTime BeginningOfMonth(DateTime date)
        {
            return date.Date.AddDays(-1 * date.Day);
        }
    }
}
