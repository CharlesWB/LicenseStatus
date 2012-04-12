// <copyright file="LmStatErrors.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to LicenseManager's License.cs for the full copyright notice.
// </copyright>

namespace LmStatReportGenerator
{
    using System;
    using System.IO;

    /// <summary>
    /// Generates an error lmstat report.
    /// </summary>
    public class LmStatErrors : LmStatWriterBase
    {
        /// <summary>
        /// Initializes a new instance of the LmStatErrors class.
        /// </summary>
        public LmStatErrors() : base()
        {
            this.ServerName = "SERVER001";
            this.ServerPort = 7601;
            this.Vendor = "theorem";
        }

        /// <summary>
        /// Writes the lmstat error report.
        /// </summary>
        /// <remarks>
        /// <para>If OutputFile is not specified the output will only write to the console.</para>
        /// <para>If ReportDate is not specified the today's date will be used.</para>
        /// </remarks>
        public override void CreateReport()
        {
            this.WriteLine("lmutil - Copyright (c) 1989-2005 Macrovision Europe Ltd. and/or Macrovision Corporation. All Rights Reserved.");
            this.WriteLine("Flexible License Manager status on {0:ddd M/d/yyyy} 15:42", ReportDate);
            this.WriteLine();
            this.WriteLine("[Detecting lmgrd processes...]");
            this.WriteLine("License server status: {0}@{1},{0}@SERVER002,{0}@SERVER003", this.ServerPort, this.ServerName);
            this.WriteLine(@"    License file(s) on {0}: D:\License Servers\Theorem\theorem.dat:", this.ServerName);
            this.WriteLine();
            this.WriteLine("  {0}: license server UP (MASTER) v10.8", this.ServerName);
            this.WriteLine("  SERVER002: license server UP (MASTER) v10.8");
            this.WriteLine("  SERVER003: license server UP v10.8");
            this.WriteLine();
            this.WriteLine("Vendor daemon status (on SERVER002):");
            this.WriteLine();
            this.WriteLine("   {0}: The desired vendor daemon is down. (-97,121)", this.Vendor);
            this.WriteLine();
            this.WriteLine("Feature usage info:");
            this.WriteLine();
            this.WriteLine("Users of Catia_GCO: Cannot get users of Catia_GCO: No such feature exists. (-5,222)");
            this.WriteLine("Users of Catia_GCO_V4: Cannot get users of Catia_GCO_V4: No such feature exists. (-5,222)");
            this.WriteLine("Users of Catia_UG: Cannot get users of Catia_UG: No such feature exists. (-5,222)");
            this.WriteLine("Users of Catia_UG_2D: Cannot get users of Catia_UG_2D: No such feature exists. (-5,222)");
            this.WriteLine("Users of GCO_Catia: Cannot get users of GCO_Catia: No such feature exists. (-5,222)");
            this.WriteLine("Users of GCO_Parasolid: Cannot get users of GCO_Parasolid: No such feature exists. (-5,222)");
            this.WriteLine("Users of GCO_UG: Cannot get users of GCO_UG: No such feature exists. (-5,222)");
            this.WriteLine("Users of GCO_Viewer: Cannot get users of GCO_Viewer: No such feature exists. (-5,222)");
            this.WriteLine("Users of UG_Catia: Cannot get users of UG_Catia: No such feature exists. (-5,222)");
            this.WriteLine("Users of UG_Catia_2D: Cannot get users of UG_Catia_2D: No such feature exists. (-5,222)");
            this.WriteLine("Users of UG_GCO: Cannot get users of UG_GCO: No such feature exists. (-5,222)");
        }
    }
}
