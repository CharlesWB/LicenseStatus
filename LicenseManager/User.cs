// <copyright file="User.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to License.cs for the full copyright notice.
// </copyright>

#region Notes
// Name Host Display Phrase:
//
// The part of the user information which is the host, name and display will
// internally be referred to as the user's identity.
// The reason for doing this is because I was tired of naming things nameHostDisplay.
//
// Spaces in Name, Host, Display:
//
// When spaces are in name, host, or display then there is no definite way to
// determine which words belong to which parts. DetermineIdentityPattern
// contains the rules which attempt to parse the words in this situation.
// One rule is the assumption that the host will never contain a space. While
// probably true, it may not be technically true.
//
// Possible Enhancements
//
// The pattern for a known host is essentially repeated in DetermineIdentityPattern
// and KnownHostSet_HostAdded.
//
// The Where query is very similar in DetermineIdentityPattern and KnownHostSet_HostAdded.
#endregion

namespace CWBozarth.LicenseManager
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents a user of a feature.
    /// </summary>
    public class User : ObservableObject
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
        /// Stores the quantity of licenses in use by this user.
        /// </summary>
        private int quantityUsed;

        /// <summary>
        /// Stores the linger time.
        /// </summary>
        private TimeSpan linger;

        /// <summary>
        /// Stores the full user information.
        /// </summary>
        private string report;

        /// <summary>
        /// Stores the position in the full license report where the first character of the user entry is found.
        /// </summary>
        private int entryIndex;

        /// <summary>
        /// Stores the length of the user entry in the license report.
        /// </summary>
        private int entryLength;

        /// <summary>
        /// Stores the identity text.
        /// </summary>
        private string identity;

        /// <summary>
        /// Stores <see cref="identity"/> as an array of words.
        /// </summary>
        private string[] identityWords;

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
        /// <param name="index">The position in the license report where the first character of the user entry is found.</param>
        internal User(string userEntry, int index)
        {
            this.report = userEntry;

            this.entryIndex = index;
            this.entryLength = this.report.Length;

            this.ParseReport();
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            private set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets the host name.
        /// </summary>
        public string Host
        {
            get
            {
                return this.host;
            }

            private set
            {
                if (value != this.host)
                {
                    this.host = value;
                    this.NotifyPropertyChanged("Host");
                }
            }
         }

        /// <summary>
        /// Gets the display.
        /// </summary>
        public string Display
        {
            get
            { 
                return this.display;
            }

            private set
            {
                if (value != this.display)
                {
                    this.display = value;
                    this.NotifyPropertyChanged("Display");
                }
            }
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
        /// Gets the quantity of licenses in use by this user.
        /// </summary>
        public int QuantityUsed
        {
            get { return this.quantityUsed; }
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
        /// Gets the position in the full license report where the first character of the user entry is found.
        /// </summary>
        public int EntryIndex
        {
            get { return this.entryIndex; }
        }

        /// <summary>
        /// Gets the length of the user entry in the license report.
        /// </summary>
        /// <remarks>This is also equivalent to Report.Length.</remarks>
        public int EntryLength
        {
            get { return this.entryLength; }
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <returns>The name of the user.</returns>
        public override string ToString()
        {
            return this.name;
        }

        /// <summary>
        /// Parses the output of lmstat to populate the fields with the user's status.
        /// </summary>
        private void ParseReport()
        {
            // Examples:
            // user001 comp001 comp001 (v22.0) (SERVER001/27001 3861), start Tue 3/17 7:13
            // user005 comp005 comp005 (v22.0) (SERVER001/27001 601), start Tue 3/17 10:18 (linger: 14437140)
            // user011 comp011 comp011 (v22.0) (SERVER001/27001 2209), start Fri 3/17 13:21, 2 licenses
            Regex userExpression = new Regex(@"(?<identity>.+) \((?<version>\S+)\) \((?<server>\S+)/(?<port>\d+) (?<handle>\d+)\), start (?<time>\w+ \d+/\d+ \d+:\d+)(, (?<inuse>\d+) licenses)?( \(linger: (?<linger>\d+))?", RegexOptions.Multiline);
            Match match = userExpression.Match(this.report);
            if (match.Success)
            {
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

                bool isDateValid = DateTime.TryParseExact(match.Groups["time"].Value, "ddd M/d H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out this.time);
                if (!isDateValid)
                {
                    // Create a string with last year for parsing.
                    string dateWithLastYear = string.Format(CultureInfo.InvariantCulture, "{0} {1}/{2} {3}", dateMatch.Groups["dayofweek"].Value, dateMatch.Groups["monthday"].Value, DateTime.Now.AddYears(-1).Year, dateMatch.Groups["time"].Value);
                    DateTime.TryParseExact(dateWithLastYear, "ddd M/d/yyyy H:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out this.time);
                }

                if (match.Groups["inuse"].Success)
                {
                    this.quantityUsed = int.Parse(match.Groups["inuse"].Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    this.quantityUsed = 1;
                }

                if (match.Groups["linger"].Success)
                {
                    this.linger = TimeSpan.FromSeconds(int.Parse(match.Groups["linger"].Value, CultureInfo.InvariantCulture));
                }
                else
                {
                    this.linger = TimeSpan.Zero;
                }

                //// The following section is inside the Success just so that it only parses
                //// if the overall is success. It does not have to be inside.

                this.identity = match.Groups["identity"].Value;

                // If this has already been populated then there is no need to do it again
                // since the identity does not change once set.
                if (this.identityWords == null)
                {
                    this.identityWords = this.identity.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }

                this.ParseIdentity(this.DetermineIdentityPattern());
            }
        }

        /// <summary>
        /// Parses the user's identity into name, host and display.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match against.</param>
        private void ParseIdentity(string pattern)
        {
            Regex identityExpression = new Regex(pattern);
            Match match = identityExpression.Match(this.identity);
            if (match.Success)
            {
                this.Name = match.Groups["name"].Value;
                this.Host = match.Groups["host"].Value;
                this.Display = match.Groups["display"].Value;
            }
        }

        /// <summary>
        /// Determine the regular expression pattern used to parse the name, host, and display
        /// parts of the user report.
        /// </summary>
        /// <remarks>
        /// The pattern contains match groups named, "name", "host" and "display".
        /// </remarks>
        /// <returns>The regular expression pattern for the name, host and display.</returns>
        private string DetermineIdentityPattern()
        {
            // The default pattern is simply three words.
            string namePattern = @"\S+";
            string hostPattern = @"\S+";
            string displayPattern = @"\S+";

            if (this.identityWords.Length == 3)
            {
                KnownHostSet.Instance.Add(this.identityWords[1]);
            }

            if (this.identityWords.Length > 3)
            {
                // Determine if a known host is one of the identity words. But only if the
                // host is not the first or last word which has to be name and display.
                string knownHost = this.identityWords.Where((w, index) =>
                    KnownHostSet.Instance.IsKnown(w) && index != 0 && index != this.identityWords.Length - 1).FirstOrDefault();

                if (!string.IsNullOrEmpty(knownHost))
                {
                    // When a known host is found in the words then assume everything before it is
                    // the user and everything after is the display, regardless of spaces.
                    // This pattern is repeated in KnownHostSet_HostAdded.
                    namePattern = ".+";
                    hostPattern = knownHost;
                    displayPattern = ".+";
                }
                else
                {
                    // When the host is not known then choose an alternate arrangement of words.

                    // Assumes the host never contains a space.

                    // When the last word begins with a digit or when it is a single character
                    // assume that it belongs with the previous word to define the display.
                    // Also assume that the name may be multiple words in this case.
                    if (char.IsDigit(this.identityWords.Last().First()) || this.identityWords.Last().Length == 1)
                    {
                        namePattern = ".+";
                        displayPattern = string.Format(@"\S+ {0}", this.identityWords.Last());
                    }
                    else
                    {
                        // When no other alternate arrangement is found then assume the extra words
                        // belong to the user name.
                        namePattern = ".+";
                    }

                    // We only need to listen for new hosts if the host could not be determined
                    // for certain.
                    KnownHostSet.Instance.HostAdded += this.KnownHostSet_HostAdded;
                }
            }

            return string.Format(@"(?<name>{0}) (?<host>{1}) (?<display>{2})", namePattern, hostPattern, displayPattern);
        }

        /// <summary>
        /// Occurs when a new host has been added to the known host collection.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void KnownHostSet_HostAdded(object sender, HostAddedEventArgs e)
        {
            // Determine if the new host is one of the identity words. But only if the
            // host is not the first or last word which has to be name and display.
            string knownHost = this.identityWords.Where((w, index) =>
                w == e.Host && index != 0 && index != this.identityWords.Length - 1).FirstOrDefault();

            if (!string.IsNullOrEmpty(knownHost))
            {
                // If the host is known then we do not need to listen for new hosts.
                //// If this is the first time the user was parsed then this will do nothing.
                //// This will have an effect only when this is the second pass and a new host
                //// was reported that is in the words.
                KnownHostSet.Instance.HostAdded -= this.KnownHostSet_HostAdded;

                // This pattern is repeated in DetermineIdentityPattern.
                this.ParseIdentity(string.Format(@"(?<name>.+) (?<host>{0}) (?<display>.+)", knownHost));
            }
        }
    }
}
