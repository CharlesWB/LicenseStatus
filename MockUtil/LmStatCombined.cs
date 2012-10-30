// <copyright file="LmStatCombined.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

// In the test there was no blank line between daemon UP and "Feature usage info:".
// The vendor daemon UP began with different number of spaces.
// The server UP began with a space.

// TODO Create a version where licenses are checked out.

namespace MockUtil
{
    using System;

    /// <summary>
    /// Generates a combined lmstat report.
    /// </summary>
    public class LmStatCombined : StatusWriter
    {
        /// <summary>
        /// Initializes a new instance of the LmStatCombined class.
        /// </summary>
        public LmStatCombined() : base()
        {
            this.ServerName = "SERVER001";
            this.ServerPort = 28000;
            this.Vendor = "ugslmd";
            this.Version = "v24.0";
        }

        /// <summary>
        /// Writes the lmstat combined report.
        /// </summary>
        /// <remarks>
        /// If ReportDate is not specified then today's date will be used.
        /// </remarks>
        public override void CreateReport()
        {
            this.WriteLine("lmutil - Copyright (c) 1989-2012 Flexera Software LLC. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 13:30", this.ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("License server status: {0}@{1}", this.ServerPort, this.ServerName);
            this.WriteLine(@"    License file(s) on {0}: C:\License Servers\Combined\Combined.lic:", this.ServerName);
            this.WriteLine();
            this.WriteLine(" {0}: license server UP (MASTER) v11.11", this.ServerName);
            this.WriteLine();
            this.WriteLine("Vendor daemon status (on {0}):", this.ServerName);
            this.WriteLine();
            this.WriteLine("    {0}: UP v11.4", this.Vendor);
            this.WriteLine("Feature usage info:");
            this.WriteLine();

            this.WriteFeature("gateway", 75);

            this.WriteFeature("assemblies", 25);

            this.WriteFeature("drafting", 50);

            // Second daemon, same vendor, different version.
            this.WriteLine("     uglmd: UP v10.8", this.Vendor);
            this.WriteLine("Feature usage info:");
            this.WriteLine();

            this.WriteFeature("gateway", 75);

            this.WriteFeature("assemblies", 25);

            this.WriteFeature("drafting", 50);
        }
    }
}
