// <copyright file="LmStatLarge.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace MockUtil
{
    using System;
    using System.IO;

    /// <summary>
    /// Generates a large lmstat report.
    /// </summary>
    public class LmStatLarge : StatusWriter
    {
        /// <summary>
        /// Initializes a new instance of the LmStatLarge class.
        /// </summary>
        public LmStatLarge() : base()
        {
            this.ServerName = "SERVER001";
            this.ServerPort = 27002;
            this.Vendor = "testlarge";
            this.Version = "v1.0";
        }

        /// <summary>
        /// Writes the lmstat large report.
        /// </summary>
        /// <remarks>
        /// If ReportDate is not specified then today's date will be used.
        /// </remarks>
        public override void CreateReport()
        {
            this.WriteLine("lmutil - Copyright (c) 1989-2006 Macrovision Europe Ltd. and/or Macrovision Corporation. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 09:53", this.ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("License server status: {0}@{1}", this.ServerPort, this.ServerName);
            this.WriteLine(@"    License file(s) on {0}: C:\License Servers\Large Test\Large.lic:", this.ServerName);
            this.WriteLine();
            this.WriteLine("{0}: license server UP (MASTER) v10.8", this.ServerName);
            this.WriteLine();
            this.WriteLine("Vendor daemon status (on {0}):", this.ServerName);
            this.WriteLine();
            this.WriteLine("     {0}: UP v10.8", this.Vendor);
            this.WriteLine();
            this.WriteLine("Feature usage info:");
            this.WriteLine();

            for (int i = 1; i < 501; i++)
            {
                this.WriteFeature(string.Format("FEATURE_{0:000#}", i), 10, 1);
                this.WriteUser("user001 comp001 comp001", 3861, "7:13");
                this.WriteLine();
            }
        }
    }
}
