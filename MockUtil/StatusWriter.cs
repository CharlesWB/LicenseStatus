// <copyright file="StatusWriter.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace MockUtil
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Base class for creating an lmstat report.
    /// </summary>
    public abstract class StatusWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusWriter"/> class.
        /// </summary>
        public StatusWriter()
        {
            this.ReportDate = DateTime.Today;
        }

        /// <summary>
        /// Gets or sets the date the report will be based off of.
        /// </summary>
        /// <remarks>The default value is today's date.</remarks>
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// Gets or sets the server name used by this report.
        /// </summary>
        public string ServerName { get; protected set; }

        /// <summary>
        /// Gets or sets the server port used by this report.
        /// </summary>
        public int ServerPort { get; protected set; }

        /// <summary>
        /// Gets or sets the vendor daemon name used by this report.
        /// </summary>
        public string Vendor { get; protected set; }

        /// <summary>
        /// Gets or sets the vendor daemon version used by this report.
        /// </summary>
        public string Version { get; protected set; }

        /// <summary>
        /// Writes the lmstat test report.
        /// </summary>
        /// <remarks>
        /// <para>If OutputFile is not specified the output will only write to the console.</para>
        /// <para>If ReportDate is not specified today's date will be used.</para>
        /// </remarks>
        public abstract void CreateReport();

        /// <summary>
        /// Writes a user to the output.
        /// </summary>
        /// <param name="userId">The combination of user name, host and display.</param>
        /// <param name="handle">The user's handle.</param>
        /// <param name="time">The checkout time.</param>
        protected void WriteUser(string userId, int handle, string time)
        {
            this.WriteUser(userId, handle, time, this.ReportDate, null);
        }

        /// <summary>
        /// Writes a user to the output with a custom date and/or linger.
        /// </summary>
        /// <param name="userId">The combination of user name, host and display.</param>
        /// <param name="handle">The user's handle.</param>
        /// <param name="time">The checkout time.</param>
        /// <param name="date">The checkout date.</param>
        /// <param name="linger">The linger value.</param>
        /// <remarks>If linger is null or empty it will not be written.</remarks>
        protected void WriteUser(string userId, int handle, string time, DateTime date, string linger)
        {
            string line = string.Format("    {0} ({1}) ({2}/{3} {4}), start {5:ddd M/d} {6}", userId, this.Version, this.ServerName, this.ServerPort, handle, date, time);
            if (string.IsNullOrEmpty(linger))
            {
                this.WriteLine(line);
            }
            else
            {
                this.WriteLine(line + " (linger: {0})", linger);
            }
        }

        /// <summary>
        /// Writes a feature that is not used to the output.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="issued">The number of licenses issued for this feature.</param>
        protected void WriteFeature(string name, int issued)
        {
            this.WriteFeature(name, issued, 0);
        }

        /// <summary>
        /// Writes a features that is used to the output.
        /// </summary>
        /// <param name="name">The feature name.</param>
        /// <param name="issued">The number of licenses issued for this feature.</param>
        /// <param name="inUse">The number of licenses in use for this feature.</param>
        protected void WriteFeature(string name, int issued, int inUse)
        {
            this.WriteLine("Users of {0}:  (Total of {1} license{3} issued;  Total of {2} license{4} in use)", name, issued, inUse, issued == 1 ? string.Empty : "s", inUse == 1 ? string.Empty : "s");

            if (inUse != 0)
            {
                this.WriteLine();
                this.WriteLine("  \"{0}\" {1}, vendor: {2}", name, this.Version, this.Vendor);
                this.WriteLine("  floating license");
            }
            
            this.WriteLine();
        }

        /// <summary>
        /// Writes a line terminator to the output.
        /// </summary>
        protected void WriteLine()
        {
            this.WriteLine(null);
        }

        /// <summary>
        /// Writes out a formatted string and a new line, using the same semantics as Format.
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="args">The object array to write into format string.</param>
        protected void WriteLine(string format, params object[] args)
        {
            // Force the culture to be en-US. As far as I know lmstat only writes in this format.
            this.WriteLine(string.Format(CultureInfo.CreateSpecificCulture("en-US"), format, args));
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the output.
        /// </summary>
        /// <param name="text">The string to write.</param>
        protected void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
