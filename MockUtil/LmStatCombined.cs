// <copyright file="LmStatCombined.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

// This may not represent all situations with a combined license because this example
// was generated with two vendor daemons that use the same feature names.
//
// This example was created using NX 4 and NX 6 licenses. NX 4 was setup with 15
// licenses of gateway and drafting. NX 6 was setup with 20 licenses of gateway
// and solid_modeling.
//
// There were some differences in this lmstat when compared to others.
// There was no blank line between vendor daemon UP and "Feature usage info:".
// The uglmd: UP v10.8 began with a different number of spaces.
// The server name "license server UP" line began with a space. Due to server name length?

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
            this.ServerName = "SERVERC01";
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
            this.WriteLine("lmutil - Copyright (c) 1989-2007 Macrovision Europe Ltd. and/or Macrovision Corporation. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 11:39", this.ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("License server status: {0}@{1}", this.ServerPort, this.ServerName);
            this.WriteLine(@"    License file(s) on {0}: C:\License Servers\Combined\Combined.lic:", this.ServerName);
            this.WriteLine();
            this.WriteLine(" {0}: license server UP (MASTER) v11.4", this.ServerName);
            this.WriteLine();
            this.WriteLine("Vendor daemon status (on {0}):", this.ServerName);
            this.WriteLine();
            this.WriteLine("    {0}: UP v11.4", this.Vendor);
            this.WriteLine("Feature usage info:");
            this.WriteLine();

            this.WriteFeature("gateway", 35, 2);
            this.Version = "v24.000";
            this.WriteUser("userc01 compc01 compc01", 101, "11:37");

            this.WriteLine();
            this.WriteLine("  \"gateway\" v22.0, vendor: uglmd");
            this.WriteLine("  floating license");
            this.WriteLine();
            this.Version = "v22.0";
            this.WriteUser("userc02 compc02 compc02", 101, "11:35");
            this.WriteLine();

            this.Version = "v24.0";
            this.WriteFeature("solid_modeling", 20, 1);
            this.Version = "v24.000";
            this.WriteUser("userc01 compc01 compc01", 201, "11:38");
            this.WriteLine();

            this.Vendor = "uglmd";
            this.Version = "v22.0";
            this.WriteFeature("drafting", 15, 1);
            this.WriteUser("userc02 compc02 compc02", 201, "11:35");
            this.WriteLine();

            // Second daemon, same vendor, different version.
            this.Vendor = "uglmd";
            this.Version = "v22.0";
            this.WriteLine("     {0}: UP v10.8", this.Vendor);
            this.WriteLine("Feature usage info:");
            this.WriteLine();

            this.Vendor = "ugslmd";
            this.Version = "v24.0";
            this.WriteFeature("gateway", 35, 2);
            this.Version = "v24.000";
            this.WriteUser("userc01 compc01 compc01", 101, "11:37");

            this.WriteLine();
            this.WriteLine("  \"gateway\" v22.0, vendor: uglmd");
            this.WriteLine("  floating license");
            this.WriteLine();
            this.Version = "v22.0";
            this.WriteUser("userc02 compc02 compc02", 101, "11:35");
            this.WriteLine();

            this.Version = "v24.0";
            this.WriteFeature("solid_modeling", 20, 1);
            this.Version = "v24.000";
            this.WriteUser("userc01 compc01 compc01", 201, "11:38");
            this.WriteLine();

            this.Vendor = "uglmd";
            this.Version = "v22.0";
            this.WriteFeature("drafting", 15, 1);
            this.WriteUser("userc02 compc02 compc02", 201, "11:35");
            this.WriteLine();
        }
    }
}
