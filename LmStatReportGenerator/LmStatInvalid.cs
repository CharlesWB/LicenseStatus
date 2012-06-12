// <copyright file="LmStatInvalid.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace LmStatReportGenerator
{
    using System;
    using System.IO;

    /// <summary>
    /// Generates an invalid lmstat report.
    /// </summary>
    public class LmStatInvalid : LmStatWriterBase
    {
        /// <summary>
        /// Initializes a new instance of the LmStatInvalid class.
        /// </summary>
        public LmStatInvalid() : base()
        {
            this.ServerName = "SERVER001";
            this.ServerPort = 28000;
            this.Vendor = "ugslmd";
        }

        /// <summary>
        /// Writes the lmstat invalid report.
        /// </summary>
        /// <remarks>
        /// <para>If OutputFile is not specified the output will only write to the console.</para>
        /// <para>If ReportDate is not specified the today's date will be used.</para>
        /// </remarks>
        public override void CreateReport()
        {
            this.WriteLine("lmutil - Copyright (c) 1989-2007 Macrovision Europe Ltd. and/or Macrovision Corporation. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 19:58", this.ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("License server status: {0}@{1}", this.ServerPort, this.ServerName);
            this.WriteLine(@"    License file(s) on {0}: C:\License Servers\NX\NX.dat:", this.ServerName);
            this.WriteLine();
            this.WriteLine("{0}: license server UP (MASTER) v11.4", this.ServerName);
            this.WriteLine();
            this.WriteLine("Vendor daemon status (on {0}):", this.ServerName);
            this.WriteLine();
            this.WriteLine("    {0}: Cannot connect to license server system. (-15,578:10049 \"WinSock: Invalid address\")", this.Vendor);
            this.WriteLine();
        }
    }
}
