// <copyright file="User.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to License.cs for the full copyright notice.
// </copyright>

// Revisions:
//
// 2009-09-10 CWB
// The regular expression for finding the user name has been changed from matching a word
// character (\w) to matching non-white space character (\S).
// A license was found where the feature name included a period which is not a word character.
// It was decided that a user name could have the same issue and was also updated.

namespace CWBozarth.LicenseManager
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a user of a feature.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Stores the name.
        /// </summary>
        private string name;

        /// <summary>
        /// Stores the host.
        /// </summary>
        private string host;

        /// <summary>
        /// Stores the display.
        /// </summary>
        private string display;

        /// <summary>
        /// Stores the version used.
        /// </summary>
        private string version;

        /// <summary>
        /// Stores the server.
        /// </summary>
        private string server;

        /// <summary>
        /// Stores the port number.
        /// </summary>
        private int port;

        /// <summary>
        /// Stores the handle.
        /// </summary>
        private int handle;

        /// <summary>
        /// Stores the checkout time.
        /// </summary>
        private DateTime time;

        /// <summary>
        /// Stores the linger time.
        /// </summary>
        private TimeSpan linger;

        /// <summary>
        /// Stores the full user information.
        /// </summary>
        private string report;

        /// <summary>
        /// Initializes a new instance of the User class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Typically a user is created for a Feature by the internal constructor. This default constructor is provided
        /// as a means of instantiating an empty user object outside of the Feature class.
        /// </para>
        /// <para>
        /// This is a work around to support the License Status program and not intended for general use.
        /// </para>
        /// </remarks>
        public User()
        {
        }

        /// <summary>
        /// Initializes a new instance of the User class by parsing the user information read from a feature.
        /// </summary>
        /// <remarks>
        /// This is an internal constructor because only the Feature class is expected to provide the
        /// proper string for initialization.
        /// </remarks>
        /// <param name="userEntry">The user's information as read from the lmstat output.</param>
        internal User(string userEntry)
        {
            this.report = userEntry;

            // Examples:
            // user001 comp001 comp001 (v22.0) (SERVER001/27001 3861), start Tue 3/17 7:13
            // user005 comp005 comp005 (v22.0) (SERVER001/27001 601), start Tue 3/17 10:18 (linger: 14437140)
            Regex userExpression = new Regex(@"(?<name>\S+) (?<host>\S+) (?<display>\S+) \((?<version>\S+)\) \((?<server>\S+)/(?<port>\d+) (?<handle>\d+)\), start (?<time>\w+ \d+/\d+ \d+:\d+)( \(linger: (?<linger>\d+))?", RegexOptions.Multiline);
            Match match = userExpression.Match(this.report);
            if (match.Success)
            {
                this.name = match.Groups["name"].Value;
                this.host = match.Groups["host"].Value;
                this.display = match.Groups["display"].Value;
                this.version = match.Groups["version"].Value;
                this.server = match.Groups["server"].Value;
                this.port = int.Parse(match.Groups["port"].Value, CultureInfo.InvariantCulture);
                this.handle = int.Parse(match.Groups["handle"].Value, CultureInfo.InvariantCulture);

                // The check out date and time does not include the year. Because the day of the week is included
                // this can cause the date to be invalid when the checkout occurred in the previous year.
                // This is usually seen with borrowed licenses but can occur with a normal checkout before January 1.
                // Rather than checking the year of the lmutil report this will assume the report was made at
                // the current year. This is only an issue if this class is used to parse older reports.
                // If the day of the week, date and time are invalid for this year then they will be checked
                // against last year. If this is also invalid then the minimum date is returned.
                // This assumes that a check out will not be more than a year old.
                // It may be possible for the day of the week, date and time would be valid for both years.
                // This situation has not been tested or determined if possible.
                Regex dateExpression = new Regex(@"(?<dayofweek>\w+) (?<monthday>\d+/\d+) (?<time>\d+:\d+)");
                Match dateMatch = dateExpression.Match(match.Groups["time"].Value);

                bool isDateValid = DateTime.TryParseExact(match.Groups["time"].Value, "ddd M/d H:mm", null, DateTimeStyles.None, out this.time);
                if (!isDateValid)
                {
                    // Create a string with last year for parsing.
                    string dateWithLastYear = string.Format(CultureInfo.InvariantCulture, "{0} {1}/{2} {3}", dateMatch.Groups["dayofweek"].Value, dateMatch.Groups["monthday"].Value, DateTime.Now.AddYears(-1).Year, dateMatch.Groups["time"].Value);
                    DateTime.TryParseExact(dateWithLastYear, "ddd M/d/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out this.time);
                }

                if (match.Groups["linger"].Success)
                {
                    this.linger = TimeSpan.FromSeconds(int.Parse(match.Groups["linger"].Value, CultureInfo.InvariantCulture));
                }
                else
                {
                    this.linger = TimeSpan.Zero;
                }
            }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the host name.
        /// </summary>
        public string Host
        {
            get { return this.host; }
         }

        /// <summary>
        /// Gets the display.
        /// </summary>
        public string Display
        {
            get { return this.display; }
        }

        /// <summary>
        /// Gets the version used.
        /// </summary>
        public string Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// Gets the server.
        /// </summary>
        public string Server
        {
            get { return this.server; }
        }

        /// <summary>
        /// Gets the port number.
        /// </summary>
        public int Port
        {
            get { return this.port; }
        }

        /// <summary>
        /// Gets the handle.
        /// </summary>
        public int Handle
        {
            get { return this.handle; }
        }

        /// <summary>
        /// Gets the time the user checked out the license.
        /// </summary>
        /// <remarks>If the parsing of the time fails then MinValue is returned.</remarks>
        public DateTime Time
        {
            get { return this.time; }
        }

        /// <summary>
        /// Gets the linger value, which is the time the borrow is for.
        /// </summary>
        public TimeSpan Linger
        {
            get { return this.linger; }
        }

        /// <summary>
        /// Gets a value indicating whether the user has borrowed the feature.
        /// </summary>
        /// <remarks>This is equivalent to testing for a linger value of 0.</remarks>
        public bool IsBorrowed
        {
            get { return this.linger != TimeSpan.Zero; }
        }

        /// <summary>
        /// Gets the date and time when a borrowed license ends.
        /// </summary>
        /// <remarks>If the license is not borrowed then MinValue is returned.</remarks>
        public DateTime BorrowEndTime
        {
            get
            {
                if (this.IsBorrowed)
                {
                    return this.time + this.linger;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        /// <summary>
        /// Gets the full user information from lmstat.
        /// </summary>
        public string Report
        {
            get { return this.report; }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <returns>The name of the user.</returns>
        public override string ToString()
        {
            return this.name;
        }
    }
}
