﻿// <copyright file="LmStatConnect.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2014 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace MockUtil
{
    using System;
    using System.IO;

    /// <summary>
    /// Generates a "cannot connect" lmstat report.
    /// </summary>
    public class LmStatConnect : StatusWriter
    {
        /// <summary>
        /// Initializes a new instance of the LmStatConnect class.
        /// </summary>
        public LmStatConnect() : base()
        {
            this.ServerName = "SERVER001";
            this.ServerPort = 28000;
        }

        /// <summary>
        /// Writes the lmstat cannot connect report.
        /// </summary>
        /// <remarks>
        /// If ReportDate is not specified then today's date will be used.
        /// </remarks>
        public override void CreateReport()
        {
            this.WriteLine("lmutil - Copyright (c) 1989-2007 Macrovision Europe Ltd. and/or Macrovision Corporation. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 14:25", this.ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("Error getting status: Cannot connect to license server system. (-15,10:10061 \"WinSock: Connection refused\")");
        }
    }
}
